/******************************************************************************/
/*
  Project   - Squash And Stretch Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SquashAndStretchKit
{
  public class SquashAndStretch : MonoBehaviour
  {
    public static readonly int MajorVersion = 1;
    public static readonly int MinorVersion = 1;
    public static readonly int Revision     = 2;
    public static string Version { get { return MajorVersion + "." + MinorVersion + "." + Revision; } }


    // properties
    //-------------------------------------------------------------------------

    public enum Mode
    {
      Immediate,
      Spring,
      SmoothImmediate
    }

    public bool enableSquash = false;
    public float maxSquash = 1.2f;

    public bool enableStretch = true;
    public float maxStretch = 1.8f;

    public Mode mode = Mode.Immediate;

    public float springHalfLife = 0.01f;

    public float minSpeedThreshold = 5.0f;
    public float maxSpeedThreshold = 20.0f;

    internal bool m_hasRigidBody = false;

    internal void EnforcePropertyLimits()
    {
      if (m_hasRigidBody)
        mode = Mode.Immediate;

      springHalfLife = Mathf.Max(0.001f, springHalfLife);

      minSpeedThreshold = Mathf.Max(0.0f, minSpeedThreshold);
      minSpeedThreshold = Mathf.Min(minSpeedThreshold, maxSpeedThreshold);

      maxSpeedThreshold = Mathf.Max(0.0f, maxSpeedThreshold);
      maxSpeedThreshold = Mathf.Max(minSpeedThreshold, maxSpeedThreshold);

      maxSquash = Mathf.Max(1.0f, maxSquash);
      maxStretch = Mathf.Max(1.0f, maxStretch);
    }

    //-------------------------------------------------------------------------
    // end: properties


    // internals
    //-------------------------------------------------------------------------

    internal const float kEpsilonSpeed = 1.0e-3f;
    internal const float kEpsilonTime = 1.0e-6f;
    internal const float kSmallHalfLife = 5.0e-3f;

    internal Vector3 m_objPositionWs;
    internal Vector3 m_objPositionLs;
    private Transform m_objParent;

    internal GameObject m_forwardRotationParent;
    internal GameObject m_inverseRotationParent;

    private SquashAndStretchModeLogic m_modeLogic;
    private bool m_waitForPostPositionUpdate = true;
    private bool m_waitForPrePositionUpdate = false;

    internal Vector3 m_rigidBodyVelocity;

    //-------------------------------------------------------------------------
    // end: internals


    // privates
    //-------------------------------------------------------------------------

    private bool HandleModeChange()
    {
      switch (mode)
      {
        case Mode.Immediate:
          if (!(m_modeLogic is SquashAndStretchImmediateModeLogic))
          {
            m_modeLogic = new SquashAndStretchImmediateModeLogic();
            return true;
          }
          break;

        case Mode.Spring:
          if (!(m_modeLogic is SquashSndStretchSpringModeLogic))
          {
            m_modeLogic = new SquashSndStretchSpringModeLogic();
            return true;
          }
          break;

        case Mode.SmoothImmediate:
          if (!(m_modeLogic is SquashSndStretchSmoothImmediateModeLogic))
          {
            m_modeLogic = new SquashSndStretchSmoothImmediateModeLogic();
            return true;
          }
          break;
      }

      return false;
    }

    //-------------------------------------------------------------------------
    // end: privates


    // utility
    //-------------------------------------------------------------------------

    // reboot when you want to restart effect from scratch
    public void Reboot()
    {
      m_modeLogic.Reboot(this);
    }

    private void Reset(bool resetParent)
    {
      if (resetParent)
        transform.SetParent(m_objParent, true);

      m_objPositionWs = transform.position;
      m_objPositionLs = transform.localPosition;

      m_forwardRotationParent.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
      m_forwardRotationParent.transform.localRotation = Quaternion.identity;
      m_inverseRotationParent.transform.localRotation = Quaternion.identity;

      m_waitForPostPositionUpdate = true;
      m_waitForPrePositionUpdate = false;

      Reboot();
    }

    private bool DeltaTimeTooTiny()
    {
      return Time.deltaTime < kEpsilonTime;
    }

    //-------------------------------------------------------------------------
    // utility


    // editor accessibility
    //-------------------------------------------------------------------------

    internal void HandleSelection()
    {
#if UNITY_EDITOR
      if (!Selection.Contains(m_forwardRotationParent) && !Selection.Contains(m_inverseRotationParent))
        return;

      UnityEngine.Object[] aObject = Selection.objects;
      for (int i = 0; i < aObject.Length; ++i)
      {
        GameObject go = (GameObject) aObject[i];
        if (go != m_forwardRotationParent && go != m_inverseRotationParent)
          continue;

        aObject[i] = aObject[aObject.Length - 1];
        Array.Resize(ref aObject, aObject.Length - 1);
      }

      if (!Selection.Contains(gameObject))
      {
        Array.Resize(ref aObject, aObject.Length + 1);
        aObject[aObject.Length - 1] = gameObject;
      }

      Selection.objects = aObject;
#endif
    }

    //-------------------------------------------------------------------------
    // end: editor accessibility


    // game events
    //-------------------------------------------------------------------------

    private void Start()
    {
      m_objParent = transform.parent;

      SquashAndStretchManager.Add(this);
    }

    private void OnDestroy()
    {
      Destroy(m_forwardRotationParent);
      Destroy(m_inverseRotationParent);

      m_forwardRotationParent = null;
      m_inverseRotationParent = null;

      SquashAndStretchManager.Remove(this);
    }

    private void OnEnable()
    {
      SquashAndStretchManager.AddActive(this);

      if (m_forwardRotationParent == null)
      {
        m_forwardRotationParent = new GameObject();
        m_forwardRotationParent.name = gameObject.name + " (Squash And Stretch outer parent)";
      }

      if (m_inverseRotationParent == null)
      {
        m_inverseRotationParent = new GameObject();
        m_inverseRotationParent.name = gameObject.name + " (Squash And Stretch inner parent)";
        m_inverseRotationParent.transform.SetParent(m_forwardRotationParent.transform, false);
      }

      HandleModeChange();
      Reset(false);

#if UNITY_EDITOR
      SceneView.duringSceneGui += OnScene;
#endif
    }

    private void OnDisable()
    {
      SquashAndStretchManager.RemoveActive(this);

#if UNITY_EDITOR
      SceneView.duringSceneGui -= OnScene;
#endif
    }

    private void FixedUpdate()
    {
      // for physics objects so this is called once before physics simulation step
      /*
      if (m_hasRigidBody)
        PrePositionUpdate(true);
      */
    }

    private void Update()
    {
      

      /*
      // for regular (non-physics) objects so we don't miss any FixedUpdate() calls in a frame
      // but we DO NEED to make sure this component updates before everything else
      // that modifies position when updated, via Edit > Project Settings > Script Execution Order
      if (!m_hasRigidBody)
        PrePositionUpdate(false);
      */
    }

    void LateUpdate()
    {
      //PostPositionUpdate();
    }

    //-------------------------------------------------------------------------
    // end: game events


    // core logic
    //-------------------------------------------------------------------------

    private bool m_wasMovingInEditor = false;
    private bool m_isMovingInEditor = false;
    private Vector3 m_posWsPreMoveInEditor = Vector3.zero;
    private Vector3 m_posLsPreMoveInEditor = Vector3.zero;
    private Vector3 m_moveCenterWsInEditor = Vector3.zero;
    private Vector3 m_movedDeltaWsInEditor = Vector3.zero;

    public void PreUpdate(bool isFixedUpdate)
    {
      if (m_waitForPostPositionUpdate)
        return;

      if (isFixedUpdate != m_hasRigidBody)
        return;

      bool shouldAdjustToMoveInEditor = false;
#if UNITY_EDITOR
      // handle in-editor mouse drags
      // doesn't work quite right
      if ((transform.localPosition - m_posLsPreMoveInEditor).sqrMagnitude > 1.0e-3f)
      {
        m_movedDeltaWsInEditor = transform.position - m_posWsPreMoveInEditor;
        shouldAdjustToMoveInEditor = true;
      }
#endif

      // restore parent
      transform.SetParent((m_objParent != null) ? m_objParent.transform : null, false);
      transform.localPosition = m_objPositionLs;

      if (shouldAdjustToMoveInEditor)
      {
        transform.position = m_moveCenterWsInEditor + m_movedDeltaWsInEditor;
      }

      m_waitForPostPositionUpdate = true;
      m_waitForPrePositionUpdate = false;
    }

    public void PostUpdate()
    {
      if (m_waitForPrePositionUpdate)
        return;

      EnforcePropertyLimits();

      m_hasRigidBody = false;
      Rigidbody rb = GetComponent<Rigidbody>();
      Rigidbody2D rb2 = GetComponent<Rigidbody2D>();
      if (rb != null)
      {
        m_hasRigidBody = true;
        m_rigidBodyVelocity = rb.velocity;
      }
      else if (rb2 != null)
      {
        m_hasRigidBody = true;
        m_rigidBodyVelocity = rb2.velocity;
      }

      // save position
      m_objPositionWs = transform.position;
      m_objPositionLs = transform.localPosition;
      if (transform.parent != m_inverseRotationParent.transform)
        m_objParent = transform.parent;

      if (HandleModeChange() || (!enableSquash && !enableStretch) || DeltaTimeTooTiny())
      {
        Reset(true);
        return;
      }

      // mode logic
      Vector3 position;
      Vector3 stretchAxis;
      float stretch;
      m_modeLogic.Update(this, out position, out stretchAxis, out stretch);

      // set up parents
      Quaternion stretchRot = (m_objParent != null ? Quaternion.Inverse(m_objParent.transform.rotation) : Quaternion.identity);
      Quaternion forwardRotation = Quaternion.FromToRotation(Vector3.right, stretchRot * stretchAxis);
      m_forwardRotationParent.transform.SetParent(m_objParent, false);
      m_forwardRotationParent.transform.localPosition = m_objPositionLs;
      transform.SetParent(m_inverseRotationParent.transform, false);
      transform.localPosition = Vector3.zero;
      m_inverseRotationParent.transform.localRotation = Quaternion.Inverse(forwardRotation);
      m_forwardRotationParent.transform.localRotation = forwardRotation;
      m_forwardRotationParent.transform.position = position;

      // apply squash & stretch
      float offAxisScale = 1.0f / Mathf.Sqrt(stretch);
      m_forwardRotationParent.transform.localScale = new Vector3(stretch, offAxisScale, offAxisScale);

      m_waitForPostPositionUpdate = false;
      m_waitForPrePositionUpdate = true;

      m_posWsPreMoveInEditor = transform.position;
      m_posLsPreMoveInEditor = transform.localPosition;

      if (m_isMovingInEditor && !m_wasMovingInEditor)
      {
        m_moveCenterWsInEditor = transform.position;
      }
      m_wasMovingInEditor = m_isMovingInEditor;
    }

#if UNITY_EDITOR
    private void OnScene(SceneView view)
    {
      var e = Event.current;

      switch (e.type)
      {
      case EventType.MouseDown:
        m_moveCenterWsInEditor = transform.position;
        m_isMovingInEditor = true;
        break;
      case EventType.MouseUp:
        m_isMovingInEditor = false;
        break;
      }
    }
#endif

    //-------------------------------------------------------------------------
    // end: core logic
  }
}
