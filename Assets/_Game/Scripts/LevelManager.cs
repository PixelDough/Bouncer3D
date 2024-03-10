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

            List<CollectableSpawner> collectableSpawners =
                new List<CollectableSpawner>(FindObjectsByType<CollectableSpawner>(FindObjectsSortMode.None));
            
            LevelProgress = new LevelProgress();
            LevelProgress.Initialize(collectableSpawners, zoneData);

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
