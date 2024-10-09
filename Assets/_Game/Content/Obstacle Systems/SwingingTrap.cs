using System;
using DrawXXL;
using FMODUnity;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class SwingingTrap : LevelFeature
    {
        [SerializeField, Range(0, 180)] private float angleRange = 25f;
        [SerializeField] private float loopTime = 1f;
        [SerializeField] private bool flipped = false;
        [SerializeField] private bool instantRepeat = false;
        [SerializeField] private Transform hingeTransform;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private EventReference wooshSound;

        private float _swingTime = 0f;
        private float _targetAngle = 0f;
        private float _lastAngle = 0f;

        public override void Initialize()
        {
            _swingTime = 0f;
        }

        private void Update()
        {
            float deltaTime = levelManager.IsPaused ? 0 : Time.deltaTime;
            _swingTime += deltaTime;
            float t = instantRepeat
                ? Mathf.Repeat(_swingTime / loopTime, 1f)
                : Mathf.PingPong(_swingTime / loopTime, 1f);
            float targetAngle = instantRepeat
                ? Mathf.Lerp(-angleRange, angleRange, t)
                : Mathf.SmoothStep(-angleRange, angleRange, t);
            if (flipped)
            {
                _targetAngle = -targetAngle;
            }
            else
            {
                _targetAngle = targetAngle;
            }

            if (rigidbody is not null) return;
            hingeTransform.localRotation = Quaternion.Euler(0, 0, targetAngle);
        }

        private void FixedUpdate()
        {
            if (rigidbody is null) return;
            float currentAngle = hingeTransform.localRotation.eulerAngles.z;
            if (currentAngle > 180) currentAngle -= 360; // Normalize angle to [-180, 180]
            float angleDifference = _targetAngle - currentAngle;
            Quaternion targetRotation = Quaternion.Euler(0, 0, currentAngle + angleDifference);
            rigidbody.MoveRotation(hingeTransform.parent.rotation * targetRotation);
        }

        private void LateUpdate()
        {
            if (MathHelpers.Sign(_targetAngle) != MathHelpers.Sign(_lastAngle))
            {
                RuntimeManager.PlayOneShot(wooshSound, rigidbody.worldCenterOfMass);
            }
            _lastAngle = _targetAngle;
        }

        private void OnDrawGizmos()
        {
            Vector3 rotatedVector = hingeTransform.rotation * Quaternion.Euler(0, 0, -angleRange) * Vector3.down;
            Vector3 rotatedVecto2 = hingeTransform.rotation * Quaternion.Euler(0, 0, angleRange) * Vector3.down;
            DrawBasics.CircleSegment(
                hingeTransform.position, 
                rotatedVector * 4 * transform.localScale.y,
                rotatedVecto2, useReflexAngleOver180deg: angleRange > 90);
        }    
    }
}