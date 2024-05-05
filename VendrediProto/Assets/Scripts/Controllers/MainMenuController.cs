using UnityEngine;
using VComponent.SceneLoader;

public class MainMenuController : MonoBehaviour
{
    public void LoadMapArtScene()
    {
        _ = HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAP_ART);
    }
    public void LoadMapDevScene()
    {
        _ = HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAP_DEV);
    }
    
    public void LoadMultiplayerLobby()
    {
        _ = HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MULTIPLAYER_LOBBY);
    }
}