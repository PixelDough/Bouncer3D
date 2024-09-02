using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LowResCameraManager : MonoBehaviour
{

    private RenderTexture _renderTexture;
    [SerializeField] private new Camera camera;
    [SerializeField] private RawImage screenRawImage;

    [SerializeField] private Vector2Int resolution = new Vector2Int(512, 288);
    
    public enum ScalingMode
    {
        OneToOne,
        Windowbox,
        Letterbox
    }
    public ScalingMode scalingMode = ScalingMode.Letterbox;

    private void Awake()
    {
        RefreshTexture();
    }

    
    public void RefreshTexture()
    {
        if (!Application.isPlaying) return;
        
        _renderTexture = new RenderTexture(resolution.x, resolution.y, 32, RenderTextureFormat.DefaultHDR)
        {
            filterMode = FilterMode.Point
        };
        camera.targetTexture = _renderTexture;
        foreach (var cam in camera.GetUniversalAdditionalCameraData().cameraStack)
        {
            cam.targetTexture = _renderTexture;
        }
        
        screenRawImage.texture = _renderTexture;
    }

    private void Update()
    {
        float smallestFit = Mathf.Min((float)Screen.width / screenRawImage.texture.width,
            (float)Screen.height / screenRawImage.texture.height);

        float multiplier = 1;
        
        switch (scalingMode)
        {
            case ScalingMode.OneToOne:
                multiplier = 1f;
                break;
            case ScalingMode.Windowbox:
                multiplier = Mathf.Floor(smallestFit);
                break;
            case ScalingMode.Letterbox:
                multiplier = smallestFit;
                break;
        }
        
        
        screenRawImage.rectTransform.sizeDelta =
            new Vector2(screenRawImage.texture.width, screenRawImage.texture.height) * multiplier;
    }
}
