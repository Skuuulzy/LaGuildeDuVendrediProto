using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

namespace Component.Multiplayer
{
    public class LobbyListSingleView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI playersText;

        private Lobby _lobby;
        private MultiplayerManager _multiplayerManager;

        public void UpdateLobby(Lobby lobby, MultiplayerManager multiplayerManager)
        {
            _lobby = lobby;
            _multiplayerManager = multiplayerManager;
            
            lobbyNameText.text = lobby.Name;
            playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        }

        public void JoinLobby()
        {
            _multiplayerManager.JoinLobby(_lobby);
        }
    }
}