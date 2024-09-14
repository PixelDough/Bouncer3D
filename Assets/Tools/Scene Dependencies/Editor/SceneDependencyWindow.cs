using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Tools.SceneDependencies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneDependencyWindow : OdinMenuEditorWindow
{
    [MenuItem ("Tools/Scene Dependency Manager")]
    public static void ShowWindow() {
        GetWindow<SceneDependencyWindow>();
    }

    private CreateNewSceneDependency _createNewSceneDependency;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (_createNewSceneDependency != null)
            DestroyImmediate(_createNewSceneDependency.SceneDependencySettingsSo);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        
        _createNewSceneDependency = new CreateNewSceneDependency();
        tree.Add("Create New", _createNewSceneDependency);
        tree.AddAllAssetsAtPath("", SceneDependencyManager.MainSettingsPath, typeof(SceneDependencySettingsSO), true,
            true);
        
        return tree;
    }

    protected override void OnBeginDrawEditors()
    {
        if (MenuTree is null) return;
        if (MenuTree.Selection is null) return;
        
        OdinMenuTreeSelection selection = MenuTree.Selection;

        if (MenuTree.Selection.SelectedValue is not CreateNewSceneDependency)
        {
            // Get current settings item selected in menu
            var selected = (SceneDependencySettingsSO)selection.SelectedValue;
            
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                // Load Button
                if (SirenixEditorGUI.ToolbarButton(SdfIconType.CollectionPlayFill))
                {
                    // Loop through the scenes already loaded in Unity.
                    // Unload the scene if it is not a dependency, and is not the target scene
                    // Add it's name to the alreadyLoadedScenes List
                    SceneDependencyManager.LoadSceneEditor(selected);
                }

                if (SirenixEditorGUI.ToolbarButton("Add to Build Settings"))
                {
                    var scenesCurrentlyInBuildSettings = EditorBuildSettings.scenes.ToList();

                    // Add target path to build settings if it is not present
                    string targetPath = AssetDatabase.GetAssetPath(selected.sceneProperties.sceneAsset);
                    if (scenesCurrentlyInBuildSettings.Find((buildScene => buildScene.path == targetPath)) == null) 
                        scenesCurrentlyInBuildSettings.Add(new EditorBuildSettingsScene(targetPath, true));
                    
                    // Add dependencies to build settings if they are not present
                    foreach (var dependencyScene in selected.dependencies)
                    {
                        string dependencyPath = AssetDatabase.GetAssetPath(dependencyScene.sceneAsset);
                        if (scenesCurrentlyInBuildSettings.Find((buildScene => buildScene.path == dependencyPath)) !=
                            null) continue;
                        scenesCurrentlyInBuildSettings.Add(new EditorBuildSettingsScene(dependencyPath, true));
                    }
                    
                    // Update build settings
                    EditorBuildSettings.scenes = scenesCurrentlyInBuildSettings.ToArray();
                }

                if (SirenixEditorGUI.ToolbarButton("Refresh"))
                {
                    selected.sceneProperties.RefreshSceneProperties();
                }
                
                GUILayout.FlexibleSpace();

                // Delete Button
                if (SirenixEditorGUI.ToolbarButton(SdfIconType.TrashFill))
                {
                    SceneDependencySettingsSO asset = selection.SelectedValue as SceneDependencySettingsSO;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }

    public class CreateNewSceneDependency
    {
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        [HideLabel]
        public SceneDependencySettingsSO SceneDependencySettingsSo = ScriptableObject.CreateInstance<SceneDependencySettingsSO>();

        [Button("Save")]
        private void Save()
        {
            AssetDatabase.CreateAsset(SceneDependencySettingsSo,
                $"{SceneDependencyManager.MainSettingsPath}/{SceneDependencySettingsSo.sceneProperties.sceneName}.asset");
        }
    }
}
