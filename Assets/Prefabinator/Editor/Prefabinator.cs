using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public static class Prefabinator
    {
        [MenuItem("GameObject/Prefabinator/Prefabinate!", false, 0)] 
        static void Init() 
        {
            foreach (var t in Selection.GetTransforms(SelectionMode.TopLevel))
            {
                int siblingIndex = t.GetSiblingIndex();
                GameObject newObj = new GameObject(t.gameObject.name);
                Undo.RegisterCreatedObjectUndo(newObj, "Prefabinated");
                newObj.transform.SetParent(t, false);
                newObj.transform.SetParent(t.parent, true);
                newObj.transform.SetSiblingIndex(siblingIndex);

                Undo.SetTransformParent(t, newObj.transform, true, "Prefabinated");
            }
        }
    }
}
