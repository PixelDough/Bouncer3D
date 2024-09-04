#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif

namespace Sisus.HierarchyFolders.Prefabs
{
    public static class GameObjectExtensions
	{
		public static bool IsPrefabAsset(this GameObject gameObject)
		{
			return PrefabUtility.IsPartOfPrefabAsset(gameObject);
		}

		public static bool IsPrefabAssetOrOpenInPrefabStage(this GameObject gameObject)
		{
			return PrefabUtility.IsPartOfPrefabAsset(gameObject) || PrefabStageUtility.GetPrefabStage(gameObject) != null;
		}

		public static bool IsPrefabAssetOrInstance(this GameObject gameObject)
		{
			return PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab;
		}

		public static bool IsConnectedPrefabInstance(this GameObject gameObject)
		{
			return PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected;
		}

		public static bool IsPartOfInstantiatedPrefabInstance(this GameObject gameObject)
		{
			for(var transform = gameObject.transform; transform; transform = transform.parent)
			{
				if(transform.name.EndsWith("(Clone)", StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsPartOfPrefabVariant(this GameObject gameObject)
        {
			return PrefabUtility.IsPartOfVariantPrefab(gameObject);
		}

		public static bool IsConnectedOrDisconnectedPrefabInstance(this GameObject gameObject)
		{
			var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);
			return prefabStatus == PrefabInstanceStatus.Connected || prefabStatus == PrefabInstanceStatus.Disconnected;
		}

		public static bool IsPrefabInstanceRoot(this GameObject gameObject)
		{
			return PrefabUtility.IsAnyPrefabInstanceRoot(gameObject);
		}

		public static bool IsOpenInPrefabStage(this GameObject gameObject)
		{
			return PrefabStageUtility.GetPrefabStage(gameObject) != null;
		}

		public static void SetTagForAllChildren(this GameObject gameObject, string tag)
		{
			var transform = gameObject.transform;
			for(int i = transform.childCount - 1; i >= 0; i--)
			{
				bool skipHierarchyFolders = !string.Equals(tag, "Untagged");
				SetTagForTransformAndAllChildren(transform.GetChild(i), tag, skipHierarchyFolders);
			}
		}

		private static void SetTagForTransformAndAllChildren(Transform transform, string tag, bool skipHierarchyFolders)
		{
			if(!skipHierarchyFolders || !transform.gameObject.IsHierarchyFolder())
			{
				transform.gameObject.tag = tag;
			}

			for(int i = transform.childCount - 1; i >= 0; i--)
			{
				SetTagForTransformAndAllChildren(transform.GetChild(i), tag, skipHierarchyFolders);
			}
		}
	}
}
#endif