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
        
        public void SetLevelData(GameLevelDataSO data)
        {
            levelData = data;
            
            bool isAvailable = levelData.sceneDependencySettings != null;
            tvScreen.materials[1].SetTexture(BaseMap, levelData.levelThumbnail);
            tvScreen.materials[1].SetFloat(StaticIntensity, staticIntensityCurve.Evaluate(isAvailable ? 0f : 1f));
        }
    }
}
