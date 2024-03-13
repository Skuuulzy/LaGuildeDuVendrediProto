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

    public void DrawLines(Vector3 currentPosition, Vector3[] lookpoints)
    {
        int size = lookpoints.Length + 1;
        waypoints.Clear();
        //Vector3[]  = new Vector3[size] { };
        waypoints.Add(currentPosition);

        foreach (Vector3 position in lookpoints)
        {
            Vector3 updatedPos = new Vector3(position.x, 5, position.z);
            waypoints.Add(updatedPos);
        }
        Debug.Log("lp leng: " + lookpoints.Length + "wp l : " + waypoints.Count);
        _lineRenderer.positionCount = waypoints.Count;
        _lineRenderer.SetPositions(waypoints.ToArray());
    }

    //TODO : dans l'update update la position avec celle du bateau
    // Update is called once per frame
   /* private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 currentpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentpos.y = 0f;
            *//*if(Vector3.Distance(currentpos, _previousPosition) > _minDistance)
            {*//*
                _lineRenderer.positionCount++;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, currentpos);
                _previousPosition = currentpos;
            *//*}*//*
        }
    }*/
}
