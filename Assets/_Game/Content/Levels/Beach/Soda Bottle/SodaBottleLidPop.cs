using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    public class SodaBottleLidPop : MonoBehaviour
    {
        [SerializeField] private Rigidbody lid;
        [SerializeField] private Transform lidTargetTransform;
        [SerializeField] private MeshRenderer lidMeshRenderer;
        [SerializeField] private MeshFilter lidMeshFilter;
        [SerializeField] private VisualEffect sprayVFX;
        [SerializeField] private float waitOnBottom = 6f;
        [SerializeField] private float waitOnTop = 2f;
        [SerializeField] private float riseTime = 5f;
        
        private bool _isLidLaunched = false;
        private Vector3 _lidStartPos = Vector3.zero;

        private CoroutineHandle _popCoroutineHandle;

        private void Start()
        {
            _lidStartPos = lid.transform.position;

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

                float heightDiff = lidTargetTransform.position.y - _lidStartPos.y;
                float tweenTimeUp = riseTime;
                float tweenTimeDown = riseTime * 0.3f;
                
                var tweenUp = lid.DOMove(lidTargetTransform.position, tweenTimeUp)
                    .SetEase(Ease.InOutSine);

                yield return Timing.WaitUntilDone(tweenUp.WaitForCompletion(true));

                yield return Timing.WaitForSeconds(waitOnTop);
                
                var tweenDown = lid.DOMove(_lidStartPos, tweenTimeDown)
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
