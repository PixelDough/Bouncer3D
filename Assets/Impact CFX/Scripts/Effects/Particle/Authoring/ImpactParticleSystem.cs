using UnityEngine;

namespace ImpactCFX.Particles
{
    [AddComponentMenu("Impact CFX/Particle/Impact Particle System")]
    public class ImpactParticleSystem : ImpactParticlesBase
    {


        /// <summary>
        /// Container for particle system min max curves.
        /// </summary>
        protected class ParticleSystemCurveContainer
        {
            /// <summary>
            /// The original curve.
            /// </summary>
            public ParticleSystem.MinMaxCurve Original;

            /// <summary>
            /// The modified curve.
            /// </summary>
            public ParticleSystem.MinMaxCurve Modified;

            public ParticleSystemCurveContainer(ParticleSystem.MinMaxCurve original)
            {
                if (original.mode == ParticleSystemCurveMode.Constant)
                {
                    Original = new ParticleSystem.MinMaxCurve(original.constant);
                    Modified = new ParticleSystem.MinMaxCurve(original.constant);
                }
                else if (original.mode == ParticleSystemCurveMode.TwoConstants)
                {
                    Original = new ParticleSystem.MinMaxCurve(original.constantMin, original.constantMax);
                    Modified = new ParticleSystem.MinMaxCurve(original.constantMin, original.constantMax);
                }
                else if (original.mode == ParticleSystemCurveMode.Curve)
                {
                    Original = new ParticleSystem.MinMaxCurve(original.curveMultiplier, new AnimationCurve(original.curveMax.keys));
                    Modified = new ParticleSystem.MinMaxCurve(original.curveMultiplier, new AnimationCurve(original.curveMax.keys));
                }
                else if (original.mode == ParticleSystemCurveMode.TwoCurves)
                {
                    Original = new ParticleSystem.MinMaxCurve(original.curveMultiplier, new AnimationCurve(original.curveMin.keys), new AnimationCurve(original.curveMax.keys));
                    Modified = new ParticleSystem.MinMaxCurve(original.curveMultiplier, new AnimationCurve(original.curveMin.keys), new AnimationCurve(original.curveMax.keys));
                }
            }

            /// <summary>
            /// Multiplies the Original curve to get values for the Modified curve.
            /// </summary>
            public void Multiply(float multiplier)
            {
                Modified.constant = Original.constant * multiplier;
                Modified.constantMin = Original.constantMin * multiplier;
                Modified.constantMax = Original.constantMax * multiplier;
                Modified.curveMultiplier = multiplier;
            }
        }

        /// <summary>
        /// Defines how the particles should be aligned with the collision normal.
        /// </summary>
        protected enum ParticleRotationMode
        {
            /// <summary>
            /// Align the particles with the surface normal.
            /// </summary>
            [Tooltip("Align the particles with the surface normal.")]
            AlignToNormal = 0,
            /// <summary>
            /// Rotate the particles to match the collision velocity.
            /// </summary>
            [Tooltip("Rotate the particles to match the collision velocity.")]
            AlignToVelocity = 1,
            /// <summary>
            /// Align the particles with the surface normal but also try to match the collision velocity.
            /// </summary>
            [Tooltip("Align the particles with the surface normal but also try to match the collision velocity.")]
            AlignToNormalAndVelocity = 2,
            /// <summary>
            /// Don't rotate the particles.
            /// </summary>
            [Tooltip("Don't rotate the particles.")]
            NoRotation = 3
        }

        [SerializeField]
        [Tooltip("How the particle object should be rotated on the surface.")]
        protected ParticleRotationMode rotationMode = ParticleRotationMode.AlignToNormal;
        [SerializeField]
        [Tooltip("How the particle object should be oriented on the surface.")]
        protected AlignmentAxis axis = AlignmentAxis.ZUp;
        [SerializeField]
        [Tooltip("List of all Particle Systems that are controlled by this effect.")]
        protected ParticleSystem[] particleSystems;

        [SerializeField]
        [Tooltip("Should the start size of the particles be scaled by the collision velocity?")]
        protected EffectVelocityModifier scaleSizeWithVelocity;
        [SerializeField]
        [Tooltip("Should the start speed of the particles be scaled by the collision velocity?")]
        protected EffectVelocityModifier scaleSpeedWithVelocity;
        [SerializeField]
        [Tooltip("Should the start lifetime of the particles be scaled by the collision velocity?")]
        protected EffectVelocityModifier scaleLifetimeWithVelocity;

        protected bool particleSystemIsPlaying;

        protected ParticleSystemCurveContainer[] lifeCurves;
        protected ParticleSystemCurveContainer[] startSizeCurves;
        protected ParticleSystemCurveContainer[] startSpeedCurves;
        protected ParticleSystemCurveContainer[] startLifetimeCurves;

        protected virtual void Awake()
        {
            if (scaleSizeWithVelocity.Enabled)
            {
                startSizeCurves = new ParticleSystemCurveContainer[particleSystems.Length];
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    startSizeCurves[i] = new ParticleSystemCurveContainer(particleSystems[i].main.startSize);
                }
            }

