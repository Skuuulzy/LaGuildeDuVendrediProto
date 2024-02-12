using GDV.SceneLoader;
using UnityEngine;

public class ToMenuButton : MonoBehaviour
{
    public void LoadMainMenuScene()
    {
        SceneLoader.OnLoadScene?.Invoke(SceneLoader.SceneIdentifier.MAIN_MENU);
    }
}