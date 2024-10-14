using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class ModeButton : MonoBehaviour
    {
        private static readonly int Tint = Shader.PropertyToID("_Tint");
        [SerializeField] private Transform iconTransform;
        [SerializeField] private MeshRenderer iconMeshRenderer;
        [SerializeField] private MeshFilter iconMeshFilter;
        [SerializeField] private List<Font3DString> modeTexts;
        [SerializeField] private Mesh lockMesh;
        [SerializeField] private string lockedText;

        private List<Color> _fontColors = new List<Color>();
        private List<string> _fontTexts = new List<string>();
        private bool _isButtonEnabled = true;
        private Mesh _iconMesh;

        public void Init()
        {
            _fontColors = modeTexts.Select(f => f.textColor).ToList();
            _fontTexts = modeTexts.Select(f => f.Text).ToList();
            _iconMesh = iconMeshFilter.sharedMesh;
        }

        private void Update()
        {
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            iconTransform.localEulerAngles = new Vector3(0, Mathf.Repeat(Time.time * -180f, 360f), 0);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            iconTransform.DOKill(true);
            iconTransform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 6);
            UpdateRotation();
            
            foreach (Font3DString font3DString in modeTexts)
            {
                font3DString.AnimPulse();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            _isButtonEnabled = true;
            for (var i = 0; i < modeTexts.Count; i++)
            {
                modeTexts[i].SetText(_fontTexts[i]);
                modeTexts[i].AnimPulse();
                
                iconMeshFilter.mesh = _iconMesh;
                
                modeTexts[i].textColor = _fontColors[i];
            }
        }

        public void Disable()
        {
            _isButtonEnabled = false;
            for (var i = 0; i < modeTexts.Count; i++)
            {
                if (i == 1) modeTexts[i].SetText(lockedText);
                modeTexts[i].AnimPulse();
                
                iconMeshFilter.mesh = lockMesh;
                
                modeTexts[i].textColor = new Color(0.75f, 0.75f, 0.75f, 1f);
            }
        }
    }
}
