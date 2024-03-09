using Sirenix.Reflection.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathfindingView : MonoBehaviour
{
    [SerializeField] private Image _pin;
    [SerializeField] private RectTransform _rectTransformCanvas;
    [SerializeField] private LineRenderer _lineRenderer;
    public void UpdateCanvasSize(int width, int height)
    {
        _rectTransformCanvas.sizeDelta = new Vector2(width, height);
    }
    public void CreatePin(Vector3 mousePos)
    {
        Vector3 pos = new Vector3(mousePos.x, 0, mousePos.z);
        _pin.rectTransform.position = pos;
      
    }    

    public void DrawLines(Vector3[] lookpoints)
    {
        Debug.Log("lenght" + lookpoints.Length);
        _lineRenderer.positionCount = lookpoints.Length;
        _lineRenderer.SetPositions(lookpoints);
    }
}
