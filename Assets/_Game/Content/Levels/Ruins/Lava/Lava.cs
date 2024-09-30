using System;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class Lava : LevelFeature
    {
        [SerializeField] private List<int> checkpointHeights = new List<int> { 0, 13 };
        [SerializeField] private FMODUnity.StudioEventEmitter lavaGurgleSoundEmitter;
        
        
        private PlayerController _player;

        private void Start()
        {
            Initialize();
        }
        

        public override void Initialize()
        {
            _player ??= FindFirstObjectByType<PlayerController>();

            var newHeight = Mathf.Max(0f, _player.transform.position.y - 10f);
            SetY(newHeight);
        }

        private void Update()
        {
            float deltaTime = LevelManager.IsPaused ? 0 : Time.deltaTime;
            _player ??= FindFirstObjectByType<PlayerController>();
            lavaGurgleSoundEmitter.transform.position = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
            var followHeight = _player.transform.position.y - 8f;
            if (followHeight < 0) return;
            var heightDiff = Mathf.Max(0, _player.transform.position.y - transform.position.y);
            transform.Translate(Vector3.up * ((0.125f + heightDiff * 0.05f) * deltaTime));
            
        }

        [Command("lava-set-y", MonoTargetType.Single)]
        public void SetY(float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

    }
}
