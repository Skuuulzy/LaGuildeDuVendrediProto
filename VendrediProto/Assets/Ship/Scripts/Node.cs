using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{


    [SerializeField] public Vector2 _nodeSize;
    [SerializeField] private Vector2 _nodePosition;
  

    private const float _sizeRef = 10f;

    public void Init(float xPosition, float zPosition)
    {
        _nodePosition = new Vector2(xPosition, zPosition);
        float scaleX = _nodeSize.x / _sizeRef;
        float scaleZ = _nodeSize.y / _sizeRef;
        gameObject.transform.localScale = new Vector3(scaleX, 1, scaleZ);
    }
}

