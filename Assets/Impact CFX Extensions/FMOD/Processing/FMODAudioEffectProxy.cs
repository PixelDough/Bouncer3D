using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace ImpactCFX.FMOD
{
    public class FMODAudioEffectProxy : PooledEffectObjectBase
    {

        private EventInstance eventInstance;

        private Vector3 previousPosition;
        private Vector3 velocity;

        private float targetIntensity;
        private float currentIntensity;

        private CollisionResultData collisionResultData;

        public float AudioFadeOutTime = 0.1f;

        public void PlayFMODAudioEffect(FMODAudioEffectResult fmodAudioEffectResult, CollisionResultData collisionResultData)
        {
            clearEventInstance();

            this.collisionResultData = collisionResultData;

            transform.position = collisionResultData.Point;
            velocity = collisionResultData.Velocity;
            currentIntensity = targetIntensity = fmodAudioEffectResult.NormalizedIntensity;

            eventInstance = RuntimeManager.CreateInstance(fmodAudioEffectResult.FMODEventGUID);

            if (eventInstance.isValid())
            {
                eventInstance.set3DAttributes(get3DAttributes());

                eventInstance.setParameterByName(ImpactFMODParameters.Intensity, fmodAudioEffectResult.NormalizedIntensity);
                eventInstance.setParameterByName(ImpactFMODParameters.RawIntensity, fmodAudioEffectResult.RawIntensity);
                eventInstance.setParameterByName(ImpactFMODParameters.CompositionValue, collisionResultData.MaterialComposition);
                eventInstance.setParameterByName(ImpactFMODParameters.CollisionType, (int)collisionResultData.CollisionType);

                eventInstance.start();

                if (collisionResultData.CollisionType == CollisionType.Collision)
                    attach(collisionResultData);

                NeedsUpdate = true;
            }
            else
            {
                ReturnToPool();
            }
        }

        public void UpdateFMODAudioEffectCollision(FMODAudioEffectResult fmodAudioEffectResult, CollisionResultData collisionResultData)
        {
            this.collisionResultData = collisionResultData;

            if (!eventInstance.isValid())
                PlayFMODAudioEffect(fmodAudioEffectResult, collisionResultData);

            if (eventInstance.isValid())
            {
                currentIntensity = targetIntensity = fmodAudioEffectResult.NormalizedIntensity;
                transform.position = collisionResultData.Point;

                eventInstance.set3DAttributes(get3DAttributes());

                eventInstance.setParameterByName(ImpactFMODParameters.Intensity, currentIntensity);
                eventInstance.setParameterByName(ImpactFMODParameters.RawIntensity, fmodAudioEffectResult.RawIntensity);
                eventInstance.setParameterByName(ImpactFMODParameters.CompositionValue, collisionResultData.MaterialComposition);
            }
            else
            {
                ReturnToPool();
            }
        }

        public override void UpdatePooledObject()
        {
            if (eventInstance.isValid())
            {
                velocity = (previousPosition - transform.position) / Time.fixedDeltaTime;
                previousPosition = transform.position;

                eventInstance.set3DAttributes(get3DAttributes());

                if (collisionResultData.CollisionType == CollisionType.Collision)
                {
                    RESULT result = eventInstance.getPlaybackState(out PLAYBACK_STATE state);
                    if (state == PLAYBACK_STATE.STOPPED)
                    {
                        ReturnToPool();
                    }
                }
                else
                {
                    currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, Time.deltaTime / AudioFadeOutTime);
                    eventInstance.setParameterByName(ImpactFMODParameters.Intensity, currentIntensity);

                    if (currentIntensity == 0)
                    {
                        ReturnToPool();
                    }

                    targetIntensity = 0;
                }
            }
            else
            {
                ReturnToPool();
            }
        }

        private ATTRIBUTES_3D get3DAttributes()
        {
            ATTRIBUTES_3D attributes3D = new ATTRIBUTES_3D()
            {
                position = transform.position.ToFMODVector(),
                forward = transform.forward.ToFMODVector(),
                up = transform.up.ToFMODVector(),
                velocity = velocity.ToFMODVector()
            };

            return attributes3D;
        }

        public override void ReturnToPool()
        {
            clearEventInstance();

            base.ReturnToPool();
        }

        private void OnDestroy()
        {
            clearEventInstance();
        }

        private void clearEventInstance()
        {
            if (eventInstance.isValid())
            {
                eventInstance.stop(STOP_MODE.IMMEDIATE);
                eventInstance.release();
                eventInstance.clearHandle();
            }
        }
    }
}

