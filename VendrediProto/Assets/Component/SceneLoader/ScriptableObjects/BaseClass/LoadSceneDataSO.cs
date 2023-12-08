using Udar.SceneManager;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

namespace VTools
{
    /// <summary>
    /// This class holds data to load and/or unload scenes
    /// </summary>
    [CreateAssetMenu(menuName = "Tools/Loader/LoadSceneData", order = 0)]
    public class LoadSceneDataSO : ScriptableObject
    {
        #region variables
		[SerializeField] private List<SceneField> _sceneFieldsToLoad = new List<SceneField>();
        [SerializeField] private List<SceneField> _sceneFieldsToUnload = new List<SceneField>();
        [SerializeField] private List<SceneField> _sceneFieldsToKeepLoaded = new List<SceneField>();
        [SerializeField] private SceneField _sceneFieldToActivate = null;
        [SerializeField] private LoadTypeSO _loadType;
        [SerializeField] private float _minimumLoadTime;

        private List<string> _sceneNamesToLoad;
        private List<string> _sceneNamesToUnload;
        private string _sceneNameToActivate = null;
        #endregion

        #region getters
        public List<string> SceneNamesToLoad
        {
            get
            {
                if (_sceneNamesToLoad == null || _sceneNamesToLoad.Count < 1)
                {
                    return _sceneFieldsToLoad.Select(x => x.Name).ToList();
                }
                else
                {
                    return _sceneNamesToLoad;
                }
            }
        }
        public List<string> SceneNamesToUnload
        {
            get
            {
                if (_sceneNamesToUnload == null || _sceneNamesToUnload.Count < 1)
                {
                    return _sceneFieldsToUnload.Select(x => x.Name).ToList();
                }
                else
                {
                    return _sceneNamesToUnload;
                }
            }
        }

        public LoadTypeSO LoadType => _loadType;
        public float MinimumLoadTime => _minimumLoadTime;
        public string SceneNameToActivate
        {
            get
            {
                if (!string.IsNullOrEmpty(_sceneNameToActivate))
                {
                    return _sceneNameToActivate;
                }
                else if (_sceneFieldToActivate != null)
                {
                    return _sceneFieldToActivate.Name;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<string> SceneNamesToKeepLoaded
        {
            get
            {
                return _sceneFieldsToKeepLoaded.Select(x => x.Name).ToList();
            }
        }
        #endregion

        #region setters
        public void SetInfo(List<string> sceneNamesToLoad, List<string> sceneNamesToUnload, /*List<SceneField> sceneFieldsToKeepLoaded,*/ LoadTypeSO loadType, string sceneNameToActivate = null, float minimumLoadTime = 0f)
        {
            _sceneNamesToLoad = sceneNamesToLoad;
            _sceneNamesToUnload = sceneNamesToUnload;
            _loadType = loadType;
            _minimumLoadTime = minimumLoadTime;
            _sceneNameToActivate = sceneNameToActivate;
            //_sceneFieldsToKeepLoaded = sceneFieldsToKeepLoaded;
        }
        #endregion

        public void Load()
        {
            LoadSceneManager.LoadScenesEvent?.Invoke(this);
        }
    }
}
