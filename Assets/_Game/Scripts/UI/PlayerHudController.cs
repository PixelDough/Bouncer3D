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
        
        [Header("Finish UI")]
        [SerializeField] private CanvasGroup finishCanvasGroup;
        [SerializeField] private TextMeshProUGUI finishTimerText;
        
        [Header("Pause UI")]
        [SerializeField] private CanvasGroup pauseCanvasGroup;
        
        private bool _isVisible = true;

        private void Start()
        {
            RenderManager.AddCameraToStack(hudCamera);
        }

        private void OnDestroy()
        {
            RenderManager.RemoveCameraFromStack(hudCamera);
        }

        private void Update()
        {
            timerText.text = levelManager.LevelTime.ToString(levelManager.LevelTime.Hours > 0 ? @"hh\:mm\:ss\.fff" : @"mm\:ss\.fff");

            finishTimerText.text = timerText.text;
            finishCanvasGroup.alpha = levelManager.LevelState == LevelManager.LevelStates.Finished ? 1 : 0;
        }

        public void SetVisibility(bool state, bool doAnimation = true)
        {
            _isVisible = state;

            canvasGroup.alpha = state ? 1 : 0;
        }
        
    }
}
