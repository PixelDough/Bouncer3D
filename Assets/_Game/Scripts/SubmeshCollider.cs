using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PixelDough.Bouncer
{
    public class SubmeshCollider : MonoBehaviour
    {
        [SerializeField] private MeshCollider meshCollider;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private int[] submeshIndices;
        [SerializeField, ReadOnly] private Mesh generatedMesh;

#if UNITY_EDITOR
        [Button("Generate Mesh")]
        private void GenerateMesh()
        {
            if (meshFilter == null || meshCollider == null)
            {
                Debug.LogWarning("MeshFilter or MeshCollider is not assigned.");
                return;
            }

            Mesh originalMesh = meshFilter.sharedMesh;
            if (originalMesh == null)
            {
                Debug.LogWarning("MeshFilter does not have a mesh assigned.");
                return;
            }

            if (submeshIndices == null || submeshIndices.Length == 0)
            {
                Debug.LogWarning("No submesh indices specified.");
                return;
            }

            // Clean up the previous generated mesh
            if (generatedMesh != null)
            {
                string oldAssetPath = AssetDatabase.GetAssetPath(generatedMesh);
                if (!string.IsNullOrEmpty(oldAssetPath))
                {
                    AssetDatabase.DeleteAsset(oldAssetPath);
                }
                else
                {
                    DestroyImmediate(generatedMesh);
                }
                generatedMesh = null;
            }

            // Create a new mesh
            generatedMesh = new Mesh();
            generatedMesh.name = originalMesh.name + "_SubmeshCollider";

            // Copy vertex data
            generatedMesh.vertices = originalMesh.vertices;
            generatedMesh.normals = originalMesh.normals;
            generatedMesh.tangents = originalMesh.tangents;
            generatedMesh.uv = originalMesh.uv;
            generatedMesh.uv2 = originalMesh.uv2;
            generatedMesh.colors = originalMesh.colors;

            // Collect triangles from specified submeshes
            List<int> triangles = new List<int>();
            foreach (int submeshIndex in submeshIndices)
            {
                if (submeshIndex < 0 || submeshIndex >= originalMesh.subMeshCount)
                {
                    Debug.LogWarning($"Invalid submesh index: {submeshIndex}");
                    continue;
                }
                triangles.AddRange(originalMesh.GetTriangles(submeshIndex));
            }

            // Assign triangles and recalculate bounds
            generatedMesh.triangles = triangles.ToArray();
            generatedMesh.RecalculateBounds();

            // Ensure the directory exists
            string assetDirectory = "Assets/SubmeshGenerator/";
            if (!AssetDatabase.IsValidFolder(assetDirectory))
            {
                AssetDatabase.CreateFolder("Assets", "SubmeshGenerator");
            }

            // Create unique asset name
            string submeshIndicesString = string.Join("_", submeshIndices);
            string assetName = $"{originalMesh.name}_SubmeshCollider_{submeshIndicesString}.asset";
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(assetDirectory + assetName);

            // Save the mesh as an asset
            AssetDatabase.CreateAsset(generatedMesh, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Assign the new mesh to the MeshCollider and store reference
            meshCollider.sharedMesh = generatedMesh;

            // Mark the scriptable object as dirty so changes are saved
            EditorUtility.SetDirty(this);
        }
#endif
    }
}