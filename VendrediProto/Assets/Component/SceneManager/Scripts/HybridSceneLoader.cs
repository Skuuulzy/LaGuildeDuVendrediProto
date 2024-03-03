using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using MyGameDevTools.SceneLoading;
using MyGameDevTools.SceneLoading.UniTaskSupport;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VComponent.Tools.Singletons;
using SceneManager = MyGameDevTools.SceneLoading.SceneManager;

namespace GDV.SceneLoader
{
    public class HybridSceneLoader : PersistentSingleton<HybridSceneLoader>
    {
        [SerializeField] private SerializableDictionary<SceneIdentifier, SceneReference> _scenes;
        
        private readonly ISceneManager _sceneManager = new SceneManager();
        private ISceneLoaderUniTask _sceneLoader;

        private static Scene _originalScene;

        private bool _localLoadScreenIsLoaded;
        private LoadingBehavior _localLoadingBehavior;

        private LoadSceneInfoName LoadingScreenReference => GetSceneReference(SceneIdentifier.LOADING_SCREEN);
        
        #region DATA

        public enum SceneIdentifier
        {
            MAIN_MENU,
            LOADING_SCREEN,
            CHARACTER_CONTROL,
            INTERACTION_EXAMPLE,
            MAP_TEST,
            MULTIPLAYER_LOBBY,
            MULTIPLAYER_GAMEPLAY
        }

        private LoadSceneInfoName GetSceneReference(SceneIdentifier identifier)
        {
            return _scenes.ContainsKey(identifier) ? new LoadSceneInfoName(_scenes[identifier].Name) : default;
        }

        #endregion

        #region MONO

        protected override void Awake()
        {
            base.Awake();
            
            _sceneLoader = new SceneLoaderUniTask(_sceneManager);
            _originalScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }

        #endregion

        #region LOCAL SCENE LOAD METHODS

        private async UniTask LoadLocalLoadingScreen()
        {
            if (_localLoadScreenIsLoaded)
            {
                return;
            }

            _localLoadScreenIsLoaded = true;
            
            await _sceneLoader.LoadSceneAsync(LoadingScreenReference);
            await UniTask.WaitForEndOfFrame(this);
            
            _localLoadingBehavior = FindObjectOfType<LoadingBehavior>();
        }
        
        private async void UnLoadLocalLoadingScreen()
        {
            await _sceneLoader.UnloadSceneAsync(LoadingScreenReference);
            _localLoadScreenIsLoaded = false;
        }
        
        public async void TransitionTo(SceneIdentifier identifier)
        {
            var targetSceneReference = GetSceneReference(identifier);

            if (targetSceneReference.Reference == null)
            {
                Debug.LogError($"No scene associated with the identifier {identifier} found in the scenes.");
                return;
            }
            
            // Used for the first transition, the original scene is stored at the awake of the singleton.
            if (_originalScene != default)
            {
                _ = await _sceneLoader.TransitionToSceneAsync(targetSceneReference, LoadingScreenReference, _originalScene);
                _originalScene = default;
                return;
            }

            _ = await _sceneLoader.TransitionToSceneAsync(targetSceneReference, LoadingScreenReference);
        }

        #endregion LOCAL SCENE LOAD METHODS

        #region NETWORK SCENE LOAD METHODS

        public async void LoadNetworkScene(SceneIdentifier identifier)
        {
            var targetSceneReference = GetSceneReference(identifier);

            if (targetSceneReference.Reference == null)
            {
                Debug.LogError($"No scene associated with the identifier {identifier} found in the scenes.");
                return;
            }
            
            await LoadLocalLoadingScreen();
            
            NetworkManager.Singleton.SceneManager.LoadScene(targetSceneReference.Reference.ToString(), LoadSceneMode.Single);
            
            // We do not need to unload the local loading screen since the network scene manager unload all local scenes.
        }

        private void HandleHostNetworkLoadEvents(SceneEvent sceneEvent)
        {
            Debug.Log($"Here with event {sceneEvent.SceneEventType} for scene {sceneEvent.SceneName}");
            
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load:
                    StartCoroutine(ReportNetworkLoadProgress(sceneEvent.AsyncOperation));
                    Debug.Log("On load");
                    break;
            }
        }
        
        private async void HandleClientNetworkLoadEvents(SceneEvent sceneEvent)
        {
            Debug.Log($"Here with event {sceneEvent.SceneEventType} for scene {sceneEvent.SceneName}");
            
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Synchronize:
                    await LoadLocalLoadingScreen();
                    break;
                case SceneEventType.SynchronizeComplete:
                    //UnLoadLocalLoadingScreen();
                    break;
                case SceneEventType.Load:
                    StartCoroutine(ReportNetworkLoadProgress(sceneEvent.AsyncOperation));
                    Debug.Log("On load");
                    break;
            }
        }

        private IEnumerator ReportNetworkLoadProgress(AsyncOperation loadOperation)
        {
            while (!loadOperation.isDone)
            {
                Debug.Log($"Progress on load: {loadOperation.progress}");
                if (_localLoadingBehavior != null)
                {
                    _localLoadingBehavior.Progress?.Report(loadOperation.progress);
                }
                yield return null;
            }
        }

        /// <summary>
        /// Start to listen to Scene Event on the Network Scene Manager, to display loading information.
        /// </summary>
        public void ListenNetworkLoading(bool isClient)
        {
            if (isClient)
            {
                NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleClientNetworkLoadEvents;
            }
            else
            {
                NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleHostNetworkLoadEvents;
            }
        }
        
        /// <summary>
        /// Stop to listen to Scene Event on the Network Scene Manager, to display loading information.
        /// </summary>
        public void UnListenNetworkLoading(bool isClient)
        {
            if (isClient)
            {
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleClientNetworkLoadEvents;
            }
            else
            {
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleHostNetworkLoadEvents;
            }
        }

        #endregion NETWORK SCENE LOAD METHODS
    }
}