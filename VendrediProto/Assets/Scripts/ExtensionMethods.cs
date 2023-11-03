using UnityEngine;

public static class ExtensionMethods
{
    #region GRAPHIC
    public static void SetAlpha(this UnityEngine.UI.Graphic graphic, float value)
    {
        Color c = graphic.color;
        c.a = value;
        graphic.color = c;
    }
    
    #endregion
}
