using System.Linq;
using UnityEngine;
using VComponent.Items.Merchandise;

[CreateAssetMenu(menuName = "ProjectV/Island")]
public class IslandSO : ScriptableObject
{
    [SerializeField] private string _islandName;
    [SerializeField] private FactionTypes _factionType;
    [SerializeField] private SerializableDictionary<MerchandiseSO, ushort> _merchandisesToSell;
    [SerializeField] private SerializableDictionary<MerchandiseSO, ushort> _merchandisesRequested;
    [SerializeField] private uint _merchandiseRequestedTimeInterval = 220;
    [SerializeField] private uint _merchandiseToTakeTimeInterval = 180;

    public string IslandName => _islandName;
    public FactionTypes FactionType => _factionType;
    public SerializableDictionary<MerchandiseSO, ushort> MerchandisesToSell => _merchandisesToSell;
    public SerializableDictionary<MerchandiseSO, ushort> MerchandisesRequested => _merchandisesRequested;
    public float MerchandiseRequestedTimeInterval => _merchandiseRequestedTimeInterval;
    public float MerchandiseToSellTimeInterval => _merchandiseToTakeTimeInterval;

    public (MerchandiseType, ushort, uint) RequestRandomMerchandiseRequest()
    {
        int randomIndex = Random.Range(0, _merchandisesRequested.ToDictionary().Count);
        var randomRequest = _merchandisesRequested.ToDictionary().ElementAt(randomIndex);

        return (randomRequest.Key.Type, randomRequest.Value, _merchandiseRequestedTimeInterval);
    }
}
