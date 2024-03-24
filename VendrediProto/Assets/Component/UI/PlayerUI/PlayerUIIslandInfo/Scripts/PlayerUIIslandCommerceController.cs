using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VComponent.Island;
using VComponent.Items.Merchandise;
using VComponent.Multiplayer.Deliveries;

public class PlayerUIIslandCommerceController : MonoBehaviour
{
	[SerializeField] private PlayerDataSO _playerDataSO;
	[SerializeField] private PlayerUIIslandInfoView _playerUIIslandInfoViewPrefab;
	[SerializeField] private Transform _parentTransform;

	private List<MultiplayerFactionIslandController> _islandControllerList;
	private Dictionary<ShipController,PlayerUIIslandInfoController> _playerUIIslandInfoControllersDictionary;

	private void Start()
	{
		_islandControllerList = FindObjectsOfType<MultiplayerFactionIslandController>().ToList();
		LinkToIslandEvent();
	}

	private void LinkToIslandEvent()
	{
		DeliveryManager.OnDeliveryCreated += SetDeliveryInfos;
	}


	private void SetDeliveryInfos(Delivery delivery)
	{
		PlayerUIIslandInfoView playerUIIslandInfoController = Instantiate(_playerUIIslandInfoViewPrefab, _parentTransform);
		playerUIIslandInfoController.Init(delivery);
	}


	public void CloseIslandDetailUI(ShipController shipController)
	{
		foreach(KeyValuePair<ShipController, PlayerUIIslandInfoController> kvp in  _playerUIIslandInfoControllersDictionary)
		{
			if(kvp.Key == shipController)
			{
				PlayerUIIslandInfoController playerUItemp = kvp.Value;
				_playerUIIslandInfoControllersDictionary.Remove(kvp.Key);
				Destroy(playerUItemp.gameObject);
				return;
			}
		}
	}
}
