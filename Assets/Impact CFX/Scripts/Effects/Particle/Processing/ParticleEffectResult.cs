using ImpactCFX.Pooling;

namespace ImpactCFX.Particles
{
    public struct ParticleEffectResult : IEffectResult, IObjectPoolRequest
    {
        //IEffectResult outputs
        public bool IsEffectValid { get; set; }
        public int CollisionIndex { get; set; }
        public int MaterialCompositionIndex { get; set; }

        //IObjectPoolRequest inputs
        public bool IsObjectPoolRequestValid { get; set; }
        public int TemplateID { get; set; }
        public float Priority { get; set; }
        public long ContactPointID { get; set; }
        public bool CheckContactPointID { get; set; }

        //IObjectPoolRequest outputs
        public int ObjectIndex { get; set; }
        public bool IsUpdate { get; set; }

        public override string ToString()
        {
            return $"[{GetType().Name.ToUpper()}] TemplateID = {TemplateID}";
        }
    }
}