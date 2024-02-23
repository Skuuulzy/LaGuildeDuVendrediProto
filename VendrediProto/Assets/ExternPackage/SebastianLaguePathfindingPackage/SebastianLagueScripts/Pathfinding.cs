﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace SebastianLague
{
	public class Pathfinding : MonoBehaviour 
	{
		private Grid _grid;
	
		void Awake() 
		{
			_grid = GetComponent<Grid>();
		}

		public void FindPath(PathRequest request, Action<PathResult> callback) 
		{
		
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Vector3[] waypoints = new Vector3[0];
			bool pathSuccess = false;
			Node startNode = _grid.NodeFromWorldPoint(request.pathStart);
			Node targetNode = _grid.NodeFromWorldPoint(request.pathEnd);
			startNode.SetParent(startNode);		
		
			if (startNode.Walkable && targetNode.Walkable) 
			{
				Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
				HashSet<Node> closedSet = new HashSet<Node>();
				openSet.Add(startNode);
			
				while (openSet.Count > 0) 
				{
					Node currentNode = openSet.RemoveFirst();
					closedSet.Add(currentNode);
				
					if (currentNode == targetNode) 
					{
						sw.Stop();
						//print ("Path found: " + sw.ElapsedMilliseconds + " ms");
						pathSuccess = true;
						break;
					}
				
					foreach (Node neighbour in _grid.GetNeighbours(currentNode)) 
					{
						if (!neighbour.Walkable || closedSet.Contains(neighbour)) 
						{
							continue;
						}
					
						int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour) + neighbour.MovementPenalty;

						if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour)) 
						{
							neighbour.SetGCost(newMovementCostToNeighbour);
							neighbour.SetHCost(GetDistance(neighbour, targetNode));
							neighbour.SetParent(currentNode);
						
							if (!openSet.Contains(neighbour))
							{
								openSet.Add(neighbour);
							}

							else
							{
								openSet.UpdateItem(neighbour);
							}
						}
					}
				}
			}

			if (pathSuccess) 
			{
				waypoints = RetracePath(startNode,targetNode);
				pathSuccess = waypoints.Length > 0;
			}

			callback (new PathResult (waypoints, pathSuccess, request.callback));
		}
		
	
		Vector3[] RetracePath(Node startNode, Node endNode) 
		{
			List<Node> path = new List<Node>();
			Node currentNode = endNode;
		
			while (currentNode != startNode) 
			{
				path.Add(currentNode);
				currentNode = currentNode.Parent;
			}

			Vector3[] waypoints = SimplifyPath(path);
			Array.Reverse(waypoints);

			return waypoints;
		}
	
		Vector3[] SimplifyPath(List<Node> path) 
		{
			List<Vector3> waypoints = new List<Vector3>();
			Vector2 directionOld = Vector2.zero;
		
			for (int i = 1; i < path.Count; i ++) 
			{
				Vector2 directionNew = new Vector2(path[i-1].GridX - path[i].GridX,path[i-1].GridY - path[i].GridY);

				if (directionNew != directionOld) 
				{
					waypoints.Add(path[i].WorldPosition);
				}

				directionOld = directionNew;
			}
			return waypoints.ToArray();
		}

		int GetDistance(Node nodeA, Node nodeB) 
		{
			int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
			int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);
		
			if (dstX > dstY)
			{
				return 14*dstY + 10* (dstX-dstY);
			}

			return 14*dstX + 10 * (dstY-dstX);
		}
	}
}
