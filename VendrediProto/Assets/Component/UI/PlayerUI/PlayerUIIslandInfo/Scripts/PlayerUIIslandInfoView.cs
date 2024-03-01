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
	//[SerializeField] private Image _merchandiseAskedImageSecond;
	//[SerializeField] private TextMeshProUGUI _merchandiseAskedTextsecond;

	[Header("Data")]
	[SerializeField] private MerchandiseSO _allMerchandise;
	public void Init(IslandController islandController, ShipController shipController)
	{
		_islandName.text = islandController.IslandSO.IslandName;
		MerchandiseData merchandiseType = _allMerchandise.GetMerchandiseData(islandController.CurrentMerchandiseAsked);
		_merchandiseAskedImageFirst.sprite = merchandiseType.MerchandiseSprite;

		int merchandiseCarriedNumber = shipController.CurrentMerchandiseCarriedType == merchandiseType.MerchandiseType ? shipController.CurrentMerchandiseCarriedNumber : 0;
		_merchandiseAskedTextFirst.text = merchandiseCarriedNumber + " / " + islandController.CurrentMerchandiseAskedValue;
		int sellPrice = merchandiseCarriedNumber * merchandiseType.SellValue;
		_sellButtonTextFirst.text = sellPrice + "$";

	}
	
}
