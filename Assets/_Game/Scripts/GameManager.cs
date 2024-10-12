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
            
            if (!SteamSettings.Initialized)
            {
                steamSettings.Initialize();
            }
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
        
        public int LoadLevelRecord(string levelID)
        {
            string levelRecordKey = "level-" + levelID + "-record";
            int levelRecordMs = ES3.Load<int>(levelRecordKey, 0);
            return levelRecordMs;
        }
        
        public int SaveLevelRecord(string levelID, int timeMs)
        {
            string levelRecordKey = "level-" + levelID + "-record";
            int previousRecord = LoadLevelRecord(levelID);

            if (timeMs >= previousRecord && previousRecord != 0)
                return previousRecord;
            
            ES3.Save<int>(levelRecordKey, timeMs);
            return timeMs;
        }
        
        public int LoadSelectedLevelIndex()
        {
            return ES3.Load("selected-level-index", 0);
        }

        public void SaveSelectedLevelIndex(int index)
        {
            ES3.Save("selected-level-index", index);
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