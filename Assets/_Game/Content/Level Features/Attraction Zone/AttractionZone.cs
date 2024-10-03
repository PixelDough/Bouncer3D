using System;
using DrawXXL;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class AttractionZone : MonoBehaviour
    {
        [SerializeField] private float attractionSpeed = 10f;
        [SerializeField] private float attractionForce = 10f;
        [SerializeField] private float attractionRadius = 3f;
        [SerializeField] private float attractionHeight = 10f;
        [SerializeField] private bool isAttracting = true;

        private int _hitCount = 0;
        private RaycastHit[] _hitResults = new RaycastHit[8];

        private void FixedUpdate()
        {
            if (!isAttracting) return;
            _hitCount = Physics.SphereCastNonAlloc(transform.position, attractionRadius, -transform.up, _hitResults, attractionHeight);
            if (_hitCount == 0) return;

            for (int i = 0; i < _hitCount; i++)
            {
                var hitResult = _hitResults[i];
                Rigidbody rbHit = hitResult.rigidbody;
                if (rbHit is null) continue;
                if (rbHit.isKinematic) continue;

                Vector3 currentVel = rbHit.linearVelocity;
                Vector3 attractionVel = Vector3.Project(currentVel, transform.up);
                Vector3 newVel = MathHelpers.ExpDecay(attractionVel, transform.up * attractionForce, attractionForce, Time.deltaTime);
                rbHit.linearVelocity = newVel + Vector3.ProjectOnPlane(currentVel, transform.up);
            }
        }

        private void OnDrawGizmos()
        {
            DrawShapes.Capsule(
                transform.position,
                transform.position - transform.up * attractionHeight,
                attractionRadius,
                Color.green,
                text: _hitCount.ToString()
            );
        }
    }
}
