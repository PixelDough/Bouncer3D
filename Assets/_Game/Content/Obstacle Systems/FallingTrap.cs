using System;
using System.Collections.Generic;
using FMODUnity;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    public class FallingTrap : LevelFeature
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform meshTransform;
        [SerializeField, HideInInspector] private Vector3 startPos;
        [SerializeField, HideInInspector] private Quaternion startRot;
        [SerializeField] private float fallDelay = 4f;
        [SerializeField] private EventReference shakeFallSound;

        private bool _isShaking = false;
        private FMOD.Studio.EventInstance _shakeFallEventInstance;
        private bool _isFalling = false;
        private CoroutineHandle _fallCoroutineHandle;
        
        private Vector3 _pauseVelocity = Vector3.zero;
        private Vector3 _pauseAngularVelocity = Vector3.zero;

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            startPos = transform.position;
            startRot = transform.rotation;
        }
        #endif

        private void Start()
        {
            _shakeFallEventInstance = RuntimeManager.CreateInstance(shakeFallSound);
            RuntimeManager.AttachInstanceToGameObject(_shakeFallEventInstance, rb.transform, rb);
            levelManager.OnPauseStateChanged += OnPauseStateChanged;
        }

        public override void Initialize()
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            transform.position = startPos;
            transform.rotation = startRot;
            _isShaking = false;
            _isFalling = false;
            Timing.KillCoroutines(_fallCoroutineHandle);
            if (_shakeFallEventInstance.isValid())
            {
                _shakeFallEventInstance.setParameterByName("IsFalling", 0);
                _shakeFallEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
        }

        private void Update()
        {
            if (levelManager.IsPaused) return;
            if (_isShaking)
            {
                meshTransform.localPosition = Random.insideUnitSphere * 0.1f;
            }
        }
        
        private void OnPauseStateChanged(bool isPaused)
        {
            if (!_isFalling) return;
            if (isPaused)
            {
                _pauseVelocity = rb.linearVelocity;
                _pauseAngularVelocity = rb.angularVelocity;
                rb.isKinematic = true;
            }
            else
            {                
                rb.isKinematic = false;
                rb.linearVelocity = _pauseVelocity;
                rb.angularVelocity = _pauseAngularVelocity;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_isShaking || _isFalling) return;
            Rigidbody hitRb = other.rigidbody;
            if (!hitRb) return;
            _isShaking = true;
            
            _shakeFallEventInstance.start();
            
            _fallCoroutineHandle = Timing.RunCoroutine(Fall().CancelWith(gameObject));
        }
        
        private IEnumerator<float> Fall()
        {
            _isShaking = true;
            yield return Timing.WaitForSeconds(fallDelay);
            yield return Timing.WaitForOneFrame;
            _shakeFallEventInstance.setParameterByName("IsFalling", 1);
            _isShaking = false;
            _isFalling = true;
            meshTransform.localPosition = Vector3.zero;
            transform.parent = null;
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        private void OnDestroy()
        {
            Timing.KillCoroutines(_fallCoroutineHandle);
            _shakeFallEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _shakeFallEventInstance.release();
            _shakeFallEventInstance.clearHandle();
            levelManager.OnPauseStateChanged -= OnPauseStateChanged;
        }
    }
}
