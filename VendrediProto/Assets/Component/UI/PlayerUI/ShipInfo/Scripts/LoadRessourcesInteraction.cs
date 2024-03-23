using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;

public class LoadRessourcesInteraction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentAmountText;
	[SerializeField] private Slider _ressourceSlider;
	[SerializeField] private Image _ressourceImage;

	private RessourcesSO _ressourceSO;
	public Slider RessourceSlider => _ressourceSlider;
	public RessourcesSO RessourcesSO => _ressourceSO;

	// Start is called before the first frame update
	public void Show(RessourcesSO ressourceSO,int freeSpace)
	{
		this.gameObject.SetActive(true);
		_ressourceSO = ressourceSO;
		_ressourceSlider.maxValue = freeSpace;
		_ressourceImage.sprite = ressourceSO.Sprite;
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
