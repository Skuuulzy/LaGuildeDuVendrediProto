using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

namespace VComponent.Multiplayer
{
    public class PlayerInLobbyView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private Button _kickPlayerButton;

        private Player _player;

        public void SetPlayer(Player player, bool allowKick)
        {
            _player = player;
            
            _playerNameText.text = player.Data[MultiplayerConnectionManager.KEY_PLAYER_NAME].Value;
            
            _kickPlayerButton.gameObject.SetActive(allowKick);
        }

        public void KickPlayer()
        {
            if (_player != null)
            {
                MultiplayerConnectionManager.Instance.KickPlayerFromLobby(_player.Id);
            }
        }
    }
}