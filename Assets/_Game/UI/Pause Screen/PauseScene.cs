using DG.Tweening;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PauseScene : MonoBehaviour
    {
        private static readonly int SubtractiveFadeAmount = Shader.PropertyToID("_SubtractiveFadeAmount");
        [SerializeField] private Transform pauseCamRoot;
        [SerializeField] private Camera pauseSceneCamera;
        [SerializeField] private PlayerController pausePlayer;
        
        private bool _isPaused = false;
        private string _currentCamLocation = "Base";
        
        private void Start()
        {
            RenderManager.AddCameraToStack(pauseSceneCamera);
        }

        private void OnDestroy()
        {
            RenderManager.RemoveCameraFromStack(pauseSceneCamera);
        }

        public async Awaitable Show()
        {
            pauseCamRoot.localPosition = new Vector3(pauseCamRoot.localPosition.x, pauseCamRoot.localPosition.y, -2.42f);
            
            gameObject.SetActive(true);
            pausePlayer.Rigidbody.isKinematic = true;
            pausePlayer.transform.localPosition = Vector3.up * 0.25f;
            
            DOTween.To(() => Shader.GetGlobalFloat(SubtractiveFadeAmount), x => Shader.SetGlobalFloat(SubtractiveFadeAmount, x), 0.75f, 0.25f).SetEase(Ease.OutQuad);
            pauseSceneCamera.transform.localEulerAngles = new Vector3(-90, pauseSceneCamera.transform.localEulerAngles.y, pauseSceneCamera.transform.localEulerAngles.z);
            await pauseSceneCamera.transform.DOLocalRotate(new Vector3(0, pauseSceneCamera.transform.localEulerAngles.y, pauseSceneCamera.transform.localEulerAngles.z), 0.25f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
            
            pausePlayer.Rigidbody.isKinematic = false;

            _isPaused = true;
        }

        public async Awaitable Hide()
        {
            _isPaused = false;
            pausePlayer.Rigidbody.isKinematic = true;
            
            DOTween.To(() => Shader.GetGlobalFloat(SubtractiveFadeAmount), x => Shader.SetGlobalFloat(SubtractiveFadeAmount, x), 0.0f, 0.25f).SetEase(Ease.OutQuad);
            await pauseSceneCamera.transform.DOLocalRotate(new Vector3(-90, pauseSceneCamera.transform.localEulerAngles.y, pauseSceneCamera.transform.localEulerAngles.z), 0.25f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }

        public async void SetCamToBase()
        {
            if (!_isPaused) return;
            if (_currentCamLocation == "Base") return;
            _currentCamLocation = "Base";
            pausePlayer.Rigidbody.isKinematic = true;
            await pauseCamRoot.DOLocalMoveZ(-2.42f, 0.5f).SetEase(Ease.OutSine).AsyncWaitForCompletion();
            pausePlayer.Rigidbody.isKinematic = false;
        }
        
        public async void SetCamToCode()
        {
            if (!_isPaused) return;
            if (_currentCamLocation == "Code") return;
            _currentCamLocation = "Code";
            pausePlayer.Rigidbody.isKinematic = true;
            await pauseCamRoot.DOLocalMoveZ(2.42f, 0.5f).SetEase(Ease.OutSine).AsyncWaitForCompletion();
            pausePlayer.Rigidbody.isKinematic = false;
        }

        #region Button Events

        public async void ResumeButton()
        {
            LevelManager.Instance.ResumeGame();
        }

        #endregion
    }
}
