using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace VComponent.Multiplayer
{
    public class LobbyView : MonoBehaviour
    {
        [Header("Authentication")]
        [SerializeField] private GameObject _authenticationWindow;
        [SerializeField] private TMP_InputField _playerNameInputField;

        [Header("Loading")]
        [SerializeField] private GameObject _loadingWindow;

        [Header("Lobby List")] 
        [SerializeField] private GameObject _lobbyListWindow;
        [SerializeField] private LobbyListSingleView _lobbySinglePrefab;
        [SerializeField] private Transform _lobbyListContainer;

        [Header("Lobby Creation")] 
        [SerializeField] private GameObject _lobbyCreationWindow;
        [SerializeField] private TMP_InputField _lobbyNameInputField;

        [Header("Lobby")] 
        [SerializeField] private GameObject _lobbyWindow;
        [SerializeField] private TMP_Text _lobbyNameTxt;
        [SerializeField] private TMP_Text _lobbyPlayerCountTxt;
        [SerializeField] private PlayerInLobbyView _playerInLobbyPlayerPrefab;
        [SerializeField] private Transform _lobbyPlayerContainer;
        [SerializeField] private Button _launchGameBtn;

        #region MONO

        public void Init()
        {
            // Security
            _authenticationWindow.SetActive(false);
            _loadingWindow.SetActive(false);
            _lobbyListWindow.SetActive(false);
            _lobbyCreationWindow.SetActive(false);
            _loadingWindow.SetActive(false);
            
            _launchGameBtn.interactable = false;
        }

        #endregion

        #region AUTHENTICATION

        public void ShowAuthenticationWindow(string playerName)
        {
            _authenticationWindow.SetActive(true);
            _playerNameInputField.text = playerName;
        }

        #endregion AUTHENTICATION

        #region LOBBY LIST

        public void ShowLobbyList(bool show)
        {
            _lobbyListWindow.SetActive(show);
        }
        
        public void UpdateLobbyList(List<Lobby> lobbyList, MultiplayerManager multiplayerManager)
        {
            ClearLobbyList();
            
            foreach (Lobby lobby in lobbyList)
            {
                var lobbyListSingleUI = Instantiate(_lobbySinglePrefab, _lobbyListContainer);
                lobbyListSingleUI.gameObject.SetActive(true);
                lobbyListSingleUI.UpdateLobby(lobby, multiplayerManager);
            }
        }
        
        private void ClearLobbyList()
        {
            foreach (Transform child in _lobbyListContainer)
            {
                Destroy(child.gameObject);
            }
        }

        #endregion LOBBY LIST

        #region LOBBY CREATION

        public void OpenLobbyCreationWindow(string lobbyName)
        {
            ShowLobbyList(false);
            
            _lobbyCreationWindow.gameObject.SetActive(true);
            _lobbyNameInputField.text = lobbyName;
        }

        public void CloseLobbyCreationWindow()
        {
            _lobbyCreationWindow.gameObject.SetActive(false);
        }

        #endregion

        #region LOBBY

        public void ShowCurrentLobby(Lobby lobby)
        {
            _lobbyWindow.SetActive(true);
            _lobbyNameTxt.text = lobby.Name;
        }

        public void HideCurrentLobby()
        {
            _lobbyWindow.SetActive(false);
            ClearPlayerInLobby();
        }

        public void UpdateLobby(Lobby lobby, bool isHost, MultiplayerManager multiplayerManager, int minPlayerCountToPlay)
        {
            ClearPlayerInLobby();

            foreach (Player player in lobby.Players)
            {
                PlayerInLobbyView playerInLobbyPlayer = Instantiate(_playerInLobbyPlayerPrefab, _lobbyPlayerContainer);
                
                // Kick button behaviour
                playerInLobbyPlayer.SetKickPlayerButtonVisible(
                    isHost && // Allow only the host to kick players
                    player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
                );
                
                playerInLobbyPlayer.UpdatePlayer(player, multiplayerManager);
            }

            _lobbyPlayerCountTxt.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";

            if (isHost && lobby.Players.Count >= minPlayerCountToPlay)
            {
                _launchGameBtn.interactable = true;
            }
        }

        private void ClearPlayerInLobby()
        {
            foreach (Transform child in _lobbyPlayerContainer)
            {
                Destroy(child.gameObject);
            }
        }

        #endregion
        
        #region LOADING

        public void ShowLoading(bool show)
        {
            _loadingWindow.SetActive(show);
        }

        #endregion LOADING
    }
}