using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectV/Island")]
public class IslandSO : ScriptableObject
{
    [SerializeField] private string _islandName;
    [SerializeField] private TribesType _tribesType;
    [SerializeField] private SerializableDictionary<MerchandiseType, int> _merchandisesToTake;
    [SerializeField] private SerializableDictionary<MerchandiseType, int> _merchandisesRequested;

    public string IslandName => _islandName;
    public TribesType TribesType => _tribesType;
    public SerializableDictionary<MerchandiseType, int> MerchandisesToTake => _merchandisesToTake;
    public SerializableDictionary<MerchandiseType, int> MerchandisesRequested => _merchandisesRequested;
}
