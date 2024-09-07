using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace PixelDough.Bouncer
{
    public abstract class LevelFeature : MonoBehaviour
    {
        [SerializeField, HideInInspector] private LevelManager levelManager;
        #if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (levelManager) return;
            if (gameObject.scene.name == null || gameObject.scene.name == gameObject.name) return;
            if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;
            levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.RegisterLevelFeature(this);
        }
        #endif

        public abstract void Initialize();
    }
}
