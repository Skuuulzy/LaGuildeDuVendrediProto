using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class HexGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridSize;
    public float radius = 1f;
    public bool isFlatTopped;

    public HexTileGenerationSettings settings;

    public void Clear()
    {
        List<GameObject> children = new List<GameObject>();
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            children.Add(child);
        }

        foreach(GameObject child in children)
        {
            DestroyImmediate(child, true);
        }
    }

    
    public void LayoutGrid()
    {
        Clear();
        for(int y = 0; y < gridSize.y; y++)
        {
            for(int x = 0; x < gridSize.x; x++)
            {
                GameObject tile = new GameObject($"Hex C{x}, R{y}");
                HexTile hextile = tile.AddComponent<HexTile>();
                hextile.settings = settings;
				hextile.RollTileType();
                hextile.AddTile();
            }
        }
    }
	
}

#if UNITY_EDITOR

[CustomEditor(typeof(HexGrid))]
public class CustomInspectorGUI : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		HexGrid hgrid = (HexGrid)target;
		if (GUILayout.Button("Layout"))
		{
			hgrid.LayoutGrid();
		}
	}
}

#endif

[ExecuteInEditMode]
public class HexTile : MonoBehaviour
{
    public HexTileGenerationSettings settings;
    public HexTileGenerationSettings.TileType tileType;

    public GameObject tile;
    public GameObject fow;

    public Vector2Int offsetCoordinate;
    public Vector3Int cubeCoordinate;
    public List<HexTile> neighbours;
    private bool isDirty = false;

	private void OnValidate()
	{
		if(tile == null)
        {
            return;
        }

        isDirty = true;
	}

	private void Update()
	{
        if (isDirty)
        {
            if (Application.isPlaying)
            {
                Destroy(tile);
            }
            else
            {
                DestroyImmediate(tile);
            }

            AddTile();
            isDirty = false;
        }
	}

	public void AddTile()
	{
        tileType = (HexTileGenerationSettings.TileType)UnityEngine.Random.Range(0, 3);
	}

	public void RollTileType()
	{
        tile = Instantiate(settings.GetTile(tileType));
        
        if(gameObject.GetComponent<MeshCollider>() == null) 
        { 
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = GetComponentInChildren<MeshFilter>().mesh;
        }
        tile.transform.parent = transform;
	}
}
