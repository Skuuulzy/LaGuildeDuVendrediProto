using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    [SerializeField] private Node _nodePrefab;


    private void Start()
    {
        GenerateGrid();
    }
    private void GenerateGrid()
    {
        for(int x = 0; x < _width; x += _nodePrefab._nodeSizeX)
        {
            for(int z = 0; z < _height; z+= _nodePrefab._nodeSizeZ)
            {
                Node spawnedNode = Instantiate(_nodePrefab, new Vector3(x, 0, z), Quaternion.identity);
                spawnedNode.name = $"Node {x} {z}";
            }
        }
    }

}
