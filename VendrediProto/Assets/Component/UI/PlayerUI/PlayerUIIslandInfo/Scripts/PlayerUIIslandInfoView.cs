using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIIslandInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _islandName;
    [SerializeField] private Image _merchandiseAskedImageFirst;
    [SerializeField] private TextMeshProUGUI _merchandiseAskedTextFirst;
    [SerializeField] private TextMeshProUGUI _sellButtonTextFirst;


	public void Init(IslandController islandController, ShipController shipController, MerchandiseData merchandiseType, int merchandiseCarriedNumber, int sellPrice)
	{
		_islandName.text = islandController.IslandSO.IslandName;
		_merchandiseAskedImageFirst.sprite = merchandiseType.MerchandiseSprite;
		merchandiseCarriedNumber = shipController.CurrentMerchandiseCarriedType == merchandiseType.MerchandiseType ? shipController.CurrentMerchandiseCarriedNumber : 0;
		_merchandiseAskedTextFirst.text = merchandiseCarriedNumber + " / " + islandController.CurrentMerchandiseAskedValue;
		sellPrice = merchandiseCarriedNumber * merchandiseType.SellValue;
		_sellButtonTextFirst.text = sellPrice + "$";

	}
	
}
