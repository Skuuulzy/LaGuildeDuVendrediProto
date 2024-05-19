using System;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Ship;

[CreateAssetMenu(menuName ="ProjectV/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    [SerializeField] private PlayerInventorySO _playerInventory;
    [SerializeField] private List<PlayerShipController> _shipControllersList;
    public PlayerInventorySO PlayerInventory => _playerInventory;
    public List<PlayerShipController> ShipControllersList => _shipControllersList;
    public Action OnShipAdded;
    
    public void AddShipToShipController(PlayerShipController shipController)
    {
        if(_shipControllersList == null)
        {
			_shipControllersList = new List<PlayerShipController>();
		}

        _shipControllersList.Add(shipController);
        OnShipAdded?.Invoke();
    }

    public void ClearShipList()
    {
        _shipControllersList = null;
    }
}