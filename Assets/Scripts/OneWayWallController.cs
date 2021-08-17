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
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody) return;
            if (other.attachedRigidbody.CompareTag("Player"))
            {
                Physics.IgnoreCollision(other, collider, true);

                float rotationDifference = 1;
                if  (Mathf.Abs(Vector3.SignedAngle(transform.up, transform.position - other.transform.position, 
                    transform.up)) > 45)
                {
                    rotationDifference *= -1f;
                }
                
                modelTransform.localRotation = Quaternion.identity;
                if (LeanTween.isTweening(_spinTweenId)) LeanTween.cancel(_spinTweenId);
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
