using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelDough.Bouncer.UI
{
    public class PlayerHudController : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Camera hudCamera;
        
        [Header("Timer")] 
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private RawImage stopwatchUI;
        
        [Header("Finish UI")]
        [SerializeField] private CanvasGroup finishCanvasGroup;
        [SerializeField] private TextMeshProUGUI finishTimerText;
        
        private bool _isVisible = true;

        private void Start()
        {
            RenderManager.AddCameraToStack(hudCamera);
            UpdateTimer();
        }

        private void OnDestroy()
        {
            RenderManager.RemoveCameraFromStack(hudCamera);
        }

        private void Update()
        {
            UpdateTimer();

            finishTimerText.text = timerText.text;
            finishCanvasGroup.alpha = levelManager.LevelState == LevelManager.LevelStates.Finished ? 1 : 0;
        }

        public void SetVisibility(bool state, bool doAnimation = true)
        {
            _isVisible = state;

            canvasGroup.alpha = state ? 1 : 0;
        }

        private void UpdateTimer()
        {
            timerText.text = levelManager.LevelTime.ToString(levelManager.LevelTime.Hours > 0 ? @"hh\:mm\:ss\.fff" : @"mm\:ss\.fff");
            timerText.gameObject.SetActive(GameManager.Instance.singlePlayerMode == GameManager.SinglePlayerMode.SpeedRun);
        }
    }
}
