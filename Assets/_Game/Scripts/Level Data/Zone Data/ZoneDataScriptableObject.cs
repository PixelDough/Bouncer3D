using UnityEngine;

namespace PixelDough.Bouncer.LevelData
{
    [CreateAssetMenu(menuName = "Bouncer/Zone Data")]
    public class ZoneDataScriptableObject : ScriptableObject
    {
        public CollectableController collectablePrefab;
    }
}
