using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private Vector2 _endPosition;
    [SerializeField] private Transform _camera;
    [SerializeField] private GameObject _gridContainer;

    [Header("Debug")]
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _offsetColor;

    public static Action<Node> OnNodeClicked;
    
    private void Start()
    {
        OnNodeClicked += HandleNodeClicked;
        GenerateGrid();
    }
    private void OnDestroy()
    {
        OnNodeClicked -= HandleNodeClicked;
    }
    private void GenerateGrid()
    {
        for(int x = 0; x < _width; x += (int)_nodePrefab._nodeSize.x)
        {
            for(int z = 0; z < _height; z+= (int)_nodePrefab._nodeSize.y)
            {
                Node spawnedNode = Instantiate(_nodePrefab, new Vector3(x, 0, z), Quaternion.identity, _gridContainer.transform);
                bool isOffset = (x%2 == 0 && z%2!=0) || (x % 2 != 0 && z % 2 == 0);
                spawnedNode.name = $"Node {x}-{z}";
                spawnedNode.GetComponent<Renderer>().material.color = isOffset ? _offsetColor : _baseColor;
                spawnedNode.Init(x, z);
            }
        }
        _camera.transform.position = new Vector3((float)_width / 2 - 0.5f, 100, (float)_height / 2 - 0.5f);
        _camera.transform.Rotate(new Vector3(90, 0, 0)); 
    }

    //Handlers
  
    public void HandleNodeClicked(Node node)
    {
        Vector2 pos = node.GetNodePosition();
        _endPosition = pos;
        Debug.Log("Manager " + pos);
    }

}
