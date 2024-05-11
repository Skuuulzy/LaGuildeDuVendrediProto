using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

namespace VComponent.Multiplayer
{
    public class PlayerInLobbyView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _playerMoneyText;

        private Player _player;
        
        public void SetPlayerData(PlayerData playerData)
        {
            _playerNameText.text = playerData.PlayerName.ToString();
            _playerMoneyText.text = "0000";
        }
        
        public void UpdatePlayerData(PlayerData playerData)
        {
            _playerMoneyText.text = playerData.Money.ToString("0000");
        }

        
        #region UN USED
        
        private void SetPlayer(Player player, bool allowKick)
        {
            _player = player;
            
            _playerNameText.text = player.Data[MultiplayerConnectionManager.KEY_PLAYER_NAME].Value;
        }

        private void KickPlayer()
        {
            if (_player != null)
            {
                MultiplayerConnectionManager.Instance.KickPlayerFromLobby(_player.Id);
            }
        }

        #endregion UN USED
    }
}