using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UIElements;

namespace ImpactCFX.EditorScripts
{
    public class ImpactCFXSettingsProvider : SettingsProvider
    {
        private const string IMPACTCFX_DEBUG_DEFINE = "IMPACTCFX_DEBUG";

        [SettingsProvider]
        public static SettingsProvider CreateImpactCFXSettingsProvider()
        {
            ImpactCFXSettingsProvider provider = new ImpactCFXSettingsProvider("Project/Impact CFX", SettingsScope.Project, new string[] { "Impact" });
            return provider;
        }

        private ImpactCFXSettings settings;

        public ImpactCFXSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = ImpactCFXSettings.instance;
            settings.SaveSettings();
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("These tags are editor-only labels you can give to Impact Tag values to make editing Impact assets, components, and scripts easier.", MessageType.Info);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < ImpactCFXSettings.TAG_COUNT; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(i + ":", GUILayout.Width(26));

                settings[i] = EditorGUILayout.TextField(settings[i]);

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                settings.SaveSettings();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Logging", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Enabling logging can help track down issues that are causing effects to not play as expected.", MessageType.Info);

            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);

            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] defines);
            bool isDefineEnabled = defines.Contains(IMPACTCFX_DEBUG_DEFINE);

            if (isDefineEnabled)
            {
                if (GUILayout.Button("Disable Impact CFX Logging"))
                {
                    setLoggingEnabled(false);
                }

                EditorGUILayout.HelpBox("Make sure to disable Burst Compilation to see logs from Impact CFX Jobs!", MessageType.Warning);

            }
            else
            {
                if (GUILayout.Button("Enable Impact CFX Logging"))
                {
                    setLoggingEnabled(true);
                }
            }
        }

        private void setLoggingEnabled(bool enabled)
        {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);

            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] defines);

            List<string> definesList = new List<string>(defines);

            if (enabled)
                definesList.Add(IMPACTCFX_DEBUG_DEFINE);
            else
                definesList.RemoveAll(s => s.Equals(IMPACTCFX_DEBUG_DEFINE));

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, definesList.ToArray());
        }
    }
}