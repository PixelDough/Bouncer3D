using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Font3DLetter : MonoBehaviour
{

    [SerializeField] private ScriptableObjectFont3D font3D;
    [SerializeField] private char character;
    [SerializeField] private MeshFilter meshFilter;

    private void Start()
    {
        if (font3D.allCaps) character = Char.ToUpper(character);
        if (font3D.fontString.Contains(character.ToString()))
        {
            int id = font3D.fontString.IndexOf(character);
            if (id < font3D.fontModels.transform.childCount)
            {
                meshFilter.mesh = font3D.fontModels.transform.GetChild(id).GetComponent<MeshFilter>().sharedMesh;
            }
        }
    }
}
