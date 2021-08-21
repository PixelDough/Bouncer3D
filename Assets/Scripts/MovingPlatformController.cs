using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class MovingPlatformController : MonoBehaviour
    {

        private List<Rigidbody> _riders = new List<Rigidbody>();

        private Vector3 _lastPosition = Vector3.zero;
        private Quaternion _lastRotation = Quaternion.identity;

        private Transform _childRotate;

        private void Start()
        {
            _childRotate = new GameObject().transform;
            _childRotate.parent = transform;
        }

        private void FixedUpdate()
        {
            Vector3 positionDelta = transform.position - _lastPosition;
            Quaternion rotationDelta = Quaternion.Inverse(transform.rotation) * _lastRotation;
            rotationDelta.ToAngleAxis(out float angleDiff, out Vector3 axis);
            foreach (var rider in _riders)
            {
                _childRotate.transform.position = rider.transform.position;
                _childRotate.RotateAround(transform.position, axis, -angleDiff);
                rider.MovePosition(_childRotate.transform.position + positionDelta);

            }
            
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.rigidbody) return;
            _riders.Add(other.rigidbody);
            
        }

        private void OnCollisionExit(Collision other)
        {
            if (!other.rigidbody) return;
            _riders.Remove(other.rigidbody);
        }
    }
}
