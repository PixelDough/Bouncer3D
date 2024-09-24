using System;
using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class Font3DString : MonoBehaviour
{
    [SerializeField] private ScriptableObjectFont3D font3D;
    [SerializeField] private Material material;
    [SerializeField, TextArea] private string text = "";
    [SerializeField] private float fontSizeInUnits = 1;
    [SerializeField] private float letterSpacingPercent = 1;
    [SerializeField] private float lineSpacingPercent = 0.2f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Camera renderInCamera;

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

    private float _animScale = 1f;

#if UNITY_EDITOR
    public void OnEnable()
    {
        SceneView.duringSceneGui += UpdateEditorScene;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= UpdateEditorScene;
    }

    private void UpdateEditorScene(SceneView sceneView)
    {
        if (Application.isPlaying) return;
        Draw(SceneView.lastActiveSceneView.camera);
    }
#endif

    private void Update()
    {
        Draw(renderInCamera);
    }

    private void Draw(Camera drawCamera = null)
    {
        if (font3D is null) return;
        
        string[] lines = text.Split(new [] { "\\n", Environment.NewLine }, StringSplitOptions.None);
        float startY = 0f;
        float allLinesHeight = lines.Length * fontSizeInUnits;
        float allSpacesHeight = (lines.Length - 1) * (fontSizeInUnits * lineSpacingPercent);
        switch (verticalAlignments)
        {
            case VerticalAlignments.Bottom:
                startY = -fontSizeInUnits / 2f + allLinesHeight + allSpacesHeight; // assume the mesh is centered already
                break;
            case VerticalAlignments.Center:
                startY = -fontSizeInUnits * 0.5f + allLinesHeight * 0.5f + allSpacesHeight * 0.5f;
                break;
            case VerticalAlignments.Top:
                startY = -fontSizeInUnits / 2f;
                break;
        }
        
        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            // Calculate vertical alignment offset
            float drawY = startY - lineIndex * fontSizeInUnits - (fontSizeInUnits * lineSpacingPercent * lineIndex);
            DrawString(line.Trim(), drawY, drawCamera);
        }
    }

    private void DrawString(string textToDraw, float y, Camera drawCamera = null)
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
                Quaternion rotation = Quaternion.identity * Quaternion.Euler(0, Mathf.Cos(Time.time * 3f + i) * 10f, 0);
                Vector3 scale = Vector3.one * (fontSizeInUnits * _animScale);

                // Calculate the transformation matrix
                Matrix4x4 matrix = transform.localToWorldMatrix * Matrix4x4.TRS(position, rotation, scale);

                // Draw the mesh directly to the world
                Graphics.DrawMesh(mesh, matrix, material, gameObject.layer, drawCamera);
            }

            // Move x position for the next character
            x -= fontSizeInUnits * letterSpacingPercent;
        }
    }

    public void SetText(String text)
    {
        this.text = text;
    }
    
    public void SetFontSize(float size)
    {
        fontSizeInUnits = size;
    }
    
    public void SetRenderInCamera(Camera cam)
    {
        renderInCamera = cam;
    }

    public void AnimPulse()
    {
        DOTween.Kill(gameObject);
        _animScale = 0.5f;
        DOTween.Sequence()
            .Append(DOTween.To(() => _animScale, x => _animScale = x, 1f, 0.25f).SetEase(Ease.OutBack))
            .SetTarget(gameObject);
    }
}
