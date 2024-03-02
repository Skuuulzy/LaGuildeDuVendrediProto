using GDV.SceneLoader;
using UnityEngine;

public class ToMenuButton : MonoBehaviour
{
    public void LoadMainMenuScene()
    {
        HybridSceneManager.Instance.LoadScene(HybridSceneManager.SceneIdentifier.MAIN_MENU);
    }
}