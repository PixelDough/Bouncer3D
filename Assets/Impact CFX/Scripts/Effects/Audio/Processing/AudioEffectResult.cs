using ImpactCFX.Pooling;

namespace ImpactCFX.Audio
{
    public struct AudioEffectResult : IEffectResult, IObjectPoolRequest
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

        //Audio effect outputs
        public int AudioClipIndex;
        public float Volume;
        public float Pitch;
        public float PitchVelocityAdd;

        public override string ToString()
        {
            return $"[{GetType().Name.ToUpper()}] AudioClipIndex = {AudioClipIndex}, Volume = {Volume}, Pitch = {Pitch}, PitchVelocityAdd = {PitchVelocityAdd}";
        }
    }
}