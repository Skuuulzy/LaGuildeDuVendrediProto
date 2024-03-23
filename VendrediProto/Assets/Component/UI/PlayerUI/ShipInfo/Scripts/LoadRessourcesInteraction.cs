using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;

public class LoadRessourcesInteraction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentAmountText;
	[SerializeField] private Slider _merchandiseSlider;
	[SerializeField] private Image _merchandiseImage;

	// Start is called before the first frame update
	public void Show(RessourcesSO merchandiseSO,int freeSpace)
	{
		this.gameObject.SetActive(true);
		_merchandiseSlider.maxValue = freeSpace;
		_merchandiseImage.sprite = merchandiseSO.Sprite;
	}

	public void Hide()
	{
		this.gameObject.SetActive(false);
	}

	public void SetSliderValue(float sliderValue)
	{
		_currentAmountText.text = sliderValue.ToString();
	}
}
