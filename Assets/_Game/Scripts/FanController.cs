using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class FanController : MonoBehaviour
    {

        [SerializeField] private float force = 1f;
        [SerializeField] private bool instant = false;

        private List<Rigidbody> _rigidbodies = new List<Rigidbody>();

        private void FixedUpdate()
        {
            if (instant) return;
            foreach (var rb in _rigidbodies)
            {
                rb.AddForce(transform.up * (force * Time.fixedDeltaTime), ForceMode.VelocityChange);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody) return;
            _rigidbodies.Add(other.attachedRigidbody);
            if (instant)
            {
                other.attachedRigidbody.linearVelocity = transform.up * force;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.attachedRigidbody) return;
            _rigidbodies.Remove(other.attachedRigidbody);
        }
    }
}
