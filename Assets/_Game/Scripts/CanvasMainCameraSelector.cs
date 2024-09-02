using System;
using System.Collections;
using System.Collections.Generic;
using PixelDough.Bouncer;
using UnityEngine;

public class CanvasMainCameraSelector : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    private void Start()
    {
        canvas.worldCamera = GameManager.Instance.uiCamera;
    }

    private void OnValidate()
    {
        // if (!canvas)
        //     canvas = GetComponent<Canvas>();
        // if (!canvas.worldCamera)
        //     canvas.worldCamera = Camera.main;
    }
}