            if (scaleSpeedWithVelocity.Enabled)
            {
                startSpeedCurves = new ParticleSystemCurveContainer[particleSystems.Length];
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    startSpeedCurves[i] = new ParticleSystemCurveContainer(particleSystems[i].main.startSpeed);
                }
            }

            if (scaleLifetimeWithVelocity.Enabled)
            {
                startLifetimeCurves = new ParticleSystemCurveContainer[particleSystems.Length];
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    startLifetimeCurves[i] = new ParticleSystemCurveContainer(particleSystems[i].main.startLifetime);
                }
            }
        }

        protected virtual void Reset()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        protected override bool isAlive()
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                if (particleSystems[i].IsAlive())
                    return true;
            }

            return false;
        }

        protected override bool isPlaying()
        {
            return particleSystemIsPlaying;
        }

        protected override void emitParticles(CollisionResultData collisionResultData)
        {
            bool hasActiveModifier = scaleSizeWithVelocity.Enabled || scaleSpeedWithVelocity.Enabled || scaleLifetimeWithVelocity.Enabled;

            for (int i = 0; i < particleSystems.Length; i++)
            {
                ParticleSystem particleSystem = particleSystems[i];
                particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                //Apply modifiers based on velocity
                if (hasActiveModifier)
                {
                    ParticleSystem.MainModule mainModule = particleSystem.main;
                    float velocityMagnitude = collisionResultData.Velocity.magnitude;

                    if (scaleSizeWithVelocity.Enabled)
                    {
                        float multiplier = scaleSizeWithVelocity.Evaluate(velocityMagnitude);
                        startSizeCurves[i].Multiply(multiplier);
                        mainModule.startSize = startSizeCurves[i].Modified;
                    }

                    if (scaleSpeedWithVelocity.Enabled)
                    {
                        float multiplier = scaleSpeedWithVelocity.Evaluate(velocityMagnitude);
                        startSpeedCurves[i].Multiply(multiplier);
                        mainModule.startSpeed = startSpeedCurves[i].Modified;
                    }

                    if (scaleLifetimeWithVelocity.Enabled)
                    {
                        float multiplier = scaleLifetimeWithVelocity.Evaluate(velocityMagnitude);
                        startLifetimeCurves[i].Multiply(multiplier);
                        mainModule.startLifetime = startLifetimeCurves[i].Modified;
                    }
                }

                particleSystem.Play();
            }

            particleSystemIsPlaying = true;
        }

        protected override void updateParticles(CollisionResultData collisionResultData)
        {
            transform.position = collisionResultData.Point;

            if (rotationMode == ParticleRotationMode.AlignToNormal)
            {
                transform.rotation = AlignmentAxisUtilities.GetRotationForAlignment(axis, collisionResultData.Normal);
            }
            else if (rotationMode == ParticleRotationMode.AlignToVelocity)
            {
                transform.rotation = AlignmentAxisUtilities.GetRotationForAlignment(axis, collisionResultData.Velocity);
            }
            else if (rotationMode == ParticleRotationMode.AlignToNormalAndVelocity)
            {
                transform.rotation = AlignmentAxisUtilities.GetVelocityRotation(axis, collisionResultData.Normal, collisionResultData.Velocity);
            }

            bool hasActiveModifier = scaleSizeWithVelocity.Enabled || scaleSpeedWithVelocity.Enabled || scaleLifetimeWithVelocity.Enabled;
            if (hasActiveModifier)
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    ParticleSystem particleSystem = particleSystems[i];
                    ParticleSystem.MainModule mainModule = particleSystem.main;

                    float velocityMagnitude = collisionResultData.Velocity.magnitude;

                    if (scaleSizeWithVelocity.Enabled)
                    {
                        float multiplier = scaleSizeWithVelocity.Evaluate(velocityMagnitude);
                        startSizeCurves[i].Multiply(multiplier);
                        mainModule.startSize = startSizeCurves[i].Modified;
                    }

                    if (scaleSpeedWithVelocity.Enabled)
                    {
                        float multiplier = scaleSpeedWithVelocity.Evaluate(velocityMagnitude);
                        startSpeedCurves[i].Multiply(multiplier);
                        mainModule.startSpeed = startSpeedCurves[i].Modified;
                    }

                    if (scaleLifetimeWithVelocity.Enabled)
                    {
                        float multiplier = scaleLifetimeWithVelocity.Evaluate(velocityMagnitude);
                        startLifetimeCurves[i].Multiply(multiplier);
                        mainModule.startLifetime = startLifetimeCurves[i].Modified;
                    }
                }
            }
        }

        protected override void stopParticles()
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                //Only stop particles if they are set to loop, since calling stop is not necessary for one-shot particles and can actually cause problems. 
                if (particleSystem.main.loop)
                    particleSystem.Stop();
            }

            particleSystemIsPlaying = false;
        }
    }
}
