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
    
    public float speed = 5f;

    private void Update()
    {
        if (rigidbodyOptional) return;
        
        float deltaTime = Time.deltaTime;
        if (ignoreTimeScale)
            deltaTime = Time.unscaledDeltaTime;

        switch (space)
        {
            case Space.World:
                transform.Rotate(axis, speed * deltaTime, Space.World);
                break;
            case Space.Self:
                /*Vector3 val = transform.localRotation.eulerAngles;
                val.x += axis.x * speed * deltaTime;
                val.y += axis.y * speed * deltaTime;
                val.z += axis.z * speed * deltaTime;
                transform.localRotation = Quaternion.Euler(val);*/
                transform.Rotate(axis, speed * deltaTime, Space.Self);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!rigidbodyOptional) return;
        
        float deltaTime = Time.fixedDeltaTime;
        if (ignoreTimeScale)
            deltaTime = Time.fixedUnscaledDeltaTime;

        switch (space)
        {
            case Space.World:
                rigidbodyOptional.MoveRotation(
                    Quaternion.Euler((speed * deltaTime) * axis) * rigidbodyOptional.rotation);
                break;
            case Space.Self:
                rigidbodyOptional.MoveRotation(
                    rigidbodyOptional.rotation * Quaternion.Euler(axis * (speed * deltaTime)));
                break;
        }
    }
}
