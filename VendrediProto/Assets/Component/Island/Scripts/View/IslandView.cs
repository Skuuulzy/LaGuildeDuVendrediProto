using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;

public class IslandView : MonoBehaviour
{
	[Header("MerchandiseRequested")]
	[SerializeField] private GameObject _merchandiseRequestedGO;
	[SerializeField] private Image _merchandiseRequestedImage;
	[SerializeField] private TextMeshProUGUI _merchandiseRequestedText;

	[Header("NeededAssets")]
	//Change Later with a SO
	[SerializeField] private MerchandiseListSO _allMerchandiseList;
	public void DisplayMerchandiseAsked(MerchandiseType currentMerchandiseAsked, int currentMerchandiseAskedValue)
	{
		_merchandiseRequestedGO.SetActive(true);
		_merchandiseRequestedImage.sprite = _allMerchandiseList.GetMerchandiseByType(currentMerchandiseAsked).Sprite;
		_merchandiseRequestedText.text = currentMerchandiseAskedValue.ToString();
	}
}
