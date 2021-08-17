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
  internal struct SpringVectorTracker
  {
    internal enum Phase
    {
      RampUp,
      Overshoot,
      Recover,
    };

    private const float kEpsilonDist = 1.0e-4f;
    private const float kEpsilonSpeed = 1.0e-3f;

    internal Vector3 value;
    internal Vector3 velocity;
    internal Phase phase;
    internal float overshootSpeed;
    internal float recoveryDelta;
    internal Vector3 rampUpDir;
    internal bool rampUpClammpedUp;

    internal void Reset()
    {
      value = Vector3.zero;
      velocity = Vector3.zero;
      phase = Phase.RampUp;
    }

    internal void Reset(Vector3 initValue)
    {
      value = initValue;
      velocity = Vector3.zero;
      phase = Phase.RampUp;
    }

    internal void Reset(Vector3 initValue, Vector3 initVelocity)
    {
      value = initValue;
      velocity = initVelocity;
      phase = Phase.RampUp;
    }

    private Vector3 Track(Vector3 targetValue, float omega, float zeta, float dt, bool allowOvershoot, bool immediateMode)
    {
      Vector3 delta = targetValue - value;
      Vector3 deltaDir = delta.normalized;
      float deltaMag = delta.magnitude;

      float f = 1.0f + 2.0f * dt * zeta * omega;
      float oo = omega * omega;
      float hoo = dt * oo;
      float hhoo = dt * hoo;
      float detInv = 1.0f / (f + hhoo);
      Vector3 detX = f * value + dt * velocity + hhoo * targetValue;
      Vector3 detV = velocity + hoo * delta;

      Vector3 prevVelocity = velocity;
      float prevSpeed = prevVelocity.magnitude;
      velocity = detV * detInv;
      value = detX * detInv;
      float currSpeed = velocity.magnitude;

      rampUpClammpedUp = false;

      if (!allowOvershoot)
      {
        if (currSpeed > kEpsilonSpeed)
        {
          rampUpDir = velocity.normalized;
        }
        phase = Phase.RampUp;
        return value;
      }

      if (deltaMag > kEpsilonDist || currSpeed > kEpsilonSpeed)
      {
        float speedDot = Vector3.Dot(velocity, deltaDir);
        switch (phase)
        {
          case Phase.RampUp:
            if (currSpeed < prevSpeed)
            {
              if (speedDot > 0.0f && !immediateMode)
              {
                velocity *= prevSpeed / currSpeed;
              }
              else
              {
                if (immediateMode)
                {
                  velocity *= prevSpeed / currSpeed;
                  value = targetValue;
                }

                phase = Phase.Overshoot;
                overshootSpeed = currSpeed;
              }
            }

            if (currSpeed > kEpsilonSpeed)
            {
              rampUpDir = velocity.normalized;
            }
            break;

          case Phase.Overshoot:
            if (speedDot > 0.0f)
            {
              phase = Phase.Recover;
              recoveryDelta = deltaMag;
              break;
            }
            float overshootMaxSpeed = overshootSpeed * Mathf.Exp(-2.0f);
            if (speedDot > overshootMaxSpeed * 1.2f)
            {
              phase = Phase.RampUp;
            }
            break;

          case Phase.Recover:
            if (deltaMag > recoveryDelta)
            {
              phase = Phase.RampUp;
            }
            break;
        }
      }
      else
      {
        phase = Phase.RampUp;
      }

      return value;
    }

    internal Vector3 TrackCriticallyDamped(Vector3 targetValue, float halfLife, float deltaTime, bool allowOvershoot, bool immediateMode)
    {
      float omega = Mathf.Log(2.0f) / halfLife;
      float zeta = 1.0f;
      return Track(targetValue, omega, zeta, deltaTime, allowOvershoot, immediateMode);
    }
  }

  internal struct SpringFloatTracker
  {
    private const float kEpsilonDist = 1.0e-4f;
    private const float kEpsilonSpeed = 1.0e-3f;
    private const float kEpsilonTime = 1.0e-2f;

    internal float value;
    internal float velocity;

    internal void Reset()
    {
      value = 0.0f;
      velocity = 0.0f;
    }

    internal void Reset(float initValue)
    {
      value = initValue;
      velocity = 0.0f;
    }

    internal void Reset(float initValue, float initVelocity)
    {
      value = initValue;
      velocity = initVelocity;
    }

    private float Track(float targetValue, float omega, float zeta, float deltaTime)
    {
      float delta = targetValue - value;

      float f = 1.0f + 2.0f * deltaTime * zeta * omega;
      float oo = omega * omega;
      float hoo = deltaTime * oo;
      float hhoo = deltaTime * hoo;
      float detInv = 1.0f / (f + hhoo);
      float detX = f * value + deltaTime * velocity + hhoo * targetValue;
      float detV = velocity + hoo * delta;

      velocity = detV * detInv;
      value = detX * detInv;

      return value;
    }

    internal float TrackCriticallyDamped(float targetValue, float halfLife, float deltaTime)
    {
      float omega = Mathf.Log(2.0f) / halfLife;
      float zeta = 1.0f;
      return Track(targetValue, omega, zeta, deltaTime);
    }
  }
}
