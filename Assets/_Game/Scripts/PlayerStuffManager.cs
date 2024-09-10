using System;
using PixelDough.Bouncer.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PlayerStuffManager : MonoBehaviour
    {

        public PlayerController playerController;

        [SerializeField] private CinemachineOrbitalFollow cinemachineFreeLook;

        public ParticleSystem sandRollParticleSystem;
        public ParticleSystem sandBurstParticleSystem;

        public void SetCameraForward(Vector3 forward)
        {
            cinemachineFreeLook.HorizontalAxis.Value = Vector3.SignedAngle(Vector3.forward, forward, Vector3.up);
            cinemachineFreeLook.VerticalAxis.Value = 50f;
        }
    }
}
