using QFSW.QC.Containers;
using Sirenix.Reflection.Editor;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PathfindingView : MonoBehaviour
{
    [SerializeField] private Image _pin;
    [SerializeField] private RectTransform _rectTransformCanvas;
    [SerializeField] private LineRenderer _lineRenderer;

    private List<Vector3> waypoints = new List<Vector3>();

    //TODO : display pin on click and remove it when we are arrived
    public void UpdateCanvasSize(int width, int height)
    {
        _rectTransformCanvas.sizeDelta = new Vector2(width, height);
    }
    public void CreatePin(Vector3 mousePos)
    {

        Vector3 pos = new Vector3(mousePos.x, 0, mousePos.z);
        _pin.rectTransform.position = pos;

    }

    public void DrawLines(Vector3 currentPosition, Vector3[] lookpoints)
    {
        int size = lookpoints.Length + 1;
        waypoints.Clear();
        //Vector3[]  = new Vector3[size] { };
        waypoints.Add(currentPosition);

        foreach (Vector3 position in lookpoints)
        {   
            waypoints.Add(position);
        }
        _lineRenderer.positionCount = waypoints.Count;
        _lineRenderer.SetPositions(waypoints.ToArray());
    }
}
