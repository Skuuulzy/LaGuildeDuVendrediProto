using UnityEngine;
using VComponent.Ship;

public class PlayerShipsViewController : MonoBehaviour
{
    [SerializeField] ShipInfoView _shipView;

    private void Awake()
    {
	    PlayerIslandController.OnShipAddedToFleet += HandleShipCreated;
    }

    private void OnDestroy()
    {
	    PlayerIslandController.OnShipAddedToFleet -= HandleShipCreated;
    }

    /// <summary>
    /// On one of our ship is created called this function to spawn an UI shipInfoController and view 
    /// </summary>
    private void HandleShipCreated(MultiplayerShipController shipController)
    {
	    ShipInfoView shipView = Instantiate(_shipView, transform);
	    shipView.Init(shipController);
    }
}