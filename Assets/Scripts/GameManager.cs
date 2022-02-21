using System;
using QFSW.QC;
using Rewired;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace PixelDough.Bouncer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                if (!_instance) _instance = FindObjectOfType<GameManager>();
                return _instance;
            }
            private set => _instance = value;
        }
        private static GameManager _instance;

        //[SerializeField] private Volume globalVolume;

        public ScreenFadeController screenFadeController;
        
        public Player Input;

        public static bool DebugOverlay = false;

        public QuantumConsole quantumConsole;
        private float _timeScaleBeforeConsole = 1f;

        public static bool DoPlayerMovement = true;
        public static bool DoPlayerPhysics = true;

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            FMODUnity.RuntimeManager.PlayOneShot("event:/Silence");

            Input = ReInput.players.GetPlayer(0);

            Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        private void Update()
        {
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

        [Command("change-scene")]
        private static void ChangeScene(string sceneName)
        {
            GameSceneManager.LoadScene(sceneName);
        }

        [Command("reload-scene")]
        private static void ReloadScene()
        {
            GameSceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}