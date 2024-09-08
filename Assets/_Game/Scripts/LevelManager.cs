using System;
using System.Collections.Generic;
using PixelDough.Bouncer.LevelData;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace PixelDough.Bouncer
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        [SerializeField] private ZoneDataScriptableObject zoneData;
        [SerializeField] private List<LevelFeature> levelFeatures = new List<LevelFeature>();
        
        public static TimeSpan LevelTime;
        public static bool CountingTime = false;

        private void OnValidate()
        {
            levelFeatures.TrimExcess();
            levelFeatures.RemoveAll(feature => 
                feature == null || feature.gameObject.scene.name == null || feature.gameObject.scene.name == feature.gameObject.name
            );
        }

        private void Start()
        {
            Instance = this;
            
            CountingTime = false;
            ResetTimer();
            
            levelFeatures.TrimExcess();
            levelFeatures.RemoveAll(feature => 
                feature == null || feature.gameObject.scene.name == null || feature.gameObject.scene.name == feature.gameObject.name
            );
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

        public void RegisterLevelFeature(LevelFeature levelFeature)
        {
            if (levelFeatures.Contains(levelFeature)) return;
            levelFeatures.Add(levelFeature);
        }
        
        public void DeregisterLevelFeature(LevelFeature levelFeature)
        {
            levelFeatures.Remove(levelFeature);
        }

        [Button]
        public void RegenerateLevelFeatureList()
        {
            levelFeatures.Clear();
            levelFeatures.AddRange(FindObjectsByType<LevelFeature>(FindObjectsInactive.Include,
                FindObjectsSortMode.None));
        }
        
        public void ResetLevelElements()
        {
            levelFeatures.ForEach(feature => feature.Initialize());
        }

        private void OnDestroy()
        {
            Instance = null;
        }

    }
}
