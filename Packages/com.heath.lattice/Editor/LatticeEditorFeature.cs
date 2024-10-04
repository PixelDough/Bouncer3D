using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lattice.Editor
{
	public static class LatticeEditorFeature
	{
		private static bool _initialised = false;
		private static int _waitAttempts = 10;

		/// <summary>
		/// Sets up the modifiers as part of the editor loop.
		/// This is called after assembly reloads.
		/// </summary>
		[InitializeOnLoadMethod]
		private static void Initialise()
		{
			if (_initialised) return;
			if (!WaitForAssetsLoaded()) return;

			LatticeFeature.Initialise();

			EditorApplication.quitting += Cleanup;
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
			EditorSceneManager.sceneSaved += OnSceneSaved;
			EditorSceneManager.sceneSaving += OnSceneSaving;

			_initialised = true;
		}

		/// <summary>
		/// Cleans up all related systems.
		/// This is called when the editor closes.
		/// </summary>
		private static void Cleanup()
		{
			if (!_initialised) return;

			LatticeFeature.Cleanup();

			EditorApplication.quitting -= Cleanup;
			AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
			EditorSceneManager.sceneSaved -= OnSceneSaved;
			EditorSceneManager.sceneSaving -= OnSceneSaving;

			_initialised = false;
		}

		/// <summary>
		/// Calls initialise several times to wait for resources to load
		/// </summary>
		private static bool WaitForAssetsLoaded()
		{
			EditorApplication.delayCall -= Initialise;

			// Loaded, return true
			if (Resources.Load(LatticeFeature.ComputeShaderName) != null) return true;

			// Decrement attempt counter
			_waitAttempts -= 1;

			if (_waitAttempts >= 0)
			{
				// Try again next editor tick
				EditorApplication.delayCall += Initialise;
			}
			else
			{
				// After several attempts log error
				Debug.LogError($"Could not load lattice compute. Make sure it's within a Resources folder and called {LatticeFeature.ComputeShaderName}");
			}
			return false;
		}

		/// <summary>
		/// Called before assembly reloads. Cleans up and disables <see cref="LatticeFeature"/>.
		/// </summary>
		private static void OnBeforeAssemblyReload()
		{
			LatticeFeature.Cleanup();
		}

		/// <summary>
		/// Changes all meshes to lattice version after saving.
		/// </summary>
		private static void OnSceneSaved(Scene scene)
		{
			ApplyMeshes();
		}

		/// <summary>
		/// Changes all meshes to non-lattice version before saving.
		/// </summary>
		private static void OnSceneSaving(Scene scene, string path)
		{
			ResetMeshes();
		}

		/// <summary>
		/// Resets meshes back to non lattice version.
		/// </summary>
		private static void ResetMeshes()
		{
#pragma warning disable 0618
			LatticeModifierBase[] components = Object.FindObjectsOfType<LatticeModifierBase>();
#pragma warning restore 0618

			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].isActiveAndEnabled) components[i].ResetMesh();
			}
		}

		/// <summary>
		/// Applies lattice meshes.
		/// </summary>
		private static void ApplyMeshes()
		{
#pragma warning disable 0618
			LatticeModifierBase[] components = Object.FindObjectsOfType<LatticeModifierBase>();
#pragma warning restore 0618

			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].isActiveAndEnabled) components[i].ApplyMesh();
			}
		}
	}
}
