using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RessourcesIslandController : MonoBehaviour
{
    [SerializeField] private RessourcesIslandSO _islandData;

    public RessourcesIslandSO IslandData => _islandData;
    public void GetMerchandise(int value)
    {

    }
}
