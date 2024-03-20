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
    [SerializeField] public PathFindingDrawer _pathFindingDrawer;
    public void UpdateCanvasSize(int width, int height)
    {
        _rectTransformCanvas.sizeDelta = new Vector2(width, height);
    }
    public void CreatePin(Vector3 mousePos)
    {
        Vector3 pos = new Vector3(mousePos.x, 0, mousePos.z);
        _pin.rectTransform.position = pos;
        _pin.gameObject.SetActive(true);
    }

   
    public void CleanPathfindingView()
    {
        _pin.gameObject.SetActive(false);
        _pathFindingDrawer.ClearLines();
    }
}
