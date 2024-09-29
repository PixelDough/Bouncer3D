using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using PixelDough.Bouncer;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Rotate : LevelFeature
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
    
    [SerializeField, ReadOnly] private Quaternion startRotation = Quaternion.identity;
    private bool _isResetting = false;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (!rigidbodyOptional && TryGetComponent(out Rigidbody rb))
        {
            rigidbodyOptional = rb;
        }

        if (UnityEngine.Application.isPlaying) return;
        startRotation = rigidbodyOptional ? rigidbodyOptional.rotation : transform.rotation;
    }

    private void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        if (rigidbodyOptional)
        {
            rigidbodyOptional.MoveRotation(startRotation);
        }
        else
        {
            transform.rotation = startRotation;
        }

        Timing.KillCoroutines(gameObject);
        Timing.RunCoroutine(C_RotateCoroutine().CancelWith(gameObject), rigidbodyOptional ? Segment.FixedUpdate : Segment.Update);

        _isResetting = true;
    }

    private IEnumerator<float> C_RotateCoroutine()
    {
        yield return Timing.WaitForOneFrame;
        while (true) {
            float deltaTime = Time.deltaTime;
            if (ignoreTimeScale)
                deltaTime = Time.unscaledDeltaTime;

            float speedModifier = GetAutoScaleMultiplier();
            
            switch (space)
            {
                case Space.World:
                    if (rigidbodyOptional)
                    {
                        rigidbodyOptional.MoveRotation(
                            Quaternion.Euler((speed * deltaTime * speedModifier) * axis) * rigidbodyOptional.rotation);
                    } else
                    {
                        transform.Rotate(axis, speed * deltaTime * speedModifier, Space.World);
                    }
                    break;
                case Space.Self:
                    if (rigidbodyOptional)
                    {
                        rigidbodyOptional.MoveRotation(
                            rigidbodyOptional.rotation * Quaternion.Euler(axis * (speed * deltaTime * speedModifier)));
                    }
                    else
                    {
                        transform.Rotate(axis, speed * deltaTime * speedModifier, Space.Self);
                    }
                    break;
            }
            yield return Timing.WaitForOneFrame;
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
