using JetBrains.Annotations;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor;

namespace Sisus.HierarchyFolders
{
	public static class HierarchyFolderBuildSceneRemover
	{
		private static bool warnedAboutRemoveFromBuildDisabled;

		[PostProcessScene(0), UsedImplicitly]
		private static void OnPostProcessScene()
		{
			// This will also get called when entering Playmode, when SceneManager.LoadScene is called,
			// but we only want to do stripping just after building the Scene.
			if(Application.isPlaying)
			{
				return;
			}

			var preferences = HierarchyFolderPreferences.Get();
			if(preferences == null)
			{
				Debug.LogWarning("Failed to find Hierarchy Folder Preferences asset; will not strip hierarchy folders from build.");
				return;
			}

			var strippingType = StrippingType.FlattenHierarchyAndRemoveGameObject;

			if(!preferences.removeFromScenes)
			{
				if(!preferences.warnWhenNotRemovedFromBuild
					|| warnedAboutRemoveFromBuildDisabled
					|| EditorUtility.DisplayDialog("Warning: Hierarchy Folder Stripping Disabled",
												   "This is a reminder that you have disabled stripping of hierarchy folders from builds."
												   +"\n\nThis will result in suboptimal performance and is not recommended when making a release build.",
												   "Continue Anyway", "Enable Stripping"))
				{
					warnedAboutRemoveFromBuildDisabled = preferences.warnWhenNotRemovedFromBuild;
					strippingType = StrippingType.RemoveComponent;
				}
			}

			HierarchyFolderUtility.ApplyStrippingTypeToAllLoadedScenes(strippingType);
		}
	}
}