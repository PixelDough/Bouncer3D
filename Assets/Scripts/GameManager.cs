using System;
using Rewired;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                if (!_instance) _instance = FindObjectOfType<GameManager>();
                return _instance;
            }
            private set => _instance = value;
        }
        private static GameManager _instance;

        public Player Input;

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Input = ReInput.players.GetPlayer(0);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}