using System;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using MEC;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PixelDough.Bouncer
{
    public class Binoculars : LevelFeature
    {
        [SerializeField] private Transform binocularsTransform;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private float fogDensityZoomed = 0.001f;
        [SerializeField] private Volume volumeComponent;
        [SerializeField] private EventReference openSound; 
        [SerializeField] private EventReference closeSound; 
        [SerializeField] private LocalizedString closePromptString;
        [SerializeField, ReadOnly] private Quaternion originalRotation;
        
        [Header("Input")]
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference jumpAction;
        

        private bool _canUse = true;
        private float _originalFOV;
        private float _originalFogDensity;
        private int _movePlayerTweenId = -1;
        private bool _isUsed = false;
        private Rigidbody _playerRb;
        private PlayerController _playerController;
        private Vector3 _angleOffset = Vector3.zero;

        private void OnValidate()
        {
            originalRotation = binocularsTransform.localRotation;
        }

        private void Start()
        {
            _originalFOV = cinemachineCamera.Lens.FieldOfView;
        }

        public override void Initialize() { }

        private void Update()
        {
            binocularsTransform.localPosition = new Vector3(0f, 1.5f + Mathf.Cos(Time.time * 2f) * 0.15f, 0f);
            
            if (!_isUsed) return;

            Vector2 rotateInput = lookAction.action.ReadValue<Vector2>() * 0.25f;
            _angleOffset.x -= rotateInput.y;
            _angleOffset.y += rotateInput.x;

            Quaternion rotationOffset = Quaternion.Euler(_angleOffset);
            if (Quaternion.Angle(Quaternion.identity, rotationOffset) > 10f)
            {
                rotationOffset = Quaternion.RotateTowards(Quaternion.identity, rotationOffset, 10f);
                _angleOffset = rotationOffset.eulerAngles;
            }

            binocularsTransform.localRotation = MathHelpers.ExpDecay(binocularsTransform.localRotation,
                originalRotation * rotationOffset, 10f, Time.deltaTime);
            
            if (jumpAction.action.WasPressedThisFrame())
            {
                Close();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_isUsed) return;
            if (!_canUse) return;
            Rigidbody attachedRigidbody = other.attachedRigidbody;
            if (attachedRigidbody is null) return;
            if (!attachedRigidbody.CompareTag("Player")) return;
            _playerRb ??= attachedRigidbody;
            _playerController ??= _playerRb.GetComponent<PlayerController>();
            Open();
        }

        private void Open()
        {
            Timing.RunCoroutine(C_OpenCoroutine().CancelWith(gameObject));
        }

        private void Close()
        {
            Timing.RunCoroutine(C_CloseCoroutine().CancelWith(gameObject));
        }

        private IEnumerator<float> C_OpenCoroutine()
        {
            _canUse = false;
            
            _playerRb.isKinematic = true;
            GameManager.DoPlayerMovement = false;
            GameManager.DoPlayerPhysics = false;
            _playerRb.transform.position = transform.position + Vector3.up * 0.35f;
            
            cinemachineCamera.Priority.Value = 100;
            _originalFogDensity = RenderSettings.fogDensity;
            DOTween.To(() => RenderSettings.fogDensity, x => RenderSettings.fogDensity = x, fogDensityZoomed, 0.25f).SetEase(Ease.OutSine);
            
            volumeComponent.enabled = true;
            volumeComponent.profile.TryGet(out Vignette vignette);
            vignette.intensity.value = 1f;
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0.5f, 0.25f).SetEase(Ease.OutSine);
            cinemachineCamera.Lens.FieldOfView = 90f;
            DOTween.To(() => cinemachineCamera.Lens.FieldOfView, x => cinemachineCamera.Lens.FieldOfView = x, _originalFOV, 0.25f).SetEase(Ease.OutSine);
            FMODUnity.RuntimeManager.PlayOneShot(openSound, binocularsTransform.position);
            
            yield return Timing.WaitForSeconds(0.25f);
            
            levelManager.ShowTutorialText(closePromptString.GetLocalizedString());
            
            _isUsed = true;
        }
        
        private IEnumerator<float> C_CloseCoroutine()
        {
            _isUsed = false;
            
            levelManager.HideTutorialText();
            
            volumeComponent.profile.TryGet(out Vignette vignette);
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 1f, 0.25f).SetEase(Ease.InSine);
            DOTween.To(() => cinemachineCamera.Lens.FieldOfView, x => cinemachineCamera.Lens.FieldOfView = x, 90f, 0.25f).SetEase(Ease.InSine);
            DOTween.To(() => RenderSettings.fogDensity, x => RenderSettings.fogDensity = x, _originalFogDensity, 0.25f).SetEase(Ease.OutSine);
            FMODUnity.RuntimeManager.PlayOneShot(closeSound, binocularsTransform.position);
            yield return Timing.WaitForSeconds(0.25f);
            
            cinemachineCamera.Priority.Value = -10;
            binocularsTransform.DOLocalRotate(originalRotation.eulerAngles, 0.5f).SetEase(Ease.OutCubic);
            volumeComponent.enabled = false;
            _playerRb.isKinematic = false;
            GameManager.DoPlayerMovement = true;
            GameManager.DoPlayerPhysics = true;
            _playerController.PlayerStuffManager.SetCameraRotation(binocularsTransform.eulerAngles);
            _angleOffset = Vector3.zero;
            
            yield return Timing.WaitForSeconds(1f);
            
            _canUse = true;
        }
    }
}
