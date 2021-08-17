/******************************************************************************/
/*
  Project   - Squash And Stretch Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace SquashAndStretchKit
{
  internal class SquashAndStretchManager : MonoBehaviour
  {
    private static GameObject s_manager;
    private static HashSet<SquashAndStretch> s_allComponents = new HashSet<SquashAndStretch>();
    private static HashSet<SquashAndStretch> s_activeComponents = new HashSet<SquashAndStretch>();

    private static void ValidateInstance()
    {
      if (s_activeComponents.Count == 0)
        return;

      if (s_manager)
        return;

      s_manager = new GameObject("Squash & Stretch Kit manager (don't delete)");
      s_manager.AddComponent<SquashAndStretchPreUpdatePump>();
      s_manager.AddComponent<SquashAndStretchPostUpdatePump>();

      DontDestroyOnLoad(s_manager);
    }

    private static void TryCleanUpInstance()
    {
      if (s_activeComponents.Count > 0)
        return;

      if (!s_manager)
        return;

      Destroy(s_manager);
      s_manager = null;
    }

    public static void Add(SquashAndStretch comp)
    {
      s_allComponents.Add(comp);
    }

    public static void Remove(SquashAndStretch comp)
    {
      s_allComponents.Remove(comp);
    }

    public static void AddActive(SquashAndStretch comp)
    {
      s_activeComponents.Add(comp);
      ValidateInstance();
    }

    public static void RemoveActive(SquashAndStretch comp)
    {
      s_activeComponents.Remove(comp);
      TryCleanUpInstance();
    }

    public static void HandleSelection()
    {
      foreach (var comp in s_allComponents)
        comp.HandleSelection();
    }

    public static void PreUpdate(bool isFixedUpdate)
    {
      foreach (var comp in s_activeComponents)
        comp.PreUpdate(isFixedUpdate);
    }

    public static void PostUpdate()
    {
      foreach (var comp in s_activeComponents)
        comp.PostUpdate();
    }
  }
}

