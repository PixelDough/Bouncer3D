using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PixelDough.Bouncer
{
    public class RenderManager : MonoBehaviour
    {
        public static RenderManager Instance;

        [SerializeField] private Camera baseCamera;
        public Camera BaseCamera => baseCamera;
        
        private static List<Camera> _camerasForStack = new List<Camera>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitOnLoad()
        {
            Instance = null;
            _camerasForStack = new List<Camera>();
        }

        private void Start()
        {
            Instance = this;
            
            foreach (Camera stackCam in _camerasForStack)
            {
                AddCameraToStack(stackCam);
            }
        }

        public static void AddCameraToStack(Camera overlayCamera)
        {
            if (!Instance || !Instance.BaseCamera)
            {
                if (!_camerasForStack.Contains(overlayCamera))
                    _camerasForStack.Add(overlayCamera);
                return;
            }
            var cameraData = Instance.BaseCamera.GetUniversalAdditionalCameraData();
            if (cameraData.cameraStack.Contains(overlayCamera)) return;
            cameraData.cameraStack.Add(overlayCamera);
        }

        public static void RemoveCameraFromStack(Camera overlayCamera)
        {
            if (!Instance || !Instance.BaseCamera)
            {
                if (_camerasForStack.Contains(overlayCamera))
                    _camerasForStack.Remove(overlayCamera);
                return;
            }
            var cameraData = Instance.BaseCamera.GetUniversalAdditionalCameraData();
            if (!cameraData.cameraStack.Contains(overlayCamera)) return;
            cameraData.cameraStack.Remove(overlayCamera);
        }
    }
}
