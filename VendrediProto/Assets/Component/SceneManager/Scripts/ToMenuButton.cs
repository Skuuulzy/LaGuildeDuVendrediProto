using GDV.SceneLoader;
using UnityEngine;

public class ToMenuButton : MonoBehaviour
{
    public void LoadMainMenuScene()
    {
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAIN_MENU);
    }
}