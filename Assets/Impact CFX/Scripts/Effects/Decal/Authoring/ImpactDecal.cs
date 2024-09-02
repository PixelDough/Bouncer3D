using UnityEngine;

namespace ImpactCFX.Decals
{
    [AddComponentMenu("Impact CFX/Decal/Impact Decal")]
    public class ImpactDecal : ImpactDecalBase
    {
        /// <summary>
        /// Defines how the decal should be rotated with the collision normal.
        /// </summary>
        protected enum DecalRotationMode
        {
            /// <summary>
            /// Randomly rotate the decal on the surface normal axis.
            /// </summary>
            [Tooltip("Randomly rotate the decal on the surface normal axis.")]
            AlignToNormalRandom = 0,
            /// <summary>
            /// Rotate the decal to match the interaction velocity and surface normal.
            /// </summary>
            [Tooltip("Rotate the decal to match the interaction velocity and surface normal.")]
            AlignToNormalAndVelocity = 1,
            /// <summary>
            /// Rotate the decal to match the surface normal and do no extra rotation.
            /// </summary>
            [Tooltip("Rotate the decal to match the surface normal and do no extra rotation.")]
            AlignToNormalOnly = 2
        }

        [Tooltip("How far away the decal should be placed above the surface to prevent Z-fighting.")]
        [SerializeField]
        protected float decalDistance = 0.01f;
        [Tooltip("How the decal should be rotated.")]
        [SerializeField]
        protected DecalRotationMode rotationMode = DecalRotationMode.AlignToNormalRandom;
        [Tooltip("How the decal should be oriented on the surface.")]
        [SerializeField]
        protected AlignmentAxis axis = AlignmentAxis.ZDown;

        [SerializeField]
        [Tooltip("Should the size of the decal be scaled by the collision velocity?")]
        protected EffectVelocityModifier scaleWithVelocity;

        protected Vector3 originalScale;

        protected virtual void Awake()
        {
            originalScale = transform.localScale;
        }

        protected override void placeDecal(CollisionResultData collisionResultData)
        {
            transform.position = collisionResultData.Point + collisionResultData.Normal * decalDistance;

            if (rotationMode == DecalRotationMode.AlignToNormalRandom)
            {
                transform.rotation = AlignmentAxisUtilities.GetRotationForAlignment(axis, collisionResultData.Normal);
                rotateRandom();
            }
            else if (rotationMode == DecalRotationMode.AlignToNormalAndVelocity)
            {
                transform.rotation = AlignmentAxisUtilities.GetVelocityRotation(axis, collisionResultData.Normal, collisionResultData.Velocity);
            }
            else
            {
                transform.rotation = AlignmentAxisUtilities.GetRotationForAlignment(axis, collisionResultData.Normal);
            }

            if (scaleWithVelocity.Enabled)
            {
                float multiplier = scaleWithVelocity.Evaluate(collisionResultData.Velocity.magnitude);
                transform.localScale = originalScale * multiplier;
            }

            attach(collisionResultData);
        }

        private void rotateRandom()
        {
            if (axis == AlignmentAxis.ZDown || axis == AlignmentAxis.ZUp)
                transform.Rotate(new Vector3(0, 0, Random.value * 360f), Space.Self);
            else
                transform.Rotate(new Vector3(0, Random.value * 360f, 0), Space.Self);
        }
    }
}