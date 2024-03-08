using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectV/PlayerInventory")]
public class PlayerInventorySO : ScriptableObject
{
    [SerializeField] private int _playerMoney;

	public Action<int> OnMoneyUpdated;
	public int PlayerMoney => _playerMoney;

    public void IncreasePlayerMoney(int money)
    {
        _playerMoney += money;
		OnMoneyUpdated?.Invoke(_playerMoney);
	}

    public void DecreasePlayerMoney(int money)
    {
        _playerMoney -= money;
		OnMoneyUpdated?.Invoke(_playerMoney);
	}
}
