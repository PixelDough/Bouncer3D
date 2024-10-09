using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PauseScene : MonoBehaviour
    {
        private static readonly int SubtractiveFadeAmount = Shader.PropertyToID("_SubtractiveFadeAmount");
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        [SerializeField] private LevelManager levelManager;
        [SerializeField] private Transform pauseCamRoot;
        [SerializeField] private Camera pauseSceneCamera;
        [SerializeField] private PlayerController pausePlayer;
        [SerializeField] private Material ledMaterial;

        [Header("Buttons")] 
        [SerializeField] private Transform resumeButton;
        [SerializeField] private Transform resetButton;
        [SerializeField] private Transform yesButton;
        [SerializeField] private Transform noButton;

        [Header("Text")] 
        [SerializeField] private Font3DString playText;
        [SerializeField] private Font3DString goBackText;
        [SerializeField] private Font3DString confirmationText;
        
        private bool _isPaused = false;
        private string _currentCamLocation = "Base";
        public static bool CanHitButton = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitOnLoad()
        {
            CanHitButton = true;
        }

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
            if (_isPaused) return;
            pauseCamRoot.localPosition = new Vector3(pauseCamRoot.localPosition.x, pauseCamRoot.localPosition.y, -2.42f);
            ledMaterial.SetTextureOffset(BaseMap, Vector2.zero);
            
            confirmationText.SetText("");
            
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
            if (!_isPaused) return;
            _isPaused = false;
            
            DOTween.To(() => Shader.GetGlobalFloat(SubtractiveFadeAmount), x => Shader.SetGlobalFloat(SubtractiveFadeAmount, x), 0.0f, 0.25f).SetEase(Ease.OutQuad);
            await pauseSceneCamera.transform.DOLocalRotate(new Vector3(-90, pauseSceneCamera.transform.localEulerAngles.y, pauseSceneCamera.transform.localEulerAngles.z), 0.25f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
            gameObject.SetActive(false);
            pausePlayer.Rigidbody.isKinematic = true;
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
        
        private async Awaitable ShowConfirmation()
        {
            if (!_isPaused) return;
            
            confirmationText.SetText("Are you" + Environment.NewLine + "sure?", true);
            
            resumeButton.gameObject.SetActive(false);
            yesButton.gameObject.SetActive(true);
            playText.SetText("Yes", true);
            yesButton.DOPunchScale(Vector3.one * -0.1f, 0.25f);
            await Task.Delay(50);
            
            resetButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(true);
            goBackText.SetText("No", true);
            await noButton.DOPunchScale(Vector3.one * -0.1f, 0.25f).AsyncWaitForCompletion();
        }

        private async Awaitable HideConfirmation()
        {
            if (!_isPaused) return;
            
            confirmationText.SetText("");
            
            noButton.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(true);
            goBackText.SetText("Go" + Environment.NewLine + "Back", true);
            resetButton.DOPunchScale(Vector3.one * -0.1f, 0.25f);
            await Task.Delay(50);
            
            yesButton.gameObject.SetActive(false);
            resumeButton.gameObject.SetActive(true);
            playText.SetText("Play", true);
            await resumeButton.DOPunchScale(Vector3.one * -0.1f, 0.25f).AsyncWaitForCompletion();
        }

        #region Button Events

        private async Awaitable BlinkLED()
        {
            ledMaterial.SetTextureOffset(BaseMap, new Vector2(0, 0));
            await ledMaterial.DOOffset(new Vector2(0, 0.5f), BaseMap, 0.1f).SetEase(Ease.OutSine).AsyncWaitForCompletion();
            await ledMaterial.DOOffset(new Vector2(0, 0), BaseMap, 0.1f).SetEase(Ease.InSine).AsyncWaitForCompletion();
        }

        public async void ResumeButton()
        {
            await BlinkLED();
            levelManager.ResumeGame();
        }

        public async void QuitButton()
        {
            await ShowConfirmation();
        }

        public async void GoBackButton()
        {
            await BlinkLED();
            await Hide();
            levelManager.ResumeGame();
            levelManager.PlayerStuffManager.playerController.Kill();
        }
        
        public async void YesButton()
        {
            await BlinkLED();
            await Hide();
            levelManager.QuitGame();
        }

        public async void NoButton()
        {
            await HideConfirmation();
        }

        #endregion
    }
}
