using System;
using UnityEngine;

[ExecuteInEditMode]
public class Font3DString : MonoBehaviour
{
    [SerializeField] private ScriptableObjectFont3D font3D;
    [SerializeField] private Material material;
    [SerializeField, TextArea] private string text;
    [SerializeField] private float fontSizeInUnits = 1;
    [SerializeField] private float letterSpacingPercent = 1;
    [SerializeField] private float lineSpacingPercent = 1.2f;
    [SerializeField] private Color textColor = Color.white;

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
    
    private void Update()
    {
        if (font3D is null) return;
        
        string[] lines = text.Split(new [] { "\\n", Environment.NewLine }, StringSplitOptions.None);
        float startY = 0f;
        float allLinesHeight = (lines.Length) * fontSizeInUnits * lineSpacingPercent;
        switch (verticalAlignments)
        {
            case VerticalAlignments.Bottom:
                startY = -fontSizeInUnits / 2f + allLinesHeight; // assume the mesh is centered already
                break;
            case VerticalAlignments.Center:
                startY = -fontSizeInUnits / 2f + allLinesHeight / 2f;
                break;
            case VerticalAlignments.Top:
                startY = -fontSizeInUnits / 2f;
                break;
        }
        
        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            // Calculate vertical alignment offset
            float drawY = startY - lineIndex * fontSizeInUnits * lineSpacingPercent;
            DrawString(line.Trim(), drawY);
        }
    }

    private void DrawString(string textToDraw, float y)
    {
        if (string.IsNullOrEmpty(textToDraw)) return;

        if (font3D.allCaps) textToDraw = textToDraw.ToUpper();
        float x = 0f;

        // Calculate initial x position based on horizontal alignment
        switch (horizontalAlignments)
        {
            case HorizontalAlignments.Left:
                x = 0f;
                break;
            case HorizontalAlignments.Center:
                x = ((textToDraw.Length - 1) / 2f) * fontSizeInUnits * letterSpacingPercent;
                break;
            case HorizontalAlignments.Right:
                x = (textToDraw.Length - 1) * fontSizeInUnits * letterSpacingPercent;
                break;
        }

        // Loop over each character in the text
        for (var i = 0; i < textToDraw.Length; i++)
        {
            var c = textToDraw[i];
            
            // Skip drawing for whitespace but adjust x position
            if (char.IsWhiteSpace(c))
            {
                x -= fontSizeInUnits * letterSpacingPercent;
                continue;
            }

            // Check if the font contains the character
            if (font3D.meshes.TryGetValue(c, out var mesh))
            {
                Vector3 position = new Vector3(x, y, 0f);
                Quaternion rotation = Quaternion.identity;
                Vector3 scale = Vector3.one * fontSizeInUnits;

                // Calculate the transformation matrix
                Matrix4x4 matrix = transform.localToWorldMatrix * Matrix4x4.TRS(position, rotation, scale);

                // Draw the mesh directly to the world
                Graphics.DrawMesh(mesh, matrix, material, gameObject.layer);
            }

            // Move x position for the next character
            x -= fontSizeInUnits * letterSpacingPercent;
        }
    }

    public void SetText(String text)
    {
        this.text = text;
    }
}
