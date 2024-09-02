namespace ImpactCFX.Particles
{
    /// <summary>
    /// Base class for particles used by the built-in particles effect.
    /// </summary>
    public abstract class ImpactParticlesBase : PooledEffectObjectBase
    {
        private bool isColliding;

        /// <summary>
        /// Emits particles for a collision.
        /// </summary>
        /// <param name="collisionResultData">Collision data holding important information like the position and normal.</param>
        public void EmitParticlesBase(CollisionResultData collisionResultData)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogEffectPlay(GetType(), collisionResultData, "");
#endif    

            isColliding = true;

            updateParticles(collisionResultData);
            emitParticles(collisionResultData);
        }

        /// <summary>
        /// Updates the particles for sliding and rolling.
        /// </summary>
        /// <param name="collisionResultData">Updated collision data holding important information like the position and normal.</param>
        public void UpdateParticlesBaseCollision(CollisionResultData collisionResultData)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogEffectUpdate(GetType(), collisionResultData, "");
#endif    
            isColliding = true;

            //Rare case if particles are trying to be updated but are not actually playing.
            if (!isPlaying())
            {
                EmitParticlesBase(collisionResultData);
            }

            updateParticles(collisionResultData);
        }

        /// <summary>
        /// Updates the particles and returns it to its pool if it is finished emitting.
        /// </summary>
        public override void UpdatePooledObject()
        {
            if (isPlaying())
            {
                if (!isColliding)
                {
                    stopParticles();
                    ContactPointID = 0;
                }

                isColliding = false;
            }
            else
            {
                if (!isAlive())
                {
                    ReturnToPool();
                }
            }
        }

        /// <summary>
        /// Check if any particles are alive.
        /// </summary>
        protected abstract bool isAlive();

        /// <summary>
        /// Check if the particles are actively playing and emitting.
        /// </summary>
        protected abstract bool isPlaying();

        /// <summary>
        /// Emit particles for a collision.
        /// </summary>
        /// <param name="collisionResultData">Collision data holding important information like the position and normal.</param>
        protected abstract void emitParticles(CollisionResultData collisionResultData);

        /// <summary>
        /// Update particles for sliding and rolling.
        /// </summary>
        /// <param name="collisionResultData">Updated collision data holding important information like the position and normal.</param>
        protected abstract void updateParticles(CollisionResultData collisionResultData);

        /// <summary>
        /// Stop the particles.
        /// </summary>
        protected abstract void stopParticles();
    }
}