using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelDough.Bouncer
{
    public class InputOverlay : MonoBehaviour
    {
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        
        [SerializeField] private Camera renderCamera;
        
        [Header("Joystick")]
        [SerializeField] private Transform joystickRoot;
        [SerializeField] private Transform joystick;
        [Header("Jump Button")]
        [SerializeField] private Transform jumpButtonRoot;
        [SerializeField] private Transform jumpButton;
        [SerializeField] private MeshRenderer jumpButtonRenderer;
        
        [Header("Input Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        
        private Vector3 _moveEulerAngles;

        private void Update()
        {
            UpdateJoystick();
            UpdateJumpButton();
        }

        private void UpdateJoystick()
        {
            joystickRoot.LookAt(renderCamera.transform, Vector3.up);

            Vector2 moveValue = -moveAction.action.ReadValue<Vector2>();
            Vector3 targetRotation = new Vector3(moveValue.y * 30f, moveValue.x * 30f, 0);
            _moveEulerAngles = MathHelpers.ExpDecay(_moveEulerAngles,
                targetRotation, 23f, Time.deltaTime);
            
            joystick.localEulerAngles = _moveEulerAngles;
        }
        
        private void UpdateJumpButton()
        {
            jumpButtonRoot.forward = jumpButtonRoot.forward;
            float jumpActionValue = jumpAction.action.ReadValue<float>();
            Vector3 targetPos = new Vector3(0, 0, jumpActionValue * -0.4f);
            jumpButton.localPosition = MathHelpers.ExpDecay(jumpButton.localPosition, targetPos, 50f, Time.deltaTime);

            Vector2 currentOffset = jumpButtonRenderer.material.GetTextureOffset(BaseMap);
            Vector2 targetOffset = new Vector2(0,  jumpActionValue * 0.2f);
            jumpButtonRenderer.material.SetTextureOffset(BaseMap,
                MathHelpers.ExpDecay(currentOffset, targetOffset, 50f, Time.deltaTime));
        }
    }
}
