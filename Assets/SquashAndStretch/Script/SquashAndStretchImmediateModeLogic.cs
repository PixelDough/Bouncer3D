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

namespace SquashAndStretchKit
{
  internal class SquashAndStretchImmediateModeLogic : SquashAndStretchModeLogic
  {
    private Vector3 m_prevPosition;
    private Vector3 m_velocityDir;
    private SpringVectorTracker m_velocityDirTracker;
    private SpringFloatTracker m_stretchTracker;
    private SpringFloatTracker m_squashTracker;

    internal SquashAndStretchImmediateModeLogic()
    {
      m_prevPosition = Vector3.zero;
      m_velocityDir = Vector3.up;
      m_velocityDirTracker.Reset(m_velocityDir, Vector3.zero);
      m_stretchTracker.Reset(1.0f, 0.0f);
      m_squashTracker.Reset(1.0f, 0.0f);
    }

    internal override void Reboot(SquashAndStretch squashAndStretch)
    {
      m_prevPosition = squashAndStretch.transform.position;
      m_velocityDir = Vector3.up;
      m_velocityDirTracker.Reset(m_velocityDir, Vector3.zero);
      m_stretchTracker.Reset(1.0f, 0.0f);
      m_squashTracker.Reset(1.0f, 0.0f);
    }

    internal override void Update
    (
      SquashAndStretch squashAndStretch,
      out Vector3 position,
      out Vector3 stretchAxis,
      out float stretch
    )
    {
      float dt = Time.deltaTime;

      Vector3 targetVelocityDir = m_velocityDir;
      float targetSquash = 1.0f;
      float stretchFromSpeed = 1.0f;
      Vector3 velocity = Vector3.zero;
      Vector3 positionDelta =
        squashAndStretch.m_hasRigidBody 
          ? squashAndStretch.m_rigidBodyVelocity * dt 
          : squashAndStretch.m_objPositionWs - m_prevPosition;
      if (positionDelta.magnitude > SquashAndStretch.kEpsilonSpeed)
      {
        targetVelocityDir = positionDelta.normalized;
        velocity = positionDelta / dt;

        float speed = velocity.magnitude;

        // squash
        if (squashAndStretch.enableSquash)
        {
          float deltaDirDot = Vector3.Dot(positionDelta.normalized, m_velocityDir);
          if (deltaDirDot <= 0.0f && speed > squashAndStretch.minSpeedThreshold)
          {
            float maxSquash = squashAndStretch.maxSquash;

            if (m_stretchTracker.value > 1.0f && squashAndStretch.enableStretch)
              maxSquash = Mathf.Min(maxSquash, m_stretchTracker.value);

            targetSquash = 1.0f - (maxSquash - 1.0f) * deltaDirDot;
          }
        }

        // stretch
        if (squashAndStretch.enableStretch)
        {
          float speedThresholdDelta = squashAndStretch.maxSpeedThreshold - squashAndStretch.minSpeedThreshold;
          if (speedThresholdDelta > SquashAndStretch.kEpsilonSpeed)
          {
            float t = Mathf.Clamp((speed - squashAndStretch.minSpeedThreshold) / speedThresholdDelta, 0.0f, 1.0f);
            stretchFromSpeed = Mathf.Lerp(1.0f, squashAndStretch.maxStretch, t);
          }
        }
      }

      m_velocityDirTracker.TrackCriticallyDamped
      (
        targetVelocityDir,
        SquashAndStretch.kSmallHalfLife,
        dt,
        false, 
        false
      );

      if (m_velocityDirTracker.value.magnitude > SquashAndStretch.kEpsilonSpeed)
        m_velocityDir = m_velocityDirTracker.value.normalized;

      m_squashTracker.TrackCriticallyDamped(2.0f - stretchFromSpeed, squashAndStretch.springHalfLife, dt);
      if (targetSquash > 1.0f)
        m_squashTracker.Reset(targetSquash, 0.0f);

      float targetStretch =
        m_squashTracker.value > 1.0f + SquashAndStretch.kEpsilonSpeed
        ? 1.0f / (m_squashTracker.value * m_squashTracker.value)
        : stretchFromSpeed;

      m_stretchTracker.TrackCriticallyDamped(targetStretch, SquashAndStretch.kSmallHalfLife, dt);

      position = squashAndStretch.m_objPositionWs;
      stretchAxis = m_velocityDir;
      stretch = m_stretchTracker.value;

      m_prevPosition = position;
    }

    internal override void Move(Vector3 offset)
    {
      m_prevPosition += offset;
    }
  }
}
