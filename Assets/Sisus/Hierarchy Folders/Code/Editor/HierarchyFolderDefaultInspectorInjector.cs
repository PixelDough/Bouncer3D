#define HF_DISABLE_GAMEOBJECT_EDITOR

#if HF_DISABLE_GAMEOBJECT_EDITOR && UNITY_2018_2_OR_NEWER // Editor.finishedDefaultHeaderGUI doesn't exist in Unity versions older than 2018.2
using System;
using System.Collections.Generic;
using Sisus.HierarchyFolders.Prefabs;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sisus.HierarchyFolders
{
	[InitializeOnLoad]
	internal static class HierarchyFolderDefaultInspectorInjector
	{
		private static readonly GUIContent label = new GUIContent("This is a hierarchy folder and can be used for organizing objects in the hierarchy.\n\nWhen a build is being made all members will be moved up the parent chain and the folder itself will be removed.");
		private static readonly int DefaultLayer = LayerMask.NameToLayer("Default");
		private static readonly HashSet<GameObject> inspectedHierarchyFolders = new();
		private static Texture2D folderIcon;
		private static Tool? restoreActiveTool = null;

		static HierarchyFolderDefaultInspectorInjector()
		{
			Editor.finishedDefaultHeaderGUI -= OnAfterGameObjectHeaderGUI;
			Editor.finishedDefaultHeaderGUI += OnAfterGameObjectHeaderGUI;

			Selection.selectionChanged -= OnSelectionChanged;
			Selection.selectionChanged += OnSelectionChanged;
		}

		private static void OnSelectionChanged()
		{
			var selectedGameObjects = Selection.gameObjects;

			if(Tools.current is Tool.None && restoreActiveTool.HasValue && !Array.Exists(selectedGameObjects, x => x.IsHierarchyFolder()))
			{
				Tools.current = restoreActiveTool.Value;
				restoreActiveTool = null;

				foreach(var sceneViewObject in SceneView.sceneViews)
				{
					if(sceneViewObject is SceneView sceneView && sceneView)
					{
						sceneView.Repaint();
					}
				}
			}

			if(inspectedHierarchyFolders.Count == 0)
			{
				return;
			}

			foreach(var previouslyInspectedHierarchyFolder in inspectedHierarchyFolders)
			{
				if(previouslyInspectedHierarchyFolder != null
					&& Array.IndexOf(selectedGameObjects, previouslyInspectedHierarchyFolder) == -1)
				{
					// Unset Hierarchy Folder icon, when not viewed in the Inspector, so that it doesn't clutter the scene view
					EditorGUIUtility.SetIconForObject(previouslyInspectedHierarchyFolder, null);
				}
			}

			inspectedHierarchyFolders.Clear();
		}

		private static void OnAfterGameObjectHeaderGUI(Editor editor)
		{
			#if POWER_INSPECTOR
			if(InspectorUtility.NowDrawingInspectorPart != InspectorPart.None)
			{
				return;
			}
			#endif

			if(editor == null)
			{
				return;
			}

			var targets = editor.targets;
			int count = targets.Length;
			if(count == 0)
			{
				return;
			}

			bool drawHelpBox = false;
			bool isPrefabAssetOrOpenInPrefabStage = false;
			bool? setTagForChildren = null;

			for(int i = targets.Length - 1; i >= 0; i--)
			{
				if(targets[i] is not GameObject gameObject || !gameObject.IsHierarchyFolder())
				{
					continue;
				}

				if(Tools.current is not (Tool.Custom or Tool.None or Tool.View))
				{
					if(!restoreActiveTool.HasValue)
					{
						restoreActiveTool = Tools.current;

						foreach(var sceneViewObject in SceneView.sceneViews)
						{
							if(sceneViewObject is SceneView sceneView && sceneView)
							{
								sceneView.Repaint();
							}
						}
					}

					Tools.current = Tool.None;
				}

				if(!gameObject.CompareTag("Untagged"))
				{
					setTagForChildren ??= EditorUtility.DisplayDialog("Change Tag", "Do you want to set tag to " + gameObject.tag + " for all child objects?", "Yes, change children", "Cancel");
					SetTag(targets, setTagForChildren.Value);
				}

				drawHelpBox = true;

				if(folderIcon == null)
				{
					var iconSizeWas = EditorGUIUtility.GetIconSize();
					EditorGUIUtility.SetIconSize(new Vector2(30f, 30f));
					var iconContent = EditorGUIUtility.IconContent("Folder Icon");
					folderIcon = iconContent?.image as Texture2D;
					EditorGUIUtility.SetIconSize(iconSizeWas);

					#if DEV_MODE
					Debug.Assert(folderIcon != null);
					#endif
				}

				// Set Hierarchy Folder icon while viewed in the Inspector
				EditorGUIUtility.SetIconForObject(gameObject, folderIcon);
				inspectedHierarchyFolders.Add(gameObject);

				bool isPrefabAsset;
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

				// Don't hide transform in prefabs or prefab instances to avoid internal Unity exceptions.
				// We can still set NotEditable true to prevent the user from making modifications via the inspector.
				if(isPrefabAssetOrOpenInPrefabStage || gameObject.IsConnectedPrefabInstance())
				{
					HandlePrefabOrPrefabInstanceStateLocking(gameObject, isPrefabAsset);
				}
				else
				{
					HandleSceneObjectStateLocking(gameObject);
				}
			}

			if(drawHelpBox)
			{
				var preferences = HierarchyFolderPreferences.Get();
				label.text = isPrefabAssetOrOpenInPrefabStage ? preferences.prefabInfoBoxText : preferences.infoBoxText;
				EditorGUILayout.LabelField(label, EditorStyles.helpBox);
			}
		}

		private static void SetTag(Object[] targets, bool setForChildren)
		{
			foreach(var target in targets)
			{
				if(target is not GameObject gameObject || !gameObject.IsHierarchyFolder())
				{
					continue;
				}

				if(setForChildren)
				{
					gameObject.SetTagForAllChildren(gameObject.tag);
				}

				gameObject.tag = "Untagged";
			}
		}

		private static void HandlePrefabOrPrefabInstanceStateLocking(GameObject gameObject, bool isPrefabAsset)
		{
			if(gameObject == null)
			{
				return;
			}

			var hierarchyFolder = gameObject.GetComponent<HierarchyFolder>();
			if(hierarchyFolder == null)
			{
				return;
			}

			HandlePrefabOrPrefabInstanceStateLocking(hierarchyFolder, isPrefabAsset);
		}

		private static void HandlePrefabOrPrefabInstanceStateLocking(HierarchyFolder hierarchyFolder, bool isPrefabAsset)
		{
			var transform = hierarchyFolder.transform;
			transform.hideFlags = HideFlags.NotEditable;

			var gameObject = transform.gameObject;
			if(gameObject.layer != DefaultLayer)
			{
				gameObject.layer = DefaultLayer;
			}

			hierarchyFolder.hideFlags = HideFlags.HideInInspector;

			if(!isPrefabAsset)
			{
				return;
			}

			if(HierarchyFolderUtility.HasSupernumeraryComponents(hierarchyFolder))
			{
				HierarchyFolderUtility.UnmakeHierarchyFolder(gameObject, hierarchyFolder);
				return;
			}

			HierarchyFolderUtility.ResetTransformStateWithoutAffectingChildren(transform, false);
		}

		private static void HandleSceneObjectStateLocking(GameObject gameObject)
        {
			if(gameObject == null)
			{
				return;
			}

			var hierarchyFolder = gameObject.GetComponent<HierarchyFolder>();
			if(hierarchyFolder == null)
			{
				return;
			}

			HandleSceneObjectStateLocking(hierarchyFolder);
        }

		private static void HandleSceneObjectStateLocking(HierarchyFolder hierarchyFolder)
		{
			var transform = hierarchyFolder.transform;
			transform.hideFlags = HideFlags.HideInInspector;
			hierarchyFolder.hideFlags = HideFlags.HideInInspector;

			if(transform.gameObject.layer != DefaultLayer)
			{
				transform.gameObject.layer = DefaultLayer;
			}
		}
	}
}
#endif