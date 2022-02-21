using System;
using System.Collections.Generic;
using PixelDough.Bouncer.LevelData;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        [SerializeField] private ZoneDataScriptableObject zoneData;
        
        private List<CollectableController> _shells = new List<CollectableController>();
        
        private int _totalShells = 0;
        public int TotalShells => _totalShells;
        
        public int shellsCollected = 0;

        public static TimeSpan LevelTime;

        private void Start()
        {
            Instance = this;
            _shells = new List<CollectableController>(FindObjectsOfType<CollectableController>());
            foreach (var shell in _shells)
            {
                _totalShells += shell.Count;
                if (zoneData) shell.SetMesh(zoneData.collectableMesh, zoneData.collectableMaterial);
            }

            LevelTime = new TimeSpan(0, 0, 0, 0, 0);
        }

        private void Update()
        {
            LevelTime = LevelTime.Add(TimeSpan.FromSeconds(Time.deltaTime));
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
