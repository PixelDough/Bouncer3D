using System;
using System.Collections;
using System.Collections.Generic;
using PixelDough.Bouncer;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    [SerializeField] private Transform respawnPointTransform;

    public int index = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.attachedRigidbody) return;

        if (other.attachedRigidbody.CompareTag("Player"))
        {
            PlayerController playerController = other.attachedRigidbody.GetComponent<PlayerController>();
            playerController.SetRespawnPoint(respawnPointTransform.position, respawnPointTransform.forward);
        }
    }
}
