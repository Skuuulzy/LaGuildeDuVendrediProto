using System;
using System.Collections.Generic;
using MyGameDevTools.SceneLoading;
using MyGameDevTools.SceneLoading.UniTaskSupport;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = MyGameDevTools.SceneLoading.SceneManager;

namespace GDV.SceneLoader
{
    public class SceneLoader : MonoBehaviour
    {
        private readonly ISceneManager _sceneManager = new SceneManager();
        private ISceneLoaderUniTask _sceneLoader;
        
        private readonly LoadSceneInfoName _loadingScreenInfo = new (SCENE_NAME[SceneIdentifier.LOADING_SCREEN]);

        private readonly LoadSceneInfoName _loadingScreenInfo = new(SCENE_NAME[SceneIdentifier.LOADING_SCREEN]);

        public static Action<SceneIdentifier> OnLoadScene;

        #region DATA

        public enum SceneIdentifier
        {
            MAIN_MENU,
            LOADING_SCREEN,
            CHARACTER_CONTROL,
            INTERACTION_EXAMPLE,
            MAP_TEST
        }

        private static readonly Dictionary<SceneIdentifier, string> SCENE_NAME = new()
        {
            { SceneIdentifier.MAIN_MENU, "MainMenu" },
            { SceneIdentifier.LOADING_SCREEN, "LoadingScreen" },
            { SceneIdentifier.CHARACTER_CONTROL, "CharacterControl" },
            { SceneIdentifier.INTERACTION_EXAMPLE, "InteractionExample" },
            { SceneIdentifier.MAP_TEST, "MapTest" },
        };

        #endregion

        #region MONO

        private void Awake()
        {
            _sceneLoader = new SceneLoaderUniTask(_sceneManager);

            OnLoadScene += HandleSceneLoad;
        }

        #endregion

        #region SCENE LOAD METHODS

        private async void HandleSceneLoad(SceneIdentifier identifier)
        {
#if UNITY_EDITOR
            // Used by the editor scene utility if the game is not launched from boot.
            if (_originalScene != default)
            {
                _ = await _sceneLoader.TransitionToSceneAsync(new LoadSceneInfoName(SCENE_NAME[identifier]), _loadingScreenInfo, _originalScene);
                _originalScene = default;
                return;
            }
#endif

            _ = await _sceneLoader.TransitionToSceneAsync(new LoadSceneInfoName(SCENE_NAME[identifier]), _loadingScreenInfo);
        }

        #endregion

        #region DEBUG

#if UNITY_EDITOR
        private static Scene _originalScene;

        // Used by the editor scene utility if the game is not launched from boot.
        public static void SetOriginalScene()
        {
            _originalScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }
#endif

        #endregion
    }
}