using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;
using VComponent.SceneLoader;

namespace VComponent.Multiplayer
{
    public partial class MultiplayerConnectionManager
    {
        private string _lobbyName;
        private int _playerCount = 1;
        private int _gameTime = 300;

        public int PlayerCount => _playerCount;
        public int GameTime => _gameTime;

        private void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            HybridSceneLoader.Instance.ListenNetworkLoading(false);
        }

        private void StopHost()
        {
            HybridSceneLoader.Instance.UnListenNetworkLoading(false);
        }
        
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
                
                StartHost();
                
                LoadCurrentLobbyScene();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("Failed to create lobby: " + e.Message);
                OnTaskFailed?.Invoke("Lobby Creation Failed",e.Message);
            }
        }

        public void UpdateLobbyName(string lobbyName)
        {
            _lobbyName = lobbyName;
        }
        
        public void UpdatePlayerCount(int playerCount)
        {
            _playerCount = playerCount;
        }
        
        public void UpdateGameTime(int gameTime)
        {
            _gameTime = gameTime;
        }

        #endregion LOBBY CREATION
        
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
    }
}