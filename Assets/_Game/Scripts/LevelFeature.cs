using System;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace PixelDough.Bouncer
{
    public abstract class LevelFeature : MonoBehaviour
    {
        [SerializeField, ReadOnly] public LevelManager levelManager;
        protected virtual void OnValidate()
        {
            if (Application.isPlaying) return;
            if (gameObject.scene.name == null || gameObject.scene.name == gameObject.name) return;
            #if UNITY_EDITOR
            if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;
            #endif
            levelManager ??= FindFirstObjectByType<LevelManager>();
            levelManager?.RegisterLevelFeature(this);
        }

        public abstract void Initialize();
    }
}
