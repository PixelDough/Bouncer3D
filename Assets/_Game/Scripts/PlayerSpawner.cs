using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject editorPreviewObject;
        [SerializeField] private GameObject playerStuffPrefab;

        private void Awake()
        {
            editorPreviewObject.SetActive(false);
            Instantiate(playerStuffPrefab, transform.position, transform.rotation);
        }
    }
}
