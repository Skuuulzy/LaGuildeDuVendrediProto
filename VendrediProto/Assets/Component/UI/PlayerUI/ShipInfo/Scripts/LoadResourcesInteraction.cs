using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;
using VComponent.Ship;

public class LoadResourcesInteraction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentAmountText;
	[SerializeField] private Slider _resourceSlider;
	[SerializeField] private Image _resourceImage;
	[SerializeField] private GameObject _loadingGO;

	private PlayerShipController _shipController;
	
	private ResourcesSO _resourceSO;
	private int _ressourceStepValue = 50;
	private bool _initialized;

	public void Show(ResourcesSO resourceSO, PlayerShipController shipController)
	{
		gameObject.SetActive(true);
		_shipController = shipController;
		_resourceSO = resourceSO;
		SetSliderMaxValue();
		_resourceImage.sprite = resourceSO.Sprite;
		_initialized = true;
	}
	
	public void LoadSelectedResources()
	{
		if (!_initialized)
		{
			Debug.LogError("Try to load resources but the data has not been correctly initialized.");
			Hide();
			
			return;
		}
		
		_shipController.Resources.LoadResourceToShip(_resourceSO, (int)_resourceSlider.value);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void DisplayLoadingGO(bool value)
	{
		_loadingGO.SetActive(value);
	}

	public void SetSliderValue(float sliderValue)
	{
		//Go 50 by 50 for the loading
		float steppedValue = Mathf.Round(sliderValue / _ressourceStepValue) * _ressourceStepValue;
		if (Mathf.Approximately(steppedValue, sliderValue))
		{
			_resourceSlider.value = steppedValue;
		}
		_currentAmountText.text = $"{_resourceSlider.value:000}";
	}

	public void SetSliderMaxValue()
	{
		_resourceSlider.maxValue = _shipController.Resources.GetFreeSpace();
		_resourceSlider.value = 0;
		SetSliderValue(_resourceSlider.value);
	}
}