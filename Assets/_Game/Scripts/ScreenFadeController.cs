using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeController : MonoBehaviour
{

    [SerializeField] private Image image;
    [SerializeField] private Gradient gradient;
    
    public LTDescr FadeToBlack(float time = 0.25f)
    {
        return LeanTween.value(image.gameObject, (f =>
        {
            image.color = gradient.Evaluate(f);
        }), 0f, 1f, time);
    }

    public LTDescr FadeFromBlack(float time = 0.25f)
    {
        return LeanTween.value(image.gameObject, (f =>
        {
            image.color = gradient.Evaluate(f);
        }), 1f, 0f, time);
    }

}
