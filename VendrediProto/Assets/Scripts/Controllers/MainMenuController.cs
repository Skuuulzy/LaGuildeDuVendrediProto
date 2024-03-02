using GDV.SceneLoader;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void LoadCharacterControlScene()
    {
        HybridSceneManager.Instance.LoadScene(HybridSceneManager.SceneIdentifier.CHARACTER_CONTROL);
    }
    
    public void LoadInteractionExampleScene()
    {
        HybridSceneManager.Instance.LoadScene(HybridSceneManager.SceneIdentifier.INTERACTION_EXAMPLE);
    }
    public void LoadMapTestScene()
    {
        HybridSceneManager.Instance.LoadScene(HybridSceneManager.SceneIdentifier.MAP_TEST);
    }
    
    public void LoadMultiplayerLobby()
    {
        HybridSceneManager.Instance.LoadScene(HybridSceneManager.SceneIdentifier.MULTIPLAYER_LOBBY);
    }
}