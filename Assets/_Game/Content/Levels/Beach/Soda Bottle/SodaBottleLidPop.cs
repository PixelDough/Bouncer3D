using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField] private Transform lidTargetTransform;
        [SerializeField] private MeshRenderer lidMeshRenderer;
        [SerializeField] private MeshFilter lidMeshFilter;
        [SerializeField] private float lidStartPos = 0;
        [SerializeField] private VisualEffect sprayVFX;
        [SerializeField] private float waitOnBottom = 6f;
        
        private bool _isLidLaunched = false; 

        private CoroutineHandle _popCoroutineHandle;

        protected override void OnValidate()
        {
            base.OnValidate();
            
            lidStartPos = lid.transform.localPosition.y;
        }

        private void Start()
        {
            Initialize();
        }

        public override void Initialize()
        {
            Timing.KillCoroutines(_popCoroutineHandle);
            _popCoroutineHandle = Timing.RunCoroutine(C_LidPopSequence());
        }

        private void Update()
        {
            sprayVFX.SetVector3("Lid Transform_position", lid.transform.position);
            sprayVFX.SetVector3("Lid Transform_angles", lid.transform.up);
            Vector3 shakeVector = Vector3.zero;
            float shakeRange = 2f;
            shakeVector = new Vector3(
                Random.Range(-shakeRange, shakeRange),
                0f,
                Random.Range(-shakeRange, shakeRange)
            );

            RenderParams renderParams = new RenderParams(lidMeshRenderer.material);
            Graphics.RenderMesh(
                renderParams,
                lidMeshFilter.mesh,
                0,
                Matrix4x4.TRS(
                    lid.transform.position, 
                    Quaternion.Euler(lid.transform.eulerAngles + shakeVector),
                    lid.transform.lossyScale
                )
            );
        }

        private IEnumerator<float> C_LidPopSequence()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(waitOnBottom);

                _isLidLaunched = true;

                float heightDiff = lidTargetTransform.localPosition.y - lidStartPos;
                float tweenTimeUp = heightDiff * 0.2f;
                float tweenTimeDown = heightDiff * 0.1f;
                
                var tweenUp = lid.DOMove(lidTargetTransform.position, tweenTimeUp)
                    .SetEase(Ease.OutSine);

                yield return Timing.WaitUntilDone(tweenUp.WaitForCompletion(true));
                
                Vector3 lidStartPosWorld = transform.TransformPoint(new Vector3(0, lidStartPos, 0));
                var tweenDown = lid.DOMove(lidStartPosWorld, tweenTimeDown)
                    .SetEase(Ease.InSine);

                yield return Timing.WaitUntilDone(tweenDown.WaitForCompletion(true));

                _isLidLaunched = false;
                yield return Timing.WaitForOneFrame;
            }
        }

        private void OnDestroy()
        {
            Timing.KillCoroutines(_popCoroutineHandle);
        }
    }
}
