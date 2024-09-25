using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PauseScene : MonoBehaviour
    {
        [SerializeField] private Camera pauseSceneCamera;
        
        private void Start()
        {
            RenderManager.AddCameraToStack(pauseSceneCamera);
        }

        private void OnDestroy()
        {
            RenderManager.RemoveCameraFromStack(pauseSceneCamera);
        }
    }
}
