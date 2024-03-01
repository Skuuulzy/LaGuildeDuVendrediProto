using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIIslandInfoController : MonoBehaviour
{
	[SerializeField] private PlayerUIIslandInfoView _islandInfoView;

	private IslandController _islandController;
	public void Init(IslandController islandController, ShipController shipController)
	{
		_islandController = islandController;
		_islandInfoView.Init(islandController, shipController);
	}

	public void SellMerchandise()
	{
		//Update Island Asked Merchandise

		//Update view

		//Update Ship Merchandise (selled merchandise)
	}

}
