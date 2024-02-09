using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    [SerializeField] public int _nodeSizeX;
    [SerializeField] public int _nodeSizeZ;
    [SerializeField] private float _xPosition;
    [SerializeField] private float _zPosition;

    //Debug
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _offsetColor;
    [SerializeField] private Material _material;
    [SerializeField] private Material _baseMaterial;
    [SerializeField] private Material _offsetMaterial;

    private const float _sizeRef = 10f;
    public void Init(float xPosition, float zPosition, bool isOffset)
    {
        _xPosition = xPosition;
        _zPosition = zPosition;
        float calculatedSizeX = _nodeSizeX / _sizeRef;
        float calculatedSizeZ = _nodeSizeZ / _sizeRef;
        gameObject.transform.localScale = new Vector3(calculatedSizeX, 1, calculatedSizeZ);
        _material = isOffset ? _offsetMaterial : _baseMaterial;
    }
}
