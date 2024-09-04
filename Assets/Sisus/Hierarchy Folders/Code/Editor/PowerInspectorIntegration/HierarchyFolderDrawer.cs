#if POWER_INSPECTOR
using System;
using UnityEngine;
using UnityEditor;
using Sisus.Attributes;
using JetBrains.Annotations;
using Sisus.HierarchyFolders.Prefabs;

namespace Sisus.HierarchyFolders
{
	/// <summary>
	/// Drawer for GameObjects that contain the HierarchyFolder component.
	/// </summary>
	[Serializable, DrawerForGameObject(typeof(HierarchyFolder))]
	public class HierarchyFolderDrawer : GameObjectDrawer
	{
		private static readonly GUIContent infoBoxContent = new GUIContent();
		private static readonly GUIContent prefabInfoBoxContent = new GUIContent();

		internal static bool staticSetupDone;
		private static string[] infoBoxTextRows;
		private static GUIContent[] infoBoxContentRows;
		private static string[] prefabInfoBoxTextRows;
		private static GUIContent[] prefabInfoBoxContentRows;
		private static Texture folderIcon;

		private float infoBoxAppendHeight = 110f;

		/// <inheritdoc/>
		public override float HeaderHeight
		{
			get
			{
				return base.HeaderHeight + infoBoxAppendHeight;
			}
		}
				
		/// <inheritdoc/>
		protected override bool ShouldIncludeAddComponentButton()
		{
			return false;
		}

		/// <inheritdoc/>
		protected override bool AllowAddingOrRemovingComponents()
		{
			return false;
		}

		/// <inheritdoc/>
		public override bool MemberIsReorderable(IReorderable member)
		{
			return false;
		}

		/// <inheritdoc/>
		public override void Setup([NotNull]GameObject[] setTargets, [CanBeNull] IParentDrawer setParent, [NotNull] IInspector setInspector)
		{
			if(!staticSetupDone)
			{
				StaticSetup();
			}

			HierarchyFolderPreferences.Get().onPreferencesChanged += OnHierarchyFolderPreferencesChanged;

			base.Setup(setTargets, setParent, setInspector);
		}

		private void StaticSetup()
		{
			if(folderIcon == null)
			{
				var iconSizeWas = EditorGUIUtility.GetIconSize();
				EditorGUIUtility.SetIconSize(new Vector2(30f, 30f));
				var iconContent = EditorGUIUtility.IconContent("Folder Icon");
				folderIcon = iconContent?.image;
				EditorGUIUtility.SetIconSize(iconSizeWas);
			}

			var infoText =   HierarchyFolderPreferences.Get().infoBoxText;
			infoBoxTextRows = infoText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
			int rowCount = infoBoxTextRows.Length;
			infoBoxContent.text = infoText;
			infoBoxContentRows = new GUIContent[rowCount];
			for(int n = rowCount - 1; n >= 0; n--)
			{
				string text = infoBoxTextRows[n];
				infoBoxContentRows[n] = new GUIContent(text);
			}

			infoText = HierarchyFolderPreferences.Get().prefabInfoBoxText;
			prefabInfoBoxTextRows = infoText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
			rowCount = prefabInfoBoxTextRows.Length;
			prefabInfoBoxContent.text = infoText;
			prefabInfoBoxContentRows = new GUIContent[rowCount];
			for(int n = rowCount - 1; n >= 0; n--)
			{
				string text = prefabInfoBoxTextRows[n];
				prefabInfoBoxContentRows[n] = new GUIContent(text);
			}

			staticSetupDone = true;
		}

		private void OnHierarchyFolderPreferencesChanged(HierarchyFolderPreferences preferences)
		{
			StaticSetup();
		}

