using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TileGen/GenerationSettings")]
public class HexTileGenerationSettings : ScriptableObject
{
    public enum TileType
    {
        Standard,
        Water,
        Cliff
    }

    [SerializeField] private GameObject _standard;
	[SerializeField] private GameObject _water;
	[SerializeField] private GameObject _cliff;

    public GameObject GetTile(TileType type)
    {
        switch(type)
        {
            case TileType.Standard:
                return _standard;
            case TileType.Water:
                return _water;
            case TileType.Cliff:
                return _cliff;
        }
        return null;
    }
}
