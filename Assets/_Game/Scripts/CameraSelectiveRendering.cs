using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class CameraSelectiveRendering : MonoBehaviour
    {
        [SerializeField] private string renderLayer;
        [SerializeField] private string doNotRenderLayer;
        [SerializeField] private GameObject objectToRender;

        private void OnPreRender()
        {
            objectToRender.layer = LayerMask.NameToLayer(renderLayer);
        }

        private void OnPostRender()
        {
            objectToRender.layer = LayerMask.NameToLayer(doNotRenderLayer);
        }
    }
}
