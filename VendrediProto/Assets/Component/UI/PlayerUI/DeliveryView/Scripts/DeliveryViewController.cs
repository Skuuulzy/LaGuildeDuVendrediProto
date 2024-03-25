using UnityEngine;
using VComponent.Items.Merchandise;
using VComponent.Multiplayer.Deliveries;

public class DeliveryViewController : MonoBehaviour
{
	[SerializeField] private DeliveryView _deliveryViewPrefab;
	[SerializeField] private Transform _parentTransform;
	
	private void Start()
	{
		DeliveryManager.OnDeliveryCreated += HandleDeliveryCreated;
	}

	private void OnDestroy()
	{
		DeliveryManager.OnDeliveryCreated += HandleDeliveryCreated;
	}

	private void HandleDeliveryCreated(Delivery delivery)
	{
		DeliveryView deliveryController = Instantiate(_deliveryViewPrefab, _parentTransform);
		deliveryController.Init(delivery);
	}
}
