using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SebastianLague
{
	public class Grid : MonoBehaviour 
	{

		[SerializeField] private bool _displayGridGizmos;
		[SerializeField] private LayerMask _unwalkableMask;
		[SerializeField] private LayerMask _interactableIsland;
		[SerializeField] private Vector2 _gridWorldSize;
		[SerializeField] private float _nodeRadius;
		[SerializeField] private TerrainType[] _walkableRegions;
		[SerializeField] private int _obstacleProximityPenalty = 10;
		private Dictionary<int,int> _walkableRegionsDictionary = new Dictionary<int, int>();
		private LayerMask _walkableMask;

		private Node[,] _grid;

		private float _nodeDiameter;
		private int _gridSizeX, _gridSizeY;

		private int _penaltyMin = int.MaxValue;
		private int _penaltyMax = int.MinValue;

		private void Awake() 
		{
			_nodeDiameter = _nodeRadius*2;
			_gridSizeX = Mathf.RoundToInt(_gridWorldSize.x/_nodeDiameter);
			_gridSizeY = Mathf.RoundToInt(_gridWorldSize.y/_nodeDiameter);

			foreach (TerrainType region in _walkableRegions) 
			{
				_walkableMask.value |= region.TerrainMask.value;
				_walkableRegionsDictionary.Add((int)Mathf.Log(region.TerrainMask.value,2),region.TerrainPenalty);
			}

			CreateGrid();
		}

		public int MaxSize 
		{
			get 
			{
				return _gridSizeX * _gridSizeY;
			}
		}

		private void CreateGrid() 
		{
			_grid = new Node[_gridSizeX,_gridSizeY];
			Vector3 worldBottomLeft = transform.position - Vector3.right * _gridWorldSize.x/2 - Vector3.forward * _gridWorldSize.y/2;

			for (int x = 0; x < _gridSizeX; x ++) 
			{
				for (int y = 0; y < _gridSizeY; y ++) 
				{
					Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + _nodeRadius) + Vector3.forward * (y * _nodeDiameter + _nodeRadius);
					bool walkable = !(Physics.CheckSphere(worldPoint,_nodeRadius,_unwalkableMask));
					bool interactableIsland = (Physics.CheckSphere(worldPoint, _nodeRadius, _unwalkableMask));
					int movementPenalty = 0;


					Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
					RaycastHit hit;
					if (Physics.Raycast(ray,out hit, 100, _walkableMask)) 
					{
						_walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
					}

					if (!walkable) 
					{
						movementPenalty += _obstacleProximityPenalty;
					}


					_grid[x,y] = new Node(walkable, interactableIsland, worldPoint, x,y, movementPenalty);
				}
			}

			BlurPenaltyMap (3);

		}

		private void BlurPenaltyMap(int blurSize) 
		{
			int kernelSize = blurSize * 2 + 1;
			int kernelExtents = (kernelSize - 1) / 2;

			int[,] penaltiesHorizontalPass = new int[_gridSizeX,_gridSizeY];
			int[,] penaltiesVerticalPass = new int[_gridSizeX,_gridSizeY];

			for (int y = 0; y < _gridSizeY; y++) 
			{
				for (int x = -kernelExtents; x <= kernelExtents; x++) 
				{
					int sampleX = Mathf.Clamp (x, 0, kernelExtents);
					penaltiesHorizontalPass [0, y] += _grid [sampleX, y].MovementPenalty;
				}

				for (int x = 1; x < _gridSizeX; x++) 
				{
					int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, _gridSizeX);
					int addIndex = Mathf.Clamp(x + kernelExtents, 0, _gridSizeX-1);

					penaltiesHorizontalPass [x, y] = penaltiesHorizontalPass [x - 1, y] - _grid [removeIndex, y].MovementPenalty + _grid [addIndex, y].MovementPenalty;
				}
			}
			
			for (int x = 0; x < _gridSizeX; x++) 
			{
				for (int y = -kernelExtents; y <= kernelExtents; y++) 
				{
						int sampleY = Mathf.Clamp (y, 0, kernelExtents);
						penaltiesVerticalPass [x, 0] += penaltiesHorizontalPass [x, sampleY];
				}

				int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass [x, 0] / (kernelSize * kernelSize));
				_grid [x, 0].SetMovementPenalty(blurredPenalty);

				for (int y = 1; y < _gridSizeY; y++) 
				{
					int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, _gridSizeY);
					int addIndex = Mathf.Clamp(y + kernelExtents, 0, _gridSizeY-1);

					penaltiesVerticalPass [x, y] = penaltiesVerticalPass [x, y-1] - penaltiesHorizontalPass [x,removeIndex] + penaltiesHorizontalPass [x, addIndex];
					blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass [x, y] / (kernelSize * kernelSize));
					_grid [x, y].SetMovementPenalty(blurredPenalty);

					if (blurredPenalty > _penaltyMax) 
					{
						_penaltyMax = blurredPenalty;
					}

					if (blurredPenalty < _penaltyMin) 
					{
						_penaltyMin = blurredPenalty;
					}
				}
			}

		}

		public List<Node> GetNeighbours(Node node) 
		{
			List<Node> neighbours = new List<Node>();

			for (int x = -1; x <= 1; x++) 
			{
				for (int y = -1; y <= 1; y++) 
				{
					if (x == 0 && y == 0)
					{
						continue;
					}

					int checkX = node.GridX + x;
					int checkY = node.GridY + y;

					if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY) 
					{
						neighbours.Add(_grid[checkX,checkY]);
					}
				}
			}

			return neighbours;
		}


		public Node NodeFromWorldPoint(Vector3 worldPosition) 
		{
			float percentX = (worldPosition.x + _gridWorldSize.x/2) / _gridWorldSize.x;
			float percentY = (worldPosition.z + _gridWorldSize.y/2) / _gridWorldSize.y;
			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);

			int x = Mathf.RoundToInt((_gridSizeX-1) * percentX);
			int y = Mathf.RoundToInt((_gridSizeY-1) * percentY);
			return _grid[x,y];
		}

		private void OnDrawGizmos() 
		{
			Gizmos.DrawWireCube(transform.position,new Vector3(_gridWorldSize.x,1,_gridWorldSize.y));

			if (_grid != null && _displayGridGizmos) 
			{
				foreach (Node n in _grid) 
				{

					Gizmos.color = Color.Lerp (Color.white, Color.black, Mathf.InverseLerp (_penaltyMin, _penaltyMax, n.MovementPenalty));
					Gizmos.color = (n.Walkable)?Gizmos.color:Color.red;
					Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_nodeDiameter));
				}
			}
		}

		[System.Serializable]
		public class TerrainType 
		{
			public LayerMask TerrainMask;
			public int TerrainPenalty;
		}


	}
}