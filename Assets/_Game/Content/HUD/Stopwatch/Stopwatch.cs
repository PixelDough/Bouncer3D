using System;
using DG.Tweening;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class Stopwatch : MonoBehaviour
    {
        [SerializeField] private Transform stopwatchTransform;
        [SerializeField] private Transform stopwatchSecondHand;
        [SerializeField] private Transform stopwatchMinuteHand;
        
        private int _lastSeconds;
        private int _lastMinutes;

        private void Update()
        {
            stopwatchTransform.localEulerAngles = new Vector3(0, Mathf.Cos(Time.time * 3f) * 15f, 0);
        }

        void LateUpdate()
        {
            if (LevelManager.LevelTime.Seconds != _lastSeconds)
            {
                Vector3 newRotation = new Vector3(0, 0, Mathf.FloorToInt((float)LevelManager.LevelTime.TotalSeconds) * 6f);
                stopwatchSecondHand.DOLocalRotate(newRotation, 0.75f, RotateMode.Fast).SetEase(Ease.OutElastic);
            }
            if (LevelManager.LevelTime.Minutes != _lastMinutes)
            {
                Vector3 newRotation = new Vector3(0, 0, Mathf.FloorToInt((float)LevelManager.LevelTime.TotalMinutes) * 6f);
                stopwatchMinuteHand.DOLocalRotate(newRotation, 0.75f, RotateMode.Fast).SetEase(Ease.OutElastic);
            }
            
            _lastSeconds = LevelManager.LevelTime.Seconds;
            _lastMinutes = LevelManager.LevelTime.Minutes;
        }
    }
}
