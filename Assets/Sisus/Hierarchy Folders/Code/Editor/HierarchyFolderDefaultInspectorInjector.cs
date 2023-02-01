#define HF_DISABLE_GAMEOBJECT_EDITOR

#if HF_DISABLE_GAMEOBJECT_EDITOR && UNITY_2018_2_OR_NEWER && !POWER_INSPECTOR // Editor.finishedDefaultHeaderGUI doesn't exist in Unity versions older than 2018.2
using UnityEngine;
using UnityEditor;
using Sisus.HierarchyFolders.Prefabs;
using static Sisus.HierarchyFolders.HierarchyFolderUtility;

namespace Sisus.HierarchyFolders
{
	[InitializeOnLoad]
	internal static class HierarchyFolderDefaultInspectorInjector
	{
		private static readonly GUIContent label = new GUIContent("This is a hierarchy folder and can be used for organizing objects in the hierarchy.\n\nWhen a build is being made all members will be moved up the parent chain and the folder itself will be removed.");
		private static readonly int DefaultLayer = LayerMask.NameToLayer("Default");

		static HierarchyFolderDefaultInspectorInjector()
		{
			Editor.finishedDefaultHeaderGUI -= OnAfterGameObjectHeaderGUI;
			Editor.finishedDefaultHeaderGUI += OnAfterGameObjectHeaderGUI;
		}

		private static void OnAfterGameObjectHeaderGUI(Editor editor)
		{
			if(editor == null)
			{
				return;
			}

			var targets = editor.targets;

			if(targets.Length == 0)
			{
				return;
			}

			var gameObject = targets[0] as GameObject;
			if(gameObject == null)
			{
				return;
			}

			HierarchyFolder hierarchyFolder;
			bool isHierarchyFolder;
			#if UNITY_2019_2_OR_NEWER
			isHierarchyFolder = gameObject.TryGetComponent(out hierarchyFolder);
			#else
			hierarchyFolder = gameObject.GetComponent<HierarchyFolder>();
			isHierarchyFolder = hierarchyFolder != null;
			#endif

			if(!isHierarchyFolder)
			{
				return;
			}

			bool isPrefabAsset, isPrefabAssetOrOpenInPrefabStage;
			if(gameObject.IsPrefabAsset())
			{
				isPrefabAsset = true;
				isPrefabAssetOrOpenInPrefabStage = true;
			}
			else if(gameObject.IsOpenInPrefabStage())
			{
				isPrefabAsset = false;
				isPrefabAssetOrOpenInPrefabStage = true;
			}
			else
			{
				isPrefabAsset = false;
				isPrefabAssetOrOpenInPrefabStage = false;
			}

			var preferences = HierarchyFolderPreferences.Get();
			label.text = isPrefabAssetOrOpenInPrefabStage ? preferences.prefabInfoBoxText : preferences.infoBoxText;

			EditorGUILayout.LabelField(label, EditorStyles.helpBox);

			if(!gameObject.CompareTag("Untagged"))
			{
				bool setForChildren = EditorUtility.DisplayDialog("Change Tag", "Do you want to set tag to " + gameObject.tag + " for all child objects?", "Yes, change children", "Cancel");
				for(int i = targets.Length - 1; i >= 0; i--)
				{
					gameObject = targets[i] as GameObject;
					if(setForChildren)
					{
						gameObject.SetTagForAllChildren(gameObject.tag);
					}
					gameObject.tag = "Untagged";
				}
			}

			if(isPrefabAssetOrOpenInPrefabStage || gameObject.IsConnectedPrefabInstance())
			{
				HandlePrefabOrPrefabInstanceStateLocking(hierarchyFolder, isPrefabAsset);
				for(int i = targets.Length - 1; i >= 1; i--)
				{
					gameObject = targets[i] as GameObject;
					if(gameObject == null)
					{
						continue;
					}

					#if UNITY_2019_2_OR_NEWER
					if(!gameObject.TryGetComponent(out hierarchyFolder))
					{
						continue;
					}
					#else
					hierarchyFolder = gameObject.GetComponent<HierarchyFolder>();
					if(hierarchyFolder == null)
					{
						continue;
					}
					#endif

					HandlePrefabOrPrefabInstanceStateLocking(hierarchyFolder, isPrefabAsset);
				}
				return;
			}

			HandleSceneObjectStateLocking(hierarchyFolder);

			for(int n = targets.Length - 1; n >= 1; n--)
			{
				gameObject = targets[n] as GameObject;
				if(gameObject == null)
                {
					continue;
                }

				#if UNITY_2019_2_OR_NEWER
				if(!gameObject.TryGetComponent(out hierarchyFolder))
                {
					continue;
                }
				#else
				hierarchyFolder = gameObject.GetComponent<HierarchyFolder>();
				if(hierarchyFolder == null)
				{
					continue;
				}
				#endif

				HandleSceneObjectStateLocking(hierarchyFolder);
			}
		}

		private static void HandlePrefabOrPrefabInstanceStateLocking(HierarchyFolder hierarchyFolder, bool isPrefabAsset)
		{
			// Don't hide transform in prefabs or prefab instances to avoid internal Unity exceptions.
			// We can still set NotEditable true to prevent the user from making modifications via the inspector.
			var transform = hierarchyFolder.transform;
			transform.hideFlags = HideFlags.NotEditable;

			var gameObject = transform.gameObject;
			if(gameObject.layer != DefaultLayer)
			{
				gameObject.layer = DefaultLayer;
			}

			hierarchyFolder.hideFlags = HierarchyFolderUtility.HierarchyFolderHideFlags;

			if(!isPrefabAsset)
			{
				return;
			}

			if(HasSupernumeraryComponents(hierarchyFolder))
			{
				UnmakeHierarchyFolder(gameObject, hierarchyFolder);
				return;
			}

			ResetTransformStateWithoutAffectingChildren(transform);
		}

		private static void HandleSceneObjectStateLocking(HierarchyFolder hierarchyFolder)
		{
			var transform = hierarchyFolder.transform;
			transform.hideFlags = HierarchyFolderUtility.GetHierarchyFolderTransformHideFlags(transform);
			hierarchyFolder.hideFlags = HierarchyFolderUtility.HierarchyFolderHideFlags;

			if(transform.gameObject.layer != DefaultLayer)
			{
				transform.gameObject.layer = DefaultLayer;
			}

			ResetTransformStateWithoutAffectingChildren(transform);
		}
	}
}
#endif