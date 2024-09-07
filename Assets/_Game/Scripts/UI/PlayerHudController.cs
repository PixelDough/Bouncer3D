using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelDough.Bouncer.UI
{
    public class PlayerHudController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Shell Counter")]
        [SerializeField] private Image shellImage;
        [SerializeField] private Animator shellAnimator;
        [SerializeField] private TextMeshProUGUI shellCountText;
        
        private int _localShellCount = 0;
        
        [SerializeField] private RectTransform shellFlying;

        [Header("Timer")] 
        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private Transform uiItemIconTransform;
        private float uiItemIconSpeed = 1f;

        private bool _isVisible = true;
        
        private void Update()
        {
            shellAnimator.speed = Mathf.Lerp(shellAnimator.speed, 1f, Time.deltaTime);
            uiItemIconSpeed = Mathf.Lerp(uiItemIconSpeed, 1f, Time.deltaTime);

            if (LevelManager.LevelTime.Hours > 0)
                timerText.text = LevelManager.LevelTime.ToString(@"hh\:mm\:ss\.fff");
            else
                timerText.text = LevelManager.LevelTime.ToString(@"mm\:ss\.fff");
            
            uiItemIconTransform.Rotate(Vector3.up * (360 * uiItemIconSpeed * Time.deltaTime));
        }

        public void SetVisibility(bool state, bool doAnimation = true)
        {
            _isVisible = state;

            if (state)
            {
                canvasGroup.alpha = 1;
            }
            else
            {
                canvasGroup.alpha = 0;
            }
        }
        
    }
}
