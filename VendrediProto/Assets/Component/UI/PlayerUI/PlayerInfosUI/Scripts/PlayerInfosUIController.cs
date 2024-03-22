using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfosUIController : MonoBehaviour
{
    [SerializeField] PlayerDataSO _playerData;
    [SerializeField] private PlayerInfosUIView _playerInfosView;

	private void Start()
	{
		_playerData.PlayerInventory.OnMoneyUpdated -= SetGoldValue;
		_playerData.PlayerInventory.OnMoneyUpdated += SetGoldValue;
	}

	private void OnDestroy()
	{
		_playerData.PlayerInventory.OnMoneyUpdated -= SetGoldValue;
	}

	public void SetGoldValue(int goldValue)
    {
        _playerInfosView.SetGoldValue(goldValue);
    }
}
