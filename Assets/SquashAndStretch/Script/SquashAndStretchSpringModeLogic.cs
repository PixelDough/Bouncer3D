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
  internal class SquashSndStretchSpringModeLogic : SquashAndStretchModeLogic
  {
    private Vector3 m_velocityDir;
    private SpringVectorTracker m_positionTracker;
    private SpringVectorTracker m_centroidTracker;
    private bool m_wasRampingUp;
    private float m_maxStretch;

    internal SquashSndStretchSpringModeLogic()
    {
      m_velocityDir = Vector3.up;
      m_positionTracker.Reset(Vector3.zero, Vector3.zero);
      m_centroidTracker.Reset(Vector3.zero, Vector3.zero);
      m_wasRampingUp = false;
      m_maxStretch = 1.0f;
    }

    internal override void Reboot(SquashAndStretch squashAndStretch)
    {
      m_velocityDir = Vector3.up;
      m_positionTracker.Reset(squashAndStretch.transform.position, Vector3.zero);
      m_centroidTracker.Reset(squashAndStretch.transform.position, Vector3.zero);
      m_wasRampingUp = false;
      m_maxStretch = 1.0f;
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

      m_positionTracker.TrackCriticallyDamped
      (
        squashAndStretch.m_objPositionWs,
        squashAndStretch.springHalfLife,
        dt,
        squashAndStretch.enableSquash, 
        false
      );

      float targetStretch = 1.0f;
      float trackSpeed = m_positionTracker.velocity.magnitude;
      float maxSquashFromStretch = squashAndStretch.enableStretch ? m_maxStretch : squashAndStretch.maxSquash;
      switch (m_positionTracker.phase)
      {
        case SpringVectorTracker.Phase.RampUp:
          {
            if (!m_wasRampingUp)
              m_maxStretch = 1.0f;

            if (squashAndStretch.enableStretch)
            {
              float speedThresholdDelta = squashAndStretch.maxSpeedThreshold - squashAndStretch.minSpeedThreshold;
              float t = Mathf.Max(0.0f, trackSpeed - squashAndStretch.minSpeedThreshold) / speedThresholdDelta;

              targetStretch =
                speedThresholdDelta > SquashAndStretch.kEpsilonSpeed
                ? Mathf.Lerp(1.0f, squashAndStretch.maxStretch, t)
                : 1.0f;
            }

            m_wasRampingUp = true;
            m_maxStretch = Mathf.Max(m_maxStretch, targetStretch);

            if (m_positionTracker.velocity.magnitude > SquashAndStretch.kEpsilonSpeed)
              m_velocityDir = m_positionTracker.velocity.normalized;
          }
          break;

        case SpringVectorTracker.Phase.Overshoot:
          {
            if (squashAndStretch.enableSquash)
            {
              float t = trackSpeed / Mathf.Max(SquashAndStretch.kEpsilonSpeed, m_positionTracker.overshootSpeed);

              targetStretch = 1.0f / Mathf.Min(squashAndStretch.maxSquash, maxSquashFromStretch);
              targetStretch *= targetStretch;
              targetStretch = Mathf.Lerp(targetStretch, maxSquashFromStretch, t);
            }
            m_wasRampingUp = false;
          }
          break;

        case SpringVectorTracker.Phase.Recover:
          {
            if (squashAndStretch.enableSquash)
            {
              float t =
                (m_positionTracker.value - squashAndStretch.m_objPositionWs).magnitude
                  / Mathf.Max(SquashAndStretch.kEpsilonSpeed, m_positionTracker.recoveryDelta);

              targetStretch = 1.0f / Mathf.Lerp(1.0f, Mathf.Min(squashAndStretch.maxSquash, maxSquashFromStretch), t);
              targetStretch *= targetStretch;
            }
            m_wasRampingUp = false;
          }
          break;
      }

      Vector3 targetCentroid =
        (m_positionTracker.phase == SpringVectorTracker.Phase.RampUp)
        ? 0.5f * (m_positionTracker.value + squashAndStretch.m_objPositionWs)
        : squashAndStretch.m_objPositionWs;

      position = 
        m_centroidTracker.TrackCriticallyDamped
        (
          targetCentroid,
          squashAndStretch.springHalfLife,
          dt,
          false, 
          false
        );

      stretchAxis = m_velocityDir;
      stretch = targetStretch;
    }

    internal override void Move(Vector3 offset)
    {
      m_centroidTracker.value += offset;
      m_positionTracker.value += offset;
    }
  }
}
