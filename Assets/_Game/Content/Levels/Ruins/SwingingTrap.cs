using System;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class SwingingTrap : MonoBehaviour
    {
        [SerializeField, Range(0, 180)] private float angleRange = 25f;
        [SerializeField] private float loopTime = 1f;
        [SerializeField] private bool flipped = false;
        [SerializeField] private bool instantRepeat = false;
        [SerializeField] private Transform hingeTransform;

        private void Update()
        {
            float t = instantRepeat 
                ? Mathf.Repeat(Time.time / loopTime, 1f) 
                : Mathf.PingPong(Time.time / loopTime, 1f);
            float angle = instantRepeat 
                ? Mathf.Lerp(-angleRange, angleRange, t) 
                : Mathf.SmoothStep(-angleRange, angleRange, t);
            if (flipped)
            {
                angle = -angle;
            }
            hingeTransform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
