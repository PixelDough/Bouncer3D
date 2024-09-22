using Tools.SceneDependencies;
using UnityEngine;

namespace PixelDough.Bouncer
{
    [CreateAssetMenu(fileName = "New Game Level Data", menuName = "Bouncer3D/Game Level Data")]
    public class GameLevelDataSO : ScriptableObject
    {
        [SerializeField] public SceneDependencySettingsSO sceneDependencySettings;
        [SerializeField] public string levelName;
        [SerializeField] public Texture2D levelThumbnail;
        [SerializeField] public string levelID;
    }
}
