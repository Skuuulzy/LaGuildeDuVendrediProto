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

	private MultiplayerShipController _shipController;
	
	private RessourcesSO _resourceSO;

	private bool _initialized;

	public void Show(RessourcesSO resourceSO, MultiplayerShipController shipController)
	{
		gameObject.SetActive(true);

		_shipController = shipController;
		
		_resourceSO = resourceSO;
		
		_resourceSlider.maxValue = _shipController.GetFreeSpace();
		_resourceSlider.value = 0;
		
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
		
		_shipController.LoadResource(_resourceSO, (int)_resourceSlider.value);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void SetSliderValue(float sliderValue)
	{
		_currentAmountText.text = $"{sliderValue:000}";
	}
}