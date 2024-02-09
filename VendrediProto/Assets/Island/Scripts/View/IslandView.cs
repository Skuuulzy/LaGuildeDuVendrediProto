using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IslandView : MonoBehaviour
{
	[Header("MerchandiseRequested")]
	[SerializeField] private GameObject _merchandiseRequestedGO;
	[SerializeField] private Image _merchandiseRequestedImage;
	[SerializeField] private TextMeshProUGUI _merchandiseRequestedText;

	[Header("MerchandiseToTake")]
	[SerializeField] private GameObject _merchandiseToSellGO;
	[SerializeField] private Image _merchandiseToSellImage;
	[SerializeField] private TextMeshProUGUI _merchandiseToSellText;

	[Header("NeededAssets")]
	[SerializeField] private SerializableDictionary<MerchandiseType, Sprite> _spriteByMerchandiseType;
	public void DisplayMerchandiseAsked(MerchandiseType currentMerchandiseAsked, int currentMerchandiseAskedValue)
	{
		_merchandiseRequestedGO.SetActive(true);
		_merchandiseRequestedImage.sprite = _spriteByMerchandiseType[currentMerchandiseAsked];
		_merchandiseRequestedText.text = currentMerchandiseAskedValue.ToString();
	}

	public void DisplayMerchandiseToSell(MerchandiseType currentMerchandiseToSell, int currentMerchandiseToSellValue)
	{
		_merchandiseToSellGO.SetActive(true);
		_merchandiseToSellImage.sprite = _spriteByMerchandiseType[currentMerchandiseToSell];
		_merchandiseToSellText.text = currentMerchandiseToSellValue.ToString();
	}
}
