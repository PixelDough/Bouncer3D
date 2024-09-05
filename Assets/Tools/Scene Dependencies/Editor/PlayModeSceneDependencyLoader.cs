using System.Collections;
using System.Collections.Generic;
using Tools.SceneDependencies;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayModeSceneDependencyLoader
{

    static PlayModeSceneDependencyLoader()
    {
        EditorApplication.playModeStateChanged += LoadSceneDependenciesEditor;
    }

    private static void LoadSceneDependenciesEditor(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredPlayMode) return;
        
        SceneDependencyManager.EnsureGameManagement();
        Debug.Log("Entered Play Mode! Loading dependencies for scenes...");
        
        List<Scene> loadedScenes = new List<Scene>();
        List<string> loadedSceneNames = new List<string>();
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            loadedScenes.Add(scene);
            loadedSceneNames.Add(scene.name);
        }
        
        var assetsAtPath = Resources.LoadAll<SceneDependencySettingsSO>("");
        foreach (var dependencySettings in assetsAtPath)
        {
            Debug.Log(dependencySettings.name);
            if (loadedSceneNames.Contains(dependencySettings.sceneProperties.sceneName))
            {
                SceneDependencyManager.LoadScene(dependencySettings);
            }
        }
    }
}
