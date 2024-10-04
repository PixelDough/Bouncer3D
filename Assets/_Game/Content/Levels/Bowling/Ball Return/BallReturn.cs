using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PixelDough.Bouncer
{
    public class BallReturn : MonoBehaviour
    {
        // Fields
        [SerializeField] private float speed;
        [SerializeField] private float scale;
        [SerializeField] private float distance;
        [SerializeField] private Lattice.Lattice lattice;

        private void Update()
        {
            for (int i = 0; i < lattice.Resolution.x; i++)
            {
                for (int j = 0; j < lattice.Resolution.y; j++)
                {
                    for (int k = 0; k < lattice.Resolution.z; k++)
                    {
                        if (k != lattice.Resolution.z - 1) continue;
                        
                        lattice.SetHandleOffset(i, j, k, new Vector3(0, 0, Mathf.Sin(Time.time * speed) * scale));
                    }
                }
            }
        }
    }
}
