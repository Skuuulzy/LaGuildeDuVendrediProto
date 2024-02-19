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
        [SerializeField] private LobbyListSingleUI _lobbySingleTemplate;
        [SerializeField] private Transform _container;
        
        public void ShowAuthenticationWindow(string playerName)
        {
            _authenticationWindow.SetActive(true);
            _playerNameInputField.text = playerName;
        }

        public void ShowLoading(bool show)
        {
            _loadingWindow.SetActive(show);
        }

        public void ShowLobbyList()
        {
            _lobbyListWindow.SetActive(true);
        }
        
        public void UpdateLobbyList(List<Lobby> lobbyList)
        {
            foreach (Transform child in _container)
            {
                if (child == _lobbySingleTemplate.transform) continue;

                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbyList)
            {
                var lobbyListSingleUI = Instantiate(_lobbySingleTemplate, _container);
                lobbyListSingleUI.gameObject.SetActive(true);
                lobbyListSingleUI.UpdateLobby(lobby);
            }
        }
    }
}