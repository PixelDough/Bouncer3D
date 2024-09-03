using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;

        private Vector3 _startPos = Vector3.zero;
        private Vector3 _resetPos = Vector3.zero;

        private void Start()
        {
            _startPos = transform.position;
            
            _resetPos = _startPos;
            _resetPos.y = -15;
        }

        private void OnTriggerEnter(Collider other)
        {
            var rb = other.attachedRigidbody;
            if (!rb) return;

            Vector3 dir = Vector3.Normalize(rb.worldCenterOfMass - transform.position);

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(dir * 20, ForceMode.Impulse);
            
            Pop();
        }

        private void Pop(bool doParticles = true)
        {
            if (doParticles)
                particleSystem.Play();

            Vector3 currentPos = transform.position;
            transform.position = _resetPos;
            particleSystem.transform.position = currentPos;
            
            LeanTween.cancel(gameObject);
            transform.LeanMove(_startPos, Random.Range(5f, 10f)).setEaseOutSine();
        }

    }
}
