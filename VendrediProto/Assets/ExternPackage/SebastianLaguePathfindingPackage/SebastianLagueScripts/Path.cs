using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SebastianLague
{
	public class Path 
	{

		public List<Vector3> LookPoints;
		public Line[] TurnBoundaries;
		public int FinishLineIndex;
		public readonly int SlowDownIndex;

		#region Constructors

		public Path()
		{
			LookPoints = new List<Vector3>();
			TurnBoundaries = new Line[0];
			FinishLineIndex = 0;
			SlowDownIndex = 0;
		}

		public Path(List<Vector3> waypoints, Vector3 startPos, float turnDst, float stoppingDst) 
		{
			LookPoints = waypoints;
			TurnBoundaries = new Line[LookPoints.Count];
			FinishLineIndex = TurnBoundaries.Length - 1;
			Vector2 previousPoint = V3ToV2 (startPos);

			for (int i = 0; i < LookPoints.Count; i++) 
			{
				Vector2 currentPoint = V3ToV2 (LookPoints [i]);
				Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
				Vector2 turnBoundaryPoint = (i == FinishLineIndex)?currentPoint : currentPoint - dirToCurrentPoint * turnDst;
				TurnBoundaries [i] = new Line (turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
				previousPoint = turnBoundaryPoint;
			}

			float dstFromEndPoint = 0;

			for (int i = LookPoints.Count - 1; i > 0; i--) 
			{
				dstFromEndPoint += Vector3.Distance (LookPoints [i], LookPoints [i - 1]);

				if (dstFromEndPoint > stoppingDst) 
				{
					SlowDownIndex = i;
					break;
				}
			}
		}


		#endregion

		Vector2 V3ToV2(Vector3 v3) 
		{
			return new Vector2 (v3.x, v3.z);
		}

		public void DrawWithGizmos() 
		{

			Gizmos.color = Color.black;
			foreach (Vector3 p in LookPoints) 
			{
				Gizmos.DrawCube (p + Vector3.up, Vector3.one);
			}

			Gizmos.color = Color.white;

			foreach (Line l in TurnBoundaries) 
			{
				l.DrawWithGizmos (10);
			}
		}
	}
}
