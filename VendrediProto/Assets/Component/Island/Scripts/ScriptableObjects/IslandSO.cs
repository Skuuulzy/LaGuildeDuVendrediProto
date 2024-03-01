using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectV/Island")]
public class IslandSO : ScriptableObject
{
    [SerializeField] private string _islandName;
    [SerializeField] private FactionTypes _factionType;
    [SerializeField] private SerializableDictionary<MerchandiseType, int> _merchandisesToSell;
    [SerializeField] private SerializableDictionary<MerchandiseType, int> _merchandisesRequested;
    [SerializeField] private float _merchandiseRequestedTimeInterval = 220f;
    [SerializeField] private float _merchandiseToTakeTimeInterval = 180f;

    public string IslandName => _islandName;
    public FactionTypes FactionType => _factionType;
    public SerializableDictionary<MerchandiseType, int> MerchandisesToSell => _merchandisesToSell;
    public SerializableDictionary<MerchandiseType, int> MerchandisesRequested => _merchandisesRequested;
    public float MerchandiseRequestedTimeInterval => _merchandiseRequestedTimeInterval;
    public float MerchandiseToSellTimeInterval => _merchandiseToTakeTimeInterval;
}
