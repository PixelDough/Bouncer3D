using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class MovingPlatformController : MonoBehaviour
    {

        [SerializeField] private bool useRigidbody = false;
        [SerializeField] private Transform root;
        
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
            if (!useRigidbody) return;
            Transform rootTransform = transform;
            if (root) rootTransform = root;
            Vector3 positionDelta = rootTransform.position - _lastPosition;
            Quaternion rotationDelta = Quaternion.Inverse(rootTransform.rotation) * _lastRotation;
            GetPositionAndRotationDelta(rootTransform, positionDelta, rotationDelta);
            
            _lastPosition = rootTransform.position;
            _lastRotation = rootTransform.rotation;
        }

        private void GetPositionAndRotationDelta(Transform rootTransform, Vector3 posDelta, Quaternion rotDelta)
        {
            rotDelta.ToAngleAxis(out float angleDiff, out Vector3 axis);
            foreach (var rider in _riders)
            {
                _childRotate.transform.position = rider.transform.position;
                _childRotate.RotateAround(rootTransform.position, axis, -angleDiff);
                /*rider.AddForce((rider.transform.position - _childRotate.transform.position) / 2f,
                    ForceMode.VelocityChange);*/
                rider.MovePosition(_childRotate.transform.position + posDelta);
            }
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
            
            /*
            Transform rootTransform = transform;
            if (root) rootTransform = root;
            Vector3 positionDelta = rootTransform.position - _lastPosition;
            Quaternion rotationDelta = Quaternion.Inverse(rootTransform.rotation) * _lastRotation;
            rotationDelta.ToAngleAxis(out float angleDiff, out Vector3 axis);
            _childRotate.transform.position = other.rigidbody.transform.position;
            _childRotate.RotateAround(rootTransform.position, axis, -angleDiff);
            other.rigidbody.AddForce(
                (positionDelta + (_childRotate.transform.position - other.rigidbody.transform.position)) * 9999,
                ForceMode.VelocityChange);*/
        }
    }
}
