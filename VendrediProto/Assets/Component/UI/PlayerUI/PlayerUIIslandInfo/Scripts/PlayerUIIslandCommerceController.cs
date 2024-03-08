using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIIslandCommerceController : MonoBehaviour
{
	[SerializeField] private PlayerDataSO _playerDataSO;
	[SerializeField] private PlayerUIIslandInfoController _playerUIIslandInfoControllerPrefab;
	[SerializeField] private Transform _parentTransform;

	private Dictionary<ShipController,PlayerUIIslandInfoController> _playerUIIslandInfoControllersDictionary;

	private void Start()
	{
		_playerUIIslandInfoControllersDictionary = new Dictionary<ShipController, PlayerUIIslandInfoController>();
		LinkToShipEvent();
		_playerDataSO.OnShipAdded += LinkToShipEvent;
	}

	private void OnDestroy()
	{
		foreach (ShipController shipController in _playerDataSO.ShipControllersList)
		{
			shipController.EnterIslandArea -= SetPlayerUIIslandInfo;
			shipController.LeaveIslandArea -= CloseIslandDetailUI;
		}
	}

	private void LinkToShipEvent()
	{
		foreach(ShipController shipController in _playerDataSO.ShipControllersList)
		{
			shipController.EnterIslandArea -= SetPlayerUIIslandInfo;
			shipController.LeaveIslandArea -= CloseIslandDetailUI;

			shipController.EnterIslandArea += SetPlayerUIIslandInfo;
			shipController.LeaveIslandArea += CloseIslandDetailUI;
		}
	}

	public void SetPlayerUIIslandInfo(IslandController islandController, ShipController shipController)
	{
		PlayerUIIslandInfoController playerUIIslandInfoController = Instantiate(_playerUIIslandInfoControllerPrefab, _parentTransform);
		playerUIIslandInfoController.Init(islandController, shipController);
		_playerUIIslandInfoControllersDictionary.Add(shipController, playerUIIslandInfoController);
	}

	public void CloseIslandDetailUI(ShipController shipController)
	{
		foreach(KeyValuePair<ShipController, PlayerUIIslandInfoController> kvp in  _playerUIIslandInfoControllersDictionary)
		{
			if(kvp.Key == shipController)
			{
				PlayerUIIslandInfoController playerUItemp = kvp.Value;
				_playerUIIslandInfoControllersDictionary.Remove(kvp.Key);
				Destroy(playerUItemp.gameObject);
			}
		}
	}
}
