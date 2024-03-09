using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using VComponent.SceneLoader;
using VComponent.Tools.Singletons;
using VComponent.Tools.Timer;
using Random = UnityEngine.Random;

namespace VComponent.Multiplayer
{
    public partial class MultiplayerConnectionManager : PersistentSingleton<MultiplayerConnectionManager>
    {
        [Header("Parameters")]
        [SerializeField] private int _maxPlayers = 4;
        [SerializeField] private EncryptionType _encryption = EncryptionType.DTLS;
        [SerializeField] private bool _logPolls;
        [SerializeField] private bool _logHeartbeats;
        
        #region CONST VAR

        // The delay in seconds between each lobby heart beat.
        private const float LOBBY_HEART_BEAT_INTERVAL = 20f;
        // The delay in seconds between each lobby data updates.
        private const float LOBBY_POLL_INTERVAL = 3f;
        
        private const string KEY_JOIN_CODE = "RelayJoinCode";
        
        private const string DTLS_ENCRYPTION = "dtls"; // Datagram Transport Layer Security
        private const string WSS_ENCRYPTION = "wss"; // Web Socket Secure, use for WebGL builds
        
        private const string MULTIPLAYER_ID_KEY = "MULTIPLAYER_ID";
        
        public const string KEY_PLAYER_NAME = "PlayerName";

        #endregion CONST VAR
        
        private string _lobbyName;
        private Lobby _currentLobby;
        private string _playerId;
        private string _playerName;
        
        private string ConnectionType => _encryption == EncryptionType.DTLS ? DTLS_ENCRYPTION : WSS_ENCRYPTION;
        
        private readonly CountdownTimer _heartbeatTimer = new(LOBBY_HEART_BEAT_INTERVAL);
        private readonly CountdownTimer _pollForUpdatesTimer = new(LOBBY_POLL_INTERVAL);

        /// <summary>
        /// Raised when a task has failed. First string is the error title, second is the error details.
        /// </summary>
        public static Action<string, string> OnTaskFailed;

        /// <summary>
        /// Raise at each lobby poll;
        /// </summary>
        public static Action<Lobby> OnLobbyPolled;

        #region MONO

        protected override void Awake()
        {
            base.Awake();
            
            // When the countdown stop re sent an heart beat and restart the timer.
            _heartbeatTimer.OnTimerStop += () =>
            {
                _ =HandleHeartbeatAsync();
                _heartbeatTimer.Start();
            };

            // When the countdown stop re sent a poll update and restart the timer.
            _pollForUpdatesTimer.OnTimerStop += () =>
            {
                _ = HandlePollForUpdatesAsync();
                _pollForUpdatesTimer.Start();
            };
        }
        
        private void Update()
        {
            _heartbeatTimer.Tick(Time.deltaTime);
            _pollForUpdatesTimer.Tick(Time.deltaTime);
        }

        #endregion MONO

        #region AUTHENTICATION

        public string GetPlayerName()
        {
#if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone())
            {
                _playerName = $"Clone{Random.Range(0, 1000)}";
                return _playerName;
            }
#endif
            
            if (PlayerPrefs.HasKey(MULTIPLAYER_ID_KEY))
            {
                _playerName = PlayerPrefs.GetString(MULTIPLAYER_ID_KEY);
            }
            else
            {
                UpdatePlayerName($"Pirate{Random.Range(0, 1000)}");
            }

            return _playerName;
        }
        
        public void UpdatePlayerName(string newName)
        {
            _playerName = newName;
            PlayerPrefs.SetString(MULTIPLAYER_ID_KEY, newName);
        }

        public async Task Authenticate()
        {
            try
            {
                if (UnityServices.State == ServicesInitializationState.Uninitialized)
                {
                    InitializationOptions options = new InitializationOptions();
                    // Each players must have a different identifier.
                    options.SetProfile(_playerName);

                    await UnityServices.InitializeAsync(options);
                }

                AuthenticationService.Instance.SignedIn += () =>
                {
                    Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
                };

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    _playerId = AuthenticationService.Instance.PlayerId;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to authenticate. {e}");
                OnTaskFailed?.Invoke("Authentication Failed",e.Message);
            }
        }

        #endregion AUTHENTICATION

        #region QUIT LOBBY

