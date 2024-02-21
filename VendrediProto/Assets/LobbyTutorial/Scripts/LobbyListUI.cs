using System;
using System.Collections.Generic;
using Component.Multiplayer;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListUI : MonoBehaviour
{
    [SerializeField] private MultiplayerManager _multiplayerManager;
    [SerializeField] private LobbyListSingleView _lobbySingleTemplate;
    [SerializeField] private Transform _container;
    
    private void Awake()
    {
        _lobbySingleTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        //_multiplayerManager.RefreshLobbyList();
        
        //_multiplayerManager.OnLobbyListUpdated += UpdateLobbyList;
        //LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        //LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        //LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
    }

    private void OnDestroy()
    {
        //_multiplayerManager.OnLobbyListUpdated += UpdateLobbyList;
    }

    private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Hide();
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in _container)
        {
            if (child == _lobbySingleTemplate.transform) continue;

            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            var lobbyListSingleUI = Instantiate(_lobbySingleTemplate, _container);
            lobbyListSingleUI.gameObject.SetActive(true);
            //lobbyListSingleUI.UpdateLobby(lobby);
        }
    }

    private void RefreshButtonClick()
    {
        LobbyManager.Instance.RefreshLobbyList();
    }

    private void CreateLobbyButtonClick()
    {
        LobbyCreateUI.Instance.Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}