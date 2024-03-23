using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;
using VComponent.Ship;

public class ShipInfoController : MonoBehaviour
{
    [SerializeField] private ShipInfoView _ship�nfoView;
    [SerializeField] private LoadRessourcesInteraction _loadRessourcesInteraction;
    [SerializeField] private MultiplayerShipController _multiplayerShipController;

    public void Init(MultiplayerShipController shipController)
    {
		_multiplayerShipController = shipController;
        _multiplayerShipController.OnResourceIslandDocked += OnRessourcesIslandDocked;
	}


    public void OnRessourcesIslandDocked(RessourcesIslandSO islandSO)
    {
        _loadRessourcesInteraction.Show(islandSO.MerchandisesToSell, _multiplayerShipController.GetFreeSpace());
    }
}
