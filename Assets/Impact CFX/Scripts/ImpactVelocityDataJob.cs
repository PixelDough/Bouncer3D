using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ImpactCFX
{
    [BurstCompile]
    public struct ImpactVelocityDataJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<CollisionInputData> CollisionData;
        [ReadOnly]
        public NativeArray<RigidbodyData> RigidbodyData;

        public NativeArray<ImpactVelocityData> Results;

        public void Execute(int index)
        {
            CollisionInputData collisionData = CollisionData[index];
            CollisionVelocityMethod collisionVelocityMethod = collisionData.CollisionVelocityMethod;

            float3 finalVelocity = float3.zero;

            if (collisionVelocityMethod == CollisionVelocityMethod.CollisionMessage)
            {
                finalVelocity = collisionData.CollisionMessageVelocity;
            }
            else
            {
                int index1 = index * 2;
                RigidbodyData r1 = RigidbodyData[index1];
                RigidbodyStateData state1Previous = r1.PreviousState;

                int index2 = index * 2 + 1;
                RigidbodyData r2 = RigidbodyData[index2];
                RigidbodyStateData state2 = r2.PreviousState;

                float3 contactPointTangentialVelocity1 = PhysicsUtility.CalculateTangentialVelocity(collisionData.Point, state1Previous.AngularVelocity, state1Previous.CenterOfMass);
                float3 contactPointTotalVelocity1 = state1Previous.LinearVelocity + contactPointTangentialVelocity1;

                if (collisionVelocityMethod == CollisionVelocityMethod.RelativeVelocities)
                {
                    float3 contactPointTangentialVelocity2 = PhysicsUtility.CalculateTangentialVelocity(collisionData.Point, state2.AngularVelocity, state2.CenterOfMass);
                    float3 contactPointTotalVelocity2 = state2.LinearVelocity + contactPointTangentialVelocity2;

                    if (collisionData.CollisionType == CollisionType.Collision || collisionData.CollisionType == CollisionType.Slide)
                    {
                        finalVelocity = contactPointTotalVelocity1 - contactPointTotalVelocity2;
                    }
                    else if (collisionData.CollisionType == CollisionType.Roll)
                    {
                        finalVelocity = contactPointTotalVelocity1 - contactPointTotalVelocity2;
                        float roll = 1 - math.clamp(math.length(finalVelocity) * 0.1f, 0, 1);
                        finalVelocity = contactPointTangentialVelocity1 * roll;
                    }
                }
                else
                {
                    RigidbodyStateData state1Current = r1.CurrentState;

                    float3 contactPointTangentialVelocity3 = PhysicsUtility.CalculateTangentialVelocity(collisionData.Point, state1Current.AngularVelocity, state1Current.CenterOfMass);
                    float3 contactPointTotalVelocity3 = state1Current.LinearVelocity + contactPointTangentialVelocity3;

                    finalVelocity = contactPointTotalVelocity3 - contactPointTotalVelocity1;
                }

            }

            ImpactVelocityData impactVelocity = new ImpactVelocityData()
            {
                ImpactVelocity = finalVelocity
            };

            Results[index] = impactVelocity;
        }
    }
}