using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace VComponent.Multiplayer
{
    /// <summary>
    /// Handle multiplayer gameplay main logic (player spawn, timer)
    /// </summary>
    public class MultiplayerGameplayManager : NetworkSingleton<MultiplayerConnectionManager>
    {
        private NetworkList<PlayerData> _playerDataNetworkList;

        
        [SerializeField] private Transform _playerPrefab;

        protected override void Awake()
        {
            base.Awake();
            
            _playerDataNetworkList = new NetworkList<PlayerData>();
            //_playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += HandleSceneLoaded;
            }
        }

        private void HandleSceneLoaded(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            // Instantiating players
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Transform playerTransform = Instantiate(_playerPrefab);
                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }
        }
    }
}