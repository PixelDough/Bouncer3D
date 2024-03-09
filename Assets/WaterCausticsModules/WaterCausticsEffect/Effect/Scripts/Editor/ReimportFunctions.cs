// WaterCausticsModules
// Copyright (c) 2021 Masataka Hakozaki

#if UNITY_EDITOR && WCE_URP
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MH.WaterCausticsModules {
    /*------------------------------------------------------------------------ 
    「missing SubGraph references」エラーを防ぐため、このスクリプトが移動した
    ことを検知した際にShaderFunctionsフォルダを再インポート。
    -------------------------------------------------------------------------*/
    public class ReimportFunctions : AssetPostprocessor {
        static private readonly string classFileName = $"{typeof (ReimportFunctions).Name}.cs";
        static void OnPostprocessAllAssets (string [] imported, string [] deleted, string [] moved, string [] movedFrom) {
            // ※移動時と名称変更時、フォルダは importedAssetsとmovedAssetsに入るので注意、アセットはmovedAssetsのみ
            if (moved.Any (a => a.EndsWith (classFileName)) && !imported.Any (a => a.EndsWith (classFileName))) {
                if (EffectModuleRef.findAsset<EffectModuleRef> (out var asset, out var path) && asset.customFunc) {
                    var folderPath = AssetDatabase.GetAssetPath (asset.customFunc);
                    if (!string.IsNullOrEmpty (folderPath) && moved.Any (a => a == folderPath)) {
                        AssetDatabase.ImportAsset (folderPath, ImportAssetOptions.ImportRecursive);
                    }
                }
            }
        }

    }
}

#endif
