using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomEditor(typeof(ImpactObjectChild))]
    [CanEditMultipleObjects]
    public class ImpactObjectChildEditor : ImpactObjectSingleMaterialEditor
    {
        protected override void drawAdditionalProperties()
        {
            base.drawAdditionalProperties();

            ImpactObjectChild r = target as ImpactObjectChild;
            ImpactObjectBase parent = r.transform.parent.GetComponentInParent<ImpactObjectBase>();

            if (parent != null)
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField(new GUIContent("Parent"), parent, typeof(ImpactObjectBase), true);
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.HelpBox("No Impact Object was found as a parent of this object!", MessageType.Error);
            }
        }
    }
}