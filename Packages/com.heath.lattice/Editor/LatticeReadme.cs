using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lattice.Editor
{
	public class LatticeReadme : ScriptableObject { }

	[CustomEditor(typeof(LatticeReadme))]
	public class LatticeReadmeEditor : UnityEditor.Editor
	{
		private static readonly string DocumentationPath = Path.Combine("Documentation", "lattice", "index.html");
		private const string DocumentationUrl = "https://harryheath.com/lattice/";

		public override void OnInspectorGUI()
		{
			Header1("Lattice Modifier for Unity");
			Header2("Documentation");
			Paragraph("Open the documentation using one of the following two sources:");

			using (new EditorGUILayout.HorizontalScope())
			{
				string readmePath = AssetDatabase.GetAssetPath(target);
				string packagePath = Path.GetDirectoryName(readmePath);
				string relativePath = Path.Combine(packagePath, DocumentationPath);
				string fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), relativePath);

				Paragraph("Local: ");
				Link(relativePath, fullPath);

				if (!File.Exists(fullPath))
				{
					Paragraph("(Missing, have you moved the README?)");
				}

				GUILayout.FlexibleSpace();

			}

			using (new EditorGUILayout.HorizontalScope())
			{
				Paragraph("Online: ");
				Link(DocumentationUrl, DocumentationUrl);
				GUILayout.FlexibleSpace();
			}
		}

		#region Utility

		private static void Header1(string text)
		{
			EditorGUILayout.Space();
			GUILayout.Label(text, Styles.Header1);
			EditorGUILayout.Space();
		}

		private static void Header2(string text)
		{
			EditorGUILayout.Space();
			GUILayout.Label(text, Styles.Header2);
			EditorGUILayout.Space();
		}

		private static void Paragraph(string text)
		{
			GUILayout.Label(text, Styles.Paragraph);
		}

		private static void Link(string text, string url)
		{
			GUIContent content = new(text);
			Rect position = GUILayoutUtility.GetRect(content, Styles.Link);

			using (new Handles.DrawingScope(Styles.Link.normal.textColor))
				Handles.DrawLine(
					new Vector3(position.xMin + Styles.Link.padding.left, position.yMax), 
					new Vector3(position.xMax - Styles.Link.padding.right, position.yMax)
				);

			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

			if (GUI.Button(position, content, Styles.Link))
			{
				Application.OpenURL(url);
			}
		}

		private static class Styles
		{
			private static GUIStyle _header1;
			private static GUIStyle _header2;
			private static GUIStyle _paragraph;
			private static GUIStyle _link;

			public static GUIStyle Header1 => _header1 ??= new(EditorStyles.boldLabel)
			{
				fontSize = 24
			};

			public static GUIStyle Header2 => _header2 ??= new(EditorStyles.boldLabel)
			{
				fontSize = 18
			};

			public static GUIStyle Paragraph => _paragraph ??= new(EditorStyles.label)
			{
				fontSize = 14
			};

			public static GUIStyle Link => _link ??= new(EditorStyles.label)
			{
				fontSize = 14,
				normal = new()
				{
					textColor = EditorStyles.linkLabel.normal.textColor
				}
			};
		}

		#endregion
	}
}
