using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	[SerializeField] private PlayerUIIslandCommerceController _playerUIIslandCommerceController;
	private MerchandiseType _currentMerchandiseCarriedType;
	private int _currentMerchandiseCarriedNumber;

	public MerchandiseType CurrentMerchandiseCarriedType => _currentMerchandiseCarriedType;
	public int CurrentMerchandiseCarriedNumber => _currentMerchandiseCarriedNumber;

	#region INTERACTIONS
	private void OnTriggerEnter(Collider other)
	{
		IslandController islandController = other.gameObject.GetComponent<IslandController>();
		if (islandController != null)
		{
			_playerUIIslandCommerceController.SetPlayerUIIslandInfo(islandController, this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		IslandController islandController = other.gameObject.GetComponent<IslandController>();
		if (islandController != null)
		{
			_playerUIIslandCommerceController.CloseIslandDetailUI(islandController);
		}
	}
	#endregion INTERACTIONS
}
