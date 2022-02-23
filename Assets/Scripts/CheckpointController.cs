using System;
using System.Collections;
using System.Collections.Generic;
using PixelDough.Bouncer;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    [SerializeField] private Transform respawnPointTransform;

    [SerializeField] private Transform flagRoot;
    
    public int index = 0;

    private void Update()
    {
        flagRoot.Rotate(new Vector3(0f, 25f, 0f) * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.attachedRigidbody) return;

        if (other.attachedRigidbody.CompareTag("Player"))
        {
            PlayerController playerController = other.attachedRigidbody.GetComponent<PlayerController>();
            playerController.SetRespawnPoint(respawnPointTransform.position, respawnPointTransform.forward);

            flagRoot.gameObject.SetActive(true);
        }
    }
}
