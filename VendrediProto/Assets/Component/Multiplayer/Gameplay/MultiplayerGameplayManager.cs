using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using VComponent.Tools.EventSystem;
using VComponent.Tools.Singletons;
using VComponent.Tools.Timer;

namespace VComponent.Multiplayer
{
    /// <summary>
    /// Handle multiplayer gameplay main logic (player spawn, timer)
    /// </summary>
    public class MultiplayerGameplayManager : NetworkSingleton<MultiplayerGameplayManager>
    {
        [Header("Components")] 
        [SerializeField] private List<PlayerIslandController> _playerIslands;
        
        [Header("Broadcasting On")]
        [SerializeField] private EventChannel<Empty> _onWaitingAllPlayerConnected;
        [SerializeField] private EventChannel<Empty> _onAllPlayerConnected;
        [SerializeField] private EventChannel<float> _onGameClockTick;
        [SerializeField] private EventChannel<Empty> _onGameFinished;

        public Action<List<PlayerData>> OnPlayerDataUpdated;
        public List<PlayerData> PlayerDataNetworkList { get; private set; }
        
        private bool _gameInProgress;

        private CountdownTimer _gameClock;

        protected override void Awake()
        {
            base.Awake();
            PlayerDataNetworkList = new List<PlayerData>();
            _onWaitingAllPlayerConnected.Invoke(new Empty());
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Add the host to the list.
                PlayerDataNetworkList.Add(CreatePlayerData());
                
                WaitForAllPlayers();
            }
            else
            {
                // Inform the server we enter the game.
                ClientEnterGameServerRpc(CreatePlayerData());
            }
        }

        private void Update()
        {
            if (!_gameInProgress)
                return;
            
            _gameClock.Tick(Time.deltaTime);
            if (_gameClock.IsRunning)
            {
                _onGameClockTick.Invoke(_gameClock.Time);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            PlayerDataNetworkList = null;
        }

        #region SERVER

        /// <summary>
        /// SERVER-SIDE
        /// The client inform the server that he is connected.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void ClientEnterGameServerRpc(PlayerData playerData)
        {
            PlayerDataNetworkList.Add(playerData);

            UpdatePlayerDataListClientRpc(PlayerDataNetworkList.ToArray());
        }
        
        /// <summary>
        /// SERVER
        /// Wait for all the player to be connected. Then instantiate the player prefabs and inform clients.
        /// </summary>
        private async void WaitForAllPlayers()
        {
            while (PlayerDataNetworkList.Count < MultiplayerConnectionManager.Instance.PlayerCount)
            {
                await Task.Delay(500);
            }

            SetUpPlayerIslands();

            // Security to be sure that everything is correctly initialized.
            await UniTask.WaitForEndOfFrame(this);
            AllClientConnectedClientRpc(MultiplayerConnectionManager.Instance.GameTime);
        }

        /// <summary>
        /// SERVER - SIDE
        /// Setup the player island with the correct id, if 
        /// </summary>
        private void SetUpPlayerIslands()
        {
            for (int i = 0; i < PlayerDataNetworkList.Count; i++)
            {
                _playerIslands[i].SetUpForPlayer(PlayerDataNetworkList[i].ClientId);
            }
            
            // Tell to all client to spawn the first boat
            for (int i = 0; i < PlayerDataNetworkList.Count; i++)
            {
                _playerIslands[i].SpawnFirstBoatClientRpc();
            }
        }
        
        /// <summary>
        /// SERVER - SIDE
        /// Raised at the end of the game clock. Inform all players that the game is finished.
        /// </summary>
        private void HandleEndOfGame()
        {
            _gameClock.OnTimerStop -= HandleEndOfGame;
            
            EndOfGameClientRpc();
        }

        
        /// <summary>
        /// SERVER - SIDE
        /// Increase a client money and inform other of the update.
        /// </summary>
        public void IncreasePlayerCurrency(ulong clientId, int moneyGained)
        {
            for (int i = 0; i < PlayerDataNetworkList.Count; i++)
            {
                if (PlayerDataNetworkList[i].ClientId == clientId)
                {
                    var clientData = PlayerDataNetworkList[i];
                    clientData.Money += (ushort)moneyGained;

                    PlayerDataNetworkList[i] = clientData;
                    
                    UpdatePlayerDataListClientRpc(PlayerDataNetworkList.ToArray());
                    
                    return;
                }
            }
            
            Debug.LogError($"Unable to find client with id: {clientId}");
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
            PlayerDataNetworkList = newPlayerData.ToList();
            OnPlayerDataUpdated?.Invoke(PlayerDataNetworkList);
        }

        /// <summary>
        /// CLIENT-SIDE
        /// The server inform that all the player are connected.
        /// </summary>
        [ClientRpc]
        private void AllClientConnectedClientRpc(int gameTime)
        {
            _onAllPlayerConnected.Invoke(new Empty());
            
            _gameClock = new CountdownTimer(gameTime);
            _gameClock.Start();

            _gameInProgress = true;

            if (IsHost)
            {
                _gameClock.OnTimerStop += HandleEndOfGame;
            }
        }
        
        /// <summary>
        /// CLIENT-SIDE
        /// The server inform that the game is finished.
        /// </summary>
        [ClientRpc]
        private void EndOfGameClientRpc()
        {
            _gameInProgress = false;
            _gameClock.Stop();
            
            _onGameFinished.Invoke(new Empty());
        }

        /// <summary>
        /// Return the player name from the client ID.
        /// </summary>
        public string GetPlayerNameFromId(ulong clientId)
        {
            for (int i = 0; i < PlayerDataNetworkList.Count; i++)
            {
                if (PlayerDataNetworkList[i].ClientId == clientId)
                {
                    return PlayerDataNetworkList[i].PlayerName.ToString();
                }
            }

            Debug.LogError($"Unable to find client with ID: {clientId}");
            return string.Empty;
        }

        #endregion CLIENT
    }
}