using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    public class LevelSelectButton : MonoBehaviour
    {
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int StaticIntensity = Shader.PropertyToID("_StaticIntensity");
        private static readonly int Brightness = Shader.PropertyToID("_Brightness");
        
        [SerializeField] private GameLevelDataSO levelData;
        [SerializeField] private MeshRenderer tvScreen;
        [SerializeField] private AnimationCurve staticIntensityCurve;
        [SerializeField] private Font3DString levelTitle;

        [Header("Medals")] 
        [SerializeField] private Transform medalsRoot;
        [SerializeField] private Medal medalRace;
        [SerializeField] private Medal medalSecret;
        [SerializeField] private Medal medalTimeAttack;
        
        private CoroutineHandle _highlightRoutineHandle;
        private float _levelTitleLocalY = 0f;
        private Color _levelTitleTint = Color.white;

        private void Start()
        {
            levelTitle.SetText("");
            _levelTitleLocalY = levelTitle.transform.localPosition.y;
            _levelTitleTint = levelTitle.textColor;
        }

        private void Update()
        {
            Vector3 titleLocalPos = levelTitle.transform.localPosition;
            float shakeRange = 0.02f;
            titleLocalPos.y = _levelTitleLocalY + Random.Range(-shakeRange, shakeRange);
            levelTitle.transform.localPosition = titleLocalPos;
            levelTitle.textColor = Color.Lerp(_levelTitleTint, Color.black, Random.Range(0f, 0.1f));
        }

        public void SetLevelData(GameLevelDataSO data)
        {
            levelData = data;
            
            bool isLevelUnlocked = GameManager.Instance.IsLevelUnlocked(levelData.levelID);
            
            tvScreen.materials[1].SetTexture(BaseMap, levelData.levelThumbnail);
            tvScreen.materials[1].SetFloat(StaticIntensity, staticIntensityCurve.Evaluate(isLevelUnlocked ? 0f : 1f));
            tvScreen.materials[1].SetFloat(Brightness, 0.5f);
            
            int levelRecordMs = GameManager.Instance.LoadLevelRecord(data.levelID);
            var medalState = GameManager.Instance.GetMedalStateForRecord(levelData, levelRecordMs);
            medalRace.SetMedalState(medalState);
        }

        public void Highlight()
        {
            _highlightRoutineHandle = Timing.RunCoroutine(HighlightRoutine().CancelWith(gameObject));
        }
        
        private IEnumerator<float> HighlightRoutine()
        {
            yield return Timing.WaitForOneFrame;
            bool isLevelUnlocked = GameManager.Instance.IsLevelUnlocked(levelData.levelID);
            
            tvScreen.materials[1].SetFloat(Brightness, 0.2f);
            
            levelTitle.SetText(isLevelUnlocked ? levelData.levelName : "???");
            levelTitle.AnimPulse();
            Vector3 titleScale = levelTitle.transform.localScale;
            titleScale.y = 0;
            levelTitle.transform.localScale = titleScale;
            levelTitle.transform.DOScaleY(1f, 0.25f);
            
            yield return Timing.WaitForSeconds(1.0f);
            levelTitle.transform.DOScaleY(0f, 0.1f);
            tvScreen.materials[1].DOFloat(1.0f, Brightness, 0.1f);
            
            yield return Timing.WaitForSeconds(0.1f);
            Unhighlight();
        }

        public void Unhighlight()
        {
            Timing.KillCoroutines(_highlightRoutineHandle);
            tvScreen.materials[1].SetFloat(Brightness, 1.0f);
            levelTitle.SetText("");
        }

        public void UpdateButton(bool isSelected)
        {
            Vector3 localPos = transform.localPosition;
            localPos.y = Mathf.Lerp(0,
                0.15f, Mathf.InverseLerp(-1, 1, Mathf.Sin((Time.time + transform.GetSiblingIndex()) * 2.2f)));
            transform.localPosition = localPos;
            
            transform.localScale = MathHelpers.ExpDecay(transform.localScale, Vector3.one * (isSelected ? 1f : 0.9f),
                10f, Time.deltaTime);

            bool isLevelUnlocked = GameManager.Instance.IsLevelUnlocked(levelData.levelID);
            tvScreen.materials[1].SetFloat(StaticIntensity,
                isLevelUnlocked
                    ? staticIntensityCurve.Evaluate(isSelected ? 0f : 0.25f)
                    : staticIntensityCurve.Evaluate(1f));
        }
    }
}
