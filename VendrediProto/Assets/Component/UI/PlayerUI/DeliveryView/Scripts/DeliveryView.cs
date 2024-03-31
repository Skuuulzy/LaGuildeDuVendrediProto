using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;

public class DeliveryView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _islandName;
    [SerializeField] private Image _merchandiseAskedImage;
    [SerializeField] private TextMeshProUGUI _merchandiseAskedText;
	[SerializeField] private Button _sellButton;

	[Header("Data")]
	[SerializeField] private ResourcesListSO _allMerchandise;

	private Delivery _currentDelivery;
	private bool _buttonBind;
	
	public void Init(Delivery delivery)
	{
		_currentDelivery = delivery;
		
		_islandName.text = _currentDelivery.Buyer.IslandData.IslandName;
		ResourcesSO merchandiseSO = _allMerchandise.GetMerchandiseByType(_currentDelivery.Data.Resource);
		_merchandiseAskedImage.sprite = merchandiseSO.Sprite;
		
		// Security
		_sellButton.gameObject.SetActive(false);
		
		UpdateDeliveryInformation();

		_currentDelivery.OnDataUpdated += UpdateDeliveryInformation;
		_currentDelivery.OnExpired += MakeDeliveryExpired;
	}
	
	private void UpdateDeliveryInformation()
	{
		_merchandiseAskedText.text = $"{_currentDelivery.Data.MerchandiseCurrentAmount}/{_currentDelivery.Data.MerchandiseDesiredAmount}";
		BindButtonIfPossible();
	}

	/// <summary>
	/// Bind button listener to correct ship seller if there is one.
	/// </summary>
	private void BindButtonIfPossible()
	{
		if (!_buttonBind && _currentDelivery.HasSeller)
		{
			_buttonBind = true;
			
			_sellButton.gameObject.SetActive(true);
			_sellButton.onClick.AddListener(SellMerchandise);
		}
		else if (_buttonBind && !_currentDelivery.HasSeller)
		{
			_buttonBind = false;
			
			_sellButton.gameObject.SetActive(false);
			_sellButton.onClick.RemoveListener(SellMerchandise);
		}
	}

	private void SellMerchandise()
	{
		_currentDelivery.Seller.SellResourceToDockedIsland(_currentDelivery.Data.Resource);
	}
	
	private void MakeDeliveryExpired()
	{
		_currentDelivery.OnDataUpdated -= UpdateDeliveryInformation;
		_currentDelivery.OnExpired -= MakeDeliveryExpired;
		Destroy(gameObject);
	}
}
