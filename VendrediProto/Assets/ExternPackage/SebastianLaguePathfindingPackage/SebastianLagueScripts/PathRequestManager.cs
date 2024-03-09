using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace SebastianLague
{
	public class PathRequestManager : MonoBehaviour 
	{

		private Queue<PathResult> _results = new Queue<PathResult>();

		static PathRequestManager Instance;
		private Pathfinding _pathfinding;

		void Awake() 
		{
			Instance = this;
			_pathfinding = GetComponent<Pathfinding>();
		}

		void Update() 
		{
			if (_results.Count > 0) 
			{
				int itemsInQueue = _results.Count;
				lock (_results) {
					for (int i = 0; i < itemsInQueue; i++) 
					{
						PathResult result = _results.Dequeue ();
						result.callback (result.path, result.success);
					}
				}
			}
		}

		public static void RequestPath(PathRequest request) 
		{
			ThreadStart threadStart = delegate {
				Instance._pathfinding.FindPath (request, Instance.FinishedProcessingPath);
			};
			threadStart.Invoke ();
		}

		public void FinishedProcessingPath(PathResult result) 
		{
			lock (_results) 
			{
				_results.Enqueue (result);
			}
		}



	}

	public struct PathResult 
	{
		public Vector3[] path;
		public bool success;
		public Action<Vector3[], bool> callback;

		public PathResult (Vector3[] path, bool success, Action<Vector3[], bool> callback)
		{
			this.path = path;
			this.success = success;
			this.callback = callback;
		}

	}

	public struct PathRequest 
	{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) 
		{
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}
	}
}
