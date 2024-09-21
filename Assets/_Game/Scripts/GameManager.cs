using System;
using System.Collections.Generic;
using Febucci.UI;
using PixelDough.Bouncer.UI;
using QFSW.QC;
using Rewired;
using TMPro;
using Tools.SceneDependencies;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
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

        //[SerializeField] private Volume globalVolume;

        public ScreenFadeController screenFadeController;

        public PlayerHudController playerHudController;
        public Camera uiCamera;
        
        public Player Input;

        public QuantumConsole quantumConsole;
        private float _timeScaleBeforeConsole = 1f;

        public static bool DoPlayerMovement = true;
        public static bool DoPlayerPhysics = true;

        private float _vfxFixedTimeStep;
        
        public List<SceneDependencySettingsSO> scenes = new List<SceneDependencySettingsSO>();

        public Countdown Countdown;

        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private TypewriterByCharacter tutorialTextTypewriter;

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

            Input = ReInput.players.GetPlayer(0);

            Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;

            _vfxFixedTimeStep = VFXManager.fixedTimeStep;
            
            Shader.SetGlobalFloat(SubtractiveFadeAmount, 0f);
        }

        private void Update()
        {
            VFXManager.fixedTimeStep = _vfxFixedTimeStep * Time.timeScale;
            Shader.SetGlobalFloat(UnscaledTime, Time.unscaledTime);
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
            {
                quantumConsole.Toggle();
                
                if (quantumConsole.IsActive)
                {
                    _timeScaleBeforeConsole = Time.timeScale;
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Time.timeScale = _timeScaleBeforeConsole;
                }
            }

            if (quantumConsole.IsActive) return;
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                /*if (globalVolume.profile.TryGet(out AnalogSignalVolume analogVolume))
                {
                    analogVolume.analogSignalEnabled.value = !analogVolume.analogSignalEnabled.value;
                }*/
            }

            

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
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