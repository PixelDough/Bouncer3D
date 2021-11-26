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
            playerHudController.SetHiddenState(false);
        }

        public void CollectShells(int count)
        {
            LevelManager.Instance.shellsCollected += count;
        }

        public void CutsceneBegin()
        {
            GameManager.Instance.screenFadeController.FadeToBlack();
            playerHudController.SetHiddenState(false);
        }
        
        public void CutsceneEnded()
        {
            playerHudController.SetHiddenState(true);
            
        }

        public void SetCameraForward(Vector3 forward)
        {
            cinemachineFreeLook.m_XAxis.Value = Vector3.SignedAngle(Vector3.forward, forward, Vector3.up);
        }
    }
}
