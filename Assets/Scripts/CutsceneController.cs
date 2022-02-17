using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using PixelDough.Bouncer;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;

    private float _timelineDuration = 0f;
    private bool _isPlayingCutscene = false;
    
    private void Start()
    {
        
        PlayCutscene();
    }

    private void Update()
    {
        if (GameManager.Instance.Input.GetButtonDown(RewiredConsts.Action.Start) || 
            GameManager.Instance.Input.GetButtonDown(RewiredConsts.Action.Back))
        {
            FinishCutscene();
        }
    }

    public void PlayCutscene()
    {
        Timing.RunCoroutine(C_CutsceneCoroutine().CancelWith(gameObject));
    }

    public void FinishCutscene()
    {
        if (!_isPlayingCutscene) return;
        _isPlayingCutscene = false;
        
        GameManager.Instance.screenFadeController.FadeToBlack(0.5f).setOnComplete(() =>
        {
            playableDirector.Stop();
            GameManager.Instance.screenFadeController.FadeFromBlack(0.5f);
        });
    }

    private IEnumerator<float> C_CutsceneCoroutine()
    {
        _isPlayingCutscene = true;
        GameManager.Instance.screenFadeController.FadeFromBlack();
        
        playableDirector.Play();

        yield return Timing.WaitForSeconds((float) playableDirector.duration);
        
        /*
        while (_isPlayingCutscene)
            yield return Timing.WaitForOneFrame;*/
        
        FinishCutscene();
    }
    
}
