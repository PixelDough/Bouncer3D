using System;
using System.Collections.Generic;
using MEC;
using Mono.CSharp.Linq;
using Tools.SceneDependencies;
using UnityEditor.Build.Content;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class GoalRing : MonoBehaviour
    {
        [SerializeField] private SceneDependencySettingsSO nextSceneDemo;
        
        private bool _hit = false;

        private bool _continuePressed = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_hit) return;
            Rigidbody rbHit = other.attachedRigidbody;
            if (rbHit is null) return;
            if (!rbHit.CompareTag("Player")) return;
            
            Debug.Log("Player reached goal ring!");
            _hit = true;
            Time.timeScale = 0.0f;

            LevelManager.StopTimer();
        }

        private void Update()
        {
            if (LevelManager.LevelState != LevelManager.LevelStates.Finished) return;
            if (_continuePressed) return;

            if (GameManager.Instance.Input.GetButtonDown(RewiredConsts.Action.Jump))
            {
                Debug.Log("Continue pressed!");
                _continuePressed = true;
                GameSceneManager.LoadScene(nextSceneDemo);
            }
        }
    }
}
