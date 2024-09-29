using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace PixelDough.Bouncer
{
    public class PauseScreenButton : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHit;

        private bool _isHit = false;
        
        private void OnCollisionEnter(Collision other)
        {
            Rigidbody rb = other.rigidbody;
            if (rb is null) return;
            if (!rb.CompareTag("Player")) return;
            if (other.GetContact(0).normal.y > -0.95f || Mathf.Abs(rb.linearVelocity.y) < 6f) return;
            if (_isHit) return;
            
            HitSequence();
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isHit) return;
            Rigidbody rb = other.attachedRigidbody;
            if (rb is null) return;
            if (!rb.CompareTag("Player")) return;
            
            transform.localPosition = MathHelpers.ExpDecay(transform.localPosition,
                new Vector3(transform.localPosition.x, -0.057f, transform.localPosition.z), 13f,
                Time.deltaTime);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isHit) return;
            Rigidbody rb = other.attachedRigidbody;
            if (rb is null) return;
            if (!rb.CompareTag("Player")) return;
            
            transform.DOLocalMoveY(0f, 0.1f).SetEase(Ease.OutSine);
        }
        
        private async Awaitable HitSequence()
        {
            _isHit = true;
            await transform.DOLocalMoveY(-0.1f, 0.1f).SetEase(Ease.OutSine).AsyncWaitForCompletion();
            await transform.DOLocalMoveY(0, 0.1f).SetEase(Ease.InSine).AsyncWaitForCompletion();
            onHit?.Invoke();
            _isHit = false;
        }
    }
}
