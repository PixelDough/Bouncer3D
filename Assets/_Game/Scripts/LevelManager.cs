using System;
using System.Collections.Generic;
using PixelDough.Bouncer.LevelData;
using UnityEngine;
using UnityEngine.Pool;

namespace PixelDough.Bouncer
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        [SerializeField] private ZoneDataScriptableObject zoneData;
        
        public static TimeSpan LevelTime;
        public static bool CountingTime = false;

        public LevelProgress LevelProgress;

        private void Start()
        {
            Instance = this;

            LevelProgress = new LevelProgress();
            LevelProgress.Initialize(
                new List<CollectableController>(FindObjectsByType<CollectableController>(FindObjectsSortMode.None)));
            
            foreach (var shell in LevelProgress.AllCollectables)
            {
                if (zoneData) shell.SetMesh(zoneData.collectableMesh, zoneData.collectableMaterial);
            }

            CountingTime = false;
            ResetTimer();
        }

        private void Update()
        {
            if (CountingTime)
                LevelTime = LevelTime.Add(TimeSpan.FromSeconds(Time.deltaTime));
        }

        public static void ResetTimer()
        {
            LevelTime = new TimeSpan(0, 0, 0, 0, 0);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
