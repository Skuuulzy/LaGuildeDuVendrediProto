using System;
using System.Collections.Generic;
using MyGameDevTools.SceneLoading;
using MyGameDevTools.SceneLoading.UniTaskSupport;
using UnityEngine;

namespace GDV.SceneLoader
{
    public class SceneLoader : MonoBehaviour
    {
        private readonly ISceneManager _sceneManager = new SceneManager();
        private ISceneLoaderUniTask _sceneLoader;

        private string _currentSceneName;

        private readonly LoadSceneInfoName _loadingScreenInfo = new LoadSceneInfoName(SCENE_NAME[SceneIdentifier.LOADING_SCREEN]);

        public static Action<SceneIdentifier> OnLoadScene;

        public enum SceneIdentifier
        {
            MAIN_MENU,
            LOADING_SCREEN,
            CHARACTER_CONTROL
        }

        private static readonly Dictionary<SceneIdentifier, string> SCENE_NAME = new()
        {
            { SceneIdentifier.MAIN_MENU, "MainMenu" },
            { SceneIdentifier.LOADING_SCREEN, "LoadingScreen" },
            { SceneIdentifier.CHARACTER_CONTROL, "CharacterControl" },
        };

        private void Awake()
        {
            _sceneLoader = new SceneLoaderUniTask(_sceneManager);

            OnLoadScene += HandleSceneLoad;
        }

        private async void HandleSceneLoad(SceneIdentifier identifier)
        {
            _ = await _sceneLoader.TransitionToSceneAsync(new LoadSceneInfoName(SCENE_NAME[identifier]), _loadingScreenInfo);
        }
    }
}