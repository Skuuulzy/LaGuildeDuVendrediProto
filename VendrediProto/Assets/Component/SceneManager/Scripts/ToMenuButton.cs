using UnityEngine;
using VComponent.SceneLoader;

public class ToMenuButton : MonoBehaviour
{
    public void LoadMainMenuScene()
    {
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAIN_MENU);
    }
}