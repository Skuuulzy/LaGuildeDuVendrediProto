using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIIslandCommerceController : MonoBehaviour
{
	[SerializeField] private PlayerUIIslandInfoController _playerUIIslandInfoControllerPrefab;
	[SerializeField] private Transform _parentTransform;

	private List<PlayerUIIslandInfoController> _playerUIIslandInfoControllersList;

	private void Start()
	{
		_playerUIIslandInfoControllersList = new List<PlayerUIIslandInfoController>();
	}
	public PlayerUIIslandInfoController SetPlayerUIIslandInfo(IslandController islandController, ShipController shipController)
	{
		PlayerUIIslandInfoController playerUIIslandInfoController = Instantiate(_playerUIIslandInfoControllerPrefab, _parentTransform);
		playerUIIslandInfoController.Init(islandController, shipController);
		_playerUIIslandInfoControllersList.Add(playerUIIslandInfoController);
		return playerUIIslandInfoController;
	}

	public void CloseIslandDetailUI(PlayerUIIslandInfoController playerUIIslandInfoController)
	{
		for(int i = 0; i <  _playerUIIslandInfoControllersList.Count; i++)
		{
			if (_playerUIIslandInfoControllersList[i] == playerUIIslandInfoController)
			{
				playerUIIslandInfoController.RemoveUpdateIslandListener();
				PlayerUIIslandInfoController playerUItemp = _playerUIIslandInfoControllersList[i];
				_playerUIIslandInfoControllersList.Remove(playerUIIslandInfoController);
				Destroy(playerUItemp.gameObject);
			}
		}
	}
}
