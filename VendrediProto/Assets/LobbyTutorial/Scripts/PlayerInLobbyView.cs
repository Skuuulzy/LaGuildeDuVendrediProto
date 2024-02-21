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

        public void SetKickPlayerButtonVisible(bool visible)
        {
            kickPlayerButton.gameObject.SetActive(visible);
        }

        public void UpdatePlayer(Player player)
        {
            _player = player;
            playerNameText.text = player.Data[MultiplayerManager.KEY_PLAYER_NAME].Value;
        }

        public void KickPlayer()
        {
            if (_player != null)
            {
                LobbyManager.Instance.KickPlayer(_player.Id);
            }
        }
    }
}