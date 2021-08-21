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
    public bool startRandom = false;
    public bool ignoreTimeScale = false;
    
    public float speed = 5f;
    
    private void Start()
    {
        if (startRandom)
        {
            switch (space)
            {
                case Space.World:
                    transform.Rotate(axis, Random.Range(0, 360), Space.World);
                    break;
                case Space.Self:
                    Vector3 val = transform.localRotation.eulerAngles;
                    val.x += axis.x * Random.Range(0, 360);
                    val.y += axis.y * Random.Range(0, 360);
                    val.z += axis.z * Random.Range(0, 360);
                    transform.localRotation = Quaternion.Euler(val);
                    break;
            }
        }
    }

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
                    rigidbodyOptional.rotation * Quaternion.Euler((speed * deltaTime) * axis));
                break;
            case Space.Self:
                rigidbodyOptional.MoveRotation(
                    rigidbodyOptional.rotation * Quaternion.Euler(axis * (speed * deltaTime)));
                break;
        }
    }
}
