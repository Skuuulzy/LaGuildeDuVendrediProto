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
        //_playerNameTxt.text = _multiplayerManager.PlayerName;
    }
    
}