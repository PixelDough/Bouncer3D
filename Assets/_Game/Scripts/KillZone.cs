using System;
using System.Collections;
using System.Collections.Generic;
using PixelDough.Bouncer;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.attachedRigidbody) return;
        
        if (other.attachedRigidbody.CompareTag("Player"))
        {
            PlayerController playerController = other.attachedRigidbody.GetComponent<PlayerController>();
            playerController.Kill();
        }
    }
}
