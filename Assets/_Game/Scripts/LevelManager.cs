using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Febucci.UI;
using PixelDough.Bouncer.LevelData;
using PixelDough.Bouncer.UI;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using Tools.SceneDependencies;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PixelDough.Bouncer
{
    public class LevelManager : MonoBehaviour
    {
        public GameLevelDataSO gameLevelData;
        [SerializeField] private ZoneDataScriptableObject zoneData;
        [SerializeField] private List<LevelFeature> levelFeatures = new List<LevelFeature>();
        [SerializeField] private List<LiveZone> liveZones = new List<LiveZone>();
        public List<LiveZone> LiveZones => liveZones;
        [SerializeField] private CutsceneController cutsceneController;
        
        [SerializeField] private FMODUnity.EventReference levelMusic;
        private FMOD.Studio.EventInstance _levelMusicInstance;
        [SerializeField] private SceneDependencySettingsSO levelSelectScene;
        [SerializeField] private FMODUnity.EventReference quitSound;

        [SerializeField] private CinemachineCamera startCinemachineCamera;
        
        [Header("Player Stuff")]
        [SerializeField] private PlayerStuffManager playerStuffPrefab;
        [SerializeField] private PlayerSpawner spawnPoint;
        
        [Header("HUD Stuff")]
        [SerializeField] private PlayerHudController playerHudController; 
        [SerializeField] public Countdown Countdown;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private TypewriterByCharacter tutorialTextTypewriter;
        
        [Header("Pause Stuff")]
        [SerializeField] private PauseScene pauseScene;
        
        [Header("Inputs")]
        [SerializeField] private InputActionReference pauseAction;
        [SerializeField] private InputActionReference quitAction;

        [NonSerialized] public PlayerStuffManager PlayerStuffManager;
        
        public enum LevelStates { Intro, Playing, Finished }
        [HideInInspector] public LevelStates LevelState = LevelStates.Intro;

        public bool IsPaused => _isPaused;
        public Action<bool> OnPauseStateChanged = delegate {  };
        private bool _isPaused = false;
        
        public TimeSpan LevelTime = new TimeSpan(0, 0, 0, 0, 0);
        public bool CountingTime = false;

        private float _muffleEffectValue = 0f;

        private void OnValidate()
        {
            levelFeatures.TrimExcess();
            levelFeatures.RemoveAll(feature => 
                feature == null || feature.gameObject.scene.name == null || feature.gameObject.scene.name == feature.gameObject.name
            );
        }

        private void OnDestroy()
        {
            GameSceneManager.OnSceneLoaded -= Initialize;
            if (_levelMusicInstance.isValid())
            {
                _levelMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _levelMusicInstance.release();
            }
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Muffle", 0f);
        }

        private void Start()
        {
            PlayerStuffManager spawnedPlayerStuff = Instantiate(playerStuffPrefab, spawnPoint.transform.position,
                spawnPoint.transform.rotation);
            spawnedPlayerStuff.levelManager = this;
            spawnedPlayerStuff.playerController.levelManager = this;
            PlayerStuffManager = spawnedPlayerStuff;
            
            GameManager.DoPlayerPhysics = true;
            
            if (!levelMusic.IsNull) _levelMusicInstance = FMODUnity.RuntimeManager.CreateInstance(levelMusic);

            startCinemachineCamera.Priority = 999999;
            startCinemachineCamera.Prioritize();

            if (GameSceneManager.IsChangingScenes)
                GameSceneManager.OnSceneLoaded += Initialize;
            else
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (cutsceneController)
            {
                cutsceneController.PlayCutscene();
            }
            else
            {
                CountingTime = true;
            }
            
            playerHudController.SetVisibility(true, true);
            Countdown.PlayCountdown(() =>
            {
                if (_levelMusicInstance.isValid())
                {
                    _levelMusicInstance.start();
                }
            });
            
            startCinemachineCamera.Priority = 0;
            
            ResetTimer();
            
            levelFeatures.TrimExcess();
            levelFeatures.RemoveAll(feature => 
                feature == null || feature.gameObject.scene.name == null || feature.gameObject.scene.name == feature.gameObject.name
            );
            levelFeatures.ForEach(feature => feature.Initialize());

            pauseScene.gameObject.SetActive(false);
            
            GameManager.Instance.currentLevelDataIndex =
                GameManager.Instance.gameLevels.FindIndex(level => level.levelID == gameLevelData.levelID);
        }

        private void Update()
        {
            if (CountingTime)
                LevelTime = LevelTime.Add(TimeSpan.FromSeconds(Time.deltaTime));
            
            _muffleEffectValue = MathHelpers.ExpDecay(_muffleEffectValue, _isPaused ? 1f : 0f, 13f, Time.deltaTime);
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Muffle", _muffleEffectValue);

            if (!CountingTime) return;
            if (GameSceneManager.IsChangingScenes) return;

            HandlePausing();
            
            if (quitAction.action.triggered)
            {
                QuitGame();
            }
        }

        private void HandlePausing()
        {
            if (!GameManager.DoPlayerMovement) return;
            if (!pauseAction.action.triggered) return;
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            _isPaused = true;
            DOTween.PauseAll();
            MEC.Timing.PauseCoroutines();
            GameManager.DoPlayerPhysics = false;
            pauseScene.Show();
            OnPauseStateChanged?.Invoke(_isPaused);
        }

        public void ResumeGame()
        {
            _isPaused = false;
            DOTween.PlayAll();
            MEC.Timing.ResumeCoroutines();
            GameManager.DoPlayerPhysics = true;
            pauseScene.Hide();
            OnPauseStateChanged?.Invoke(_isPaused);
        }
        
        public void QuitGame()
        {
            CountingTime = false;
            GameSceneManager.LoadScene(levelSelectScene);
            FMODUnity.RuntimeManager.PlayOneShot(quitSound);
        }

        public void ResetTimer()
        {
            LevelTime = new TimeSpan(0, 0, 0, 0, 0);
        }

        public void StopTimer()
        {
            CountingTime = false;
            LevelState = LevelStates.Finished;
        }

        public void SaveRecord()
        {
            int previousRecord = GameManager.Instance.SaveLevelRecord(gameLevelData.levelID, (int)(LevelTime.TotalMilliseconds));
        }

        public void RegisterLevelFeature(LevelFeature levelFeature)
        {
            if (levelFeatures.Contains(levelFeature)) return;
            levelFeatures.Add(levelFeature);
        }
        
        public void DeregisterLevelFeature(LevelFeature levelFeature)
        {
            levelFeatures.Remove(levelFeature);
        }
        
        public void CutsceneBegin()
        {
            playerHudController.SetVisibility(false);
        }
        
        public void CutsceneEnded()
        {
            playerHudController.SetVisibility(true);
        }
        
        public void ShowTutorialText(string text)
        {
            tutorialText.text = text;
            tutorialTextTypewriter.StopDisappearingText();
            tutorialTextTypewriter.StartShowingText(true);
        }

        public void HideTutorialText()
        {
            tutorialTextTypewriter.StopShowingText();
            tutorialTextTypewriter.StartDisappearingText();
        }


        [Button]
        public void RegenerateLevelFeatureList()
        {
            levelFeatures.Clear();
            levelFeatures.AddRange(FindObjectsByType<LevelFeature>(FindObjectsInactive.Include,
                FindObjectsSortMode.None));
            foreach (LevelFeature levelFeature in levelFeatures)
            {
                levelFeature.levelManager = this;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(levelFeature);
                #endif
            }
        }
        
        public void ResetLevelElements()
        {
            DOTween.KillAll();
            MEC.Timing.KillCoroutines();
            levelFeatures.ForEach(feature => feature.Initialize());
        }

        [Button]
        public void RegenerateLiveZoneList()
        {
            liveZones.Clear();
            liveZones.AddRange(FindObjectsByType<LiveZone>(FindObjectsInactive.Include,
                FindObjectsSortMode.None));
        }
    }
}
