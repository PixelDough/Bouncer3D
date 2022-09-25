using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelDough.Bouncer
{
    public static class GameSceneManager
    {

        public static Action OnSceneLoaded;

        public static bool IsChangingScenes => _isChangingScenes;
        private static bool _isChangingScenes;

        public static void LoadScene(string sceneName)
        {
            if (_isChangingScenes) return;
            
            Timing.RunCoroutine(C_LoadScene(sceneName));
        }

        private static IEnumerator<float> C_LoadScene(string sceneName)
        {
            _isChangingScenes = true;
            
            // Fade out
            int fadeID = GameManager.Instance.screenFadeController.FadeToBlack().uniqueId;
            while (LeanTween.isTweening(fadeID)) yield return Timing.WaitForOneFrame;

            // Wait for scene to load
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            yield return Timing.WaitUntilDone(asyncOperation);

            // Wait a buffer time for any start methods to process and lag the game
            yield return Timing.WaitForSeconds(1f);

            // Fade from black, and set the isChangingScenes variable to false on complete
            GameManager.Instance.screenFadeController.FadeFromBlack().setOnComplete(() => _isChangingScenes = false);
            
            // Run any events scheduled to run once the scene is loaded and finished buffering
            OnSceneLoaded?.Invoke();
        }
    }
}
