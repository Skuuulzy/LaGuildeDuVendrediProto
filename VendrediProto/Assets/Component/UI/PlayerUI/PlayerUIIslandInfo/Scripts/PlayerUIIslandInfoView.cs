using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;

public class PlayerUIIslandInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _islandName;
    [SerializeField] private Image _merchandiseAskedImage;
    [SerializeField] private TextMeshProUGUI _merchandiseAskedText;
    [SerializeField] private TextMeshProUGUI _merchandiseAmountText;
	[SerializeField] private Button _sellButton;
    [SerializeField] private TextMeshProUGUI _sellButtonText;

	[Header("Data")]
	[SerializeField] private RessourcesListSO _allMerchandise;

	private Delivery _currentDelivery;
	private bool _buttonBind;
	
	public void Init(Delivery delivery)
	{
		_currentDelivery = delivery;
		
		_islandName.text = _currentDelivery.Buyer.IslandData.IslandName;
		RessourcesSO merchandiseSO = _allMerchandise.GetMerchandiseByType(_currentDelivery.Data.Resource);
		_merchandiseAskedImage.sprite = merchandiseSO.Sprite;
		
		// Security
		_sellButton.gameObject.SetActive(false);
		
		UpdateDeliveryInformations();

		_currentDelivery.OnDataUpdated += UpdateDeliveryInformations;
		_currentDelivery.OnExpired += MakeDeliveryExpired;
	}
	
	private void UpdateDeliveryInformations()
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
		_currentDelivery.Seller.SellMerchandiseToDockedIsland(_currentDelivery.Data.Resource);
	}
	
	private void MakeDeliveryExpired()
	{
		_currentDelivery.OnDataUpdated -= UpdateDeliveryInformations;
		_currentDelivery.OnExpired -= MakeDeliveryExpired;
		Destroy(gameObject);
	}
}
