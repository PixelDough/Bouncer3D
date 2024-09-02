using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Implementation of an Impact Object with a single material for the object.
    /// </summary>
    [AddComponentMenu("Impact CFX/Objects/Impact Object Single Material")]
    [DisallowMultipleComponent]
    public class ImpactObjectSingleMaterial : ImpactObjectBase
    {
        [Tooltip("The Impact Material to use for this object.")]
        [SerializeField]
        private ImpactMaterialAuthoring impactMaterial;
        [Tooltip("If checked, this object will automatically register it's material with the Impact CFX Manager on Start.")]
        [SerializeField]
        private bool registerMaterialOnStart = true;

        /// <summary>
        /// The Impact Material associated with this object.
        /// </summary>
        public ImpactMaterialAuthoring Material { get => impactMaterial; set => impactMaterial = value; }

        private void Start()
        {
            if (registerMaterialOnStart)
            {
                RegisterMaterials();
            }
        }
        public override void RegisterMaterials()
        {
            ImpactCFXGlobal.RegisterMaterial(impactMaterial);
        }

        public override RigidbodyData GetRigidbodyData()
        {
            return RigidbodyData.Default;
        }
    }
}