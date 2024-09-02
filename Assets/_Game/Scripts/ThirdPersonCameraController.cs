using System;
using System.Collections;
using Rewired;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class ThirdPersonCameraController : MonoBehaviour
    {

        public CinemachineVirtualCamera virtualCamera;
        private CinemachineFreeLook _freeLook;

        public enum CameraMode
        {
            Passive,
            Active
        }
        public CameraMode cameraMode
        {
            get;
            private set;
        } = CameraMode.Passive;

        private float _waitWhileStillTime = 0f;
        private readonly float _waitWhileStillTimeMax = 1f;

        private bool _doMovement = true;

        private void LateUpdate()
        {
            #if (UNITY_EDITOR)
                if (!EditorWindow.mouseOverWindow) return;
            #endif
            
            
        }

        /*private void LateUpdate()
        {
            // #if (UNITY_EDITOR)
            //     if (!EditorWindow.mouseOverWindow) return;
            // #endif

            //if (!PlayerStuffManager.playerController) return;

            //if (LetterboxController.Instance.IsActive) return;
        
            if (!Application.isFocused || Cursor.lockState == CursorLockMode.None) return;
            //_freeLook.m_XAxis.Value += _input.GetAxis(RewiredConsts.Action.LookYaw) / 8f;
            //_freeLook.m_YAxis.Value -= _input.GetAxis(RewiredConsts.Action.LookPitch) / 240f;

            if (!_doMovement) return;

            Vector3 _lookInput =
                GameManager.Instance.Input.GetAxis2D(RewiredConsts.Action.LookHorizontal,
                    RewiredConsts.Action.LookVertical) * (2f * Time.timeScale);
            Vector3 rot = transform.eulerAngles;
            rot.x -= _lookInput.x / 3;
            rot.y += _lookInput.y / 3;
            if (rot.x > 180) rot.x = Mathf.Max(rot.x, 360-60);
            else rot.x = Mathf.Min(rot.x, 80);
        
            transform.rotation = Quaternion.Euler(rot);
        }

        private ControllerType GetLastUsedController()
        {
            // Get last controller from a Player and the determine the type of controller being used
            Controller controller = GameManager.Instance.Input.controllers.GetLastActiveController();
            if (controller != null)
                return controller.type;
        
            return default;
        }

        public void HandleCameraState()
        {
            //if (LetterboxController.Instance.IsActive) return;
        
            if (!_doMovement) return;
        
            Vector2 playerMoveInput = GameManager.Instance.Input.GetAxis2D(RewiredConsts.Action.MoveHorizontal,
                RewiredConsts.Action.MoveVertical);

            if (GetLastUsedController() == ControllerType.Joystick)
            {
                bool camBehind = false; //GameManager.Instance.Input.GetButton(RewiredConsts.Action.CamBehind);

                _waitWhileStillTime += Time.deltaTime;

                if (GameManager.Instance.Input.GetAxis2D(RewiredConsts.Action.LookVertical, 
                        RewiredConsts.Action.LookHorizontal)
                    .magnitude >= 0.01f)
                    _waitWhileStillTime = 0f;
                else if (playerMoveInput.magnitude > 0.01f)
                {
                    _waitWhileStillTime = 0f;
                    Vector3 rot = transform.eulerAngles;

                    float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(rot.y, PlayerStuffManager.playerController.transform.eulerAngles.y));
                    float deltaSin = Mathf.Sin(deltaAngle * Mathf.Deg2Rad);

                    float mul = deltaSin / 2f;

                    if (mul > 0.2f)
                    {
                        if (camBehind) mul *= 5f;
                        rot.y = Mathf.LerpAngle(rot.y, PlayerStuffManager.playerController.transform.eulerAngles.y, mul * Time.deltaTime);
                        transform.rotation = Quaternion.Euler(rot);
                    }

                }
                else if (_waitWhileStillTime >= _waitWhileStillTimeMax || camBehind)
                {
                    _waitWhileStillTime = _waitWhileStillTimeMax;

                    float mul = 1f;
                    if (camBehind)
                        mul *= 5f;

                    Vector3 rot = transform.eulerAngles;
                    rot.y = Mathf.MoveTowardsAngle(rot.y, PlayerStuffManager.playerController.transform.eulerAngles.y, 30f * mul * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(rot);
                }
            }
        }

        public void SetForward(Vector3 forward)
        {
            transform.forward = forward;
        }

        public void FreezeMovementForTime(float time)
        {
            if (!_doMovement) return;
            StartCoroutine(_FreezeMovementForTime(time));
        }

        private IEnumerator _FreezeMovementForTime(float time)
        {
            _doMovement = false;
            yield return new WaitForSeconds(time);
            _doMovement = true;
        }*/
    
    }
}
