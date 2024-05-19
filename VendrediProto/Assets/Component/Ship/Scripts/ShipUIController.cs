using TMPro;
using UnityEngine;
using VComponent.Multiplayer;

public class ShipUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameTxt;
    
    public void SetPlayerName(ulong playerID)
    {
        _playerNameTxt.text = MultiplayerGameplayManager.Instance.GetPlayerNameFromId(playerID);
    }
}