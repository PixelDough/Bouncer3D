using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Rotate : MonoBehaviour
{
    public Rigidbody rigidbodyOptional;
    public Space space = Space.World;
    public Vector3 axis = new Vector3(0, 1, 0);
    public bool ignoreTimeScale = false;
    public bool autoScale = false;
    public Vector2 autoScaleMinMaxScale = new Vector2(1f, 75f);
    public float autoScaleMaxMultiplier = 0.333f;
    [Range(0.0001f, 1f)]
    public float autoScalePower = 0.2f; 
    
    public float speed = 5f;

    private void Update()
    {
        if (rigidbodyOptional) return;
        
        float deltaTime = Time.deltaTime;
        if (ignoreTimeScale)
            deltaTime = Time.unscaledDeltaTime;

        float speedModifier = GetAutoScaleMultiplier();
        
        switch (space)
        {
            case Space.World:
                transform.Rotate(axis, speed * deltaTime * speedModifier, Space.World);
                break;
            case Space.Self:
                /*Vector3 val = transform.localRotation.eulerAngles;
                val.x += axis.x * speed * deltaTime;
                val.y += axis.y * speed * deltaTime;
                val.z += axis.z * speed * deltaTime;
                transform.localRotation = Quaternion.Euler(val);*/
                transform.Rotate(axis, speed * deltaTime * speedModifier, Space.Self);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!rigidbodyOptional) return;
        
        float deltaTime = Time.fixedDeltaTime;
        if (ignoreTimeScale)
            deltaTime = Time.fixedUnscaledDeltaTime;
        
        float speedModifier = GetAutoScaleMultiplier();

        switch (space)
        {
            case Space.World:
                rigidbodyOptional.MoveRotation(
                    Quaternion.Euler((speed * deltaTime * speedModifier) * axis) * rigidbodyOptional.rotation);
                break;
            case Space.Self:
                rigidbodyOptional.MoveRotation(
                    rigidbodyOptional.rotation * Quaternion.Euler(axis * (speed * deltaTime * speedModifier)));
                break;
        }
    }

    private float GetAutoScaleMultiplier()
    {
        if (!autoScale) return 1f;
        float scale = transform.localScale.x;
        float autoScalePercent =
            Mathf.Pow(Mathf.InverseLerp(autoScaleMinMaxScale.x, autoScaleMinMaxScale.y, scale), autoScalePower);
        float autoScaleModifier = Mathf.Lerp(1f, autoScaleMaxMultiplier, autoScalePercent);
        return autoScaleModifier;
    }
}
