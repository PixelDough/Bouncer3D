using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Bewildered.SmartLibrary
{
    public class PrefabPreviewGenerator : PreviewGeneratorBase<GameObject>
    {
        private GameObject _targetInstance;
        private GameObject _canvasGameObject;
        private CanvasScaler _scaler;

        public PrefabPreviewGenerator(PreviewRenderer renderer) : base(renderer)
        {
            _canvasGameObject = renderer.Stage.CreateGameObject("Canvas");
            Canvas canvas = _canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Renderer.Camera;
            canvas.planeDistance = 5;

            _scaler = _canvasGameObject.AddComponent<CanvasScaler>();
            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }

        protected override bool BeforeRender(GameObject target)
        {
            if (target.transform is RectTransform)
            {
                return BeforeRenderGUI(target);
            }
            else
            {
                return BeforeRenderObject(target);
            }
        }

        private bool BeforeRenderGUI(GameObject target)
        {
            // Note: We don't need to add the target instance to the renderer scene
            // because it is instantiated as a child of the canvas.
            _targetInstance = Object.Instantiate(target, _canvasGameObject.transform);
            RectTransform rectTransform = _targetInstance.GetComponent<RectTransform>();
            
            // The Canvas must be enabled before getting the render bounds
            // otherwise layout components will not layout children correctly.
            _canvasGameObject.SetActive(true);

            // We get the renderable bounds after creating the instance because we need
            // to parent it to the canvas so we can force rebuild its layout (e.g. VerticalLayoutGroup component)
            Rect rect = PreviewEditorUtility.GetGUIRenderableBounds(rectTransform);
            
            if (rect == Rect.zero)
                return false;

            Vector2 transformSize = rectTransform.rect.size;

            // Note that 0,0 is the center of the canvas, with up and right being positive of their respective axis.
            _targetInstance.transform.localPosition = new Vector3(
                -(rect.width / 2 - transformSize.x / 2),
                rect.height / 2 - transformSize.y / 2);
            
            _scaler.referenceResolution = rect.size;
            
            // Canvas' are not rendered in the Preview CameraType so we need to change it to the Game type.
            Renderer.Camera.cameraType = CameraType.Game;
            Renderer.Camera.nearClipPlane = 0.001f;
            Renderer.Camera.farClipPlane = 10;

            return true;
        }

        private bool BeforeRenderObject(GameObject target)
        {
            // Note: We use the target instance to get the renderable bounds because to get the bounds
            // of a particle system it needs to be played, and cannot be played on prefab assets.
            _targetInstance = Object.Instantiate(target, Vector3.zero, Quaternion.identity);
            Renderer.AddGameObject(_targetInstance);

            Bounds bounds = PreviewEditorUtility.GetRenderableBounds(_targetInstance, out bool has2DRenderer);

            if (bounds.size == Vector3.zero)
                return false;
            

            if (EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D && has2DRenderer)
                PreviewEditorUtility.PositionCamera2D(Renderer.Camera, bounds, 7.5f);
            else
                PreviewEditorUtility.PositionCamera3D(Renderer.Camera, bounds, 7.5f);

            return true;
        }

        protected override void AfterRender()
        {
            Renderer.Camera.cameraType = CameraType.Preview;
            _canvasGameObject.SetActive(false);
            
            if (_targetInstance != null)
                Object.DestroyImmediate(_targetInstance);
        }

        public override void Cleanup()
        {
            if (_canvasGameObject != null)
                Object.DestroyImmediate(_canvasGameObject);
        }
    }
}
