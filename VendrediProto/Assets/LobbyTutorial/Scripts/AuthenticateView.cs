using System;
using Component.Multiplayer;
using TMPro;
using UnityEngine;

public class AuthenticateView : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameTxt;
    [SerializeField] private MultiplayerManager _multiplayerManager;

    private void OnEnable()
    {
        _playerNameTxt.text = _multiplayerManager.PlayerName;
    }

    public void OpenEditNamePopup()
    {
        UI_InputWindow.Show_Static("Player Name", _multiplayerManager.PlayerName, 
            "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
            () =>
            {
                // Cancel
            },
            (string newName) =>
            {
                _multiplayerManager.UpdatePlayerName(newName);
                _playerNameTxt.text = newName;
            });
    }
}