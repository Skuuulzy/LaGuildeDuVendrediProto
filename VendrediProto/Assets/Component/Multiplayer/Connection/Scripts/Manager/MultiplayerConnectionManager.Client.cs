
using System;
using GDV.SceneLoader;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace VComponent.Multiplayer
{
    public partial class MultiplayerConnectionManager
    {
        private void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            HybridSceneLoader.Instance.ListenNetworkLoading(true);
        }

        private void StopClient()
        {
            HybridSceneLoader.Instance.UnListenNetworkLoading(true);
        }
        
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

                StartClient();
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
        
        private void HandleClientKickedFromLobby()
        {
            Debug.Log("You have been kicked from the lobby !");
            HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAIN_MENU);
        }
    }
}