using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventorySO : MonoBehaviour
{
    [SerializeField] private int _playerMoney;

	public Action<int> OnMoneyUpdated;
	public int PlayerMoney => _playerMoney;

    public void IncreasePlayerMoney(int money)
    {
        _playerMoney += money;
		OnMoneyUpdated?.Invoke(money);
	}

    public void DecreasePlayerMoney(int money)
    {
        _playerMoney -= money;
		OnMoneyUpdated?.Invoke(money);
	}
}
