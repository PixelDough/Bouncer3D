using System;
using System.Collections;
using System.Collections.Generic;
using PixelDough.Bouncer.LevelData;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class LevelProgress
    {
        private List<CollectableSpawner> _allCollectables;
        public List<CollectableSpawner> AllCollectables => _allCollectables;
        public int TotalCollectables => _allCollectables.Count;
        public int CurrentCollectables;

        public static Action OnCollectableCountChange;

        public void Initialize(List<CollectableSpawner> collectableSpawners, ZoneDataScriptableObject zoneData)
        {
            foreach (var collectableSpawner in collectableSpawners)
            {
                collectableSpawner.Spawn(zoneData.collectablePrefab);
            }
            
            _allCollectables = collectableSpawners;
        }
        
        public void AddCollectable(CollectableController collectableController)
        {
            collectableController.collectedState = CollectableController.CollectedStates.Held;
            RefreshCollectableCount();
            OnCollectableCountChange?.Invoke();
        }

        public void RefreshCollectableCount()
        {
            CurrentCollectables = 0;
            foreach (var collectable in _allCollectables)
            {
                if (collectable.spawnedCollectable.collectedState == CollectableController.CollectedStates.Held ||
                    collectable.spawnedCollectable.collectedState == CollectableController.CollectedStates.LockedIn)
                {
                    CurrentCollectables++;
                }
            }
        }

        public void LoseCollectables()
        {
            foreach (var collectable in _allCollectables)
            {
                if (collectable.spawnedCollectable.collectedState != CollectableController.CollectedStates.Held) continue;

                collectable.spawnedCollectable.collectedState = CollectableController.CollectedStates.None;
                collectable.spawnedCollectable.Activate();
            }
            
            RefreshCollectableCount();
            OnCollectableCountChange?.Invoke();
        }

        public void LockInCollectables()
        {
            foreach (var collectable in _allCollectables)
            {
                if (collectable.spawnedCollectable.collectedState != CollectableController.CollectedStates.Held) continue;

                collectable.spawnedCollectable.collectedState = CollectableController.CollectedStates.LockedIn;
            }
        }
    }
}
