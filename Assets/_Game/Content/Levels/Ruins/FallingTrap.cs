using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    public class FallingTrap : MonoBehaviour, ILevelFeature
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform meshTransform;
        [SerializeField, HideInInspector] private Vector3 startPos;
        [SerializeField, HideInInspector] private Quaternion startRot;
        [SerializeField] private float fallDelay = 4f;

        private bool _isShaking = false;
        private bool _isFalling = false;

        #if UNITY_EDITOR
        private void OnValidate()
        {
            startPos = transform.position;
            startRot = transform.rotation;
        }
        #endif
        
        public void Initialize()
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            transform.position = startPos;
            transform.rotation = startRot;
            _isShaking = false;
            _isFalling = false;
        }

        private void Update()
        {
            if (_isShaking)
            {
                meshTransform.localPosition = Random.insideUnitSphere * 0.1f;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            Rigidbody hitRb = other.rigidbody;
            if (!hitRb) return;
            
            Timing.RunCoroutine(Fall());
        }
        
        private IEnumerator<float> Fall()
        {
            _isShaking = true;
            yield return Timing.WaitForSeconds(fallDelay);
            _isShaking = false;
            _isFalling = true;
            meshTransform.localPosition = Vector3.zero;
            transform.parent = null;
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}
