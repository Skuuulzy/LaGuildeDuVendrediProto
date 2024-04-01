using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using MyGameDevTools.SceneLoading;
using MyGameDevTools.SceneLoading.UniTaskSupport;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VComponent.Tools.Singletons;
using SceneManager = MyGameDevTools.SceneLoading.SceneManager;

namespace VComponent.SceneLoader
{
    public class HybridSceneLoader : PersistentSingleton<HybridSceneLoader>
    {
        [SerializeField] private SerializableDictionary<SceneIdentifier, SceneReference> _scenes;
        
        private readonly ISceneManager _sceneManager = new SceneManager();
        private ISceneLoaderUniTask _sceneLoader;

        private static Scene _originalScene;

        private bool _localLoadScreenIsLoaded;
        private bool _localLoadScreenIsListening;
        private LoadingBehavior _localLoadingBehavior;

        private LoadSceneInfoName _lastNetworkScene;

        private LoadSceneInfoName LoadingScreenReference => GetSceneReference(SceneIdentifier.LOADING_SCREEN);
        
        #region DATA

        public enum SceneIdentifier
        {
            MAIN_MENU,
            LOADING_SCREEN,
            MAP_ART,
            MAP_DEV,
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
        
        public async UniTask TransitionTo(SceneIdentifier identifier)
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

            _lastNetworkScene = targetSceneReference;
            
            await LoadLocalLoadingScreen();
            
            NetworkManager.Singleton.SceneManager.LoadScene(targetSceneReference.Reference.ToString(), LoadSceneMode.Single);
            
            // We do not need to unload the local loading screen since the network scene manager unload all local scenes.
        }

        public async Task QuitNetwork(bool isClient)
        {
            UnListenNetworkLoading(isClient);
            
            await TransitionTo(SceneIdentifier.MAIN_MENU);
            
            // We need to unload the scene with the basic manager since our custom does know the network scenes.
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_lastNetworkScene.Reference.ToString());
        }
        
        #endregion NETWORK SCENE LOAD METHODS

        #region NETWORK SCENE LOAD CALLBACKS

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
        private void UnListenNetworkLoading(bool isClient)
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
        
        private void HandleHostNetworkLoadEvents(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load:
                    StartCoroutine(ReportNetworkLoadProgress(sceneEvent.AsyncOperation));
                    break;
                case SceneEventType.LoadEventCompleted :
                    _localLoadScreenIsLoaded = false;
                    _localLoadScreenIsListening = false;
                    break;
            }
        }
        
        private async void HandleClientNetworkLoadEvents(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Synchronize:
                    await LoadLocalLoadingScreen();
                    break;
                case SceneEventType.Load:
                    StartCoroutine(ReportNetworkLoadProgress(sceneEvent.AsyncOperation));
                    break;
                case SceneEventType.LoadEventCompleted :
                    _localLoadScreenIsLoaded = false;
                    _localLoadScreenIsListening = false;
                    break;
            }
        }

        #endregion

        #region LOCAL LOADING SCREEN

        /// <summary>
        /// Load a local loading screen. Only used for network load !
        /// Local load automatically handle the loading screen behavior.
        /// </summary>
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
            if (_localLoadingBehavior != null)
            {
                _localLoadScreenIsListening = true;
            }
        }
        
        private IEnumerator ReportNetworkLoadProgress(AsyncOperation loadOperation)
        {
            while (!loadOperation.isDone)
            {
                if (_localLoadScreenIsListening)
                {
                    _localLoadingBehavior.Progress?.Report(loadOperation.progress);
                }
                yield return null;
            }
        }

        #endregion LOCAL LOADING SCREEN
    }
}