        public async void LeaveLobby()
        {
            if (_currentLobby == null)
            {
                Debug.LogError("Cannot leave a lobby when you are not in one !");
                return;
            }
            
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, AuthenticationService.Instance.PlayerId);
                _currentLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to leave lobby: " + e.Message);
                OnTaskFailed?.Invoke("Lobby Leave Failed",e.Message);
            }
        }

        #endregion QUIT LOBBY

        #region FETCH LOBBIES
        
        public string GetLobbyName()
        {
            if (_lobbyName == null)
            {
                _lobbyName = $"PirateParty{Random.Range(0, 1000)}";
            }

            return _lobbyName;
        }

        public async Task<List<Lobby>> FetchLobbies()
        {
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions
                {
                    Count = 25,
                    // Filter for open lobbies only
                    Filters = new List<QueryFilter>
                    {
                        // Fetching lobbies with at least one slot.
                        new(
                            field: QueryFilter.FieldOptions.AvailableSlots,
                            op: QueryFilter.OpOptions.GT,
                            value: "0")
                    },
                    // Order by newest lobbies first
                    Order = new List<QueryOrder>
                    {
                        new (
                            asc: false,
                            field: QueryOrder.FieldOptions.Created)
                    }
                };

                var request = await Lobbies.Instance.QueryLobbiesAsync(options);
                
                return request.Results;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Unable to fetch any lobbies. {e}");
                OnTaskFailed?.Invoke("Fetch Lobbies Failed", e.Message);
                return null;
            }
        }

        #endregion FETCH LOBBIES

        #region QUIT

        public void QuitNetwork()
        {
            if (IsLobbyHost())
            {
                StopHost();
            }
            else
            {
                StopClient();
            }
            
            // Destroying multiplayer singletons instances
            Destroy(NetworkManager.Singleton.gameObject);
            Destroy(this.gameObject);
        }

        #endregion

        #region RELAY METHODS

        /// <summary>
        /// Try to create a relay allocation.
        /// </summary>
        /// <returns>The created allocation. Default if the creation has failed.</returns>
        private async Task<Allocation> AllocateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maxPlayers - 1); // Exclude the host
                return allocation;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Failed to allocate relay. {e}");
                OnTaskFailed?.Invoke("Allocate Relay Failed", e.Message);
                return default;
            }
        }

        /// <summary>
        /// Try to get the join code from an allocation. Used by the host.
        /// </summary>
        /// <param name="allocation">The targeted allocation</param>
        /// <returns>The join code. Default if the retrieve has failed.</returns>
        private static async Task<string> GetRelayJoinCode(Allocation allocation)
        {
            try
            {
                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return relayJoinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Failed to get relay join code. {e}");
                OnTaskFailed?.Invoke("Get Relay Code Failed", e.Message);
                return default;
            }
        }

        /// <summary>
        /// Try to join a relay. Used by the clients.
        /// </summary>
        /// <param name="relayJoinCode">The join code to join the relay.</param>
        /// <returns>The join allocation. Default if the join relay has failed.</returns>
        private static async Task<JoinAllocation> JoinRelay(string relayJoinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                return joinAllocation;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Failed to join relay. {e}");
                OnTaskFailed?.Invoke("Join Relay Failed", e.Message);
                return default;
            }
        }

        #endregion

        #region HEARTBEAT AND POLL

        /// <summary>
        /// Manage the heartbeat of the lobby.
        /// </summary>
        private async Task HandleHeartbeatAsync()
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
                if (_logHeartbeats)
                {
                    Debug.Log($"Sent heartbeat ping to lobby: {_currentLobby.Name}.");
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to heartbeat lobby. {e}");
                OnTaskFailed?.Invoke("Heartbeat Lobby Failed", e.Message);
            }
        }

        /// <summary>
        /// Responsible for initiating the process of checking for updates in the lobby.
        /// In this context, "polling" refers to periodically checking for changes or updates in the lobby's state or data.
        /// </summary>
        private async Task HandlePollForUpdatesAsync()
        {
            try
            {
                _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);

                // The player has been kicked from the lobby
                if (!IsInLobby())
                {
                    _currentLobby = null;
                    HandleClientKickedFromLobby();
                    
                    return;
                }

                //Debug.Log($"Polled for updates on lobby: {_currentLobby.Name}");
                OnLobbyPolled?.Invoke(_currentLobby);

                if (_logPolls)
                {
                    Debug.Log($"Polling lobby: {_currentLobby.Name}.");
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to poll for updates on lobby.{e}");
                OnTaskFailed?.Invoke("Poll Lobby Failed", e.Message);
            }
        }

        #endregion

        #region HELPERS METHODS
        
        private Player GetPlayer()
        {
            return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject>
            {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName) }
            });
        }

        public bool IsLobbyHost()
        {
            return _currentLobby != null && _currentLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        private bool IsInLobby()
        {
            if (_currentLobby != null && _currentLobby.Players != null)
            {
                foreach (Player player in _currentLobby.Players)
                {
                    if (player.Id == AuthenticationService.Instance.PlayerId)
                    {
                        // This player is in this lobby
                        return true;
                    }
                }
            }

            return false;
        }
        
        /// <summary>
        /// Load the current lobby parameter scene.
        /// </summary>
        private void LoadCurrentLobbyScene()
        {
            if (!IsLobbyHost())
            {
                Debug.LogError("You cannot launch the game if you are not the host !");
                return;
            }
            
            HybridSceneLoader.Instance.LoadNetworkScene(HybridSceneLoader.SceneIdentifier.MULTIPLAYER_GAMEPLAY);
        }

        #endregion HELPERS METHODS
    }
    
    [Serializable]
    public enum EncryptionType
    {
        DTLS, // Datagram Transport Layer Security
        WSS // Web Socket Secure
    }
    // Note: Also Udp and Ws are possible choices
}