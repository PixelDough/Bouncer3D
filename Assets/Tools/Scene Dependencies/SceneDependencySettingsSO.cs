using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Tools.SceneDependencies
{
    [CreateAssetMenu(menuName = "Scene Tools/Scene Dependency Settings", fileName = "New Scene Dependency Settings")]
    public class SceneDependencySettingsSO : ScriptableObject
    {
        [HideLabel]
        public SceneProperties sceneProperties;
            
        [ListDrawerSettings(ShowFoldout = false, DraggableItems = false, ShowItemCount = false)]
        public List<SceneProperties> dependencies;

        [System.Serializable]
        public struct SceneProperties
        {
#if UNITY_EDITOR 
            [HideLabel]
            [OnValueChanged("RefreshSceneProperties", false)]
            public SceneAsset sceneAsset;
            public void RefreshSceneProperties()
            {
                sceneName = sceneAsset.name;
                scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            }
#endif
            
            [ReadOnly] public string sceneName;
            [ReadOnly] public string scenePath;
        }
    }
}