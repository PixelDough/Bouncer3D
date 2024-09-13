using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PixelDough.Bouncer
{
    [RequireComponent(typeof(Camera))]
    public class CameraOverlayMatcher : MonoBehaviour
    {
        [SerializeField] private new Camera camera;

        private List<Camera> _cameraStack = new List<Camera>();
        
        private void OnValidate()
        {
            camera ??= GetComponent<Camera>();
        }

        private void Start()
        {
            _cameraStack = camera.GetUniversalAdditionalCameraData().cameraStack;
        }

        private void LateUpdate()
        {
            foreach (Camera overlayCam in _cameraStack)
            {
                overlayCam.aspect = camera.aspect;
                overlayCam.rect = camera.rect;
            }
        }
    }
}
