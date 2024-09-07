using System;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public abstract class LevelFeature : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;
        protected virtual void OnValidate()
        {
            if (levelManager) return;
            levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.RegisterLevelFeature(this);
        }

        public abstract void Initialize();
    }
}
