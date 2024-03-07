using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	[SerializeField] private PlayerUIIslandCommerceController _playerUIIslandCommerceController;
	[SerializeField] private MerchandiseType _currentMerchandiseCarriedType;
	[SerializeField] private int _currentMerchandiseCarriedNumber;
	private PlayerUIIslandInfoController _playerUIIslandInfoController;

	public MerchandiseType CurrentMerchandiseCarriedType => _currentMerchandiseCarriedType;
	public int CurrentMerchandiseCarriedNumber => _currentMerchandiseCarriedNumber;
	

	#region INTERACTIONS
	private void OnTriggerEnter(Collider other)
	{
		IslandController islandController = other.gameObject.GetComponent<IslandController>();
		if (islandController != null)
		{
			 _playerUIIslandInfoController = _playerUIIslandCommerceController.SetPlayerUIIslandInfo(islandController, this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		IslandController islandController = other.gameObject.GetComponent<IslandController>();
		if (islandController != null)
		{
			_playerUIIslandCommerceController.CloseIslandDetailUI(_playerUIIslandInfoController);
		}
	}
	#endregion INTERACTIONS
}
