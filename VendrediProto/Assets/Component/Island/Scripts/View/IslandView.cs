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

	[Header("NeededAssets")]
	//Change Later with a SO
	[SerializeField] private MerchandiseSO _allMerchandise;
	public void DisplayMerchandiseAsked(MerchandiseType currentMerchandiseAsked, int currentMerchandiseAskedValue)
	{
		_merchandiseRequestedGO.SetActive(true);
		_merchandiseRequestedImage.sprite = _allMerchandise.GetMerchandiseData(currentMerchandiseAsked).MerchandiseSprite;
		_merchandiseRequestedText.text = currentMerchandiseAskedValue.ToString();
	}
}
