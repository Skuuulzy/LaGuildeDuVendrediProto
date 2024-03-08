using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Tools.EventSystem;

public class ShipController : MonoBehaviour
{
	[SerializeField] private PlayerDataSO _playerDataSO;
	[SerializeField] private PlayerUIIslandCommerceController _playerUIIslandCommerceController;
	[SerializeField] private MerchandiseType _currentMerchandiseCarriedType;
	[SerializeField] private int _currentMerchandiseCarriedNumber;


	private PlayerUIIslandInfoController _playerUIIslandInfoController;
	public Action<IslandController,ShipController> EnterIslandArea; 
	public Action<ShipController> LeaveIslandArea; 
	public Action<int> OnPlayerSaleMerchandise; 
	public MerchandiseType CurrentMerchandiseCarriedType => _currentMerchandiseCarriedType;
	public int CurrentMerchandiseCarriedNumber => _currentMerchandiseCarriedNumber;

	#region INTERACTIONS
	private void OnTriggerEnter(Collider other)
	{
		IslandController islandController = other.gameObject.GetComponent<IslandController>();
		if (islandController != null)
		{
			EnterIslandArea?.Invoke(islandController, this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		IslandController islandController = other.gameObject.GetComponent<IslandController>();
		if (islandController != null)
		{
			LeaveIslandArea?.Invoke(this);
		}
	}

	public void SellMerchandise(int sellPrice)
	{
		_playerDataSO.PlayerInventory.IncreasePlayerMoney(sellPrice);
		_currentMerchandiseCarriedType = MerchandiseType.NONE;
		_currentMerchandiseCarriedNumber = 0;
	}
	#endregion INTERACTIONS
}
