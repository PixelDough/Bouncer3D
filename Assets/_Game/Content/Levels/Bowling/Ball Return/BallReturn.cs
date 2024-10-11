using System;
using System.Collections.Generic;
using System.Threading;
using Animancer;
using DG.Tweening;
using DrawXXL;
using MEC;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace PixelDough.Bouncer
{
    public class BallReturn : LevelFeature
    {
        [SerializeField] private Transform ballReturnTransform;
        [SerializeField] private Transform playerAttachPoint;
        [SerializeField] private BoxCollider swallowTrigger;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private AnimancerComponent animancer;
        [SerializeField] private AnimationClip swallowClip;
        [SerializeField] private AnimationClip spitClip;
        [SerializeField] private Vector3 targetAngles;
        [SerializeField] private float spitForce = 60f;

        private PlayerController _playerController;
        
        private readonly Collider[] _swallowColliderOverlaps = new Collider[8];
        
        private bool _isSwallowing = false;
        private Quaternion _originalRotation;

        private void Start()
        {
            _originalRotation = ballReturnTransform.rotation;
        }

        public override void Initialize()
        {
            ballReturnTransform.rotation = _originalRotation;
            Timing.KillCoroutines(gameObject);
            cinemachineCamera.Priority = -100;
            cinemachineCamera.enabled = false;
        }

        private void Update()
        {
            if (!_isSwallowing) return;
            if (!_playerController) return;
            
            _playerController.transform.position = transform.position;
        }

        private void FixedUpdate()
        {
            if (_isSwallowing) return;
            int overlapCount = Physics.OverlapBoxNonAlloc(swallowTrigger.transform.TransformPoint(swallowTrigger.center), Vector3.Scale(swallowTrigger.size, swallowTrigger.transform.lossyScale) * 0.5f, _swallowColliderOverlaps, swallowTrigger.transform.rotation);
            for (int i = 0; i < overlapCount; i++)
            {
                SwallowTest(_swallowColliderOverlaps[i]);
            }
        }

        private IEnumerator<float> C_SwallowAndShoot()
        {
            _isSwallowing = true;
            
            cinemachineCamera.enabled = true;
            cinemachineCamera.Priority = 999999;

            var swallowState = animancer.Play(swallowClip);
            yield return Timing.WaitUntilTrue(() => swallowState.NormalizedTime >= 1);

            var rotateTween = ballReturnTransform.DORotate(targetAngles, 0.5f, RotateMode.Fast)
                .SetEase(Ease.InOutSine);
            yield return Timing.WaitForSeconds(0.5f);
            
            var spitState = animancer.Play(spitClip);
            yield return Timing.WaitUntilTrue(() => spitState.NormalizedTime >= 1);

            yield return Timing.WaitForSeconds(0.5f);
            
            var rotateBackTween = ballReturnTransform.DORotate(_originalRotation.eulerAngles, 0.5f, RotateMode.Fast)
                .SetEase(Ease.InOutSine);
            
            _isSwallowing = false;
        }

        public void SpitPlayer()
        {
            if (!_playerController) return;
            if (!_isSwallowing) return;
            _playerController.transform.position = playerAttachPoint.position;
            _playerController.PlayerStuffManager.SetCameraForward(cinemachineCamera.transform.forward);
            GameManager.DoPlayerPhysics = true;
            GameManager.DoPlayerMovement = true;
            _playerController.Rigidbody.isKinematic = false;
            _playerController.Rigidbody.AddForce(playerAttachPoint.forward * spitForce, ForceMode.Impulse);
            
            _playerController = null;
            cinemachineCamera.Priority = -100;
            cinemachineCamera.enabled = false;
            
        }

        private void SwallowTest(Collider other)
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

        private void OnDrawGizmos()
        {
            Vector3 targetDirection = Quaternion.Euler(targetAngles) * Vector3.forward;
            
            #region Target Visualization
            
            DrawEngineBasics.CoordinateAxesGizmoLocal(transform.position, Quaternion.Euler(targetAngles), Vector3.one, 3f);

            #endregion
            
            #region Velocity Estimation
            
            DrawBasics.Ray(playerAttachPoint.position, targetDirection, Color.cyan);
        
            Vector3 position = playerAttachPoint.position;
            Vector3 velocity = targetDirection * spitForce;
            float timeStep = 10f / 20f; // 10 seconds divided by 20 steps
            Vector3 gravity = Physics.gravity * timeStep;
        
            for (int i = 0; i < 20; i++)
            {
                Vector3 nextPosition = position + velocity * timeStep + 0.5f * gravity * timeStep * timeStep;
                DrawBasics.Ray(position, nextPosition - position, Color.green);
                position = nextPosition;
                velocity += gravity;
            
                // Calculate and apply drag
                Vector3 velTimeStepped = velocity;
                float projectedMagnitude = Vector3.ProjectOnPlane(velTimeStepped, Vector3.up).magnitude;
                float drag = 1f / (Mathf.Max(projectedMagnitude, 1) * 2);
                velocity = velocity * (1 - timeStep * drag);
            }

            #endregion

        }
    }
}
