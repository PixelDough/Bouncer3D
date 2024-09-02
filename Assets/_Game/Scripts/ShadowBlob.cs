using System;
using UnityEngine;

public class ShadowBlob : MonoBehaviour
{

    [SerializeField] private Transform quad;
    [SerializeField] private float maxDistance = 4f;
    [SerializeField] private float groundOffset = 0.02f;
    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        if (layerMask == 0)
            layerMask = LayerMask.GetMask("CamBlocker");
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
        
        
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            quad.gameObject.SetActive(true);
            quad.position = hit.point + (Vector3.up * groundOffset);
            
            float percentToMaxDistance = 1 - (hit.distance / maxDistance);
            quad.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, percentToMaxDistance);
            quad.forward = -hit.normal;
        }
        else
        {
            quad.gameObject.SetActive(false);
        }
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

}
