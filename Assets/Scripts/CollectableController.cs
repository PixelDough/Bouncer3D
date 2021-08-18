using System;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class CollectableController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem collectParticleSystem;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody) return;
            if (other.attachedRigidbody.CompareTag("Player"))
            {
                collectParticleSystem.transform.parent = null;
                collectParticleSystem.Play();
                gameObject.SetActive(false);
            }
        }
    }
}
