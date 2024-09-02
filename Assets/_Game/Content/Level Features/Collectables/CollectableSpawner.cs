using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class CollectableSpawner : MonoBehaviour
    {
        [HideInInspector]
        public CollectableController spawnedCollectable;

        public CollectableController Spawn(CollectableController prefab)
        {
            spawnedCollectable = Instantiate(prefab, transform, false);
            return spawnedCollectable;
        }
    }
}
