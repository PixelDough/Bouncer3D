using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildAnimator : MonoBehaviour
{
    [SerializeField] private float strength = 1f;
    [SerializeField] private float timeMultiplier = 1f;
    [SerializeField] private float perLetterDifference = 1f;
    [SerializeField] private bool doLockedFramerate = false;
    [SerializeField] private float lockedFramerate = 12;

    [SerializeField] private AnimationTypes animationType = AnimationTypes.Wave;
    public enum AnimationTypes
    {
        Wave,
        Swing
    }
    
    private void Update()
    {
        int i = 0;
        foreach (Transform t in transform)
        {
            float time = Time.time;
            if (doLockedFramerate)
                time = (Mathf.Floor(Time.time * Application.targetFrameRate * lockedFramerate) / lockedFramerate) / Application.targetFrameRate;
            
            switch(animationType)
            {
                case AnimationTypes.Wave:
                    Vector3 pos = t.localPosition;
                    pos.y = Mathf.Sin((time * timeMultiplier) + (i * perLetterDifference)) * strength;
                    t.localPosition = pos;
                    break;
                case AnimationTypes.Swing:
                    Vector3 localRot = t.localRotation.eulerAngles;
                    localRot.y = Mathf.Sin((time * timeMultiplier) + (i * perLetterDifference)) * strength;
                    t.localRotation = Quaternion.Euler(localRot);
                    break;
            }
            i++;
        }
    }
}
