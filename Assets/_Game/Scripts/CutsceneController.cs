using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using PixelDough.Bouncer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private PlayableDirector playableDirector;

    private PlayerController _playerController;
    private float _timelineDuration = 0f;
    private bool _isPlayingCutscene = false;
    
    private InputActionReference _startAction;
    private InputActionReference _backAction;

    private void OnDestroy()
    {
        GameSceneManager.OnSceneLoaded -= PlayCutscene;
    }

    private void Update()
    {
        if (_startAction.action.WasPressedThisFrame() || _backAction.action.WasPressedThisFrame())
        {
            FinishCutscene();
        }
        {
            FinishCutscene();
        }
    }

    public void PlayCutscene()
    {
        GameManager.DoPlayerMovement = false;
        GameManager.DoPlayerPhysics = false;
        levelManager.CountingTime = false;
        levelManager.ResetTimer();
        levelManager.CutsceneBegin();
        
        Timing.RunCoroutine(C_CutsceneCoroutine().CancelWith(gameObject));
    }

    public void FinishCutscene()
    {
        if (!_isPlayingCutscene) return;
        _isPlayingCutscene = false;
        
        GameManager.Instance.screenFadeController.FadeToBlack(0.5f).setOnComplete(() =>
        {
            playableDirector.Stop();
            levelManager.CutsceneEnded();
            GameManager.Instance.screenFadeController.FadeFromBlack(0.5f).setOnComplete(() =>
            {
                GameManager.DoPlayerMovement = true;
                GameManager.DoPlayerPhysics = true;
                levelManager.CountingTime = true;
            });
        });
    }

    private IEnumerator<float> C_CutsceneCoroutine()
    {
        _isPlayingCutscene = true;
        //GameManager.Instance.screenFadeController.FadeFromBlack();
        
        playableDirector.Play();

        yield return Timing.WaitForSeconds((float) playableDirector.duration);
        
        /*
        while (_isPlayingCutscene)
            yield return Timing.WaitForOneFrame;*/
        
        FinishCutscene();
    }
    
}
