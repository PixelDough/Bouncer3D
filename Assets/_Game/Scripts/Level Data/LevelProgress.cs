using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class LevelProgress
    {
        private List<CollectableController> _allCollectables;
        public List<CollectableController> AllCollectables => _allCollectables;
        public int TotalCollectables => _allCollectables.Count;
        public int CurrentCollectables;

        public static Action OnCollectableCountChange;

        public void Initialize(List<CollectableController> collectableControllers)
        {
            _allCollectables = collectableControllers;
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
                if (collectable.collectedState == CollectableController.CollectedStates.Held ||
                    collectable.collectedState == CollectableController.CollectedStates.LockedIn)
                {
                    CurrentCollectables++;
                }
            }
        }

        public void LoseCollectables()
        {
            foreach (var collectable in _allCollectables)
            {
                if (collectable.collectedState != CollectableController.CollectedStates.Held) continue;

                collectable.collectedState = CollectableController.CollectedStates.None;
                collectable.Activate();
            }
            
            RefreshCollectableCount();
            OnCollectableCountChange?.Invoke();
        }

        public void LockInCollectables()
        {
            foreach (var collectable in _allCollectables)
            {
                if (collectable.collectedState != CollectableController.CollectedStates.Held) continue;

                collectable.collectedState = CollectableController.CollectedStates.LockedIn;
            }
        }
    }
}
