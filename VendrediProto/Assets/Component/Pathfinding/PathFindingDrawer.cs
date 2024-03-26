using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingDrawer : MonoBehaviour
{
    [SerializeField] private float _minDistance;
    private LineRenderer _lineRenderer;
    private Vector3 _previousPosition;

    private List<Vector3> waypoints = new List<Vector3>();
 
    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _previousPosition = transform.position;
    }

    public void DrawLines(Vector3 currentPosition, List<Vector3> lookpoints)
    {
        int size = lookpoints.Count + 1;
        waypoints.Clear();
        waypoints.Add(currentPosition);
        foreach (Vector3 position in lookpoints)
        {
            Vector3 updatedPos = new Vector3(position.x, 5, position.z);
            waypoints.Add(updatedPos);
        }

        _lineRenderer.positionCount = waypoints.Count;
        _lineRenderer.SetPositions(waypoints.ToArray());
    }

    public void ClearLines()
    {
        _lineRenderer.positionCount=0;
    }
}
