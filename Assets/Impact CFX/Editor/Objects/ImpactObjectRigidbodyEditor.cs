using UnityEditor;

namespace ImpactCFX.EditorScripts
{
    [CustomEditor(typeof(ImpactObjectRigidbody))]
    [CanEditMultipleObjects]
    public class ImpactObjectRigidbodyEditor : ImpactObjectSingleMaterialEditor
    {

    }

    [CustomEditor(typeof(ImpactObjectRigidbodyCheap))]
    [CanEditMultipleObjects]
    public class ImpactObjectRigidbodyCheapEditor : ImpactObjectSingleMaterialEditor
    {

    }
}