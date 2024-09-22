using System;
using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField] private Font3DString levelRecordText;
        [SerializeField] private InputActionReference uiMoveAction;
        [SerializeField] private InputActionReference jumpAction;

        [Header("FMOD Events")] 
        [SerializeField] private FMODUnity.StudioEventEmitter tvChangeEvent;
        [SerializeField] private FMODUnity.StudioEventEmitter tvChatterEvent;
        
        private List<GameLevelDataSO> levels = new List<GameLevelDataSO>();
        private int _currentLevelIndex = 0;

        private bool _isEnteringLevel = false;
 
        private void Start()
        {
            levels = GameManager.Instance.gameLevels;
            for (int i = 0; i < levels.Count; i++)
            {
                GameLevelDataSO levelData = levels[i];
                levelSelectButtons[i].SetLevelData(levelData);
            }
            
            UpdateCurrentLevelInfo();
        }

        private void Update()
        {
            characterTransform.localPosition = Vector3.Lerp(Vector3.zero,
                Vector3.up * 0.1f, Mathf.InverseLerp(-1, 1, Mathf.Cos(Time.time)));
            
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
                tvChatterEvent.Stop();
                tvChatterEvent.Play();
                
                DOTween.Kill(carouselContent);
                carouselContent.DOLocalRotate(new Vector3(0, _currentLevelIndex * 30f, 0f), 0.5f)
                    .SetEase(Ease.OutBack);
            }

            HandleSelectLevel();
        }

        private void UpdateCurrentLevelInfo()
        {
            if (_currentLevelIndex < levels.Count)
            {
                levelNameText.SetText(levels[_currentLevelIndex].levelName);
            }
            else
            {
                levelNameText.SetText("???");
            }
            levelNameText.AnimPulse();
            
            UpdateLevelRecord();
        }

        private void UpdateLevelRecord()
        {
            String levelRecord = "Unavailable";

            if (_currentLevelIndex < levels.Count)
            {
                int levelRecordMs = GameManager.Instance.LoadLevelRecord(levels[_currentLevelIndex].levelID);
                if (levelRecordMs > 0)
                {
                    levelRecord = TimeSpan.FromMilliseconds(levelRecordMs).ToString("mm':'ss'.'fff");
                }
            }

            levelRecordText.SetText("Best Time: \n " + levelRecord);
            levelRecordText.AnimPulse();
        }

        private void HandleSelectLevel()
        {
            if (!jumpAction.action.WasPressedThisFrame()) return;
            if (_currentLevelIndex >= levels.Count) return;
            if (_isEnteringLevel) return;
            _isEnteringLevel = true;
            GameSceneManager.LoadScene(levels[_currentLevelIndex].sceneDependencySettings);
        }
    }
}
