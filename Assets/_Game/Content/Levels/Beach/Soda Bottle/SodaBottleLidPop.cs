using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using MEC;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    public class SodaBottleLidPop : LevelFeature
    {
        [SerializeField] private Rigidbody lid;
        [SerializeField] private Transform lidVisualsTransform;
        [SerializeField] private Transform lidTargetTransform;
        [SerializeField] private float lidStartPos = 0;
        [SerializeField] private VisualEffect sprayVFX;
        [SerializeField] private float waitOnBottom = 6f;

        [Header("FMOD Events")] 
        [SerializeField] private FMODUnity.EventReference shakeSound;
        [SerializeField] private FMODUnity.EventReference popSound;
        [SerializeField] private FMODUnity.EventReference capLandSound;
        
        private bool _isLidLaunched = false;
        private bool _isShaking = false;

        private CoroutineHandle _popCoroutineHandle;
        private FMOD.Studio.EventInstance _shakeSoundInstance;

        protected override void OnValidate()
        {
            base.OnValidate();
            
            lidStartPos = lid.transform.localPosition.y;
        }

        private void Start()
        {
            Initialize();
            _shakeSoundInstance = FMODUnity.RuntimeManager.CreateInstance(shakeSound);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_shakeSoundInstance, lid.transform, lid);
        }

        public override void Initialize()
        {
            Timing.KillCoroutines(_popCoroutineHandle);
            _popCoroutineHandle = Timing.RunCoroutine(C_LidPopSequence().CancelWith(gameObject));
            _shakeSoundInstance.stop(STOP_MODE.IMMEDIATE);
        }

        private void Update()
        {
            sprayVFX.SetVector3("Lid Transform_position", lid.transform.position);
            sprayVFX.SetVector3("Lid Transform_angles", lid.transform.up);
            
            Vector3 shakeVector = Vector3.zero;
            if (_isShaking)
            {
                float shakeRange = 6f;
                shakeVector = new Vector3(
                    Random.Range(-shakeRange, shakeRange),
                    0f,
                    Random.Range(-shakeRange, shakeRange)
                );
            }

            lidVisualsTransform.localRotation = Quaternion.Euler(shakeVector);
        }

        private IEnumerator<float> C_LidPopSequence()
        {
            while (true)
            {
                _isShaking = false;
                yield return Timing.WaitForSeconds(waitOnBottom - 2f);
                _isShaking = true;
                _shakeSoundInstance.start();
                yield return Timing.WaitForSeconds(2f);
                _shakeSoundInstance.stop(STOP_MODE.IMMEDIATE);
                
                _isLidLaunched = true;

                float heightDiff = lidTargetTransform.localPosition.y - lidStartPos;
                float tweenTimeUp = heightDiff * 0.2f;
                float tweenTimeDown = heightDiff * 0.1f;
                
                FMODUnity.RuntimeManager.PlayOneShot(popSound, lid.position);
                var tweenUp = lid.DOMove(lidTargetTransform.position, tweenTimeUp)
                    .SetEase(Ease.OutSine);

                yield return Timing.WaitUntilDone(tweenUp.WaitForCompletion(true));
                
                Vector3 lidStartPosWorld = transform.TransformPoint(new Vector3(0, lidStartPos, 0));
                var tweenDown = lid.DOMove(lidStartPosWorld, tweenTimeDown)
                    .SetEase(Ease.InSine);

                yield return Timing.WaitUntilDone(tweenDown.WaitForCompletion(true));
                FMODUnity.RuntimeManager.PlayOneShot(capLandSound, lid.position);

                _isLidLaunched = false;
                yield return Timing.WaitForOneFrame;
            }
        }

        private void OnDestroy()
        {
            Timing.KillCoroutines(_popCoroutineHandle);
            _shakeSoundInstance.stop(STOP_MODE.IMMEDIATE);
            _shakeSoundInstance.release();
        }
    }
}
