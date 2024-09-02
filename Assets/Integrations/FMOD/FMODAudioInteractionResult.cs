using Impact.Interactions;
using Impact.Objects;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Impact.Utility.ObjectPool;
using FMOD;

namespace Impact.Integration.FMOD
{
    /// <summary>
    /// The result of an FMOD audio interaction.
    /// Handles collision, sliding, and rolling sounds.
    /// </summary>
    public class FMODAudioInteractionResult : IContinuousInteractionResult, IPoolable
    {
        /// <summary>
        /// The original interaction data this result was created from.
        /// </summary>
        public InteractionData OriginalData { get; set; }

        /// <summary>
        /// The result key for updating sliding and rolling.
        /// </summary>
        public long Key { get; set; }

        /// <summary>
        /// The FMOD event this result uses.
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// The overall intensity of the interaction.
        /// </summary>
        public float Intensity { get; set; }

        /// <summary>
        /// Is the intensity non-zero and is the FMOD event defined?
        /// </summary>
        public bool IsValid
        {
            get { return Intensity > 0 && !string.IsNullOrEmpty(Event); }
        }

        /// <summary>
        /// Is the audio still playing?
        /// </summary>
        public bool IsAlive
        {
            get { return currentIntensity > 0 || targetIntensity > 0; }
        }

        private IImpactObject parent;
        private EventInstance fmodEventInstance;

        private float targetIntensity;
        private float currentIntensity;

        private bool isAvailable = true;

        /// <summary>
        /// Play audio using our data.
        /// </summary>
        /// <param name="parent">The Impact Object that created this result.</param>
        public void Process(IImpactObject parent)
        {
            this.parent = parent;

            currentIntensity = targetIntensity = Intensity;

            fmodEventInstance = FMODUnity.RuntimeManager.CreateInstance(Event);

            fmodEventInstance.set3DAttributes(get3DAttributes(OriginalData, parent));

            fmodEventInstance.setParameterByName(ImpactFMODParameters.Intensity, currentIntensity);
            fmodEventInstance.setParameterByName(ImpactFMODParameters.Velocity, OriginalData.Velocity.magnitude);
            fmodEventInstance.setParameterByName(ImpactFMODParameters.InteractionType, OriginalData.InteractionType);
            fmodEventInstance.setParameterByName(ImpactFMODParameters.CompositionValue, OriginalData.CompositionValue);

            fmodEventInstance.start();

            //Dispose immediately for Collision interaction types
            if (OriginalData.InteractionType == InteractionData.InteractionTypeCollision)
                Dispose();
        }

        /// <summary>
        /// Update the parameters for sliding and rolling and update position.
        /// </summary>
        public void FixedUpdate()
        {
            currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, 0.1f);

            fmodEventInstance.setParameterByName(ImpactFMODParameters.Intensity, currentIntensity);

            targetIntensity = 0;
        }

        /// <summary>
        /// Updates this result to adjust the parameters.
        /// </summary>
        /// <param name="newResult">The new interaction result with updated data.</param>
        public void KeepAlive(IInteractionResult newResult)
        {
            FMODAudioInteractionResult r = newResult as FMODAudioInteractionResult;
            fmodEventInstance.setParameterByName(ImpactFMODParameters.Velocity, r.OriginalData.Velocity.magnitude);
            fmodEventInstance.setParameterByName(ImpactFMODParameters.CompositionValue, r.OriginalData.CompositionValue);

            fmodEventInstance.set3DAttributes(get3DAttributes(r.OriginalData, parent));

            targetIntensity = r.Intensity;
        }

        /// <summary>
        /// Stopm, clear, and release the FMOD event instance.
        /// </summary>
        public void Dispose()
        {
            if (OriginalData.InteractionType == InteractionData.InteractionTypeSlide || OriginalData.InteractionType == InteractionData.InteractionTypeRoll)
                fmodEventInstance.stop(global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            fmodEventInstance.clearHandle();
            fmodEventInstance.release();

            parent = null;
            Event = null;

            MakeAvailable();
        }

        private ATTRIBUTES_3D get3DAttributes(InteractionData interactionData, IImpactObject parent)
        {
            ATTRIBUTES_3D attributes3D = new ATTRIBUTES_3D()
            {
                position = interactionData.Point.ToFMODVector(),
                forward = Vector3.forward.ToFMODVector(),
                up = Vector3.up.ToFMODVector()
            };

            if (parent != null && !parent.Equals(null))
                attributes3D.velocity = parent.GetVelocityDataAtPoint(interactionData.Point).LinearVelocity.ToFMODVector();

            return attributes3D;
        }

        public void Retrieve()
        {
            isAvailable = false;
        }

        public void MakeAvailable()
        {
            isAvailable = true;
        }

        public bool IsAvailable()
        {
            return isAvailable;
        }
    }
}