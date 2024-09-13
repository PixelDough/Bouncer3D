using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace PixelDough.Bouncer
{
    public class RenderBufferGenerator : MonoBehaviour
    {
        [SerializeField] private int targetHeight = 288;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private RawImage targetRawImage;

        private RenderTexture _renderTexture;
        private int _previousScreenWidth;
        private int _previousScreenHeight;
        private static readonly int DitherResolution = Shader.PropertyToID("_DitherResolution");

        private void Start()
        {
            CreateRenderTexture();
            _previousScreenWidth = Screen.width;
            _previousScreenHeight = Screen.height;
        }

        private void Update()
        {
            if (Screen.width != _previousScreenWidth || Screen.height != _previousScreenHeight)
            {
                // Screen size changed, regenerate the render texture
                CreateRenderTexture();
                _previousScreenWidth = Screen.width;
                _previousScreenHeight = Screen.height;
            }
            
            // Set the global shader value _DitherResolution
            Shader.SetGlobalVector(DitherResolution,
                new Vector4(_renderTexture.width, targetHeight, 1.0f / _renderTexture.width, 1.0f / targetHeight));
        }

        private void CreateRenderTexture()
        {
            // Release the previous render texture
            if (_renderTexture != null)
            {
                _renderTexture.Release();
            }

            // Calculate new texture dimensions
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float ratio = screenWidth / screenHeight;
            int targetWidth = Mathf.FloorToInt(targetHeight * ratio);

            // Create a new render texture
            _renderTexture = new RenderTexture(targetWidth, targetHeight, 32, RenderTextureFormat.ARGB32);
            if (!_renderTexture.IsCreated())
            {
                _renderTexture.Create();
            }

            // Set the filter mode to point (no bilinear filtering)
            _renderTexture.filterMode = FilterMode.Point;

            // Assign the render texture to the camera and raw image
            targetCamera.targetTexture = _renderTexture;
            targetRawImage.texture = _renderTexture;
        }

        private void OnDisable()
        {
            if (_renderTexture != null)
            {
                _renderTexture.Release();
                _renderTexture = null;
            }
        }
    }
}