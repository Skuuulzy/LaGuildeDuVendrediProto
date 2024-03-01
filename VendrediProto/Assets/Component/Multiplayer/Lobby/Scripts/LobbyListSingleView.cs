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
        private LobbiesListView _lobbiesListView;

        public void SetLobbyView(Lobby lobby, LobbiesListView lobbiesListView)
        {
            _lobby = lobby;
            _lobbiesListView = lobbiesListView;
            
            _lobbyNameText.text = lobby.Name;
            _playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        }

        public void JoinLobby()
        {
            _lobbiesListView.ShowLoading(true);
            MultiplayerManager.Instance.JoinLobby(_lobby);
        }
    }
}