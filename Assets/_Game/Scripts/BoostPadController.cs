using System;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class BoostPadController : MonoBehaviour
    {
        [SerializeField] private float boostAmount = 15f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody) return;
            if (other.attachedRigidbody.CompareTag("Player"))
            {
                other.attachedRigidbody.transform.position = transform.position + (transform.up * 0.25f);
                other.attachedRigidbody.velocity = transform.forward * boostAmount;
                other.attachedRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
