using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using VComponent.InputSystem;

namespace VComponent.Multiplayer
{
    /// <summary>
    /// A view that control and update the UI of the current lobby (player inside, ready status, ...)
    /// </summary>
    public class CurrentLobbyView : MonoBehaviour
    {
        [Header("Lobby")] 
        [SerializeField] private GameObject _lobbyWindow;
        [SerializeField] private TMP_Text _lobbyNameTxt;
        [SerializeField] private TMP_Text _lobbyPlayerCountTxt;
        [SerializeField] private PlayerInLobbyView _playerInLobbyPlayerPrefab;
        [SerializeField] private Transform _lobbyPlayerContainer;

        private bool _isHost;
        
        private void Start()
        {
            ShowCurrentLobby(MultiplayerConnectionManager.Instance.GetLobbyName());
            _isHost = MultiplayerConnectionManager.Instance.IsLobbyHost();

            MultiplayerConnectionManager.OnLobbyPolled += UpdateLobby;
        }

        private void OnDestroy()
        {
            MultiplayerConnectionManager.OnLobbyPolled -= UpdateLobby;
        }

        private void Update()
        {
            if (InputsManager.Instance.ShowLobbyInformation)
            {
                _lobbyWindow.SetActive(true);
            }
            else
            {
                _lobbyWindow.SetActive(false);
            }
        }

        private void ShowCurrentLobby(string lobbyName)
        {
            _lobbyWindow.SetActive(true);
            _lobbyNameTxt.text = lobbyName;
        }

        private void UpdateLobby(Lobby lobby)
        {
            ClearPlayerInLobby();
            
            Debug.Log($"Updating lobby UI: {lobby.Players.Count} players");
            _lobbyPlayerCountTxt.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";

            foreach (Player player in lobby.Players)
            {
                PlayerInLobbyView playerInLobbyPlayer = Instantiate(_playerInLobbyPlayerPrefab, _lobbyPlayerContainer);

                bool canKick = _isHost && player.Id != AuthenticationService.Instance.PlayerId; //Don't allow to kick itself

                playerInLobbyPlayer.SetPlayer(player, canKick);
            }
        }

        private void ClearPlayerInLobby()
        {
            foreach (Transform child in _lobbyPlayerContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public void Quit()
        {
            // The scene loader does nor exist anymore ...
            MultiplayerConnectionManager.Instance.QuitNetwork();
            SceneManager.LoadScene(0);
        }
    }
}