		/// <inheritdoc/>
		public override bool DrawPrefix(Rect position)
		{
			var hideTagRect = TagFieldPosition;
			hideTagRect.height += 2f;
			
			//block inputs to open tag popup
			GUI.Button(hideTagRect, GUIContent.none, InspectorPreferences.Styles.Blank);

			if(Event.current.type != EventType.Repaint)
            {
				return base.DrawPrefix(position);
			}

			bool dirty = base.DrawPrefix(position);

			var infoBoxRect = position;
			infoBoxRect.width -= 20f;
			var helpBoxStyle = InspectorPreferences.Styles.HelpBox;

			var gameObject = targets.Length == 0 ? null : targets[0];
			bool isPrefab, isPrefabAsset;
			if(gameObject == null)
            {
				isPrefab = false;
				isPrefabAsset = false;
            }
			else
            {
				if(gameObject.IsPrefabAsset())
                {
					isPrefabAsset = true;
					isPrefab = true;

                    for(int i = targets.Length - 1; i >= 0; i--)
                    {
						var target = targets[i];
						var hierarchyFolder = target.GetComponent<HierarchyFolder>();
						if(hierarchyFolder == null)
						{
							Inspector.RebuildDrawers(true);
							ExitGUIUtility.ExitGUI();
							return false;
						}

						if(HierarchyFolderUtility.HasSupernumeraryComponents(hierarchyFolder))
                        {
							HierarchyFolderUtility.UnmakeHierarchyFolder(hierarchyFolder.gameObject, hierarchyFolder);
							continue;
						}

						HierarchyFolderUtility.ResetTransformStateWithoutAffectingChildren(hierarchyFolder.transform, false);
					}
                }
				else if(gameObject.IsPrefabAssetOrOpenInPrefabStage())
                {
					isPrefabAsset = false;
					isPrefab = true;
				}
				else
                {
					isPrefabAsset = false;
					isPrefab = false;
				}
			}

			GUIContent[] infoRows;
			GUIContent infoContent;
			if(isPrefab)
			{
				infoRows = prefabInfoBoxContentRows;
				infoContent = prefabInfoBoxContent;
			}
			else
			{
				infoRows = infoBoxContentRows;
				infoContent = infoBoxContent;
			}

			float infoBoxHeight = 0f;
			int rowCount = infoRows.Length;
			for(int n = rowCount - 1; n >= 0; n--)
			{
				var rowContent = infoRows[n];
				if(rowContent.text.Length == 0)
				{
					infoBoxHeight += 5f;
				}
				else
				{
					infoBoxHeight += helpBoxStyle.CalcHeight(rowContent, infoBoxRect.width);
				}
			}

			infoBoxAppendHeight = infoBoxHeight + 18f;

			infoBoxRect.y += position.height - infoBoxAppendHeight + 12f;
			
			if(isPrefabAsset)
			{
				infoBoxRect.y -= GameObjectHeaderDrawer.OpenInPrefabModeButtonHeight;
			}

			infoBoxRect.height = infoBoxHeight;
			infoBoxRect.x += 10f;

			#if UNITY_2020_1_OR_NEWER
			infoBoxRect.y += 5f;
			#endif

			GUI.Label(infoBoxRect, infoContent.text, helpBoxStyle);

			position.height -= infoBoxAppendHeight;

			Color backgroundColor = inspector.Preferences.theme.AssetHeaderBackground;
			Color guiColorWas = GUI.color;

			GUI.color = Color.white;
			EditorGUI.DrawRect(hideTagRect, backgroundColor);
			GUI.color = guiColorWas;

			var folderIconRect = position;
			folderIconRect.x += 7f;
			folderIconRect.y += 7f;
			folderIconRect.width = 30f;
			folderIconRect.height = 30f;

			var folderIconBackgroundRect = folderIconRect;
			folderIconBackgroundRect.x = 0f;
			folderIconBackgroundRect.width = 40f;
			folderIconBackgroundRect.height = 40f;

			GUI.color = Color.white;
			EditorGUI.DrawRect(folderIconBackgroundRect, backgroundColor);
			GUI.color = guiColorWas;

			GUI.DrawTexture(folderIconRect, folderIcon);

			var bottomColorFixRect = position;
			bottomColorFixRect.y += EditorGUIDrawer.GameObjectTitlebarHeight(false);
			bottomColorFixRect.height = 5f;

			GUI.color = Color.white;
			EditorGUI.DrawRect(bottomColorFixRect, backgroundColor);
			GUI.color = guiColorWas;

			var subtitleRect = hideTagRect;
			subtitleRect.height = DrawGUI.SingleLineHeight;
			#if UNITY_2019_3_OR_NEWER
			subtitleRect.x = position.x + 43f;
			#else
			subtitleRect.x = position.x + 41f;
			#endif
			subtitleRect.width = 110f;
			GUI.Label(subtitleRect, "Hierarchy Folder", InspectorPreferences.Styles.SubHeader);

			return dirty;
		}

		/// <inheritdoc cref="IDrawer.OnClick" />
		public override bool OnClick(Event inputEvent)
		{
			if(mouseoveredPart == GameObjectHeaderPart.TagField)
			{
				inputEvent.Use();
				return true;
			}
			return base.OnClick(inputEvent);
		}

		/// <inheritdoc cref="IDrawer.OnRightClick" />
		public override bool OnRightClick(Event inputEvent)
		{
			if(mouseoveredPart == GameObjectHeaderPart.TagField)
			{
				inputEvent.Use();
				return true;
			}
			return base.OnRightClick(inputEvent);
		}

		/// <inheritdoc cref="IDrawer.OnKeyboardInputGiven" />
		public override bool OnKeyboardInputGiven(Event inputEvent, KeyConfigs keys)
		{
			if(selectedPart == GameObjectHeaderPart.TagField)
			{
				inputEvent.Use();
				return true;
			}
			return base.OnKeyboardInputGiven(inputEvent, keys);
		}

		/// <inheritdoc/>
		public override void Dispose()
		{
			HierarchyFolderPreferences.Get().onPreferencesChanged -= OnHierarchyFolderPreferencesChanged;
			base.Dispose();
		}
	}
}
#endif