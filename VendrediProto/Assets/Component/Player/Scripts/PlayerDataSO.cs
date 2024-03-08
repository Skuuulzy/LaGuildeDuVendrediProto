using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataSO : ScriptableObject
{
    [SerializeField] private PlayerInventorySO _playerInventory;
    [SerializeField] private List<ShipController> _shipControllersList;
    public PlayerInventorySO PlayerInventory => _playerInventory;
    public List<ShipController> ShipControllersList => _shipControllersList;
    public Action OnShipAdded;
    public void AddShipToShipController(ShipController shipController)
    {
        if(_shipControllersList == null)
        {
			_shipControllersList = new List<ShipController>();
		}

        _shipControllersList.Add(shipController);
        OnShipAdded?.Invoke();
    }
}
