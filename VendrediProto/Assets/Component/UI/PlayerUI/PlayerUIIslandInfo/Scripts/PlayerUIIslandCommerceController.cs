using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VComponent.Island;
using VComponent.Items.Merchandise;
using VComponent.Multiplayer.Deliveries;

public class PlayerUIIslandCommerceController : MonoBehaviour
{
	[SerializeField] private PlayerDataSO _playerDataSO;
	[SerializeField] private PlayerUIIslandInfoView _playerUIIslandInfoViewPrefab;
	[SerializeField] private Transform _parentTransform;

	private List<MultiplayerIslandController> _islandControllerList;
	private Dictionary<ShipController,PlayerUIIslandInfoController> _playerUIIslandInfoControllersDictionary;

	private void Start()
	{
		_islandControllerList = FindObjectsOfType<MultiplayerIslandController>().ToList();
		LinkToIslandEvent();


		//_playerUIIslandInfoControllersDictionary = new Dictionary<ShipController, PlayerUIIslandInfoController>();
		//LinkToShipEvent();
		//_playerDataSO.OnShipAdded += LinkToShipEvent;
	}

	private void LinkToIslandEvent()
	{
		DeliveryManager.OnDeliveryCreated += SetDeliveryInfos;
	}

	//private void LinkToShipEvent()
	//{
	//	foreach(ShipController shipController in _playerDataSO.ShipControllersList)
	//	{
	//		shipController.EnterIslandArea -= SetPlayerUIIslandInfo;
	//		shipController.LeaveIslandArea -= CloseIslandDetailUI;

	//		shipController.EnterIslandArea += SetPlayerUIIslandInfo;
	//		shipController.LeaveIslandArea += CloseIslandDetailUI;
	//	}
	//}

	private void SetDeliveryInfos(Delivery delivery)
	{
		PlayerUIIslandInfoView playerUIIslandInfoController = Instantiate(_playerUIIslandInfoViewPrefab, _parentTransform);
		playerUIIslandInfoController.Init(delivery);
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
				return;
			}
		}
	}
}
