using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "PixelDough/Font 3D")]
public class ScriptableObjectFont3D : SerializedScriptableObject
{
    public GameObject fontModels;
    public string fontString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public bool allCaps = true;
    
    [DictionaryDrawerSettings(KeyLabel = "Character", DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<char, Mesh> meshes = new Dictionary<char, Mesh>();
    
    [Button]
    public void UpdateFont()
    {
        meshes.Clear();
        meshes.Add(' ', null);
        
        int i = 0;
        foreach (Transform t in fontModels.transform)
        {
            if (i >= fontString.Length) continue;
            meshes.Add(fontString[i], t.GetComponent<MeshFilter>().sharedMesh);
            i++;
        }
    }
    
    
}
