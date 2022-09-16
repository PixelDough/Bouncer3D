using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PixelDough.LowResVolume
{
// Define the Volume Component for the custom post processing effect 
    [Serializable, VolumeComponentMenu("LowRes/Low Res Volume")]
    public class LowResVolume : VolumeComponent, IPostProcessComponent
    {

        [Tooltip("Controls whether the Low Res volume is enabled or not.")]
        public BoolParameter isEnabled = new BoolParameter(false);

        public ClampedIntParameter referenceResolution = new ClampedIntParameter(224, 1, 1920);

        public bool IsActive() => isEnabled.value;

        public bool IsTileCompatible() => true;
    }
}
