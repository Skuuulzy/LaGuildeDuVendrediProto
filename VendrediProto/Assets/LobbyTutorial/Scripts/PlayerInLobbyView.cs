using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

namespace Component.Multiplayer
{
    public class PlayerInLobbyView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Button kickPlayerButton;

        private Player _player;
        private MultiplayerManager _multiplayerManager;

        public void SetKickPlayerButtonVisible(bool visible)
        {
            kickPlayerButton.gameObject.SetActive(visible);
        }

        public void UpdatePlayer(Player player, MultiplayerManager multiplayerManager)
        {
            _player = player;
            _multiplayerManager = multiplayerManager;
            
            playerNameText.text = player.Data[MultiplayerManager.KEY_PLAYER_NAME].Value;
        }

        public void KickPlayer()
        {
            if (_player != null)
            {
                _multiplayerManager.KickPlayerFromLobby(_player.Id);
            }
        }
    }
}