using System.Linq;
using UnityEngine;
using VComponent.Items.Merchandise;

[CreateAssetMenu(menuName = "ProjectV/Island/FactionIslandSO")]
public class FactionIslandSO : IslandSO
{
    [SerializeField] private FactionTypes _factionType;
	[SerializeField] protected SerializableDictionary<ResourcesSO, ushort> _merchandisesRequested;
	[SerializeField] protected uint _merchandiseRequestedTimeInterval = 220;
	public FactionTypes FactionType => _factionType;
	

	public SerializableDictionary<ResourcesSO, ushort> MerchandisesRequested => _merchandisesRequested;
	public float MerchandiseRequestedTimeInterval => _merchandiseRequestedTimeInterval;

	public (ResourceType, ushort, uint) RequestRandomMerchandiseRequest()
	{
		int randomIndex = Random.Range(0, _merchandisesRequested.ToDictionary().Count);
		var randomRequest = _merchandisesRequested.ToDictionary().ElementAt(randomIndex);

		return (randomRequest.Key.Type, randomRequest.Value, _merchandiseRequestedTimeInterval);
	}

	public ushort GetResourceSellPrice(ResourceType type)
	{
		var resource = _merchandisesRequested.ToDictionary().Keys.SingleOrDefault(res => res.Type == type);
		
		if (resource == null)
		{
			Debug.LogError($"Unable to find the resource of type {type} to get is sell price.");
			return 0;
		}

		return (ushort)resource.SellValue;
	}
}