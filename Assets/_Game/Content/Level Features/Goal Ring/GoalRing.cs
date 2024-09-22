using Tools.SceneDependencies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelDough.Bouncer
{
    public class GoalRing : MonoBehaviour
    {
        [SerializeField] private SceneDependencySettingsSO levelSelectScene;
        [SerializeField] private InputActionReference continueAction;
        [SerializeField] private FMODUnity.EventReference goalHitSound;
        [SerializeField] private FMODUnity.EventReference continueSound;
        
        private bool _hit = false;

        private bool _continuePressed = false;
        
        private static readonly int SubtractiveFadeAmount = Shader.PropertyToID("_SubtractiveFadeAmount");
        
        private void OnTriggerEnter(Collider other)
        {
            if (_hit) return;
            Rigidbody rbHit = other.attachedRigidbody;
            if (rbHit is null) return;
            if (!rbHit.CompareTag("Player")) return;
            
            Debug.Log("Player reached goal ring!");
            _hit = true;
            Time.timeScale = 0.0f;

            LevelManager.StopTimer();
            Shader.SetGlobalFloat(SubtractiveFadeAmount, 0.5f);

            FMODUnity.RuntimeManager.PlayOneShot(goalHitSound);
            
            LevelManager.Instance.SaveRecord();
        }

        private void Update()
        {
            if (!_hit) return;
            if (LevelManager.LevelState != LevelManager.LevelStates.Finished) return;
            if (_continuePressed) return;

            if (GameSceneManager.IsChangingScenes) return;
            if (continueAction.action.WasPressedThisFrame())
            {
                _continuePressed = true;
                GameSceneManager.LoadScene(levelSelectScene);
                FMODUnity.RuntimeManager.PlayOneShot(continueSound);
            }
        }
    }
}
