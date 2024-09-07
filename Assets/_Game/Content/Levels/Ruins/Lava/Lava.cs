using System;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class Lava : LevelFeature
    {
        [SerializeField] private List<int> checkpointHeights = new List<int> { 0, 13 };
        
        public override void Initialize()
        {
            SetY(checkpointHeights[0]);
        }

        private void Update()
        {
            transform.Translate(Vector3.up * (0.2f * Time.deltaTime));
        }

        [Command("lava-set-y", MonoTargetType.Single)]
        public void SetY(float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

    }
}
