using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using GDV.SceneLoader;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using VComponent.Tools.Singletons;
using VComponent.Tools.Timer;
using Random = UnityEngine.Random;

namespace VComponent.Multiplayer
{
    public class MultiplayerManager : PersistentSingleton<MultiplayerManager>
    {
        [Header("Parameters")]
        [SerializeField] private string _lobbyName = "Lobby";
        [SerializeField] private int _maxPlayers = 4;
        [SerializeField] private EncryptionType _encryption = EncryptionType.DTLS;
        
        [Header("References")]
        [SerializeField] private SceneReference _currentLobbyScene;
        
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

        #region LOBBY CREATION

        /// <summary>
        /// Create a public lobby.
        /// </summary>
        /// <param name="useRelayCode">Do you want to use a join code to access to it ?</param>
        /// <remarks>If you use a relay join code the lobby won't be visible from the lobby list.</remarks>
        public async Task CreateLobby(bool useRelayCode)
        {
            try
            {
                Allocation allocation = await AllocateRelay();

                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    Player = GetPlayer(),
                    // The lobby is public.
                    IsPrivate = false
                };

                _currentLobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, _maxPlayers, options);
                Debug.Log("Created lobby: " + _currentLobby.Name + " with code " + _currentLobby.LobbyCode);

                // Starting heartbeats for lobby heartbeat and pool updates for the lobby
                _heartbeatTimer.Start();
                _pollForUpdatesTimer.Start();

                // Setup the lobby with the relay join code.
                if (useRelayCode)
                {
                    string relayJoinCode = await GetRelayJoinCode(allocation);

                    await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                        {
                            { KEY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                        }
                    });
                }

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, ConnectionType));

                NetworkManager.Singleton.StartHost();
                
                LoadCurrentLobbyScene();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to create lobby: " + e.Message);
                OnTaskFailed?.Invoke("Lobby Creation Failed",e.Message);
            }
        }

        public string GetLobbyName()
        {
            return _lobbyName = $"PirateParty{Random.Range(0, 1000)}";
        }

        public void UpdateLobbyName(string lobbyName)
        {
            _lobbyName = lobbyName;
        }

        #endregion LOBBY CREATION

        #region JOIN LOBBY

        public async void JoinLobby(Lobby lobby)
        {
            try
            {
                Player player = GetPlayer();

                _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
                {
                    Player = player
                });
            
                // Joining the relay if available.
                if (_currentLobby.Data.TryGetValue(KEY_JOIN_CODE, out var joinCode))
                {
                    Debug.Log("Logged with relay");
                    string relayJoinCode = joinCode.Value;
                    JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                    
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, ConnectionType));
                }
            
                _pollForUpdatesTimer.Start();

                NetworkManager.Singleton.StartClient();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join lobby.{e}");
                OnTaskFailed?.Invoke("Lobby Join Failed",e.Message);
            }
        }
        
        /// <summary>
        /// Quick join the first accessible lobby.
        /// </summary>
        public async void QuickJoinLobby()
        {
            try
            {
                // Quick join a lobby.
                _currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                _pollForUpdatesTimer.Start();

                // Joining the relay if available.
                if (_currentLobby.Data.TryGetValue(KEY_JOIN_CODE, out var joinCode))
                {
                    string relayJoinCode = joinCode.Value;
                    JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                    
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, ConnectionType));
                }

                NetworkManager.Singleton.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to quick join lobby: " + e.Message);
                OnTaskFailed?.Invoke("Lobby Quick Join Failed",e.Message);
            }
        }

        #endregion JOIN LOBBY

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

        public async void KickPlayerFromLobby(string playerId)
        {
            if (IsLobbyHost())
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, playerId);
                    Debug.Log($"Player{playerId} successfully kicked from the lobby.");
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogError($"Unable to kick player{playerId} from lobby. {e}");
                    OnTaskFailed?.Invoke("Kick Player Failed",e.Message);
                }
            }
        }
        
        private void HandlePlayerKickedFromLobby()
        {
            Debug.Log("You have been kicked from the lobby !");
            SceneLoader.OnLoadScene?.Invoke(SceneLoader.SceneIdentifier.MAIN_MENU);
        }

        #endregion QUIT LOBBY

        #region FETCH LOBBIES

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

        #region HELPERS METHODS

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

        /// <summary>
        /// Manage the heartbeat of the lobby.
        /// </summary>
        private async Task HandleHeartbeatAsync()
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
                Debug.Log($"Sent heartbeat ping to lobby: {_currentLobby.Name}.");
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
                    HandlePlayerKickedFromLobby();
                    
                    return;
                }

                //Debug.Log($"Polled for updates on lobby: {_currentLobby.Name}");
                OnLobbyPolled?.Invoke(_currentLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to poll for updates on lobby.{e}");
                OnTaskFailed?.Invoke("Poll Lobby Failed", e.Message);
            }
        }

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
            
            MultiplayerSceneLoader.LoadNetwork(_currentLobbyScene);
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