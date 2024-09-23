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
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Camera hudCamera;
        
        [Header("Timer")] 
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI finishTimerText;
        [SerializeField] private CanvasGroup finishCanvasGroup;
        
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
            if (LevelManager.LevelTime.Hours > 0)
                timerText.text = LevelManager.LevelTime.ToString(@"hh\:mm\:ss\.fff");
            else
                timerText.text = LevelManager.LevelTime.ToString(@"mm\:ss\.fff");

            finishTimerText.text = timerText.text;
            finishCanvasGroup.alpha = LevelManager.LevelState == LevelManager.LevelStates.Finished ? 1 : 0;
        }

        public void SetVisibility(bool state, bool doAnimation = true)
        {
            _isVisible = state;

            canvasGroup.alpha = state ? 1 : 0;
        }
        
    }
}
