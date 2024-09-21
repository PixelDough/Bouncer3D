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
        if (!other.attachedRigidbody.CompareTag("Player")) return;
        
        PlayerController playerController = other.attachedRigidbody.GetComponent<PlayerController>();
        playerController.Kill();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.rigidbody) return;
        if (!other.rigidbody.CompareTag("Player")) return;
        
        PlayerController playerController = other.rigidbody.GetComponent<PlayerController>();
        playerController.Kill();
    }
}
