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
    [SerializeField] private Image _merchandiseAskedImage;
    [SerializeField] private TextMeshProUGUI _merchandiseAskedText;
	[SerializeField] private Button _sellButton;
    [SerializeField] private TextMeshProUGUI _sellButtonText;

	[Header("Data")]
	[SerializeField] private MerchandiseListSO _allMerchandise;
	
	public void Init(Delivery delivery)
	{
		_islandName.text = delivery.Buyer.IslandData.IslandName;
		MerchandiseSO merchandiseSO = _allMerchandise.GetMerchandiseByType(delivery.Data.Merchandise);
		_merchandiseAskedImage.sprite = merchandiseSO.Sprite;
		_merchandiseAskedText.text =  delivery.Data.MerchandiseDesiredAmount.ToString();
		_sellButton.gameObject.SetActive(false);
	}
}
