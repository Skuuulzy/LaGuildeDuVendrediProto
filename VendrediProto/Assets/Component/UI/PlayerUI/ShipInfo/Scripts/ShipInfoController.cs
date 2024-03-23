using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;
using VComponent.Ship;

public class ShipInfoController : MonoBehaviour
{
    [SerializeField] private ShipInfoView _shipÎnfoView;
    [SerializeField] private LoadRessourcesInteraction _loadRessourcesInteraction;
    [SerializeField] private MultiplayerShipController _shipController;

    public void Init(MultiplayerShipController shipController)
    {
		_shipController = shipController;
        _shipController.OnResourceIslandDocked += OnRessourcesIslandDocked;
	}


    public void OnRessourcesIslandDocked(RessourcesIslandSO islandSO)
    {
        _loadRessourcesInteraction.Show(islandSO.MerchandisesToSell, _shipController.GetFreeSpace());
    }

    public void OnRessourceLoaded()
    {
        _shipController.LoadRessource(_loadRessourcesInteraction.RessourcesSO, (int)_loadRessourcesInteraction.RessourceSlider.value);
    }
}
