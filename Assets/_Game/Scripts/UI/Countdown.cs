using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using QFSW.QC;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class Countdown : MonoBehaviour
    {
        [SerializeField] private Transform[] countdownTransforms;

        private bool _isPlayingCountdown = false;

        private void Start()
        {
            foreach (Transform countdownTransform in countdownTransforms)
            {
                countdownTransform.gameObject.SetActive(false);
            }
        }

        [Command("play-countdown", "Plays the countdown animation")]
        public void PlayCountdown()
        {
            if (_isPlayingCountdown) return;
            _isPlayingCountdown = true;
            
            Timing.RunCoroutine(C_CountdownCoroutine(), Segment.RealtimeUpdate);
        }
        
        private IEnumerator<float> C_CountdownCoroutine()
        {
            LevelManager.LevelState = LevelManager.LevelStates.Intro;
            GameManager.DoPlayerMovement = false;
            // GameManager.DoPlayerPhysics = false;
            LevelManager.CountingTime = false;
            LevelManager.ResetTimer();
            
            foreach (var timerItem in countdownTransforms)
            {
                timerItem.gameObject.SetActive(true);
                timerItem.localScale = Vector3.zero;
                timerItem.eulerAngles = new Vector3(-90f, 0f, 0f);
                timerItem.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
                timerItem.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);
                yield return Timing.WaitForSeconds(0.5f);
                timerItem.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCubic).SetUpdate(true);
                timerItem.DORotate(new Vector3(90f, 0f, 0f), 0.5f).SetEase(Ease.InCubic).SetUpdate(true);
                yield return Timing.WaitForSeconds(0.5f);
                timerItem.gameObject.SetActive(false);
            }
            
            Time.timeScale = 1f;
            _isPlayingCountdown = false;
            LevelManager.LevelState = LevelManager.LevelStates.Playing;
            
            GameManager.DoPlayerMovement = true;
            // GameManager.DoPlayerPhysics = true;
            LevelManager.CountingTime = true;
        }
    }
}
