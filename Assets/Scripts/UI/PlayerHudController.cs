using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelDough.Bouncer.UI
{
    public class PlayerHudController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Shell Counter")]
        [SerializeField] private Image shellImage;
        [SerializeField] private Animator shellAnimator;
        [SerializeField] private TextMeshProUGUI shellCountText;
        
        private int _localShellCount = 0;
        
        [SerializeField] private RectTransform shellFlying;

        [Header("Timer")] 
        [SerializeField] private TextMeshProUGUI timerText;

        private bool _isVisible = true;
        
        private void Start()
        {
            UpdateShellCountText();
            StartCoroutine(ShellAddCoroutine());
        }

        private void Update()
        {
            shellAnimator.speed = Mathf.Lerp(shellAnimator.speed, 1f, Time.deltaTime);

            if (LevelManager.LevelTime.Hours > 0)
                timerText.text = LevelManager.LevelTime.ToString(@"hh\:mm\:ss\.fff");
            else
                timerText.text = LevelManager.LevelTime.ToString(@"mm\:ss\.fff");
        }

        private void UpdateShellCountText()
        {
            bool hasAllShells = _localShellCount == LevelManager.Instance.TotalShells;
            
            string str = "{size}";
            if (hasAllShells)
                str += "<pastel>";
            str += "<wave>" + _localShellCount;
            if (!hasAllShells)
                str += "</>{/}";
            str += "<size=18>/" + LevelManager.Instance.TotalShells;
            shellCountText.text = str;
        }

        private IEnumerator ShellAddCoroutine()
        {
            while (true)
            {
                while (_localShellCount < LevelManager.Instance.shellsCollected)
                {
                    _localShellCount++;
                    UpdateShellCountText();

                    RectTransform shell = Instantiate(shellFlying, shellFlying.parent).transform as RectTransform;
                    shell.gameObject.SetActive(true);
                    shell.LeanMoveLocal(shellImage.transform.localPosition, 0.2f).setEaseOutSine()
                        .setOnComplete(() =>
                        {
                            shellAnimator.speed += 1f;
                            Destroy(shell.gameObject);
                        });
                    
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SHELLS/Shell Get");
                    
                    yield return new WaitForSeconds(0.1f);
                }
                yield return null;
            }
            yield return null;
        }

        public void SetVisibility(bool state, bool doAnimation = true)
        {
            _isVisible = state;

            if (state)
            {
                canvasGroup.alpha = 1;
            }
            else
            {
                canvasGroup.alpha = 0;
            }
        }
        
    }
}
