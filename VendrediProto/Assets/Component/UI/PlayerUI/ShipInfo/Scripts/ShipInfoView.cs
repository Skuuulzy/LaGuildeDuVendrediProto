using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Items.Merchandise;
using VComponent.Ship;

public class ShipInfoView : MonoBehaviour
{
	private MultiplayerShipController _multiplayerShipController;
	[SerializeField] private List<ShipRessourcesCarriedView> _shipRessourcesCarriedViews;

	public void Init(MultiplayerShipController shipController)
    {
		_multiplayerShipController = shipController;
		_multiplayerShipController.OnRessourceAdded += AddNewRessource;
		_multiplayerShipController.OnRessourceCarriedUpdated += UpdateExistedRessourceCarried;
		_multiplayerShipController.OnRessourceCarriedDelivered += HideCarrriedRessource;
	}

	private void OnDestroy()
	{
		_multiplayerShipController.OnRessourceAdded -= AddNewRessource;
		_multiplayerShipController.OnRessourceCarriedUpdated -= UpdateExistedRessourceCarried;
		_multiplayerShipController.OnRessourceCarriedDelivered -= HideCarrriedRessource;
	}

	public void AddNewRessource(RessourcesSO ressourceType, int amount)
	{
		//Search which ShipRessourcesCarriedView display this RessourceType
		for (int i = 0; i < _shipRessourcesCarriedViews.Count; i++)
		{
			if (_shipRessourcesCarriedViews[i].CurrentRessourceType == RessourceType.NONE)
			{
				//Add a new shipRessourceCarriedView
				_shipRessourcesCarriedViews[i].Init(amount, ressourceType);
				return;
			}
		}

		Debug.LogError("Can't add new ressource");
	}

	public void UpdateExistedRessourceCarried(RessourceType ressourceType, int amount)
	{
		//Search which ShipRessourcesCarriedView display this RessourceType
		for (int i = 0; i < _shipRessourcesCarriedViews.Count; i++)
		{
			if (_shipRessourcesCarriedViews[i].CurrentRessourceType == ressourceType)
			{
				//Update a current shipRessourceCarriedView
				_shipRessourcesCarriedViews[i].UpdateNumberOfRessourceCarried(amount);
				return;
			}
		}

		Debug.LogError("Try to update a ressource which is not present in the ship");
	}

	public void HideCarrriedRessource(RessourceType ressourceType)
	{
		//Search which ShipRessourcesCarriedView display this RessourceType
		for (int i = 0; i < _shipRessourcesCarriedViews.Count; i++)
		{
			if (_shipRessourcesCarriedViews[i].CurrentRessourceType == ressourceType)
			{
				_shipRessourcesCarriedViews[i].Hide();
				return;
			}
		}

		Debug.LogError("Try to hide a ressource which is not present in the ship");
	}

}
