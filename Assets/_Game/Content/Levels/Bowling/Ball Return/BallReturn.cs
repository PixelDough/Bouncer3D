using System;
using System.Collections.Generic;
using System.Threading;
using Animancer;
using DG.Tweening;
using MEC;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace PixelDough.Bouncer
{
    public class BallReturn : LevelFeature
    {
        [SerializeField] private Transform playerAttachPoint;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private AnimancerComponent animancer;
        [SerializeField] private AnimationClip swallowClip;
        [SerializeField] private AnimationClip spitClip;
        [SerializeField] private Vector3 targetAngles;
        [SerializeField] private float spitForce = 60f;

        private PlayerController _playerController;
        
        private bool _isSwallowing = false;
        private Quaternion _originalRotation;

        private void Start()
        {
            _originalRotation = transform.rotation;
        }

        public override void Initialize()
        {
            transform.rotation = _originalRotation;
            Timing.KillCoroutines(gameObject);
            cinemachineCamera.Priority = -100;
            cinemachineCamera.enabled = false;
        }

        private void Update()
        {
            if (!_isSwallowing) return;
            if (!_playerController) return;
            
            _playerController.transform.position = playerAttachPoint.position;
        }

        private IEnumerator<float> C_SwallowAndShoot()
        {
            _isSwallowing = true;
            
            cinemachineCamera.enabled = true;
            cinemachineCamera.Priority = 999999;

            var swallowState = animancer.Play(swallowClip);
            yield return Timing.WaitUntilTrue(() => swallowState.NormalizedTime >= 1);

            var rotateTween = transform.DORotate(targetAngles, 1f, RotateMode.Fast)
                .SetEase(Ease.InOutSine);
            yield return Timing.WaitForSeconds(1.1f);
            
            var spitState = animancer.Play(spitClip);
            yield return Timing.WaitUntilTrue(() => spitState.NormalizedTime >= 1);

            yield return Timing.WaitForSeconds(5f);
            
            _isSwallowing = false;
        }

        public void SpitPlayer()
        {
            if (!_playerController) return;
            if (!_isSwallowing) return;
            GameManager.DoPlayerPhysics = true;
            GameManager.DoPlayerMovement = true;
            _playerController.Rigidbody.isKinematic = false;
            _playerController.Rigidbody.AddForce(playerAttachPoint.forward * spitForce, ForceMode.Impulse);
            
            _playerController = null;
            cinemachineCamera.Priority = -100;
            cinemachineCamera.enabled = false;
            
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_isSwallowing) return;
            Rigidbody rb = other.attachedRigidbody;
            if (rb is null) return;
            if (!rb.CompareTag("Player")) return;
            
            _playerController = rb.GetComponent<PlayerController>();
            GameManager.DoPlayerPhysics = false;
            GameManager.DoPlayerMovement = false;
            _playerController.Rigidbody.isKinematic = true;
            
            Timing.RunCoroutine(C_SwallowAndShoot().CancelWith(gameObject));
        }
    }
}
