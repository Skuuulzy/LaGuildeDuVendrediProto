using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ExtensionMethods
{
    #region LOAD SCENE

    public static List<string> LoadedSceneNames()
    {
        List<string> loadedSceneNames = new List<string>();
        
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            loadedSceneNames.Add(sceneName);
        }

        return loadedSceneNames;
    }

    #endregion
    
    #region GRAPHIC
    public static void SetAlpha(this UnityEngine.UI.Graphic graphic, float value)
    {
        Color c = graphic.color;
        c.a = value;
        graphic.color = c;
    }
    
    #endregion
}
