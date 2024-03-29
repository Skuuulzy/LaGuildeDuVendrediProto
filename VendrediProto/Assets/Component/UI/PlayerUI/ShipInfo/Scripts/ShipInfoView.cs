using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VComponent.CameraSystem;
using VComponent.Items.Merchandise;
using VComponent.Ship;

public class ShipInfoView : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] private TextMeshProUGUI _shipNameText;
	[SerializeField] private TextMeshProUGUI _currentStateText;
	[SerializeField] private List<ShipResourcesCarriedView> _shipResourcesCarriedViews;
	[SerializeField] private LoadResourcesInteraction _loadResourcesInteraction;
	
	private MultiplayerShipController _shipController;
	private CameraController _cameraController;

	public void Init(MultiplayerShipController shipController, CameraController cameraController)
	{
		_shipController = shipController;
		_cameraController = cameraController;
		// Hiding UI since the ship is empty at start.
		_loadResourcesInteraction.Hide();
		foreach (var resourcesView in _shipResourcesCarriedViews)
		{
			resourcesView.Hide();
		}

		_shipController.OnResourceIslandDocked += HandleShipDockedToResourceIsland;
		_shipController.OnResourceAdded += HandleResourcesAdded;
		_shipController.OnResourceCarriedUpdated += HandleResourceCarriedUpdated;
		_shipController.OnResourceCarriedDelivered += HandleResourcesDelivered;
		_shipController.OnShipStateUpdated += HandleStateUpdated;

		HandleStateUpdated(ShipState.DEFAULT, "");
		SetCameraPositionToShipPosition();
	}

	private void OnDestroy()
	{
		_shipController.OnResourceIslandDocked -= HandleShipDockedToResourceIsland;
		_shipController.OnResourceAdded -= HandleResourcesAdded;
		_shipController.OnResourceCarriedUpdated -= HandleResourceCarriedUpdated;
		_shipController.OnResourceCarriedDelivered -= HandleResourcesDelivered;
		_shipController.OnShipStateUpdated -= HandleStateUpdated;

	}

	private void HandleStateUpdated(ShipState shipState, string islandName)
	{
		switch (shipState)
		{
			case ShipState.DEFAULT:
			case ShipState.IN_SEA:
				_currentStateText.text = $"Sailing on sea";
				break;
			case ShipState.DOCKED:
				_currentStateText.text = $"Docked to {islandName}";
				break;
			case ShipState.LOAD_RESOURCES:
				_currentStateText.text = $"Loading resources";
				break;
			case ShipState.SELL_RESOURCES:
				_currentStateText.text = $"Selling resources";
				break;
			case ShipState.ATTACKED:
				_currentStateText.text = $"Attacked by a ship";
				break;
		}
	}

	public void SetShipName(string shipName)
	{
		_shipNameText.text = shipName;
	}

	public void SetCurrentState(string stateText)
	{
		_currentStateText.text = stateText;
	}

	private void HandleShipDockedToResourceIsland(bool docked, RessourcesIslandSO resourcesIslandSO)
	{
		if (!docked)
		{
			_loadResourcesInteraction.Hide();
			
			return;
		}
		
		// We cannot load anything the ship is already full
		if (_shipController.GetFreeSpace() <= 0)
		{
			return;
		}
		
		_loadResourcesInteraction.Show(resourcesIslandSO.MerchandisesToSell, _shipController);
	}

	private void HandleResourcesAdded(RessourcesSO resourceType, int amount)
	{
		for (int i = 0; i < _shipResourcesCarriedViews.Count; i++)
		{
			if (_shipResourcesCarriedViews[i].CurrentResourceType != ResourceType.NONE) 
				continue;
			
			//Add a new shipResourceCarriedView
			_shipResourcesCarriedViews[i].Init(amount, resourceType);
			_loadResourcesInteraction.SetSliderMaxValue();
			return;
		}

		Debug.LogError("Can't add new resource");
	}

	private void HandleResourceCarriedUpdated(ResourceType resourceType, int amount)
	{
		//Search which ShipResourcesCarriedView display this ResourceType
		for (int i = 0; i < _shipResourcesCarriedViews.Count; i++)
		{
			if (_shipResourcesCarriedViews[i].CurrentResourceType != resourceType) 
				continue;
			
			//Update a current ship Resource Carried View
			_shipResourcesCarriedViews[i].UpdateNumberOfResourceCarried(amount);
			_loadResourcesInteraction.SetSliderMaxValue();
			return;
		}

		Debug.LogError("Try to update a resource which is not present in the ship");
	}
	
	private void HandleResourcesDelivered(ResourceType resourceType)
	{
		//Search which ShipResourcesCarriedView display this ResourceType
		for (int i = 0; i < _shipResourcesCarriedViews.Count; i++)
		{
			if (_shipResourcesCarriedViews[i].CurrentResourceType == resourceType)
			{
				_shipResourcesCarriedViews[i].Hide();
				return;
			}
		}

		Debug.LogError("Try to hide a resource which is not present in the ship");
	}

	#region CAMERA
	public void SetCameraPositionToShipPosition()
	{
		_cameraController.SetCameraPosition(new Vector3(_shipController.transform.position.x, _cameraController.transform.position.y, _shipController.transform.position.z));
	}
	#endregion CAMERA
}