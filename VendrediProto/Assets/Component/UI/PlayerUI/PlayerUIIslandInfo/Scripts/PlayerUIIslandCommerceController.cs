using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIIslandCommerceController : MonoBehaviour
{
	[SerializeField] private PlayerUIIslandInfoController _playerUIIslandInfoControllerPrefab;
	[SerializeField] private Transform _parentTransform;

	private Dictionary<IslandController ,PlayerUIIslandInfoController> _playerUIIslandInfoControllersDictionary;

	private void Start()
	{
		_playerUIIslandInfoControllersDictionary = new Dictionary<IslandController, PlayerUIIslandInfoController>();
	}
	public void SetPlayerUIIslandInfo(IslandController islandController, ShipController shipController)
	{
		PlayerUIIslandInfoController playerUIIslandInfoController = Instantiate(_playerUIIslandInfoControllerPrefab, _parentTransform);
		_playerUIIslandInfoControllerPrefab.Init(islandController, shipController);
	}

	public void CloseIslandDetailUI(IslandController islandController)
	{
		foreach(var controller in _playerUIIslandInfoControllersDictionary)
		{
			if(controller.Key == islandController)
			{
				Destroy(_playerUIIslandInfoControllersDictionary[controller.Key]);
				return;
			}
		}
	}
}
