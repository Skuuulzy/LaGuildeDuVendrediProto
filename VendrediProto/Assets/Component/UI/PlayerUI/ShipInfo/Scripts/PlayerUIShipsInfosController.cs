using System;
using UnityEngine;
using VComponent.Ship;

public class PlayerUIShipsInfosController : MonoBehaviour
{
    [SerializeField] ShipInfoController _shipInfoControllerPrefab;

    private void Awake()
    {
	    PlayerIslandController.OnShipAddedToFleet += HandleShipCreated;
    }

    /// <summary>
	/// On one of our ship is created called this function to spawn an UI shipInfoController and view 
	/// </summary>
	public void HandleShipCreated(MultiplayerShipController shipController)
    {
		ShipInfoController shipInfoController = Instantiate(_shipInfoControllerPrefab, transform);
		shipInfoController.Init(shipController);
	}
}