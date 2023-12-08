using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int _gridSize;

    [Header("Tile Settings")]
    [SerializeField] private float _outerSize = 1f;
    [SerializeField] private float _innerSize = 0f;
    [SerializeField] private float _height = 1f;
    [SerializeField] private bool _isFlatTopped;
    [SerializeField] private Material _material;

	private void OnEnable()
	{
		LayoutGrid();
	}

	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			LayoutGrid();
		}
	}

	private void LayoutGrid()
	{
		for(int y = 0; y < _gridSize.y; y++)
		{
			for(int x = 0; x < _gridSize.x; x++)
			{
				GameObject tile = new GameObject($"Hex {x},{y}", typeof(HexRenderer));
				tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y));

				HexRenderer hexRenderer = tile.GetComponent<HexRenderer>();
				hexRenderer.Init(_isFlatTopped, _outerSize, _innerSize, _height, _material);
				hexRenderer.DrawMesh();

				tile.transform.SetParent(transform, true);
			}
		}
	}

	public Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinate)
	{
		int column = coordinate.x;
		int row = coordinate.y;
		float width;
		float height;
		float xPosition;
		float yPosition;
		bool shouldOffset;
		float horizontalDistance;
		float verticalDistance;
		float offset;
		float size = _outerSize;

		if(_isFlatTopped == false) 
		{
			shouldOffset = (row % 2) == 0;
			width = Mathf.Sqrt(3) * size;
			height = 2f * size;

			horizontalDistance = width;
			verticalDistance = height * (3f/4f);

			offset = (shouldOffset) ? width / 2 : 0;

			xPosition = (column * horizontalDistance) + offset;
			yPosition = (row * verticalDistance);
		}
		else
		{
			shouldOffset = (column % 2) == 0;
			width = 2f * size;
			height = Mathf.Sqrt(3) * size;

			horizontalDistance = width * (3f / 4f);
			verticalDistance = height;

			offset = (shouldOffset) ? height / 2 : 0;

			xPosition = (column * horizontalDistance) ;
			yPosition = (row * verticalDistance) - offset;
		}

		return new Vector3(xPosition, 0, -yPosition);
	}
}
