using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SebastianLague
{
	public class Path 
	{

		public readonly Vector3[] LookPoints;
		public readonly Line[] TurnBoundaries;
		public readonly int FinishLineIndex;
		public readonly int SlowDownIndex;

		public Path(Vector3[] waypoints, Vector3 startPos, float turnDst, float stoppingDst) 
		{
			LookPoints = waypoints;
			TurnBoundaries = new Line[LookPoints.Length];
			FinishLineIndex = TurnBoundaries.Length - 1;
			Vector2 previousPoint = V3ToV2 (startPos);

			for (int i = 0; i < LookPoints.Length; i++) 
			{
				Vector2 currentPoint = V3ToV2 (LookPoints [i]);
				Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
				Vector2 turnBoundaryPoint = (i == FinishLineIndex)?currentPoint : currentPoint - dirToCurrentPoint * turnDst;
				TurnBoundaries [i] = new Line (turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
				previousPoint = turnBoundaryPoint;
			}

			float dstFromEndPoint = 0;

			for (int i = LookPoints.Length - 1; i > 0; i--) 
			{
				dstFromEndPoint += Vector3.Distance (LookPoints [i], LookPoints [i - 1]);

				if (dstFromEndPoint > stoppingDst) 
				{
					SlowDownIndex = i;
					break;
				}
			}
		}

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
