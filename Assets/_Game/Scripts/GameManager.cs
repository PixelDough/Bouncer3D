using System;
using System.Collections.Generic;
using Febucci.UI;
using HeathenEngineering.SteamworksIntegration;
using Newtonsoft.Json;
using PixelDough.Bouncer.UI;
using QFSW.QC;
using TMPro;
using Tools.SceneDependencies;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using CloudAPI = HeathenEngineering.SteamworksIntegration.API.RemoteStorage.Client;

namespace PixelDough.Bouncer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                if (!_instance) _instance = FindFirstObjectByType<GameManager>();
                return _instance;
            }
            private set => _instance = value;
        }
        private static GameManager _instance;
        private static readonly int SubtractiveFadeAmount = Shader.PropertyToID("_SubtractiveFadeAmount");
        private static readonly int UnscaledTime = Shader.PropertyToID("_UnscaledTime");

        public SteamSettings steamSettings;
        
        public List<GameLevelDataSO> gameLevels = new List<GameLevelDataSO>();
        public int currentLevelDataIndex;
        public GameLevelDataSO CurrentLevelData => gameLevels[currentLevelDataIndex];

        //[SerializeField] private Volume globalVolume;

        public ScreenFadeController screenFadeController;
        
        public QuantumConsole quantumConsole;
        private float _timeScaleBeforeConsole = 1f;
        private CursorLockMode _cursorLockStateBeforeConsole = CursorLockMode.None;

        public static bool DoPlayerMovement = true;
        public static bool DoPlayerPhysics = true;

        private float _vfxFixedTimeStep;
        
        public List<SceneDependencySettingsSO> scenes = new List<SceneDependencySettingsSO>();

        [SerializeField] private PlayerInput playerInput;
        public static bool IsGamepadInput = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitOnLoad()
        {
            _instance = null;
            DoPlayerMovement = true;
            DoPlayerPhysics = true;
            LeanTween.reset();
        }
        
        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;

            FMODUnity.RuntimeManager.PlayOneShot("event:/Silence");

            Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;

            _vfxFixedTimeStep = VFXManager.fixedTimeStep;
            
            Shader.SetGlobalFloat(SubtractiveFadeAmount, 0f);
            
            quantumConsole.OnActivate += OnQcActivate;
            quantumConsole.OnDeactivate += OnQcDeactivate;
            InputUser.onChange += OnControlsChanged;
            
            ConvertES3ToSteamCloud();
        }

        private void Update()
        {
            VFXManager.fixedTimeStep = _vfxFixedTimeStep * Time.timeScale;
            Shader.SetGlobalFloat(UnscaledTime, Time.unscaledTime);

            if (quantumConsole.IsActive) return;
        }

        private void OnQcActivate()
        {
            _timeScaleBeforeConsole = Time.timeScale;
            _cursorLockStateBeforeConsole = Cursor.lockState;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
        
        private void OnQcDeactivate()
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = _cursorLockStateBeforeConsole;
        }

        public void ConvertES3ToSteamCloud()
        {
            // On launch, if the player has ES3 data, and CloudAPI is Enabled, convert the ES3 data to Steam Cloud, and delete the ES3 data.
            if (!SteamSettings.Initialized) steamSettings.Initialize();
            if (!CloudAPI.IsEnabled) return;
            
            if (!ES3.FileExists("SaveFile.es3")) return;
            
            Debug.Log("MIGRATING SAVES FROM ES3 TO STEAM CLOUD!");
            
            Dictionary<string, object> buildJson = new Dictionary<string, object>();
            foreach (var key in ES3.GetKeys("SaveFile.es3"))
            {
                buildJson[key] = ES3.Load(key);
            }

            string jsonString = JsonConvert.SerializeObject(buildJson);
            Debug.Log(jsonString);
            bool fileOverwritten = CloudAPI.FileWrite("SaveFile", jsonString);
            
            ES3.DeleteFile("SaveFile.es3");
            Debug.Log("MIGRATION SUCCESSFUL!");
        }

        public int LoadLevelRecord(string levelID)
        {
            string levelRecordKey = "level-" + levelID + "-record";
            
            if (!SteamSettings.Initialized) steamSettings.Initialize();
            if(CloudAPI.IsEnabled)
            {
                CloudAPI.GetQuota(out ulong total, out ulong remaining);
                Debug.Log("Used " + (total - remaining) + " of " + total + " bytes.");
                
                string saveDataString = CloudAPI.FileReadString("SaveFile", System.Text.Encoding.UTF8);
                Dictionary<string, object> savedJson =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(saveDataString);
                if (savedJson is null || !savedJson.TryGetValue(levelRecordKey, out var value)) return 0;
                return Convert.ToInt32(value);
            }
            else
            {
                int levelRecordMs = ES3.Load<int>(levelRecordKey, 0);
                return levelRecordMs;
            }
        }
        
        public int SaveLevelRecord(string levelID, int timeMs)
        {
            string levelRecordKey = "level-" + levelID + "-record";
            int previousRecord = LoadLevelRecord(levelID);

            if (timeMs >= previousRecord && previousRecord != 0)
                return previousRecord;
            
            if (CloudAPI.IsEnabled)
            {
                string saveDataString = CloudAPI.FileReadString("SaveFile", System.Text.Encoding.UTF8);
                Dictionary<string, object> savedJson =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(saveDataString);
                savedJson[levelRecordKey] = timeMs;
                bool fileOverwritten = CloudAPI.FileWrite("SaveFile", JsonConvert.SerializeObject(savedJson));
                return timeMs;
            }
            else
            {
                ES3.Save<int>(levelRecordKey, timeMs);
                return timeMs;
            }
        }
        
        public int LoadSelectedLevelIndex()
        {
            if (CloudAPI.IsEnabled)
            {
                string saveDataString = CloudAPI.FileReadString("SaveFile", System.Text.Encoding.UTF8);
                Dictionary<string, object> savedJson =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(saveDataString);
                if (savedJson is null || !savedJson.TryGetValue("selected-level-index", out var value)) return 0;
                return Convert.ToInt32(value);
            }
            else
            {
                return ES3.Load("selected-level-index", 0);
            }
        }

        public void SaveSelectedLevelIndex(int index)
        {
            if (CloudAPI.IsEnabled)
            {
                string saveDataString = CloudAPI.FileReadString("SaveFile", System.Text.Encoding.UTF8);
                Dictionary<string, object> savedJson =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(saveDataString);
                savedJson["selected-level-index"] = index;
                bool fileOverwritten = CloudAPI.FileWrite("SaveFile", JsonConvert.SerializeObject(savedJson));
            }
            else
            {
                ES3.Save("selected-level-index", index);
            }
        }

        private void OnControlsChanged(InputUser inputUser, InputUserChange inputUserChange, InputDevice inputDevice)
        {
            Debug.Log("Controls changed: " + inputUserChange);
            if (inputUserChange == InputUserChange.ControlSchemeChanged)
            {
                IsGamepadInput = inputUser.controlScheme?.name == "Gamepad";
                Debug.Log("Is Gamepad Input: " + IsGamepadInput);
            }
        }
        
        [Command("set-level-record")]
        private static void SetLevelRecord(string levelID, int timeMs)
        {
            Instance.SaveLevelRecord(levelID, timeMs);
        }
        
        [Command("change-scene-by-index")]
        private static void ChangeScene(int index)
        {
            GameSceneManager.LoadScene(Instance.scenes[index]);
        }
        
        private void OnDestroy()
        {
            Shader.SetGlobalFloat(SubtractiveFadeAmount, 0f);
            quantumConsole.OnActivate -= OnQcActivate;
            quantumConsole.OnDeactivate -= OnQcDeactivate;
            InputUser.onChange -= OnControlsChanged;
        }
    }
}