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
        [SerializeField] private List<GameLevelDataSO> levels = new List<GameLevelDataSO>();
        [SerializeField] private List<LevelSelectButton> levelSelectButtons = new List<LevelSelectButton>();
        [SerializeField] private Font3DString levelNameText;
        [SerializeField] private InputActionReference uiMoveAction;
        
        private int _currentLevelIndex = 0;
 
        private void Start()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                GameLevelDataSO levelData = levels[i];
                levelSelectButtons[i].SetLevelData(levelData);
            }
            
            UpdateCurrentLevelInfo();
        }

        private void Update()
        {
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
                
                DOTween.Kill(carouselContent);
                carouselContent.DOLocalRotate(new Vector3(0, _currentLevelIndex * 30f, 0f), 0.5f)
                    .SetEase(Ease.OutBack);
            }
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
        }
    }
}
