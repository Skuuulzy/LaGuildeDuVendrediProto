using GDV.SceneLoader;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void LoadCharacterControlScene()
    {
        SceneLoader.OnLoadScene?.Invoke(SceneLoader.SceneIdentifier.CHARACTER_CONTROL);
    }
}