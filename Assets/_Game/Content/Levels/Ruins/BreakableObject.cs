using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    [RequireComponent(typeof(Rigidbody))]
    public class BreakableObject : LevelFeature
    {
        [SerializeField, ReadOnly] private new Rigidbody rigidbody;
        [SerializeField, ReadOnly] private bool isKinematic = false;
        [SerializeField] private GameObject solidObject;
        [SerializeField] private List<Rigidbody> brokenParts = new List<Rigidbody>();
        [SerializeField] private float breakForce = 10f;
        
        private Vector3 _startPosition = Vector3.zero;
        private Quaternion _startRotation = Quaternion.identity;
        private bool _isBroken = false;

        protected override void OnValidate()
        {
            base.OnValidate();
            rigidbody ??= GetComponent<Rigidbody>();
            isKinematic = rigidbody.isKinematic;
            
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        public override void Initialize()
        {
            solidObject.SetActive(true);
            rigidbody.isKinematic = isKinematic;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.MovePosition(_startPosition);
            rigidbody.MoveRotation(_startRotation);
            brokenParts.ForEach(part =>
            {
                part.isKinematic = true;
                part.transform.position = _startPosition;
                part.transform.rotation = _startRotation;
                part.gameObject.SetActive(false);
            });
            _isBroken = false;
        }

        public void Break(Vector3 velocity)
        {
            solidObject.SetActive(false);
            rigidbody.isKinematic = true;
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            brokenParts.ForEach(part =>
            {
                part.gameObject.SetActive(true);
                part.transform.position = rigidbody.position;
                part.transform.rotation = rigidbody.rotation;
                part.isKinematic = false;
                part.angularVelocity = rigidbody.angularVelocity;
                part.linearVelocity = rigidbody.linearVelocity;
                part.AddForce(velocity * 0.2f, ForceMode.Impulse);
                part.AddForce(Random.insideUnitSphere, ForceMode.VelocityChange);
            });
            _isBroken = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_isBroken) return;
            if (other.relativeVelocity.magnitude < breakForce) return;
            Break(Vector3.zero);
        }
    }
}
