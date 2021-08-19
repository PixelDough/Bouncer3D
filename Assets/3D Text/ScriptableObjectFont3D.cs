using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

[CreateAssetMenu(menuName = "PixelDough/Font 3D")]
public class ScriptableObjectFont3D : ScriptableObject
{
    public GameObject fontModels;
    public string fontString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public bool allCaps = true;
    
    public Dictionary<char, MeshFilter> meshFilters = new Dictionary<char, MeshFilter>();


    public void UpdateFont()
    {
        meshFilters.Clear();
        meshFilters.Add(' ', null);
        
        int i = 0;
        foreach (Transform t in fontModels.transform)
        {
            if (i >= fontString.Length) continue;
            meshFilters.Add(fontString[i], t.GetComponent<MeshFilter>());
            i++;
        }
    }
    
    
}
