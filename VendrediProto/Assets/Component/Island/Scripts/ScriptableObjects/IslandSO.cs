using System.Linq;
using UnityEngine;
using VComponent.Items.Merchandise;

[CreateAssetMenu(menuName = "ProjectV/Island")]
public class IslandSO : ScriptableObject
{
    [SerializeField] private string _islandName;
    [SerializeField] private FactionTypes _factionType;
    [SerializeField] private SerializableDictionary<MerchandiseSO, int> _merchandisesToSell;
    [SerializeField] private SerializableDictionary<MerchandiseSO, int> _merchandisesRequested;
    [SerializeField] private float _merchandiseRequestedTimeInterval = 220f;
    [SerializeField] private float _merchandiseToTakeTimeInterval = 180f;

    public string IslandName => _islandName;
    public FactionTypes FactionType => _factionType;
    public SerializableDictionary<MerchandiseSO, int> MerchandisesToSell => _merchandisesToSell;
    public SerializableDictionary<MerchandiseSO, int> MerchandisesRequested => _merchandisesRequested;
    public float MerchandiseRequestedTimeInterval => _merchandiseRequestedTimeInterval;
    public float MerchandiseToSellTimeInterval => _merchandiseToTakeTimeInterval;

    public (MerchandiseType, int, int) RequestRandomMerchandiseRequest()
    {
        int randomIndex = Random.Range(0, _merchandisesRequested.ToDictionary().Count);
        var randomRequest = _merchandisesRequested.ToDictionary().ElementAt(randomIndex);

        return (randomRequest.Key.Type, randomRequest.Value, (int)_merchandiseRequestedTimeInterval);
    }
}
