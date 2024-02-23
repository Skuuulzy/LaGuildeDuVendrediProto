using UnityEngine;
using System.Collections;

namespace SebastianLague
{
	public class Node : IHeapItem<Node> 
	{
		private bool _walkable;
		private bool _interactableIsland;
		private Vector3 _worldPosition;
		private int _gridX;
		private int _gridY;
		private int _movementPenalty;
		private int _gCost;
		private int _hCost;
		private Node _parent;
		private int _heapIndex;
		public bool Walkable => _walkable;
		public bool InteractableIsland => _interactableIsland;
		public Vector3 WorldPosition => _worldPosition;
		public int GridX => _gridX;
		public int GridY => _gridY;
		public int MovementPenalty => _movementPenalty;

		public int GCost => _gCost;
		public int HCost => _hCost;
		public Node Parent => _parent;
	
		public Node(bool walkable, bool interactableIsland, Vector3 worldPos, int gridX, int gridY, int penalty) 
		{
			_walkable = walkable;
			_interactableIsland = interactableIsland;
			_worldPosition = worldPos;
			this._gridX = gridX;
			this._gridY = gridY;
			_movementPenalty = penalty;
		}

		public int FCost 
		{
			get 
			{
				return _gCost + _hCost;
			}
		}

		public int HeapIndex 
		{
			get 
			{
				return _heapIndex;
			}
			set 
			{
				_heapIndex = value;
			}
		}

		public int CompareTo(Node nodeToCompare) 
		{
			int compare = FCost.CompareTo(nodeToCompare.FCost);

			if (compare == 0) 
			{
				compare = HCost.CompareTo(nodeToCompare.HCost);
			}
			return -compare;
		}

		public void SetMovementPenalty(int value)
		{
			_movementPenalty = value;
		}

		public void SetParent(Node value)
		{
			_parent = value;
		}

		public void SetGCost(int value)
		{
			_gCost = value;
		}

		public void SetHCost(int value)
		{
			_hCost = value;
		}
	}
}
