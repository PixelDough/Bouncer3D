using System;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class CollectableAttractor : MonoBehaviour
    {
        [SerializeField] private Transform transformToMove;
        private Transform _targetTransform;
        [SerializeField] private SphereCollider sphereCollider;
        
        /*private void Update()
        {
            if (!_targetTransform) return;

            transformToMove.position =
                Vector3.MoveTowards(transformToMove.position, _targetTransform.position, 2f * Time.deltaTime);
        }*/

        public void ResetValues()
        {
            _targetTransform = null;
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (_targetTransform) return;
            if (!other.attachedRigidbody) return;

            Vector3 direction = other.attachedRigidbody.transform.position - transform.position;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit))
            {
                if (!hit.rigidbody) return;
                if (hit.rigidbody != other.attachedRigidbody) return;
            }
            
            if (other.attachedRigidbody.gameObject.CompareTag("Player"))
            {
                float distanceCheckMax = sphereCollider.radius;
                float distanceModifier = Mathf.InverseLerp(0, 20, other.attachedRigidbody.velocity.magnitude);
                if (direction.magnitude > Mathf.Lerp(0.75f, distanceCheckMax, distanceModifier)) return;
                
                _targetTransform = other.attachedRigidbody.transform;
                LeanTween.move(transformToMove.gameObject, _targetTransform, 0.25f).setEaseInSine();
            }
        }
    }
}
