using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace FischlWorks_FogWar
{
    /// The non-static high-level mono behaviour interface of the AOS Fog of War module.
    /// This class holds serialized data for various configuration properties,\n
    /// and is responsible for scanning / saving / loading the LevelData object.\n
    /// The class handles the update frequency of the fog, plus some shader businesses.\n
    /// Various public interfaces related to Fog Revealer's FOV are also available.
    public partial class FogWar : MonoBehaviour
    {
        #region SERIALIZED FIELDS

        [BigHeader("Basic Properties")] 
        [SerializeField] private List<FogRevealer> _fogRevealers;
        [SerializeField] private Transform _levelMidPoint;
        [SerializeField] [Range(1, 30)] private float _fogRefreshRate = 10;

        [BigHeader("Fog Properties")] 
        [SerializeField] [Range(0, 100)] private float _fogPlaneHeight = 1;
        [SerializeField] private Material _fogPlaneMaterial;
        [SerializeField] private Color _fogColor = new Color32(5, 15, 25, 255);
        [SerializeField] [Range(0, 1)] private float _fogPlaneAlpha = 1;
        [SerializeField] [Range(1, 5)] private float _fogLerpSpeed = 2.5f;
        
        [Header("Debug")] 
        [SerializeField] private Texture2D _fogPlaneTextureLerpTarget;
        [SerializeField] private Texture2D _fogPlaneTextureLerpBuffer;

        [BigHeader("Level Data")] 
        [SerializeField] private TextAsset _levelDataToLoad;
        [SerializeField] private bool _saveDataOnScan = true;
        [ShowIf("_saveDataOnScan")] 
        [SerializeField] private string _levelNameToSave = "Default";

        [BigHeader("Scan Properties")] 
        [SerializeField] [Range(1, 300)] private int _levelDimensionX = 11;
        [SerializeField] [Range(1, 300)] private int _levelDimensionY = 11;
        [SerializeField] private float _unitScale = 1;
        [SerializeField] private float _scanSpacingPerUnit = 0.25f;
        [SerializeField] private float _rayStartHeight = 5;
        [SerializeField] private float _rayMaxDistance = 10;
        [SerializeField] private LayerMask _obstacleLayers;
        [SerializeField] private bool _ignoreTriggers = true;
        [Tooltip("If true the fog will always reappear if the player move away.")] 
        [SerializeField] private bool _resetAfterLooseSight;

        [BigHeader("Debug Options")] 
        [SerializeField] private bool _drawGizmos;
        [SerializeField] private bool _logOutOfRange;

        #endregion SERIALIZED FIELDS

        #region GETTERS

        public List<FogRevealer> FogRevealers => _fogRevealers;
        public Transform LevelMidPoint => _levelMidPoint;
        public float UnitScale => _unitScale;
        public FogLevelData LevelData { get; private set; } = new ();

        #endregion GETTERS

        #region PRIVATE FIELDS

        // The primitive plane which will act as a mesh for rendering the fog with
        private GameObject _fogPlane;
        private float _fogRefreshRateTimer;
        private const string LEVEL_SCAN_DATA_PATH = "/LevelData";
        private readonly Shadowcaster _shadowcaster = new();
        private static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
        private static readonly int COLOR = Shader.PropertyToID("_Color");

        #endregion PRIVATE FIELDS

        #region MONO

        private void Start()
        {
            CheckProperties();

            InitializeVariables();

            if (_levelDataToLoad == null)
            {
                ScanLevel();

                if (_saveDataOnScan)
                {
#if UNITY_EDITOR
                    SaveScanAsLevelData();
#endif
                }
            }
            else
            {
                LoadLevelData();
            }

            InitializeFog();

            // This part passes the needed references to the shadowcaster
            _shadowcaster.Initialize(this);

            // This is needed because we do not update the fog when there's no unit-scale movement of each fog revealers
            ForceUpdateFog();
        }
        
        private void Update()
        {
            UpdateFog();
        }

        #endregion MONO

        #region INITIALZATION

        private void CheckProperties()
        {
            foreach (FogRevealer fogRevealer in _fogRevealers)
            {
                if (fogRevealer._RevealerTransform == null)
                {
                    Debug.LogErrorFormat("Please assign a Transform component to each Fog Revealer!");
                }
            }

            if (_unitScale <= 0)
            {
                Debug.LogErrorFormat("Unit Scale must be bigger than 0!");
            }

            if (_scanSpacingPerUnit <= 0)
            {
                Debug.LogErrorFormat("Scan Spacing Per Unit must be bigger than 0!");
            }

            if (_levelMidPoint == null)
            {
                Debug.LogErrorFormat("Please assign the Level Mid Point property!");
            }

            if (_fogPlaneMaterial == null)
            {
                Debug.LogErrorFormat("Please assign the \"FogPlane\" material to the Fog Plane Material property!");
            }
        }
        
        private void InitializeVariables()
        {
            // This is for faster development iteration purposes
            if (_obstacleLayers.value == 0)
            {
                _obstacleLayers = LayerMask.GetMask("Default");
            }

            // This is also for faster development iteration purposes
            if (_levelNameToSave == String.Empty)
            {
                _levelNameToSave = "Default";
            }
        }
        
        private void InitializeFog()
        {
            _fogPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            _fogPlane.name = "[RUNTIME] Fog_Plane";

            _fogPlane.transform.position = new Vector3(
                _levelMidPoint.position.x,
                _levelMidPoint.position.y + _fogPlaneHeight,
                _levelMidPoint.position.z);

            _fogPlane.transform.localScale = new Vector3(
                (_levelDimensionX * _unitScale) / 10.0f,
                1,
                (_levelDimensionY * _unitScale) / 10.0f);

            _fogPlaneTextureLerpTarget = new Texture2D(_levelDimensionX, _levelDimensionY);
            _fogPlaneTextureLerpBuffer = new Texture2D(_levelDimensionX, _levelDimensionY)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            _fogPlane.GetComponent<MeshRenderer>().material = new Material(_fogPlaneMaterial);

            _fogPlane.GetComponent<MeshRenderer>().material.SetTexture(MAIN_TEX, _fogPlaneTextureLerpBuffer);

            _fogPlane.GetComponent<MeshCollider>().enabled = false;
        }

        #endregion INITIALZATION

        #region FOG UPDATE

        private void ForceUpdateFog()
        {
            UpdateFogField();

            Graphics.CopyTexture(_fogPlaneTextureLerpTarget, _fogPlaneTextureLerpBuffer);
        }
        
        private void UpdateFog()
        {
            _fogPlane.transform.position = new Vector3(
                _levelMidPoint.position.x,
                _levelMidPoint.position.y + _fogPlaneHeight,
                _levelMidPoint.position.z);

            _fogRefreshRateTimer += Time.deltaTime;

            if (_fogRefreshRateTimer < 1 / _fogRefreshRate)
            {
                UpdateFogPlaneTextureBuffer();

                return;
            }
            else
            {
                // This is to cancel out minor excess values
                _fogRefreshRateTimer -= 1 / _fogRefreshRate;
            }

            foreach (FogRevealer fogRevealer in _fogRevealers)
            {
                if (fogRevealer._UpdateOnlyOnMove == false)
                {
                    break;
                }

                Vector2Int currentLevelCoordinates = fogRevealer.GetCurrentLevelCoordinates(this);

                if (currentLevelCoordinates != fogRevealer._LastSeenAt)
                {
                    break;
                }

                if (fogRevealer == _fogRevealers.Last())
                {
                    return;
                }
            }

            UpdateFogField();

            UpdateFogPlaneTextureBuffer();
        }
        
        private void UpdateFogField()
        {
            if (_resetAfterLooseSight)
            {
                _shadowcaster.ResetTileVisibility();
            }

            foreach (FogRevealer fogRevealer in _fogRevealers)
            {
                fogRevealer.GetCurrentLevelCoordinates(this);

                _shadowcaster.ProcessLevelData(
                    fogRevealer._CurrentLevelCoordinates,
                    Mathf.RoundToInt(fogRevealer._SightRange / _unitScale));
            }

            UpdateFogPlaneTextureTarget();
        }

        // Doing shader business on the script, if we pull this out as a shader pass, same operations must be repeated
        private void UpdateFogPlaneTextureBuffer()
        {
            for (int xIterator = 0; xIterator < _levelDimensionX; xIterator++)
            {
                for (int yIterator = 0; yIterator < _levelDimensionY; yIterator++)
                {
                    Color bufferPixel = _fogPlaneTextureLerpBuffer.GetPixel(xIterator, yIterator);
                    Color targetPixel = _fogPlaneTextureLerpTarget.GetPixel(xIterator, yIterator);

                    _fogPlaneTextureLerpBuffer.SetPixel(xIterator, yIterator, Color.Lerp(
                        bufferPixel,
                        targetPixel,
                        _fogLerpSpeed * Time.deltaTime));
                }
            }

            _fogPlaneTextureLerpBuffer.Apply();
        }

        private void UpdateFogPlaneTextureTarget()
        {
            _fogPlane.GetComponent<MeshRenderer>().material.SetColor(COLOR, _fogColor);

            _fogPlaneTextureLerpTarget.SetPixels(_shadowcaster.fogField.GetColors(_fogPlaneAlpha));

            _fogPlaneTextureLerpTarget.Apply();
        }

        #endregion FOG UPDATE

        #region LEVEL SCANNING

        private void ScanLevel()
        {
            Debug.LogFormat("There is no level data file assigned, scanning level...");

            // These operations have no real computational meaning, but it will bring consistency to the data
            LevelData.levelDimensionX = _levelDimensionX;
            LevelData.levelDimensionY = _levelDimensionY;
            LevelData.unitScale = _unitScale;
            LevelData.scanSpacingPerUnit = _scanSpacingPerUnit;

            for (int xIterator = 0; xIterator < _levelDimensionX; xIterator++)
            {
                // Adding a new list for column (y-axis) for each unit in row (x-axis)
                LevelData.AddColumn(new LevelColumn(Enumerable.Repeat(LevelColumn.ETileState.Empty, _levelDimensionY)));

                for (int yIterator = 0; yIterator < _levelDimensionY; yIterator++)
                {
                    bool isObstacleHit = Physics.BoxCast(
                        new Vector3(
                            GetWorldX(xIterator),
                            _levelMidPoint.position.y + _rayStartHeight,
                            GetWorldY(yIterator)),
                        new Vector3(
                            (_unitScale - _scanSpacingPerUnit) / 2.0f,
                            _unitScale / 2.0f,
                            (_unitScale - _scanSpacingPerUnit) / 2.0f),
                        Vector3.down,
                        Quaternion.identity,
                        _rayMaxDistance,
                        _obstacleLayers,
                        (QueryTriggerInteraction)(2 - Convert.ToInt32(_ignoreTriggers)));

                    if (isObstacleHit)
                    {
                        LevelData[xIterator][yIterator] = LevelColumn.ETileState.Obstacle;
                    }
                }
            }

            Debug.LogFormat("Successfully scanned level with a scale of {0} x {1}", _levelDimensionX, _levelDimensionY);
        }


        // We intend to use Application.dataPath only for accessing project files directory (only in unity editor)
#if UNITY_EDITOR
        private void SaveScanAsLevelData()
        {
            string fullPath = Application.dataPath + LEVEL_SCAN_DATA_PATH + "/" + _levelNameToSave + ".json";

            if (Directory.Exists(Application.dataPath + LEVEL_SCAN_DATA_PATH) == false)
            {
                Directory.CreateDirectory(Application.dataPath + LEVEL_SCAN_DATA_PATH);

                Debug.LogFormat("level scan data folder at \"{0}\" is missing, creating...", LEVEL_SCAN_DATA_PATH);
            }

            if (File.Exists(fullPath))
            {
                Debug.LogFormat("level scan data already exists, overwriting...");
            }

            string levelJson = JsonUtility.ToJson(LevelData);

            File.WriteAllText(fullPath, levelJson);

            Debug.LogFormat("Successfully saved level scan data at \"{0}\"", fullPath);
        }
#endif
        
        private void LoadLevelData()
        {
            Debug.LogFormat("Level scan data with a name of \"{0}\" is assigned, loading...", _levelDataToLoad.name);

            // Exception check is indirectly performed through branching on the upper part of the code
            string levelJson = _levelDataToLoad.ToString();

            LevelData = JsonUtility.FromJson<FogLevelData>(levelJson);

            _levelDimensionX = LevelData.levelDimensionX;
            _levelDimensionY = LevelData.levelDimensionY;
            _unitScale = LevelData.unitScale;
            _scanSpacingPerUnit = LevelData.scanSpacingPerUnit;

            Debug.LogFormat("Successfully loaded level scan data with the name of \"{0}\"", _levelDataToLoad.name);
        }

        #endregion LEVEL SCANNING

        #region PUBLIC METHODS

        /// Adds a new FogRevealer instance to the list and returns its index
        public int AddFogRevealer(FogRevealer fogRevealer)
        {
            _fogRevealers.Add(fogRevealer);

            return _fogRevealers.Count - 1;
        }

        /// Removes a FogRevealer instance from the list with index
        public void RemoveFogRevealer(int revealerIndex)
        {
            if (_fogRevealers.Count > revealerIndex && revealerIndex > -1)
            {
                _fogRevealers.RemoveAt(revealerIndex);
            }
            else
            {
                Debug.LogFormat("Given index of {0} exceeds the revealers' container range", revealerIndex);
            }
        }
        
        /// Replaces the FogRevealer list with the given one
        public void ReplaceFogRevealerList(List<FogRevealer> fogRevealers)
        {
            _fogRevealers = fogRevealers;
        }

        /// Checks if the given level coordinates are within level dimension range.
        public bool CheckLevelGridRange(Vector2Int levelCoordinates)
        {
            bool result =
                levelCoordinates.x >= 0 &&
                levelCoordinates.x < LevelData.levelDimensionX &&
                levelCoordinates.y >= 0 &&
                levelCoordinates.y < LevelData.levelDimensionY;

            if (result == false && _logOutOfRange == true)
            {
                Debug.LogFormat("Level coordinates \"{0}\" is out of grid range", levelCoordinates);
            }

            return result;
        }

        /// Checks if the given world coordinates are within level dimension range.
        public bool CheckWorldGridRange(Vector3 worldCoordinates)
        {
            Vector2Int levelCoordinates = WorldToLevel(worldCoordinates);

            return CheckLevelGridRange(levelCoordinates);
        }

        /// Checks if the given pair of world coordinates and additionalRadius is visible by FogRevealers.
        public bool CheckVisibility(Vector3 worldCoordinates, int additionalRadius)
        {
            Vector2Int levelCoordinates = WorldToLevel(worldCoordinates);

            if (additionalRadius == 0)
            {
                return _shadowcaster.fogField[levelCoordinates.x][levelCoordinates.y] ==
                       Shadowcaster.LevelColumn.ETileVisibility.Revealed;
            }

            int scanResult = 0;

            for (int xIterator = -1; xIterator < additionalRadius + 1; xIterator++)
            {
                for (int yIterator = -1; yIterator < additionalRadius + 1; yIterator++)
                {
                    if (CheckLevelGridRange(new Vector2Int(
                            levelCoordinates.x + xIterator,
                            levelCoordinates.y + yIterator)) == false)
                    {
                        scanResult = 0;

                        break;
                    }

                    scanResult += Convert.ToInt32(
                        _shadowcaster.fogField[levelCoordinates.x + xIterator][levelCoordinates.y + yIterator] ==
                        Shadowcaster.LevelColumn.ETileVisibility.Revealed);
                }
            }

            if (scanResult > 0)
            {
                return true;
            }

            return false;
        }

        #endregion PUBLIC METHODS

        #region HELPERS METHODS

        /// Converts unit (divided by unitScale, then rounded) world coordinates to level coordinates.
        public Vector2Int WorldToLevel(Vector3 worldCoordinates)
        {
            Vector2Int unitWorldCoordinates = GetUnitVector(worldCoordinates);

            return new Vector2Int(
                unitWorldCoordinates.x + (_levelDimensionX / 2),
                unitWorldCoordinates.y + (_levelDimensionY / 2));
        }
        
        /// Converts level coordinates into world coordinates.
        public Vector3 GetWorldVector(Vector2Int worldCoordinates)
        {
            return new Vector3(
                GetWorldX(worldCoordinates.x + (_levelDimensionX / 2)),
                0,
                GetWorldY(worldCoordinates.y + (_levelDimensionY / 2)));
        }
        
        /// Converts "pure" world coordinates into unit world coordinates.
        private Vector2Int GetUnitVector(Vector3 worldCoordinates)
        {
            return new Vector2Int(GetUnitX(worldCoordinates.x), GetUnitY(worldCoordinates.z));
        }
        
        /// Converts level coordinate to corresponding unit world coordinates.
        private float GetWorldX(int xValue)
        {
            if (LevelData.levelDimensionX % 2 == 0)
            {
                return (_levelMidPoint.position.x - ((_levelDimensionX / 2.0f) - xValue) * _unitScale);
            }

            return (_levelMidPoint.position.x - ((_levelDimensionX / 2.0f) - (xValue + 0.5f)) * _unitScale);
        }

        /// Converts world coordinate to unit world coordinates.
        private int GetUnitX(float xValue)
        {
            return Mathf.RoundToInt((xValue - _levelMidPoint.position.x) / _unitScale);
        }
        
        /// Converts level coordinate to corresponding unit world coordinates.
        private float GetWorldY(int yValue)
        {
            if (LevelData.levelDimensionY % 2 == 0)
            {
                return (_levelMidPoint.position.z - ((_levelDimensionY / 2.0f) - yValue) * _unitScale);
            }

            return (_levelMidPoint.position.z - ((_levelDimensionY / 2.0f) - (yValue + 0.5f)) * _unitScale);
        }
        
        /// Converts world coordinate to unit world coordinates.
        private int GetUnitY(float yValue)
        {
            return Mathf.RoundToInt((yValue - _levelMidPoint.position.z) / _unitScale);
        }

        #endregion HELPERS METHODS

        #region DEBUG

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying == false)
            {
                return;
            }

            if (_drawGizmos == false)
            {
                return;
            }

            Handles.color = Color.yellow;

            for (int xIterator = 0; xIterator < _levelDimensionX; xIterator++)
            {
                for (int yIterator = 0; yIterator < _levelDimensionY; yIterator++)
                {
                    if (LevelData[xIterator][yIterator] == LevelColumn.ETileState.Obstacle)
                    {
                        Handles.color = _shadowcaster.fogField[xIterator][yIterator] == Shadowcaster.LevelColumn.ETileVisibility.Revealed ? 
                            Color.green : Color.red;

                        Handles.DrawWireCube(
                            new Vector3(
                                GetWorldX(xIterator),
                                _levelMidPoint.position.y,
                                GetWorldY(yIterator)),
                            new Vector3(
                                _unitScale - _scanSpacingPerUnit,
                                _unitScale,
                                _unitScale - _scanSpacingPerUnit));
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;

                        Gizmos.DrawSphere(
                            new Vector3(
                                GetWorldX(xIterator),
                                _levelMidPoint.position.y,
                                GetWorldY(yIterator)),
                            _unitScale / 5.0f);
                    }

                    if (_shadowcaster.fogField[xIterator][yIterator] == Shadowcaster.LevelColumn.ETileVisibility.Revealed)
                    {
                        Gizmos.color = Color.green;

                        Gizmos.DrawSphere(
                            new Vector3(
                                GetWorldX(xIterator),
                                _levelMidPoint.position.y,
                                GetWorldY(yIterator)),
                            _unitScale / 3.0f);
                    }
                }
            }
        }
#endif

        #endregion DEBUG

        #region CREDITS

        /*
         * Created :    Winter 2022
         * Author :     SeungGeon Kim (keithrek@hanmail.net)
         * Project :    FogWar
         * Filename :   csHomebrewFogWar.cs (non-static monobehaviour module)
         *
         * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
         */

        #endregion CREDITS
    }
}