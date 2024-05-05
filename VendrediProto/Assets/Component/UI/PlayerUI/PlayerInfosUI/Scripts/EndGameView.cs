using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using VComponent.Multiplayer;

public class EndGameView : MonoBehaviour
{
    [SerializeField] private GameObject _windows;
    
    [SerializeField] private List<TMP_Text> _podiumText;
    [SerializeField] private GameObject _looserHeader;
    [SerializeField] private TMP_Text _looserText;

    public void ShowFinalResults()
    {
        _windows.SetActive(true);
        
        var allPlayerData = MultiplayerGameplayManager.Instance.PlayerDataNetworkList;
        var sortedPlayerData = allPlayerData.OrderByDescending(playerData => playerData.Money).ToList();

        if (allPlayerData.Count == 1)
        {
            _podiumText[0].text = $"1. {allPlayerData[0].PlayerName}";
            _podiumText[0].gameObject.SetActive(true);

            return;
        }

        var looser = sortedPlayerData.Last();
        sortedPlayerData.Remove(looser);
        
        for (int i = 0; i < sortedPlayerData.Count; i++)
        {
            _podiumText[i].text = $"{i}. {allPlayerData[i].PlayerName}";
            _podiumText[i].gameObject.SetActive(true);
        }

        _looserText.text = $"{looser.PlayerName}";
        _looserText.gameObject.SetActive(true);
        _looserHeader.SetActive(true);
    }

    public void QuitEndGame()
    {
        MultiplayerConnectionManager.Instance.QuitNetwork();
    }
}