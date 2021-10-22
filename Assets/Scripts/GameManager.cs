using System;
using HauntedPSX.RenderPipelines.PSX.Runtime;
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

        [SerializeField] private Volume globalVolume;
        
        public Player Input;

        public static bool DebugOverlay = false;

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
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
            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                /*if (globalVolume.profile.TryGet(out AnalogSignalVolume analogVolume))
                {
                    analogVolume.analogSignalEnabled.value = !analogVolume.analogSignalEnabled.value;
                }*/
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}