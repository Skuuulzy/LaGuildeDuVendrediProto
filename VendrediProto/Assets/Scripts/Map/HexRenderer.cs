using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    private Mesh _mesh;
	[SerializeField] private MeshFilter _meshFilter;
	[SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _material;
	[SerializeField] private float _innerSize;
	[SerializeField] private float _outerSize;
	[SerializeField] private float _height;
	[SerializeField] private bool _isFlatTopped;

	private List<Face> _faces;

	//private void Awake()
	//{
	//	_mesh = new Mesh();
	//	_mesh.name = "Hex";

	//	_meshFilter.mesh = _mesh;
	//	_meshRenderer.material = _material;
	//}

	//private void OnEnable()
	//{
	//	DrawMesh();
	//}

	//private void OnValidate()
	//{
	//	if (Application.isPlaying)
	//	{
	//		DrawMesh();
	//	}
	//}

	public void Init(bool isFlatTopped, float outerSize, float innerSize, float height, Material material)
	{
		_isFlatTopped = isFlatTopped;
		_outerSize = outerSize;
		_innerSize = innerSize;
		_height = height;
		_material = material;

		_mesh = new Mesh();
		_mesh.name = "Hex";
		_meshFilter = GetComponent<MeshFilter>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_meshFilter.mesh = _mesh;
		_meshRenderer.material = _material;
	}

	public void DrawMesh()
	{
		DrawFaces();
		CombineFaces();
	}

	private void DrawFaces()
	{
		_faces = new List<Face>();

		//Top faces
		for(int point = 0; point < 6; point++)
		{
			_faces.Add(CreateFace(_innerSize, _outerSize, _height / 2f, _height / 2f, point));
		}

		//Bottom faces
		for (int point = 0; point < 6; point++)
		{
			_faces.Add(CreateFace(_innerSize, _outerSize, -_height / 2f, -_height / 2f, point, true));
		}

		//Outer faces
		for (int point = 0; point < 6; point++)
		{
			_faces.Add(CreateFace(_outerSize, _outerSize, _height / 2f, -_height / 2f, point, true));
		}

		//Inner faces
		for (int point = 0; point < 6; point++)
		{
			_faces.Add(CreateFace(_innerSize, _innerSize, _height / 2f, -_height / 2f, point, false));
		}

	}

	private void CombineFaces()
	{
		List<Vector3> vertices = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		for (int i = 0; i < _faces.Count; i++)
		{
			//Add the vertices
			vertices.AddRange(_faces[i].vertices);
			uvs.AddRange(_faces[i].uvs);

			int offset = (4 * i);
			foreach (int triangle in _faces[i].triangles)
			{
				tris.Add(triangle + offset);
			}
		}

		_mesh.vertices = vertices.ToArray();
		_mesh.triangles = tris.ToArray();
		_mesh.uv = uvs.ToArray();
		_mesh.RecalculateNormals();
	}

	private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false)
	{
		Vector3 pointA = GetPoint(innerRad, heightB, point);
		Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
		Vector3 pointC = GetPoint(innerRad, heightA, (point < 5) ? point + 1 : 0);
		Vector3 pointD = GetPoint(innerRad, heightA, point);
		
		List<Vector3> vertices = new List<Vector3>() {pointA, pointB, pointC, pointD};
		List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
		List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };

		if (reverse)
		{
			vertices.Reverse();
		}

		return new Face(vertices, triangles, uvs);
	}

	protected Vector3 GetPoint(float size, float height, int index)
	{
		float angle_deg = _isFlatTopped ? 60 * index : 60 * index - 30;
		float angle_rad = Mathf.PI / 180f * angle_deg;
		return new Vector3((size * Mathf.Cos(angle_rad)), height, size * Mathf.Sin(angle_rad));
	}
}

public struct Face
{
	public List<Vector3> vertices { get; private set; }
	public List<int> triangles { get; private set;}
	public List<Vector2> uvs { get; private set; }

	public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
	{
		this.vertices = vertices;
		this.triangles = triangles;
		this.uvs = uvs;
	}
}
