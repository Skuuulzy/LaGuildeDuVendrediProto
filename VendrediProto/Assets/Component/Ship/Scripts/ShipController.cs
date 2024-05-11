using System;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	[SerializeField] private PlayerDataSO _playerDataSO;
	[SerializeField] private ResourceType _currentMerchandiseCarriedType;
	[SerializeField] private int _currentMerchandiseCarriedNumber;
	
	public Action<IslandController,ShipController> EnterIslandArea; 
	public Action<ShipController> LeaveIslandArea; 
	public Action<int> OnPlayerSaleMerchandise; 
	public ResourceType CurrentMerchandiseCarriedType => _currentMerchandiseCarriedType;
	public int CurrentMerchandiseCarriedNumber => _currentMerchandiseCarriedNumber;


	private void Start()
	{
		_playerDataSO.AddShipToShipController(this);
	}

	private void OnDestroy()
	{
		_playerDataSO.ClearShipList();
	}

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
		_playerDataSO.PlayerInventory.IncreasePlayerMoney(sellPrice * _currentMerchandiseCarriedNumber);
		_currentMerchandiseCarriedType = ResourceType.NONE;
		_currentMerchandiseCarriedNumber = 0;
	}
	#endregion INTERACTIONS
}
