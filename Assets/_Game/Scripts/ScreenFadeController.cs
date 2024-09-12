using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeController : MonoBehaviour
{
    private static readonly int SubtractiveFadeAmount = Shader.PropertyToID("_SubtractiveFadeAmount");

    public LTDescr FadeToBlack(float time = 0.25f)
    {
        float currentFadeAmount = Shader.GetGlobalFloat(SubtractiveFadeAmount);
        return LeanTween.value(gameObject, (f =>
        {
            Shader.SetGlobalFloat(SubtractiveFadeAmount, f);
        }), currentFadeAmount, 1f, time).setIgnoreTimeScale(true);
    }

    public LTDescr FadeFromBlack(float time = 0.25f)
    {
        float currentFadeAmount = Shader.GetGlobalFloat(SubtractiveFadeAmount);
        return LeanTween.value(gameObject, (f =>
        {
            Shader.SetGlobalFloat(SubtractiveFadeAmount, f);
        }), currentFadeAmount, 0f, time).setIgnoreTimeScale(true);
    }
}