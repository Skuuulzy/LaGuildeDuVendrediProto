using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using VComponent.Tools.EventSystem;
using VComponent.Tools.Singletons;

namespace VComponent.Multiplayer
{
    /// <summary>
    /// Handle multiplayer gameplay main logic (player spawn, timer)
    /// </summary>
    public class MultiplayerGameplayManager : NetworkSingleton<MultiplayerGameplayManager>
    {
        [Header("Parameters")] 
        [SerializeField] private int _playerMinCount = 1;

        [Header("Broadcasting On")] 
        [SerializeField] private EventChannel<Empty> _onWaitingAllPlayerConnected;
        [SerializeField] private EventChannel<Empty> _onAllPlayerConnected;

        [Header("Components")] 
        [SerializeField] private Transform _playerPrefab;

        
        private List<PlayerData> _playerDataNetworkList;

        protected override void Awake()
        {
            base.Awake();
            _playerDataNetworkList = new List<PlayerData>();
            _onWaitingAllPlayerConnected.Invoke(new Empty());
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _playerDataNetworkList = null;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Add the host to the list.
                _playerDataNetworkList.Add(CreatePlayerData());
                
                WaitForAllPlayers();
            }
            else
            {
                // Inform the server we enter the game.
                ClientEnterGameServerRpc(CreatePlayerData());
            }
        }

        #region SERVER

        /// <summary>
        /// SERVER-SIDE
        /// The client inform the server that he is connected.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void ClientEnterGameServerRpc(PlayerData playerData)
        {
            _playerDataNetworkList.Add(playerData);

            UpdatePlayerDataListClientRpc(_playerDataNetworkList.ToArray());
        }
        
        /// <summary>
        /// SERVER
        /// Wait for all the player to be connected. Then instantiate the player prefabs and inform clients.
        /// </summary>
        private async void WaitForAllPlayers()
        {
            while (_playerDataNetworkList.Count < _playerMinCount)
            {
                await Task.Delay(500);
            }

            InstantiatePlayersPrefabs();
            AllClientConnectedClientRpc();
        }

        /// <summary>
        /// SERVER - SIDE
        /// Instantiate the base boat player prefab for all connected players.
        /// </summary>
        private void InstantiatePlayersPrefabs()
        {
            // Instantiating players
            foreach (PlayerData playerData in _playerDataNetworkList)
            {
                Transform playerTransform = Instantiate(_playerPrefab);
                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerData.ClientId, true);
            }
        }

        #endregion SERVER

        #region CLIENT

        private PlayerData CreatePlayerData()
        {
            return new PlayerData(NetworkManager.Singleton.LocalClientId, (FixedString64Bytes)MultiplayerConnectionManager.Instance.GetPlayerName(), "");
        }
        
        /// <summary>
        /// CLIENT-SIDE
        /// Called when the server update the player data list and inform all the clients of the update.
        /// </summary>
        [ClientRpc]
        private void UpdatePlayerDataListClientRpc(PlayerData[] newPlayerData)
        {
            _playerDataNetworkList = newPlayerData.ToList();
        }

        /// <summary>
        /// CLIENT-SIDE
        /// The server inform that all the player are connected.
        /// </summary>
        [ClientRpc]
        private void AllClientConnectedClientRpc()
        {
            _onAllPlayerConnected.Invoke(new Empty());
        }

        /// <summary>
        /// Return the player name from the client ID.
        /// </summary>
        public string GetPlayerNameFromId(ulong clientId)
        {
            for (int i = 0; i < _playerDataNetworkList.Count; i++)
            {
                if (_playerDataNetworkList[i].ClientId == clientId)
                {
                    return _playerDataNetworkList[i].PlayerName.ToString();
                }
            }

            Debug.LogError($"Unable to find client with ID: {clientId}");
            return string.Empty;
        }

        #endregion CLIENT
    }
}