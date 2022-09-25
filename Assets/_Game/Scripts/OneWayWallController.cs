using System;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class OneWayWallController : MonoBehaviour
    {
        [SerializeField] private Transform modelTransform;
        [SerializeField] private new Collider collider;
        [SerializeField] private AnimationCurve spinCurve;

        private int _spinTweenId = -1;

        private static bool _hasHitPlayerOnThisFrame = false;

        private void Update()
        {
            _hasHitPlayerOnThisFrame = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody) return;
            if (other.attachedRigidbody.CompareTag("Player"))
            {
                //Debug.Log(other.transform.position);
                if (Vector3.Dot(other.transform.position - transform.position, -transform.forward) > 0.1f)
                    return;
                /*if (Vector3.Angle(transform.forward, other.transform.position - transform.position) > 90)
                    return;*/
                
                Physics.IgnoreCollision(other, collider, true);

                if (Vector3.Dot(other.attachedRigidbody.velocity, -transform.forward) < 5f && !_hasHitPlayerOnThisFrame)
                {
                    other.attachedRigidbody.AddForce(-transform.forward * 6f, ForceMode.VelocityChange);
                    _hasHitPlayerOnThisFrame = true;
                }
                
                float rotationDifference = 1;
                if  (Mathf.Abs(Vector3.SignedAngle(transform.up, transform.position - other.transform.position, 
                    transform.right)) > 90)
                {
                    rotationDifference *= -1f;
                }
                
                modelTransform.localRotation = Quaternion.identity;
                if (LeanTween.isTweening(_spinTweenId)) LeanTween.cancel(_spinTweenId);
                FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/ONE WAY WALL/Spin", gameObject);
                _spinTweenId = LeanTween.value(gameObject, (float value) =>
                    {
                        modelTransform.localRotation = Quaternion.Euler(value, 0f, 0f);
                    }, modelTransform.localRotation.eulerAngles.x % 360f, rotationDifference * 360f * 2f, 1f)
                    .setEase(spinCurve)
                    .uniqueId;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.attachedRigidbody) return;
            if (other.attachedRigidbody.CompareTag("Player"))
            {
                Physics.IgnoreCollision(other, collider, false);
            }
        }
    }
}
