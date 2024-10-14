using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class ModeButton : MonoBehaviour
    {
        [SerializeField] private Transform iconTransform;
        [SerializeField] private List<Font3DString> modeTexts;
        
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
            iconTransform.localScale = Vector3.one;
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
    }
}
