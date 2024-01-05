using GDV.SceneLoader;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void LoadCharacterControlScene()
    {
        SceneLoader.OnLoadScene?.Invoke(SceneLoader.SceneIdentifier.CHARACTER_CONTROL);
    }
    
    public void LoadInteractionExampleScene()
    {
        SceneLoader.OnLoadScene?.Invoke(SceneLoader.SceneIdentifier.INTERACTION_EXAMPLE);
    }
    public void LoadMapTestScene()
    {
        SceneLoader.OnLoadScene?.Invoke(SceneLoader.SceneIdentifier.MAP_TEST);
    }
}