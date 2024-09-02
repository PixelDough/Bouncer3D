using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [FilePath(SETTINGS_PATH, FilePathAttribute.Location.ProjectFolder)]
    public class ImpactCFXSettings : ScriptableSingleton<ImpactCFXSettings>
    {
        public const string SETTINGS_PATH = "ProjectSettings/ImpactCFXSettings.asset";
        public const int TAG_COUNT = 32;

        [SerializeField]
        private string[] tags = new string[TAG_COUNT];
        [SerializeField]
        private bool hasSeenDemoSetup = false;

        public string this[int index]
        {
            get => tags[index];
            set => tags[index] = value;
        }

        public bool HasSeenDemoSetup
        {
            get => hasSeenDemoSetup;
            set => hasSeenDemoSetup = value;
        }

        public int GetDefinedTagCount()
        {
            int count = 0;

            for (int i = 0; i < TAG_COUNT; i++)
            {
                if (IsTagDefined(i))
                    count++;
            }

            return count;
        }

        public bool IsTagDefined(int index)
        {
            return !string.IsNullOrEmpty(tags[index]);
        }

        public void SaveSettings()
        {
            Save(true);
        }
    }
}