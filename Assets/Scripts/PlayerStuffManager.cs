using System;
using Cinemachine;
using PixelDough.Bouncer.UI;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PlayerStuffManager : MonoBehaviour
    {

        public PlayerController playerController;
        public PlayerHudController playerHudController;

        [SerializeField] private CinemachineFreeLook cinemachineFreeLook;

        private void Start()
        {
            playerHudController.SetVisibility(false);
        }

        public void CollectShells(int count)
        {
            LevelManager.Instance.shellsCollected += count;
        }

        public void CutsceneBegin()
        {
            playerHudController.SetVisibility(false);
        }
        
        public void CutsceneEnded()
        {
            playerHudController.SetVisibility(true);
            
        }

        public void SetCameraForward(Vector3 forward)
        {
            cinemachineFreeLook.m_XAxis.Value = Vector3.SignedAngle(Vector3.forward, forward, Vector3.up);
        }
    }
}
