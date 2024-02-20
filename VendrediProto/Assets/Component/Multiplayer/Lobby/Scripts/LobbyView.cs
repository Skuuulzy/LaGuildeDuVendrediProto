using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Component.Multiplayer
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
        [SerializeField] private LobbyListSingleUI _lobbySinglePrefab;
        [SerializeField] private Transform _container;

        [Header("Lobby Creation")] 
        [SerializeField] private GameObject _lobbyCreationWindow;
        [SerializeField] private TMP_InputField _lobbyNameInputField;

        [Header("Lobby")] 
        [SerializeField] private GameObject _lobbyWindow;
        [SerializeField] private TMP_Text _lobbyNameTxt;

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
        
        public void UpdateLobbyList(List<Lobby> lobbyList)
        {
            foreach (Lobby lobby in lobbyList)
            {
                var lobbyListSingleUI = Instantiate(_lobbySinglePrefab, _container);
                lobbyListSingleUI.gameObject.SetActive(true);
                lobbyListSingleUI.UpdateLobby(lobby);
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

        #endregion

        #region LOBBY

        public void ShowLobby(Lobby currentLobby)
        {
            _lobbyWindow.SetActive(true);
            _lobbyNameTxt.text = currentLobby.Name;
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