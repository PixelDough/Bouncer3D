using UnityEngine;

namespace PixelDough.Bouncer
{
    public class LevelSelectButton : MonoBehaviour
    {
        [SerializeField] private GameLevelDataSO levelData;
        [SerializeField] private MeshRenderer tvScreen;
        [SerializeField] private AnimationCurve staticIntensityCurve;
        
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int StaticIntensity = Shader.PropertyToID("_StaticIntensity");
        private static readonly int Brightness = Shader.PropertyToID("_Brightness");
        
        public void SetLevelData(GameLevelDataSO data)
        {
            levelData = data;
            
            bool isAvailable = levelData.sceneDependencySettings != null;
            tvScreen.materials[1].SetTexture(BaseMap, levelData.levelThumbnail);
            tvScreen.materials[1].SetFloat(StaticIntensity, staticIntensityCurve.Evaluate(isAvailable ? 0f : 1f));
            tvScreen.materials[1].SetFloat(Brightness, 0.5f);
        }

        public void UpdateButton(bool isSelected)
        {
            Vector3 localPos = transform.localPosition;
            localPos.y = Mathf.Lerp(0,
                0.15f, Mathf.InverseLerp(-1, 1, Mathf.Sin((Time.time + transform.GetSiblingIndex()) * 2.2f)));
            transform.localPosition = localPos;
            
            transform.localScale = MathHelpers.ExpDecay(transform.localScale, Vector3.one * (isSelected ? 1f : 0.9f),
                10f, Time.deltaTime);
            tvScreen.materials[1].SetFloat(Brightness, isSelected ? 1f : 0.5f);
            
            if (levelData.sceneDependencySettings is null) return;
            tvScreen.materials[1].SetFloat(StaticIntensity, staticIntensityCurve.Evaluate(isSelected ? 0f : 0.25f));
        }
    }
}
