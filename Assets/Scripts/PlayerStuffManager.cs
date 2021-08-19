using System;
using PixelDough.Bouncer.UI;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PlayerStuffManager : MonoBehaviour
    {

        public PlayerController playerController;
        public PlayerHudController playerHudController;


        public void CollectShells(int count)
        {
            LevelManager.Instance.shellsCollected += count;
        }
    }
}
