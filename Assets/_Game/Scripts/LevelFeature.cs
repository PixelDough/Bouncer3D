using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace PixelDough.Bouncer
{
    [ExecuteInEditMode]
    public abstract class LevelFeature : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;
        protected virtual void OnValidate()
        {
            if (levelManager) return;
            if (gameObject.scene.name == null || gameObject.scene.name == gameObject.name) return;
            #if UNITY_EDITOR
            if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;
            #endif
            levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.RegisterLevelFeature(this);
        }
        
        protected virtual void OnDestroy()
        {
            if (levelManager) levelManager.DeregisterLevelFeature(this);
        }

        public abstract void Initialize();
    }
}
