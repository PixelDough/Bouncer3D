using System;
using PixelDough.Bouncer.UI;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PlayerStuffManager : MonoBehaviour
    {

        public PlayerController playerController;
        public PlayerHudController playerHudController;

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
            playerHudController.SetHiddenState(false);
        }
        
        public void CutsceneEnded()
        {
            playerHudController.SetHiddenState(false);
            
        }
    }
}
