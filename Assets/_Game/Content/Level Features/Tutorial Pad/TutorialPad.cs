using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization;

namespace PixelDough.Bouncer
{
    public class TutorialPad : MonoBehaviour
    {
        [SerializeField] private Transform questionMark;
        [SerializeField] private LocalizedString tutorialText;

        private int _scaleTweenID = -1;
        private Camera _camera;

        private float _initialY = 2f;
        private Vector3 _initialScale = new Vector3(2, 2, 1);
        private Vector3 _shrunkScale = new Vector3(1, 1, 0.5f);
        private Quaternion _spinRotation = Quaternion.identity;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            Quaternion targetRotation = Quaternion.LookRotation(_camera.transform.position - questionMark.position, transform.up); 
            Quaternion newRotation = MathHelpers.ExpDecay(questionMark.rotation, targetRotation, 16f, Time.deltaTime);
            questionMark.rotation = newRotation;

            Vector3 pos = questionMark.localPosition;
            pos.y = Mathf.Cos(Time.time) * 0.25f + _initialY;
            questionMark.localPosition = pos;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody) return;
            if (!other.attachedRigidbody.CompareTag("Player")) return;

            if (DOTween.IsTweening(_scaleTweenID)) DOTween.Kill(_scaleTweenID);
            _scaleTweenID = questionMark.DOScale(_shrunkScale, 0.5f).SetEase(Ease.OutCirc).intId;
            GameManager.Instance.ShowTutorialText(tutorialText.GetLocalizedString());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.attachedRigidbody) return;
            if (!other.attachedRigidbody.CompareTag("Player")) return;
            
            if (DOTween.IsTweening(_scaleTweenID)) DOTween.Kill(_scaleTweenID);
            _scaleTweenID = questionMark.DOScale(_initialScale, 0.5f).SetEase(Ease.OutBack).intId;
            GameManager.Instance.HideTutorialText();
        }
    }
}
