using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Items.Merchandise;
using VComponent.Ship;

public class PlayerUIShipsInfosController : MonoBehaviour
{
    [SerializeField] ShipInfoController _shipInfoControllerPrefab;
	[SerializeField] private Transform _parentTransform;


	/// <summary>
	/// On one of our ship is created called this function to spawn an UI shipInfoController and view 
	/// </summary>
	public void OnShipCreated(MultiplayerShipController shipController)
    {
		ShipInfoController shipInfoController = Instantiate(_shipInfoControllerPrefab, _parentTransform);
		shipInfoController.Init(shipController);
	}
}
