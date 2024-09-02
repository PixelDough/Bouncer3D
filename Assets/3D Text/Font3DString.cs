using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Font3DString : MonoBehaviour
{
    
    [SerializeField] private ScriptableObjectFont3D font3D;
    [SerializeField] private string text;
    [SerializeField] private float fontSizeInUnits = 1;
    [SerializeField] private float letterSpacingPercent = 1;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private bool updateOnValidate = false;

    public string Text
    {
        get => text;
        set
        {
            text = value;
            UpdateText();
        }
    }
    
    private enum HorizontalAlignments
    {
        Left,
        Center,
        Right
    }
    private enum VerticalAlignments
    {
        Bottom,
        Center,
        Top
    }
    [SerializeField] private HorizontalAlignments horizontalAlignments;
    [SerializeField] private VerticalAlignments verticalAlignments;

    public List<MeshFilter> _meshFilters = new List<MeshFilter>();
    
    private void Start()
    {
        UpdateText(true);
    }
    
#if UNITY_EDITOR
    [ContextMenu("Update Text")]
    private void UpdateTextInEditMode()
    {
        UpdateText(true, false);
    }
#endif

    private void UpdateText(bool updateFont = false, bool isOnValidate = false)
    {
        if (updateFont)
        {
            font3D.UpdateFont();
        }

        _meshFilters = _meshFilters.Where(item => item != null).ToList();
        
        float x = 0;

        switch (horizontalAlignments)
        {
            case HorizontalAlignments.Left:
                x = 0;
                break;
            case HorizontalAlignments.Center:
                x = ((text.Length - 1) / 2f) * fontSizeInUnits * letterSpacingPercent * -1;
                break;
            case HorizontalAlignments.Right:
                x = (text.Length - 1) * fontSizeInUnits * letterSpacingPercent * -1;
                break;
        }
        
        if (font3D.allCaps) text = text.ToUpper();

        foreach (var mf in _meshFilters)
        {
            mf.mesh = null;
        }
        
        for (int i = 0; i < Mathf.Max(text.Length, _meshFilters.Count); i++)
        {
            // If the text is longer, set all the mesh filters, and add new mesh filters if needed.
            // If the mesh filter list is longer than the text, set all the mesh filters for those letters and remove the remaining mesh filters.
            
            if (i >= text.Length)
            {
                while (i < _meshFilters.Count)
                {
                    if (Application.isPlaying)
                        Destroy(_meshFilters[i].gameObject);
                    else
                        DestroyImmediate(_meshFilters[i].gameObject);
                    _meshFilters.RemoveAt(i);
                }

                break;
            }
            char c = text[i];

            // If the font doesn't have the character, skip it.
            if (font3D.meshFilters.ContainsKey(c) || char.IsWhiteSpace(c))
            {
                // If there is already a mesh filter at that letter's index, change it's mesh.
                if (_meshFilters.Count > i)
                {
                    _meshFilters[i].mesh = char.IsWhiteSpace(c) ? null : font3D.meshFilters[c].sharedMesh;
                    
                    _meshFilters[i].gameObject.name = char.IsWhiteSpace(c) ? "Space" : c.ToString();

                    _meshFilters[i].transform.localPosition = new Vector3(x, 0);
                    _meshFilters[i].transform.localRotation = Quaternion.identity;
                    _meshFilters[i].transform.localScale = Vector3.one * fontSizeInUnits;
                }
                else // Otherwise, add a new letter.
                {
                    Transform letter;
                    if (char.IsWhiteSpace(c))
                    {
                        letter = Instantiate(font3D.meshFilters['A'].transform, transform);
                        letter.GetComponent<MeshFilter>().mesh = null;
                    }
                    else
                    {
                        letter = Instantiate(font3D.meshFilters[c].transform, transform);
                    }

                    letter.localPosition = new Vector3(x, 0);
                    letter.localRotation = Quaternion.identity;
                    letter.localScale = Vector3.one * fontSizeInUnits;

                    _meshFilters.Add(letter.GetComponent<MeshFilter>());

                    letter.gameObject.name = char.IsWhiteSpace(c) ? "Space" : c.ToString();
                    letter.gameObject.layer = gameObject.layer;
                    
                    if (Application.isPlaying)
                        letter.GetComponent<MeshRenderer>().material.SetColor("_MainColor", textColor);
                }
            }

            x += fontSizeInUnits * letterSpacingPercent;
        }
    }
    
}
