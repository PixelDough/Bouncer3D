using System;
using System.Collections.Generic;
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
        [SerializeField] private InputActionReference quitAction;
        [SerializeField] private FMODUnity.EventReference quitSound;
        
        [Header("HUD Stuff")]
        [SerializeField] private PlayerHudController playerHudController; 
        [SerializeField] public Countdown Countdown;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private TypewriterByCharacter tutorialTextTypewriter;
        
        public enum LevelStates { Intro, Playing, Finished }
        public static LevelStates LevelState = LevelStates.Intro;
        
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

            GameManager.Instance.currentLevelDataIndex =
                GameManager.Instance.gameLevels.FindIndex(level => level.levelID == gameLevelData.levelID);
        }

        private void Update()
        {
            if (CountingTime)
                LevelTime = LevelTime.Add(TimeSpan.FromSeconds(Time.deltaTime));

            if (!CountingTime) return;
            if (GameSceneManager.IsChangingScenes) return;
            if (quitAction.action.triggered)
            {
                CountingTime = false;
                GameSceneManager.LoadScene(levelSelectScene);
                FMODUnity.RuntimeManager.PlayOneShot(quitSound);
            }
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
