using System;
using System.Collections.Generic;
using Tools.SceneDependencies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelDough.Bouncer
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private SceneDependencySettingsSO levelSelectScene;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private List<Transform> stuff = new List<Transform>();
        [SerializeField] private Font3DString infoText;

        [SerializeField] private FMODUnity.EventReference continueSound;

        private readonly List<Vector3> _originalPositions = new List<Vector3>();

        private bool _isLoadingScene = false;

        private void Start()
        {
            for (var i = 0; i < stuff.Count; i++)
            {
                _originalPositions.Add(stuff[i].position);
            }
            
            infoText.SetText("Annie's Dev Den - 2024 - " + Application.version);
        }

        private void Update()
        {
            for (var i = 0; i < stuff.Count; i++)
            {
                float timeOffset = i * 59201.125f;
                stuff[i].position = _originalPositions[i] + new Vector3(0, Mathf.Sin((Time.time + timeOffset) * 0.5f) * 0.25f, 0);
            }

            HandleLoadLevelSelect();
        }

        private void HandleLoadLevelSelect()
        {
            if (GameSceneManager.IsChangingScenes) return;
            if (_isLoadingScene) return;
            if (!jumpAction.action.WasPressedThisFrame()) return;
            _isLoadingScene = true;
            GameSceneManager.LoadScene(levelSelectScene);
            FMODUnity.RuntimeManager.PlayOneShot(continueSound);
        }
    }
}
