using System;
using PixelDough.Bouncer.UI;
using Rewired.Integration.Cinemachine3;
using Unity.Cinemachine;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PlayerStuffManager : MonoBehaviour
    {

        public PlayerController playerController;

        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private CinemachineOrbitalFollow cinemachineFreeLook;
        [SerializeField] private RewiredCinemachineInputAxisController cinemachineInputAxisController;

        public ParticleSystem sandRollParticleSystem;
        public ParticleSystem sandBurstParticleSystem;

        public void SetCameraForward(Vector3 forward)
        {
            cinemachineFreeLook.HorizontalAxis.Value = Vector3.SignedAngle(Vector3.forward, forward, Vector3.up);
            cinemachineFreeLook.VerticalAxis.Value = 50f;
        }
        
        public void SetCameraRotation(Vector3 angles)
        {
            cinemachineFreeLook.HorizontalAxis.Value = angles.y;
            if (angles.x > 90) angles.x = 360 - angles.x;
            cinemachineFreeLook.VerticalAxis.Value = Mathf.Lerp(40, 60, Mathf.InverseLerp(90, -90, angles.x));
        }

        private void LateUpdate()
        {
            float targetFOV = Mathf.InverseLerp(0f, 2000, playerController.Velocity.sqrMagnitude) * 30f + 100f;
            cinemachineCamera.Lens.FieldOfView = MathHelpers.ExpDecay(cinemachineCamera.Lens.FieldOfView, targetFOV, 5f, Time.deltaTime);
            cinemachineInputAxisController.enabled = GameManager.DoPlayerMovement;
        }
    }
}
