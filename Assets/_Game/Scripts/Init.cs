using Tools.SceneDependencies;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class Init : MonoBehaviour
    {
        [SerializeField] private SceneDependencySettingsSO sceneDependencySettingsSo;
        
        private void Awake()
        {
            var _ = SceneDependencyManager.LoadScene(sceneDependencySettingsSo);
        }
    }
}
