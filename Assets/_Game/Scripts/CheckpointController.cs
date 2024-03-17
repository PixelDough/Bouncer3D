using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PixelDough.Bouncer;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckpointController : MonoBehaviour
{
    [SerializeField] private Transform respawnPointTransform;

    [SerializeField] private Transform flagRoot;
    [SerializeField] private Transform flagBone;
    
    public int index = 0;

    private void Start()
    {
        HideFlag();
    }

    private void Update()
    {
        flagBone.Rotate(new Vector3(25f, 0f, 0f) * Time.deltaTime, Space.Self);
    }

    public void HideFlag()
    {
        flagBone.localScale = new Vector3(1f, 1f, 0.001f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.attachedRigidbody) return;

        if (other.attachedRigidbody.CompareTag("Player"))
        {
            PlayerController playerController = other.attachedRigidbody.GetComponent<PlayerController>();
            if (playerController.currentCheckpoint) playerController.currentCheckpoint.HideFlag();
            playerController.SetRespawnPoint(
                respawnPointTransform.position,
                respawnPointTransform.forward, 
                this
            );

            flagBone.DOScaleZ(1, 0.5f)
                .SetEase(Ease.OutBack);

            Vector3 dir = Random.onUnitSphere;
            Vector3 dirFlat = new Vector3(dir.x, 0f, dir.z).normalized;
            float angleAmount = Mathf.Clamp(other.attachedRigidbody.velocity.magnitude, 5f, 25f);
            flagRoot.transform.rotation *= Quaternion.AngleAxis(angleAmount, dirFlat); 
            flagRoot.DORotate(Vector3.zero, 1.5f)
                .SetEase(Ease.OutElastic);
        }
    }
}
