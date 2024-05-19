using UnityEngine;
using VComponent.CameraSystem;
using VComponent.Ship;

public class PlayerShipsViewController : MonoBehaviour
{
    [SerializeField] ShipInfoView _shipView;
    [SerializeField] private CameraController _cameraController;

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
    private void HandleShipCreated(PlayerShipController shipController)
    {
	    ShipInfoView shipView = Instantiate(_shipView, transform);
	    shipView.Init(shipController, _cameraController);
    }

}