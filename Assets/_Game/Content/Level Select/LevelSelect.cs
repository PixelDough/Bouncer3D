using System;
using System.Collections.Generic;
using DG.Tweening;
using Tools.SceneDependencies;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PixelDough.Bouncer
{
    public class LevelSelect : MonoBehaviour
    { 
        [SerializeField] private Transform carouselContent;
        [SerializeField] private Transform characterTransform;
        [SerializeField] private SceneDependencySettingsSO mainMenuScene;
        
        [Header("Level Select")]
        [SerializeField] private List<LevelSelectButton> levelSelectButtons = new List<LevelSelectButton>();
        [SerializeField] private CinemachineCamera levelSelectCam;

        [Header("Mode Select")] 
        [SerializeField] private Transform modeUpArrow;
        [SerializeField] private Transform modeDownArrow;
        [SerializeField] private List<ModeButton> modeButtons;
        [SerializeField] private Font3DString speedrunScoreText;
        [SerializeField] private Font3DString timeAttackScoreText;
        
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
            
            levelSelectButtons[_currentLevelIndex].Highlight();
        }

        private void Update()
        {
            characterTransform.localPosition = Vector3.Lerp(Vector3.zero,
                Vector3.up * 0.1f, Mathf.InverseLerp(-1, 1, Mathf.Cos(Time.time)));
            
            for (int i = 0; i < levelSelectButtons.Count; i++)
            {
                levelSelectButtons[i].UpdateButton(i == _currentLevelIndex);
            }
            
            HandleBack();

            int gameModeInt = (int)GameManager.Instance.singlePlayerMode;
            modeUpArrow.gameObject.SetActive(gameModeInt > 0);
            modeDownArrow.gameObject.SetActive(gameModeInt < modeButtons.Count - 1);
            
            bool wasPressedThisFrame = uiMoveAction.action.WasPressedThisFrame();
            if (wasPressedThisFrame)
            {
                Vector2 moveValue = uiMoveAction.action.ReadValue<Vector2>();
                int pressedDirectionX = MathHelpers.Sign(moveValue.x);
                int pressedDirectionY = MathHelpers.Sign(moveValue.y);
                if (pressedDirectionX != 0)
                {
                    levelSelectButtons[_currentLevelIndex].Unhighlight();
                    _currentLevelIndex += pressedDirectionX;

                    if (_currentLevelIndex < 0)
                        _currentLevelIndex = levelSelectButtons.Count - 1;
                    else if (_currentLevelIndex >= levelSelectButtons.Count)
                        _currentLevelIndex = 0;

                    levelSelectButtons[_currentLevelIndex].Highlight();

                    UpdateCurrentLevelInfo();

                    tvChangeEvent.Play();

                    carouselContent.DOKill();
                    carouselContent.DOLocalRotate(new Vector3(0, _currentLevelIndex * 30f, 0f), 0.5f)
                        .SetEase(Ease.OutBack);
                } 
                else if (pressedDirectionY != 0)
                {
                    if (Mathf.Clamp(gameModeInt - pressedDirectionY, 0, modeButtons.Count - 1) == gameModeInt) return;
                    GameManager.Instance.singlePlayerMode =
                        (GameManager.SinglePlayerMode)Mathf.Clamp(gameModeInt - pressedDirectionY, 0,
                            modeButtons.Count - 1);
                    modeButtons[gameModeInt].Hide();
                    gameModeInt -= pressedDirectionY;
                    modeButtons[gameModeInt].Show();
                    
                    Transform arrowTransform = pressedDirectionY > 0 ? modeUpArrow : modeDownArrow;
                    arrowTransform.DOKill(true);
                    arrowTransform.DOPunchPosition(Vector3.up * (pressedDirectionY * 0.1f), 0.25f, 4);
                }
            }

            HandleSelectLevel();
        }

        private void UpdateCurrentLevelInfo()
        {
            if (_currentLevelIndex < _levels.Count)
            {
                
            }
            else
            {
                tvChatterEvent.Play();
            }
            
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

                var levelData = _levels[_currentLevelIndex];
            }

            // levelRecordText.SetText("Best Time: " + Environment.NewLine + levelRecord);
            // levelRecordText.AnimPulse();
        }

        private Medal.MedalState GetMedalStateForRecord(GameLevelDataSO levelData, int recordMs)
        {
            return recordMs == 0 ? Medal.MedalState.None :
                recordMs <= levelData.platinumTime ? Medal.MedalState.Platinum :
                recordMs <= levelData.goldTime ? Medal.MedalState.Gold :
                recordMs <= levelData.silverTime ? Medal.MedalState.Silver :
                recordMs <= levelData.bronzeTime ? Medal.MedalState.Bronze : 
                Medal.MedalState.None;
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
