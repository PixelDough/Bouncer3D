using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Tools.SceneDependencies;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelDough.Bouncer
{
    public static class GameSceneManager
    {

        public static Action OnSceneLoaded;

        public static bool IsChangingScenes => _isChangingScenes;
        private static bool _isChangingScenes;

        public static void LoadScene(SceneDependencySettingsSO sceneDependencySettings)
        {
            if (_isChangingScenes) return;
            
            Timing.RunCoroutine(C_LoadScene(sceneDependencySettings), Segment.RealtimeUpdate);
        }

        private static IEnumerator<float> C_LoadScene(SceneDependencySettingsSO sceneDependencySettings)
        {
            _isChangingScenes = true;
            Time.timeScale = 0f;
            
            // Fade out
            int fadeID = GameManager.Instance.screenFadeController.FadeToBlack().uniqueId;
            while (LeanTween.isTweening(fadeID)) yield return Timing.WaitForOneFrame;

            // Wait for scene to load
            Awaitable asyncOperation = SceneDependencyManager.LoadScene(sceneDependencySettings);
            while (!asyncOperation.IsCompleted)
            {
                yield return Timing.WaitForOneFrame;
            }

            // Wait a buffer time for any start methods to process and lag the game
            yield return Timing.WaitForSeconds(1f);
            Time.timeScale = 1f;

            // Fade from black, and set the isChangingScenes variable to false on complete
            GameManager.Instance.screenFadeController.FadeFromBlack().setOnComplete(() => _isChangingScenes = false);
            
            // Run any events scheduled to run once the scene is loaded and finished buffering
            OnSceneLoaded?.Invoke();
        }
    }
}
