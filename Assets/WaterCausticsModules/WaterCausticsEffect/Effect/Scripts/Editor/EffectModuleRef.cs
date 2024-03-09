// WaterCausticsModules
// Copyright (c) 2021 Masataka Hakozaki

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MH.WaterCausticsModules {
#if WCE_DEVELOPMENT
    [CreateAssetMenu]
#endif
    public class EffectModuleRef : ScriptableObject {
        // WaterCausticEffectフォルダ
        [SerializeField] private Object m_effectModule;
        internal Object effectModule => m_effectModule;

        // ShaderFunctionsフォルダ
        [SerializeField] private Object m_customFunc;
        internal Object customFunc => m_customFunc;

        // ForAmplifyShaderEditor.unitypackageファイル
        [SerializeField] private Object m_packageForASE;
        internal Object packageForASE => m_packageForASE;

        static internal bool findAsset<T> (out T asset, out string path) where T : Object {
            asset = null;
            path = null;
            var guids = AssetDatabase.FindAssets ($"t:{typeof (T).ToString()}", new [] { "Assets" });
            if (guids.Length == 0) return false;
            path = AssetDatabase.GUIDToAssetPath (guids [0]);
            asset = AssetDatabase.LoadAssetAtPath<T> (path);
            return asset != null;
        }
    }
}
#endif
