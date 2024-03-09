using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;

public class PlayerUIIslandInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _islandName;
    [SerializeField] private Image _merchandiseAskedImageFirst;
    [SerializeField] private TextMeshProUGUI _merchandiseAskedTextFirst;
    [SerializeField] private TextMeshProUGUI _sellButtonTextFirst;


	public void Init(IslandController islandController, ShipController shipController, MerchandiseSO merchandiseData)
	{
		_islandName.text = islandController.IslandSO.IslandName;
		_merchandiseAskedImageFirst.sprite = merchandiseData.Sprite;
		int merchandiseCarriedNumber = shipController.CurrentMerchandiseCarriedType == merchandiseData.Type ? shipController.CurrentMerchandiseCarriedNumber : 0;
		_merchandiseAskedTextFirst.text = merchandiseCarriedNumber + " / " + islandController.CurrentMerchandiseAskedValue;
		int sellPrice = merchandiseCarriedNumber * merchandiseData.SellValue;
		_sellButtonTextFirst.text = sellPrice + "$";

	}
	
}
