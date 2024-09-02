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

        [SerializeField] private Transform uiItemIconTransform;
        private float uiItemIconSpeed = 1f;

        private bool _isVisible = true;
        
        private void Start()
        {
            UpdateShellCountText();
            StartCoroutine(ShellAddCoroutine());
        }

        private void Update()
        {
            shellAnimator.speed = Mathf.Lerp(shellAnimator.speed, 1f, Time.deltaTime);
            uiItemIconSpeed = Mathf.Lerp(uiItemIconSpeed, 1f, Time.deltaTime);

            if (LevelManager.LevelTime.Hours > 0)
                timerText.text = LevelManager.LevelTime.ToString(@"hh\:mm\:ss\.fff");
            else
                timerText.text = LevelManager.LevelTime.ToString(@"mm\:ss\.fff");
            
            uiItemIconTransform.Rotate(Vector3.up * (360 * uiItemIconSpeed * Time.deltaTime));
        }

        private void UpdateShellCountText()
        {
            bool hasAllShells = _localShellCount == LevelManager.Instance.LevelProgress.TotalCollectables;
            
            string str = "{size}";
            if (hasAllShells)
            {
                str += "<pastel>";
                str += "<wave>";
            }
            str += _localShellCount;
            if (!hasAllShells)
                str += "</>{/}";
            str += "<size=18>/" + LevelManager.Instance.LevelProgress.TotalCollectables;
            shellCountText.text = str;
        }

        private IEnumerator ShellAddCoroutine()
        {
            while (true)
            {
                if (_localShellCount > LevelManager.Instance.LevelProgress.CurrentCollectables)
                {
                    _localShellCount = LevelManager.Instance.LevelProgress.CurrentCollectables;
                    UpdateShellCountText();
                }
                while (_localShellCount < LevelManager.Instance.LevelProgress.CurrentCollectables)
                {
                    _localShellCount++;
                    UpdateShellCountText();

                    // TODO: Pool collectables flying towards icon in UI
                    RectTransform shell = Instantiate(shellFlying, shellFlying.parent).transform as RectTransform;
                    shell.gameObject.SetActive(true);
                    shell.LeanMoveLocal(shellImage.transform.localPosition, 0.2f).setEaseOutSine()
                        .setOnComplete(() =>
                        {
                            shellAnimator.speed += 1f;
                            uiItemIconSpeed += 1f;
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
