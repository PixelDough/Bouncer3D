using System;
using System.Collections.Generic;
using DG.Tweening;
using Tools.SceneDependencies;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PixelDough.Bouncer
{
    public class LevelSelect : MonoBehaviour
    { 
        [SerializeField] private Transform carouselContent;
        [SerializeField] private List<LevelSelectButton> levelSelectButtons = new List<LevelSelectButton>();
        [SerializeField] private Transform characterTransform;
        [SerializeField] private Font3DString levelNameText;
        [SerializeField] private List<Transform> medalRoots;
        [SerializeField] private SceneDependencySettingsSO mainMenuScene;
        
        [Header("Input Actions")]
        [SerializeField] private InputActionReference uiMoveAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference backAction;

        [Header("FMOD Events")] 
        [SerializeField] private FMODUnity.StudioEventEmitter tvChangeEvent;
        [SerializeField] private FMODUnity.StudioEventEmitter tvChatterEvent;
        [SerializeField] private FMODUnity.EventReference levelSelectEvent;
        [SerializeField] private FMODUnity.EventReference selectErrorEvent;
        
        private List<GameLevelDataSO> _levels = new List<GameLevelDataSO>();
        private int _currentLevelIndex = 0;

        private bool _isEnteringLevel = false;
 
        private void Start()
        {
            _levels = GameManager.Instance.gameLevels;
            for (int i = 0; i < _levels.Count; i++)
            {
                GameLevelDataSO levelData = _levels[i];
                levelSelectButtons[i].SetLevelData(levelData);
            }
            
            _currentLevelIndex = GameManager.Instance.LoadSelectedLevelIndex();
            carouselContent.localEulerAngles = new Vector3(0, _currentLevelIndex * 30f, 0f);
            
            UpdateCurrentLevelInfo();
        }

        private void Update()
        {
            characterTransform.localPosition = Vector3.Lerp(Vector3.zero,
                Vector3.up * 0.1f, Mathf.InverseLerp(-1, 1, Mathf.Cos(Time.time)));

            foreach (Transform medalRoot in medalRoots)
            {
                medalRoot.localEulerAngles = new Vector3(Mathf.Sin(Time.time * 2f) * 15, Mathf.Cos(Time.time * 2f) * 15, 0f);
            }
            
            for (int i = 0; i < levelSelectButtons.Count; i++)
            {
                levelSelectButtons[i].UpdateButton(i == _currentLevelIndex);
            }
            
            bool wasPressedThisFrame = uiMoveAction.action.WasPressedThisFrame();
            if (wasPressedThisFrame)
            {
                int pressedDirection = MathHelpers.Sign(uiMoveAction.action.ReadValue<Vector2>().x);
                _currentLevelIndex += pressedDirection;
                
                if (_currentLevelIndex < 0)
                    _currentLevelIndex = levelSelectButtons.Count - 1;
                else if (_currentLevelIndex >= levelSelectButtons.Count)
                    _currentLevelIndex = 0;

                UpdateCurrentLevelInfo();
                
                tvChangeEvent.Play();
                
                DOTween.Kill(carouselContent);
                carouselContent.DOLocalRotate(new Vector3(0, _currentLevelIndex * 30f, 0f), 0.5f)
                    .SetEase(Ease.OutBack);
            }

            HandleSelectLevel();

            HandleBack();
        }

        private void UpdateCurrentLevelInfo()
        {
            if (_currentLevelIndex < _levels.Count)
            {
                levelNameText.SetText(_levels[_currentLevelIndex].levelName);
            }
            else
            {
                levelNameText.SetText("???");
                tvChatterEvent.Play();
            }
            levelNameText.AnimPulse();
            
            UpdateLevelRecord();
        }

        private void UpdateLevelRecord()
        {
            String levelRecord = "Unavailable";

            if (_currentLevelIndex < _levels.Count)
            {
                int levelRecordMs = GameManager.Instance.LoadLevelRecord(_levels[_currentLevelIndex].levelID);
                if (levelRecordMs > 0)
                {
                    levelRecord = TimeSpan.FromMilliseconds(levelRecordMs).ToString("mm':'ss'.'fff");
                }
            }

            // levelRecordText.SetText("Best Time: " + Environment.NewLine + levelRecord);
            // levelRecordText.AnimPulse();
        }

        private void HandleSelectLevel()
        {
            if (GameSceneManager.IsChangingScenes) return;
            if (!jumpAction.action.WasPressedThisFrame()) return;
            if (_isEnteringLevel) return;
            if (_currentLevelIndex >= _levels.Count)
            {
                FMODUnity.RuntimeManager.PlayOneShot(selectErrorEvent);
                return;
            }
            _isEnteringLevel = true;
            GameSceneManager.LoadScene(_levels[_currentLevelIndex].sceneDependencySettings);
            FMODUnity.RuntimeManager.PlayOneShot(levelSelectEvent);
        }

        private void HandleBack()
        {
            if (GameSceneManager.IsChangingScenes) return;
            if (!backAction.action.triggered) return;
            if (_isEnteringLevel) return;
            _isEnteringLevel = true;
            GameSceneManager.LoadScene(mainMenuScene);
            FMODUnity.RuntimeManager.PlayOneShot(levelSelectEvent);
        }

        private void OnDestroy()
        {
            GameManager.Instance.SaveSelectedLevelIndex(_currentLevelIndex);
        }
    }
}
