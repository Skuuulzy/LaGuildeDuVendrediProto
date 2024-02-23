using System.Collections.Generic;
using System.Threading.Tasks;
using Eflatun.SceneReference;
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
using VComponent.Tools.Timer;
using Random = UnityEngine.Random;

namespace VComponent.Multiplayer
{
    public class MultiplayerManager : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private string _lobbyName = "Lobby";
        [SerializeField] private int _maxPlayers = 4;
        [SerializeField] private int _minPlayerCountToPlay = 2;
        [SerializeField] private EncryptionType _encryption = EncryptionType.DTLS;
        
        [Header("References")]
        [SerializeField] private LobbyView _view;
        [SerializeField] private SceneReference _multiScene;
        
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

        #region MONO

        private void Awake()
        {
            DontDestroyOnLoad(this);
            
            _view.Init();
            
            RetrievePlayerName();
            _view.ShowAuthenticationWindow(_playerName);
            
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

        private async Task Authenticate(string playerName)
        {
            _view.ShowLoading(true);
            
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                InitializationOptions options = new InitializationOptions();
                // Each players must have a different identifier.
                options.SetProfile(playerName);

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
                _playerName = playerName;
            }
            
            _view.ShowLoading(false);
        }
        
        private void RetrievePlayerName()
        {
            if (PlayerPrefs.HasKey(MULTIPLAYER_ID_KEY))
            {
                _playerName = PlayerPrefs.GetString(MULTIPLAYER_ID_KEY);
            }
            else
            {
                UpdatePlayerName($"Pirate{Random.Range(0, 1000)}");
            }
        }

        #endregion AUTHENTICATION

        #region LOBBY CREATION

        /// <summary>
        /// Create a public lobby.
        /// </summary>
        /// <param name="useRelayCode">Do you want to use a join code to access to it ?</param>
        /// <remarks>If you use a relay join code the lobby won't be visible from the lobby list.</remarks>
        private async void CreateLobby(bool useRelayCode)
        {
            _view.ShowLoading(true);
            _view.CloseLobbyCreationWindow();
            
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
                
                _view.ShowLoading(false);
                _view.ShowCurrentLobby(_currentLobby);
                _view.UpdateLobby(_currentLobby, IsLobbyHost(), this, _minPlayerCountToPlay);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to create lobby: " + e.Message);
                
                // TODO: Add an error panel
                _view.ShowLoading(false);
            }
        }

        #endregion LOBBY CREATION

        #region JOIN LOBBY

        public async void JoinLobby(Lobby lobby)
        {
            _view.ShowLoading(true);
            _view.ShowLobbyList(false);
            
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
            
            _view.ShowLoading(false);
            _view.ShowCurrentLobby(_currentLobby);
            _view.UpdateLobby(_currentLobby, IsLobbyHost(), this, _minPlayerCountToPlay);
        }
        
        /// <summary>
        /// Quick join the first accessible lobby.
        /// </summary>
        /// <remarks>This method do not seem to work very well future Yona.</remarks>
        public async void QuickJoinLobby()
        {
            _view.ShowLoading(true);
            
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
                _view.ShowLoading(false);
            }
            
            _view.ShowLoading(false);
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
            
            _view.ShowLoading(true);
            _view.HideCurrentLobby();
            
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, AuthenticationService.Instance.PlayerId);
                Debug.Log("Looby succesfully leaved.");
                _currentLobby = null;

                await FetchLobbies();
                _view.ShowLobbyList(true);
                _view.ShowLoading(false);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Cannot leave the lobby. {e}");
            }
        }

        public async void KickPlayerFromLobby(string playerId)
        {
            if (IsLobbyHost())
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, playerId);
                    Debug.Log($"Player{playerId} sucessfully kicked from the lobby.");
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log($"Unable to kick player{playerId} from lobby. {e}");
                }
            }
        }
        
        private async void HandlePlayerKickedFromLobby()
        {
            Debug.Log("You have been kicked from the lobby !");
            
            _view.ShowLoading(true);
            _view.HideCurrentLobby();
            
            await FetchLobbies();
            _view.ShowLobbyList(true);
            
            _view.ShowLoading(false);
        }

        #endregion QUIT LOBBY

        #region FETCH LOBBIES

        private async Task FetchLobbies()
        {
            _view.ShowLoading(true);
            
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

                QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);

                _view.UpdateLobbyList(lobbyListQueryResponse.Results, this);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Unable to fetch any lobbies. {e}");
            }
            
            _view.ShowLoading(false);
        }

        #endregion FETCH LOBBIES

        #region PUBLIC UI METHODS
        
        public async void Authenticate()
        {
            // Authenticate
            await Authenticate(_playerName);
            // Fetch available lobbies
            await FetchLobbies();
            
            _view.ShowLobbyList(true);
        }
        
        public void CreatePublicLobby()
        {
            CreateLobby(true);
        }

        public void RefreshLobbyListBtn()
        {
            _ = FetchLobbies();
        }
        
        public void UpdatePlayerName(string newName)
        {
            _playerName = newName;
            PlayerPrefs.SetString(MULTIPLAYER_ID_KEY, newName);
        }

        public void OpenLobbyCreation()
        {
            _lobbyName = $"PirateParty{Random.Range(0, 1000)}";
            _view.OpenLobbyCreationWindow(_lobbyName);
        }

        public void UpdateLobbyName(string newName)
        {
            _lobbyName = newName;
        }

        public void LoadGameplayScene()
        {
            if (!IsLobbyHost())
            {
                Debug.LogError("You cannot launch the game if you are not the host !");
                return;
            }
            
            MultiplayerSceneLoader.LoadNetwork(_multiScene);
        }

        #endregion PUBLIC UI METHODS

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
                Debug.LogError("Failed to allocate relay: " + e.Message);
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
                Debug.LogError("Failed to get relay join code: " + e.Message);
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
                Debug.LogError("Failed to join relay: " + e.Message);
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
                Debug.Log("Sent heartbeat ping to lobby: " + _currentLobby.Name);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to heartbeat lobby: " + e.Message);
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

                _view.UpdateLobby(_currentLobby, IsLobbyHost(), this, _minPlayerCountToPlay);
                Debug.Log("Polled for updates on lobby: " + _currentLobby.Name);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to poll for updates on lobby: " + e.Message);
            }
        }

        private Player GetPlayer()
        {
            return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject>
            {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName) }
            });
        }

        private bool IsLobbyHost()
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

        #endregion HELPERS METHODS
    }
    
    [System.Serializable]
    public enum EncryptionType
    {
        DTLS, // Datagram Transport Layer Security
        WSS // Web Socket Secure
    }
    // Note: Also Udp and Ws are possible choices
}