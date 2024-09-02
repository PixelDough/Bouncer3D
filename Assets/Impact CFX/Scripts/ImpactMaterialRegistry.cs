using UnityEngine;

namespace ImpactCFX
{
    [CreateAssetMenu(fileName = "New Impact Material Registry", menuName = "Impact CFX/Material Registry", order = 2)]
    public class ImpactMaterialRegistry : ScriptableObject
    {
        public ImpactMaterialAuthoring[] Materials;
    }
}

