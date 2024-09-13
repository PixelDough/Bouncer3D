using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.SceneDependencies;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneDependencyManager
{
    public static string MainSettingsPath = "Assets/Tools/Scene Dependencies/Resources/";

    public static void EnsureGameManagement()
    {
        if (!SceneManager.GetSceneByName("GameManagement").IsValid())
            SceneManager.LoadScene("GameManagement", LoadSceneMode.Additive);
    }
    
    public static Scene[] GetAllLoadedScenes()
    {
        var currentLoadedScenes = new Scene[SceneManager.loadedSceneCount];
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            currentLoadedScenes[i] = SceneManager.GetSceneAt(i);
        }

        return currentLoadedScenes;
    }

    #if UNITY_EDITOR
    public static void EnsureGameManagementEditor()
    {
        if (!SceneManager.GetSceneByName("GameManagement").IsValid())
        {
            var gameManagementSceneAsset = AssetDatabase.LoadAssetAtPath<SceneDependencySettingsSO>(MainSettingsPath + "GameManagement.asset");
            EditorSceneManager.OpenScene(gameManagementSceneAsset.sceneProperties.scenePath, OpenSceneMode.Additive);
        }
    }
    public static void LoadSceneEditor(SceneDependencySettingsSO scene)
    {
        EnsureGameManagementEditor();

        List<Scene> scenesToUnload = new List<Scene>();
        List<string> alreadyLoadedScenes = new List<string>();
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            var loadedScene = SceneManager.GetSceneAt(i);
            bool isTargetScene = loadedScene.name == scene.sceneProperties.sceneName;
            bool loadedSceneIsDependency = scene.dependencies.Exists(properties => properties.sceneName == loadedScene.name);
            if (!loadedSceneIsDependency && !isTargetScene)
                scenesToUnload.Add(loadedScene);
        
            alreadyLoadedScenes.Add(loadedScene.name);
        }
        
        // Get the target scene, open it, and set it as the active scene
        string targetScenePath = AssetDatabase.GetAssetPath(scene.sceneProperties.sceneAsset);
        Scene targetSceneLoaded = EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Additive);
        SceneManager.SetActiveScene(targetSceneLoaded);
        
        // Loop through all the dependencies of the scene, and load them if they are not in the alreadyLoadedScenes
        foreach (var dependency in scene.dependencies)
        {
            if (alreadyLoadedScenes.Contains(dependency.sceneName)) continue;
            string scenePath = AssetDatabase.GetAssetPath(dependency.sceneAsset);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }

        // Unload the non-needed scenes
        foreach (var sceneToUnload in scenesToUnload)
        {
            if (sceneToUnload.name == "GameManagement") continue;
            EditorSceneManager.CloseScene(sceneToUnload, true);
        }
    }
    #endif
    
    public static async Awaitable LoadScene(SceneDependencySettingsSO scene)
    {
        Debug.Log($"Loading Scene {scene.sceneProperties.sceneName}...");
        // Time.timeScale = 0;
        
        await LoadDependencies(scene);

        bool isSceneAlreadyLoaded = SceneManager.GetSceneByName(scene.sceneProperties.sceneName).IsValid();
        if (!isSceneAlreadyLoaded)
        {
            // Queue loading the target scene
            var asyncLoad = SceneManager.LoadSceneAsync(scene.sceneProperties.sceneName, LoadSceneMode.Additive);
            await asyncLoad;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.sceneProperties.sceneName));
        }

        await UnloadNonNeededScenes(scene);
        
        // Time.timeScale = 1;
    }

    public static async Awaitable LoadDependencies(SceneDependencySettingsSO scene)
    {
        EnsureGameManagement();
        
        Debug.Log("Loading dependencies...");
        var dependencyNames = scene.dependencies.Select((asset => asset.sceneName)).ToArray();
        var currentLoadedScenes = GetAllLoadedScenes();
        var currentLoadedSceneNames = currentLoadedScenes.Select((loadedScene => loadedScene.name)).ToArray();
        
        // Queue loading the dependency scenes that are not already loaded
        var dependencyLoadOperations = new List<AsyncOperation>();
        foreach (string dependencyName in dependencyNames)
        {
            if (currentLoadedSceneNames.Contains(dependencyName)) continue;
            var dependencyLoadAsync = SceneManager.LoadSceneAsync(dependencyName, LoadSceneMode.Additive);
            if (dependencyLoadAsync != null)
            {
                dependencyLoadAsync.allowSceneActivation = false;
                dependencyLoadOperations.Add(dependencyLoadAsync);
            }
        }

        // Wait for the dependencies to finish loading
        foreach (AsyncOperation dependencyLoadOperation in dependencyLoadOperations)
        {
            while (true)
            {
                if (dependencyLoadOperation.progress >= 0.9f)
                    break;
            }
        }
        
        // Tell dependencies to activate
        foreach (AsyncOperation dependencyLoadOperation in dependencyLoadOperations)
        {
            dependencyLoadOperation.allowSceneActivation = true;
            Debug.Log($"Dependency {dependencyLoadOperation} loaded!");
        }
        
        Debug.Log("All dependencies loaded!");
    }

    public static async Awaitable UnloadNonNeededScenes(SceneDependencySettingsSO scene)
    {
        // Unload the unneeded scenes
        var currentLoadedScenes = GetAllLoadedScenes();
        var dependencyNames = scene.dependencies.Select((asset => asset.sceneName)).ToArray();
        foreach (Scene currentLoadedScene in currentLoadedScenes)
        {
            if (currentLoadedScene.name == "GameManagement") continue;
            if (!dependencyNames.Contains(currentLoadedScene.name) && currentLoadedScene.name != scene.sceneProperties.sceneName)
            {
                var unloadOperation = SceneManager.UnloadSceneAsync(currentLoadedScene);
                await unloadOperation;
            }
        }
    }
}
