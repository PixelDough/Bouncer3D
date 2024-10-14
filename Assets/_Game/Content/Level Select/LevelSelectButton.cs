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

        private void Start()
        {
            levelTitle.SetText("");
        }

        public void SetLevelData(GameLevelDataSO data)
        {
            levelData = data;
            
            bool isAvailable = levelData.sceneDependencySettings != null;
            tvScreen.materials[1].SetTexture(BaseMap, levelData.levelThumbnail);
            tvScreen.materials[1].SetFloat(StaticIntensity, staticIntensityCurve.Evaluate(isAvailable ? 0f : 1f));
            tvScreen.materials[1].SetFloat(Brightness, 0.5f);
            
            int levelRecordMs = GameManager.Instance.LoadLevelRecord(data.levelID);
            var medalState = GetMedalStateForRecord(levelData, levelRecordMs);
            medalRace.SetMedalState(medalState);
        }

        public void Highlight()
        {
            _highlightRoutineHandle = Timing.RunCoroutine(HighlightRoutine());
        }
        
        private IEnumerator<float> HighlightRoutine()
        {
            bool isAvailable = levelData.sceneDependencySettings != null;
            
            tvScreen.materials[1].SetFloat(Brightness, 0.2f);
            
            levelTitle.SetText(isAvailable ? levelData.levelName : "???");
            levelTitle.AnimPulse();
            Vector3 titleScale = levelTitle.transform.localScale;
            titleScale.y = 0;
            levelTitle.transform.localScale = titleScale;
            levelTitle.transform.DOScaleY(1f, 0.25f);
            
            yield return Timing.WaitForSeconds(1.0f);
            levelTitle.transform.DOScaleY(0f, 0.25f);
            tvScreen.materials[1].DOFloat(1.0f, Brightness, 0.5f);
            
            yield return Timing.WaitForSeconds(0.25f);
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
            
            if (levelData.sceneDependencySettings is null) return;
            tvScreen.materials[1].SetFloat(StaticIntensity, staticIntensityCurve.Evaluate(isSelected ? 0f : 0.25f));
        }
        
        private Medal.MedalState GetMedalStateForRecord(GameLevelDataSO levelData, int recordMs)
        {
            return recordMs == 0 ? Medal.MedalState.None :
                recordMs <= levelData.platinumTime ? Medal.MedalState.Platinum :
                recordMs <= levelData.goldTime ? Medal.MedalState.Gold :
                recordMs <= levelData.silverTime ? Medal.MedalState.Silver :
                recordMs <= levelData.bronzeTime ? Medal.MedalState.Bronze : 
                Medal.MedalState.None;
        }
    }
}
