using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIIslandInfoController : MonoBehaviour
{
	[SerializeField] private PlayerUIIslandInfoView _islandInfoView;

	private IslandController _islandController;
	private ShipController _shipController;
	public void Init(IslandController islandController, ShipController shipController)
	{
		_islandController = islandController;
		islandController.OnUpdateMerchandise -= UpdateView;
		islandController.OnUpdateMerchandise += UpdateView;
		_islandController = islandController;
		_shipController = shipController;
		_islandInfoView.Init(islandController, shipController);
	}
	private void OnDestroy()
	{
		if (_islandController != null)
		{
			_islandController.OnUpdateMerchandise -= UpdateView;
		}
	}
	public void UpdateView()
	{
        if (_islandController == null || _shipController == null)
        {
			Debug.LogError("No IslandController or ShipController saved");
			return;
        }

        _islandInfoView.Init(_islandController, _shipController);
	}

	public void RemoveUpdateIslandListener()
	{
		if (_islandController == null )
		{
			Debug.LogError("No IslandController saved");
			return;
		}

		_islandController.OnUpdateMerchandise -= UpdateView;
		_islandController = null;
		_shipController = null;
	}
	public void SellMerchandise()
	{
		//_shipController.
		//Update Island Asked Merchandise

		//Update view

		//Update Ship Merchandise (selled merchandise)
	}

}
