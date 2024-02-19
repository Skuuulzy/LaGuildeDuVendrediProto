using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Component.Tools.Timer;
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
using Random = UnityEngine.Random;

namespace Component.Multiplayer
{
    [System.Serializable]
    public enum EncryptionType
    {
        DTLS, // Datagram Transport Layer Security
        WSS // Web Socket Secure
    }
    // Note: Also Udp and Ws are possible choices

    public class MultiplayerManager : MonoBehaviour
    {
        [SerializeField] private LobbyView _view;
        [SerializeField] private string _lobbyName = "Lobby";
        [SerializeField] private int _maxPlayers = 4;
        [SerializeField] private EncryptionType _encryption = EncryptionType.DTLS;
        
        public string PlayerId { get; private set; }
        public string PlayerName { get; private set; }
        
        private Lobby _currentLobby;
        private string ConnectionType => _encryption == EncryptionType.DTLS ? DTLS_ENCRYPTION : WSS_ENCRYPTION;

        // The delay in seconds between each lobby heart beat.
        private const float LOBBY_HEART_BEAT_INTERVAL = 20f;
        // The delay in seconds between each lobby data updates.
        private const float LOBBY_POLL_INTERVAL = 65f;
        
        private const string KEY_JOIN_CODE = "RelayJoinCode";
        
        private const string DTLS_ENCRYPTION = "dtls"; // Datagram Transport Layer Security
        private const string WSS_ENCRYPTION = "wss"; // Web Socket Secure, use for WebGL builds
        
        private const string MULTIPLAYER_ID_KEY = "MULTIPLAYER_ID";

        private readonly CountdownTimer _heartbeatTimer = new(LOBBY_HEART_BEAT_INTERVAL);
        private readonly CountdownTimer _pollForUpdatesTimer = new(LOBBY_POLL_INTERVAL);
        
        private void Awake()
        {
            RetrievePlayerName();
            _view.ShowAuthenticationWindow(PlayerName);
            
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

        private void RetrievePlayerName()
        {
            if (PlayerPrefs.HasKey(MULTIPLAYER_ID_KEY))
            {
                PlayerName = PlayerPrefs.GetString(MULTIPLAYER_ID_KEY);
            }
            else
            {
                UpdatePlayerName($"Pirate{Random.Range(0, 1000)}");
            }
        }

        public async void Authenticate()
        {
            // Authenticate
            await Authenticate(PlayerName);
            // Fetch available lobbies
            await RefreshLobbyList();
            
            _view.ShowLobbyList();
        }

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
                PlayerId = AuthenticationService.Instance.PlayerId;
                PlayerName = playerName;
            }
            
            _view.ShowLoading(false);
        }

        /// <summary>
        /// Create a lobby.
        /// </summary>
        public async void CreateLobby()
        {
            try
            {
                Allocation allocation = await AllocateRelay();
                string relayJoinCode = await GetRelayJoinCode(allocation);

                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    // The lobby is public.
                    IsPrivate = false
                };

                _currentLobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, _maxPlayers, options);
                Debug.Log("Created lobby: " + _currentLobby.Name + " with code " + _currentLobby.LobbyCode);

                // Starting heartbeats for lobby heartbeat and pool updates for the lobby
                _heartbeatTimer.Start();
                _pollForUpdatesTimer.Start();

                // Setup the lobby with the relay join code.
                await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { KEY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                    }
                });

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, ConnectionType));

                NetworkManager.Singleton.StartHost();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to create lobby: " + e.Message);
            }
        }

        /// <summary>
        /// Quick join the first accessible lobby.
        /// </summary>
        public async Task QuickJoinLobby()
        {
            try
            {
                // Quick join a lobby.
                _currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                _pollForUpdatesTimer.Start();

                // Joining the relay.
                string relayJoinCode = _currentLobby.Data[KEY_JOIN_CODE].Value;
                JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, ConnectionType));

                NetworkManager.Singleton.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to quick join lobby: " + e.Message);
            }
        }

        private async Task RefreshLobbyList()
        {
            _view.ShowLoading(false);
            
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

                QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();
                
                _view.UpdateLobbyList(lobbyListQueryResponse.Results);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Unable to fetch any lobbies. {e}");
            }
            
            _view.ShowLoading(false);
        }
        
        public void UpdatePlayerName(string newName)
        {
            PlayerName = newName;
            PlayerPrefs.SetString(MULTIPLAYER_ID_KEY, newName);
        }

        #region PUBLIC BUTTONS METHODS

        public void RefreshLobbyListBtn()
        {
            _ = RefreshLobbyList();
        }

        #endregion

        #region PRIVATE METHODS

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
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
                Debug.Log("Polled for updates on lobby: " + lobby.Name);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to poll for updates on lobby: " + e.Message);
            }
        }

        #endregion PRIVATE METHODS
    }
}