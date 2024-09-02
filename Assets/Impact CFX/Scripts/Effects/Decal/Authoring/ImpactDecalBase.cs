using UnityEngine;

namespace ImpactCFX.Decals
{
    /// <summary>
    /// Base for decals placed by decal effects
    /// </summary>
    public abstract class ImpactDecalBase : PooledEffectObjectBase
    {
        private float intervalTracker;
        private float intervalRange;
        private EffectIntervalType intervalType;
        private bool isColliding;

        private CollisionResultData initialCollision;

        /// <summary>
        /// Places the decal for a collision.
        /// </summary>
        /// <param name="collisionResultData">Collision data holding important information like the position and normal.</param>
        /// <param name="decalEffectResult">Has data for sliding and rolling intervals.</param>
        public void PlaceDecalBase(CollisionResultData collisionResultData, DecalEffectResult decalEffectResult)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogEffectPlay(GetType(), collisionResultData, "");
#endif   

            isColliding = true;
            initialCollision = collisionResultData;

            intervalRange = decalEffectResult.CreationInterval.RandomInRange();
            intervalType = decalEffectResult.CreationIntervalType;

            placeDecal(collisionResultData);
        }

        /// <summary>
        /// Updates the decal for sliding and rolling.
        /// </summary>
        /// <param name="collisionResultData">Updated collision data holding important information like the position and normal.</param>
        public void UpdateDecalBaseCollision(CollisionResultData collisionResultData)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogEffectUpdate(GetType(), collisionResultData, "");
#endif    
            if (intervalType == EffectIntervalType.Time)
            {
                intervalTracker += Time.fixedDeltaTime;

                if (intervalTracker > intervalRange)
                {
                    reset();
                }
            }
            else
            {
                intervalTracker = (collisionResultData.Point - initialCollision.Point).sqrMagnitude;

                if (intervalTracker > intervalRange * intervalRange)
                {
                    reset();
                }
            }

            isColliding = true;
        }

        ///
        public override void UpdatePooledObject()
        {
            if (isColliding)
            {
                isColliding = false;
            }
            else
            {
                reset();
            }
        }

        /// <summary>
        /// Perform any needed logic to set the decal's position and rotation.
        /// </summary>
        /// <param name="collisionResultData">Collision data holding important information like the position and normal.</param>
        protected abstract void placeDecal(CollisionResultData collisionResultData);

        /// <summary>
        /// Resets this decal for sliding and rolling decals.
        /// </summary>
        protected void reset()
        {
            ContactPointID = 0;
            NeedsUpdate = false;
        }
    }
}
