using System;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PauseTextRenderer : MonoBehaviour
    {
        [SerializeField] private Font3DString font3DString;
        [SerializeField] private Camera renderInCamera;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private string text = "Text";
        [SerializeField] private float fontSizeInUnits = 1;

        private void OnValidate()
        {
            RefreshObjects();
        }

        private void Update()
        {
            RefreshObjects();
        }

        private void RefreshObjects()
        {
            font3DString?.SetText(text);
            font3DString?.SetFontSize(fontSizeInUnits);
            font3DString?.SetRenderInCamera(renderInCamera);
            if (renderInCamera) renderInCamera.targetTexture = renderTexture;
        }
    }
}
