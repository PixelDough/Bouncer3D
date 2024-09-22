using System;
using System.Collections.Generic;
using Febucci.UI;
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
        
        public List<GameLevelDataSO> gameLevels = new List<GameLevelDataSO>();
        public int currentLevelDataIndex;
        public GameLevelDataSO CurrentLevelData => gameLevels[currentLevelDataIndex];

        //[SerializeField] private Volume globalVolume;

        public ScreenFadeController screenFadeController;

        public PlayerHudController playerHudController;
        public Camera uiCamera;

        public QuantumConsole quantumConsole;
        private float _timeScaleBeforeConsole = 1f;
        private CursorLockMode _cursorLockStateBeforeConsole = CursorLockMode.None;

        public static bool DoPlayerMovement = true;
        public static bool DoPlayerPhysics = true;

        private float _vfxFixedTimeStep;
        
        public List<SceneDependencySettingsSO> scenes = new List<SceneDependencySettingsSO>();

        public Countdown Countdown;

        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private TypewriterByCharacter tutorialTextTypewriter;

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
            int previousRecord = ES3.Load<int>(levelRecordKey, 0);
            if (timeMs < previousRecord || previousRecord == 0)
            {
                ES3.Save<int>(levelRecordKey, timeMs);
                return timeMs;
            }

            return previousRecord;
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
        
        public void CutsceneBegin()
        {
            playerHudController.SetVisibility(false);
        }
        
        public void CutsceneEnded()
        {
            playerHudController.SetVisibility(true);
        }

        private void OnDestroy()
        {
            Shader.SetGlobalFloat(SubtractiveFadeAmount, 0f);
            quantumConsole.OnActivate -= OnQcActivate;
            quantumConsole.OnDeactivate -= OnQcDeactivate;
            InputUser.onChange -= OnControlsChanged;
        }
        
        public void ShowTutorialText(string text)
        {
            tutorialText.text = text;
            tutorialTextTypewriter.StopDisappearingText();
            tutorialTextTypewriter.StartShowingText(true);
        }

        public void HideTutorialText()
        {
            tutorialTextTypewriter.StopShowingText();
            tutorialTextTypewriter.StartDisappearingText();
        }
    }
}