using System;
using System.Collections;
using System.Collections.Generic;
using DrawXXL;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PlayerSpawner : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            DrawShapes.Sphere(
                transform.position + Vector3.up * 0.25f,
                0.25f,
                Color.cyan,
                text: "Player Spawn"
            );
            DrawBasics.VectorFrom(transform.position + Vector3.up * 0.25f, transform.forward, Color.cyan, 0.025f);
        }
    }
}
