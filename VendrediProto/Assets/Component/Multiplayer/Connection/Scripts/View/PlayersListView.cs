using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using VComponent.InputSystem;

namespace VComponent.Multiplayer
{
    /// <summary>
    /// A view that control and update the UI of the current lobby players information
    /// </summary>
    public class PlayersListView : MonoBehaviour
    {
        [Header("Lobby")] 
        [SerializeField] private GameObject _lobbyWindow;
        [SerializeField] private TMP_Text _lobbyNameTxt;
        [SerializeField] private PlayerInLobbyView _playerInLobbyPlayerPrefab;
        [SerializeField] private Transform _lobbyPlayerContainer;

        private Dictionary<ulong, PlayerInLobbyView> _playerViews;

        private bool _initialized;
        
        public void StartUpdatingView()
        {
            _lobbyNameTxt.text = MultiplayerConnectionManager.Instance.GetLobbyName();
            
            // First setup
            CreatePlayerViews(MultiplayerGameplayManager.Instance.PlayerDataNetworkList);
            
            // Listening for updates
            MultiplayerGameplayManager.Instance.OnPlayerDataUpdated += UpdatePlayerViews;

            _initialized = true;
        }

        private void OnDestroy()
        {
            MultiplayerGameplayManager.Instance.OnPlayerDataUpdated -= UpdatePlayerViews;
        }

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            switch (InputsManager.Instance.ShowLobbyInformation)
            {
                case false when !_lobbyWindow.activeInHierarchy:
                    return;
                case false when _lobbyWindow.activeInHierarchy:
                    _lobbyWindow.SetActive(false);
                    break;
                default:
                    _lobbyWindow.SetActive(true);
                    break;
            }
        }

        private void CreatePlayerViews(List<PlayerData> allPlayerData)
        {
            ClearPlayerViews();
            _playerViews = new Dictionary<ulong, PlayerInLobbyView>();
            
            foreach (PlayerData player in allPlayerData)
            {
                PlayerInLobbyView playerInLobbyView = Instantiate(_playerInLobbyPlayerPrefab, _lobbyPlayerContainer);
                playerInLobbyView.SetPlayerData(player);

                _playerViews.Add(player.ClientId, playerInLobbyView);
            }
        }
        
        /// <summary>
        /// Update the player view information and sort the player view position by money.
        /// </summary>
        private void UpdatePlayerViews(List<PlayerData> allPlayerData)
        {
            // Sort the player data by currency amount
            var sortedPlayerData = allPlayerData.OrderByDescending(playerData => playerData.Money).ToList();

            for (int i = 0; i < sortedPlayerData.Count; i++)
            {
                if (i >= allPlayerData.Count)
                {
                    Debug.LogError("The count between player data and player view don't match !");
                    break;
                }
                
                var playerView = _playerViews[sortedPlayerData[i].ClientId];
                
                playerView.UpdatePlayerData(sortedPlayerData[i]);
                playerView.transform.SetSiblingIndex(i);
            }
        }

        private void ClearPlayerViews()
        {
            foreach (Transform child in _lobbyPlayerContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public void Quit()
        {
            MultiplayerConnectionManager.Instance.QuitNetwork();
            SceneManager.LoadScene(0);
        }
    }
}