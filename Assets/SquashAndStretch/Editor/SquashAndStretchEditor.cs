/******************************************************************************/
/*
  Project   - Squash And Stretch Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace SquashAndStretchKit
{
  [CustomEditor(typeof(SquashAndStretch))]
  [CanEditMultipleObjects]
  internal class SquashAndStretchEditor : Editor
  {
    SerializedProperty m_enableSquash;
    SerializedProperty m_maxSquash;
    SerializedProperty m_enableStretch;
    SerializedProperty m_maxStretch;
    SerializedProperty m_mode;
    SerializedProperty m_springHalfLife;
    SerializedProperty m_minSpeedThreshold;
    SerializedProperty m_maxSpeedThreshold;

    void OnEnable()
    {
      m_enableSquash = serializedObject.FindProperty("enableSquash");
      m_maxSquash = serializedObject.FindProperty("maxSquash");
      m_enableStretch = serializedObject.FindProperty("enableStretch");
      m_maxStretch = serializedObject.FindProperty("maxStretch");
      m_mode = serializedObject.FindProperty("mode");
      m_springHalfLife = serializedObject.FindProperty("springHalfLife");
      m_minSpeedThreshold = serializedObject.FindProperty("minSpeedThreshold");
      m_maxSpeedThreshold = serializedObject.FindProperty("maxSpeedThreshold");
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      EditorGUILayout.LabelField
      (
        new GUIContent() { text = $"Squash & Stretch Kit Version {SquashAndStretch.Version}" }, 
        new GUIStyle("label") { fontStyle = FontStyle.Bold }
      );
      EditorGUILayout.Space();

      bool hasRigidBody = false;
      foreach (UnityEngine.Object obj in serializedObject.targetObjects)
      {
        SquashAndStretch squashAndStretch = (SquashAndStretch) obj;
        if (obj == null)
          continue;

        if (squashAndStretch.GetComponent<Rigidbody>() == null && squashAndStretch.GetComponent<Rigidbody2D>() == null)
          continue;

        hasRigidBody = true;
        break;
      }

      if (hasRigidBody)
      {
        EditorGUILayout.LabelField("Rigid body must use immediate mode.");
      }
      else
      {
        GUIContent modeGuiContent =
          new GUIContent
          (
            "Mode",
              "Immediate:\n"
                + "Object and squash & stretch effect immediately reflect transform position.\n"
                + "Good for objects with mostly smooth position updates while immediate position reflection is desired, "
                + "such as physically simulated objects.\n\n"
            + "Spring:\n"
                + "Object springs behind transform position.\n"
                + "Rate at which spring effect wears off is governed by spring half life specified below.\n"
                + "Good for when objects stretching past original frontal boundary is undesired.\n\n"
            + "Smooth Immediate:\n"
                + "Object position works like immediate mode, "
                + "squash & stretch effect works like spring mode, "
                + "and squash & stretch direction is smoothed.\n"
                + "Good for objects with potentially jittery movement that could result in visually popping artifacts in immediate mode, "
                + "such as object directly controlled by screen-space mouse position where a one-pixel cursor delta can cause a 45-, 90-, 135-, or even 180- degree directional change."
          );
        m_mode.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup(modeGuiContent, (SquashAndStretch.Mode)m_mode.enumValueIndex));
      }

      EditorGUILayout.BeginHorizontal();
      {
        GUIContent squashGuiContent = new GUIContent
        (
          "Squash", 
            "Enable squash effect.\n\n" 
          + "Squash effect kicks in when object experiences sudden impacts.\n\n" 
          + "Rate at which squash effect wears off is governed by spring half life specified below.\n\n" 
          + "Effective max squash scale is maximum stretch scale reached before squash, or max squash scale specified on the right, whichever is lower."
        );
        m_enableSquash.boolValue  = EditorGUILayout.Toggle(squashGuiContent, m_enableSquash.boolValue);
        if (m_enableSquash.boolValue)
        {
          GUIContent maxSquashGuiContent = new GUIContent("  Max Scale", "Max squash scale in the direction perpendicular to the movement direction.");
          EditorGUILayout.LabelField(maxSquashGuiContent, GUILayout.Width(GUI.skin.label.CalcSize(maxSquashGuiContent).x));
          m_maxSquash.floatValue = EditorGUILayout.DelayedFloatField(m_maxSquash.floatValue);
          m_maxSquash.floatValue = Mathf.Max(1.0f, m_maxSquash.floatValue);
        }
      }
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      {
        GUIContent stretchGuiContent = new GUIContent
        (
          "Stretch", 
            "Enable stretch effect.\n\n" 
          + "Stretch effect happens when object moves fast enough (see min/max speed thresholds below)."
        );
        m_enableStretch.boolValue = EditorGUILayout.Toggle(stretchGuiContent, m_enableStretch.boolValue);
        if (m_enableStretch.boolValue)
        {
          GUIContent maxStretchGuiContent = new GUIContent("  Max Scale", "Max stretch scale in the direction parallel to the movement direction.");
          EditorGUILayout.LabelField(maxStretchGuiContent, GUILayout.Width(GUI.skin.label.CalcSize(maxStretchGuiContent).x));
          m_maxStretch.floatValue = EditorGUILayout.DelayedFloatField(m_maxStretch.floatValue);
          m_maxStretch.floatValue = Mathf.Max(1.0f, m_maxStretch.floatValue);
        }
      }
      EditorGUILayout.EndHorizontal();

      GUIContent springHalfLifeGuiContent = 
        new GUIContent
        (
          "Spring Half Life", 
            "Time in seconds for spring effect to reduce to 50%.\n\n" 
          + "Multiplying it by 10 gives roughly the time for spring effect to reduce to 0.1%.\n\n" 
          + "If you're ever lost, 0.015 is a good value to start tweaking and experimenting from."
        );
      m_springHalfLife.floatValue = EditorGUILayout.DelayedFloatField(springHalfLifeGuiContent, m_springHalfLife.floatValue);
      m_springHalfLife.floatValue = Math.Max(0.001f, m_springHalfLife.floatValue);

      GUIContent minSpeedThresholdGuiContent = 
        new GUIContent
        (
          "Min Speed Threshold", 
            "Object slower than this speed does not stretch.\n\n" 
          + "Speed unit is distance per second."
        );
      float newMinSpeedThreshold = EditorGUILayout.DelayedFloatField(minSpeedThresholdGuiContent, m_minSpeedThreshold.floatValue);
      if (m_minSpeedThreshold.floatValue != newMinSpeedThreshold)
      {
        m_minSpeedThreshold.floatValue = newMinSpeedThreshold;
        m_minSpeedThreshold.floatValue = Mathf.Max(0.0f, m_minSpeedThreshold.floatValue);
        m_maxSpeedThreshold.floatValue = Mathf.Max(m_minSpeedThreshold.floatValue, m_maxSpeedThreshold.floatValue);
        serializedObject.ApplyModifiedProperties();
      }

      GUIContent maxSpeedThresholdGuiContent = 
        new GUIContent
        (
          "Max Speed Threshold", 
            "Object faster than this speed stretches to max stretch scale.\n\n" 
          + "Speed unit is distance per second."
        );
      float newMaxSpeedThreshold = EditorGUILayout.DelayedFloatField(maxSpeedThresholdGuiContent, m_maxSpeedThreshold.floatValue);
      if (m_maxSpeedThreshold.floatValue != newMaxSpeedThreshold)
      {
        m_maxSpeedThreshold.floatValue = newMaxSpeedThreshold;
        m_maxSpeedThreshold.floatValue = Mathf.Max(0.0f, m_maxSpeedThreshold.floatValue);
        m_minSpeedThreshold.floatValue = Mathf.Min(m_minSpeedThreshold.floatValue, m_maxSpeedThreshold.floatValue);
      }

      serializedObject.ApplyModifiedProperties();
    }
  }
}

#endif
