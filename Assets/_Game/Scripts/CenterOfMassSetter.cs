using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class CenterOfMassSetter : MonoBehaviour
    {

        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private Transform centerOfMass;

        private void Start()
        {
            rigidbody.centerOfMass = rigidbody.transform.InverseTransformPoint(centerOfMass.position);
        }
    }
}
