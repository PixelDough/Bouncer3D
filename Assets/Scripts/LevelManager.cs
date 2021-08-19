using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        private List<CollectableController> _shells = new List<CollectableController>();
        
        private int _totalShells = 0;
        public int TotalShells => _totalShells;
        
        public int shellsCollected = 0;

        private void Start()
        {
            Instance = this;
            _shells = new List<CollectableController>(FindObjectsOfType<CollectableController>());
            foreach (var shell in _shells)
            {
                _totalShells += shell.Count;
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
