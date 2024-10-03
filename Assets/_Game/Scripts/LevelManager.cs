using System;
using System.Collections.Generic;
using DG.Tweening;
using Febucci.UI;
using PixelDough.Bouncer.LevelData;
using PixelDough.Bouncer.UI;
using Sirenix.OdinInspector;
using TMPro;
using Tools.SceneDependencies;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace PixelDough.Bouncer
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        
        public GameLevelDataSO gameLevelData;
        [SerializeField] private ZoneDataScriptableObject zoneData;
        [SerializeField] private List<LevelFeature> levelFeatures = new List<LevelFeature>();
        [SerializeField] private List<LiveZone> liveZones = new List<LiveZone>();
        public List<LiveZone> LiveZones => liveZones;
        [SerializeField] private CutsceneController cutsceneController;
        
        [SerializeField] private SceneDependencySettingsSO levelSelectScene;
        [SerializeField] private FMODUnity.EventReference quitSound;
        
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
        [HideInInspector] public static LevelStates LevelState = LevelStates.Intro;

        public static bool IsPaused => Instance._isPaused;
        public Action<bool> OnPauseStateChanged = delegate {  };
        private bool _isPaused = false;
        
        public static TimeSpan LevelTime;
        public static bool CountingTime = false;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitOnLoad()
        {
            Instance = null;
            LevelState = LevelStates.Intro;
            LevelTime = new TimeSpan(0, 0, 0, 0, 0);
            CountingTime = false;
        }

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
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            Instance = this;
            
            PlayerStuffManager spawnedPlayerStuff = Instantiate(playerStuffPrefab, spawnPoint.transform.position,
                spawnPoint.transform.rotation);
            PlayerStuffManager = spawnedPlayerStuff;
            
            GameManager.DoPlayerPhysics = true;

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
            Countdown.PlayCountdown();
            
            ResetTimer();
            
            levelFeatures.TrimExcess();
            levelFeatures.RemoveAll(feature => 
                feature == null || feature.gameObject.scene.name == null || feature.gameObject.scene.name == feature.gameObject.name
            );

            pauseScene.gameObject.SetActive(false);
            
            GameManager.Instance.currentLevelDataIndex =
                GameManager.Instance.gameLevels.FindIndex(level => level.levelID == gameLevelData.levelID);
        }

        private void Update()
        {
            if (CountingTime)
                LevelTime = LevelTime.Add(TimeSpan.FromSeconds(Time.deltaTime));

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

        public static void ResetTimer()
        {
            LevelTime = new TimeSpan(0, 0, 0, 0, 0);
        }

        public static void StopTimer()
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
