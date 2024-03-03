using GDV.SceneLoader;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void LoadCharacterControlScene()
    {
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.CHARACTER_CONTROL);
    }
    
    public void LoadInteractionExampleScene()
    {
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.INTERACTION_EXAMPLE);
    }
    public void LoadMapTestScene()
    {
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAP_TEST);
    }
    
    public void LoadMultiplayerLobby()
    {
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MULTIPLAYER_LOBBY);
    }
}