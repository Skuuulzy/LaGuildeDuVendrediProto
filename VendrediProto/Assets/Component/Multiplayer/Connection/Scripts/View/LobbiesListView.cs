using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace VComponent.Multiplayer
{
    /// <summary>
    /// A view that control the UI to authenticate then search and join a lobby or create a lobby.
    /// </summary>
    public class LobbiesListView : MonoBehaviour
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
        
        #region MONO

        private void Awake()
        {
            // Security
            _authenticationWindow.SetActive(false);
            _loadingWindow.SetActive(false);
            _lobbyListWindow.SetActive(false);
            _lobbyCreationWindow.SetActive(false);
            _loadingWindow.SetActive(false);
        }

        private void Start()
        {
            ShowAuthenticationWindow(MultiplayerConnectionManager.Instance.GetPlayerName());
        }

        #endregion

        #region AUTHENTICATION

        private void ShowAuthenticationWindow(string playerName)
        {
            _authenticationWindow.SetActive(true);
            _playerNameInputField.text = playerName;
        }

        #endregion AUTHENTICATION

        #region LOBBY LIST

        private void ShowLobbyList(bool show)
        {
            _lobbyListWindow.SetActive(show);
        }
        
        private void UpdateLobbyList(List<Lobby> lobbyList)
        {
            ClearLobbyList();

            if (lobbyList == null)
            {
                return;
            }
            
            foreach (Lobby lobby in lobbyList)
            {
                var lobbyListSingleUI = Instantiate(_lobbySinglePrefab, _lobbyListContainer);
                lobbyListSingleUI.gameObject.SetActive(true);
                lobbyListSingleUI.SetLobbyView(lobby, this);
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

        private void OpenLobbyCreationWindow(string lobbyName)
        {
            ShowLobbyList(false);
            
            _lobbyCreationWindow.gameObject.SetActive(true);
            _lobbyNameInputField.text = lobbyName;
        }

        #endregion
        
        #region LOADING

        public void ShowLoading(bool show)
        {
            _loadingWindow.SetActive(show);
        }

        #endregion LOADING
        
        #region PUBLIC UI METHODS
        
        public void UpdatePlayerName(string newName)
        {
            MultiplayerConnectionManager.Instance.UpdatePlayerName(newName);
        }
        
        public async void Authenticate()
        {
            ShowLoading(true);
            
            _authenticationWindow.SetActive(false);
            
            // Authenticate
            await MultiplayerConnectionManager.Instance.Authenticate();
            
            // Fetch available lobbies
            ShowLobbyList(true);
            UpdateLobbyList(await MultiplayerConnectionManager.Instance.FetchLobbies());
            
            ShowLoading(false);
        }
        
        public async void CreatePublicLobby()
        {
            ShowLoading(true);
            
            await MultiplayerConnectionManager.Instance.CreateLobby(true);
            
            ShowLoading(false);
        }

        public async void RefreshLobbyList()
        {
            ShowLoading(true);
            
            // Fetch available lobbies
            UpdateLobbyList(await MultiplayerConnectionManager.Instance.FetchLobbies());
            
            ShowLoading(false);
        }

        public void OpenLobbyCreation()
        {
            ShowLobbyList(false);
            OpenLobbyCreationWindow(MultiplayerConnectionManager.Instance.GetLobbyName());
        }

        public void UpdateLobbyName(string newName)
        {
            MultiplayerConnectionManager.Instance.UpdateLobbyName(newName);
        }

        #endregion PUBLIC UI METHODS
    }
}