using UnityEngine;

namespace PixelDough.Bouncer.LevelData
{
    [CreateAssetMenu(menuName = "Bouncer/Zone Data")]
    public class ZoneDataScriptableObject : ScriptableObject
    {
        public Mesh collectableMesh;
        public Material collectableMaterial;
    }
}
