using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

namespace VComponent.Multiplayer
{
    public class LobbyListSingleView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private TextMeshProUGUI _playersText;

        private Lobby _lobby;

        public void SetLobbyView(Lobby lobby)
        {
            _lobby = lobby;
            
            _lobbyNameText.text = lobby.Name;
            _playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        }

        public void JoinLobby()
        {
            MultiplayerManager.Instance.JoinLobby(_lobby);
        }
    }
}