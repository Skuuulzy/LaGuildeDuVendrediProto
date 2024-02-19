using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Component.Multiplayer
{
    public class EditPlayerName : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;

        private string _playerName = "Code Monkey";

        private void Start()
        {
            // Get the name or generate a random one.
            //_playerName = PlayerPrefs.HasKey(MULTIPLAYER_ID_KEY) ? PlayerPrefs.GetString(MULTIPLAYER_ID_KEY) : $"Player + {Random.Range(0, 1000)}";

            playerNameText.text = _playerName;

            //MultiplayerManager.Instance.PlayerName = _playerName;
        }

        
    }
}