using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace ImpactCFX.Pooling
{
    /// <summary>
    /// General job for getting objects from an object pool.
    /// </summary>
    /// <typeparam name="TRequest">The type of the requests being used.</typeparam>
    [BurstCompile]
    public struct ObjectPoolJob<TRequest> : IJob where TRequest : struct, IObjectPoolRequest
    {
        /// <summary>
        /// The ID of the template object this job corresponds to.
        /// </summary>
        public int TemplateID;

        /// <summary>
        /// How objects should be "stolen" if there are no available objects in the pool.
        /// </summary>
        public ObjectPoolStealing Stealing;

        /// <summary>
        /// Array of all pooled object data.
        /// </summary>
        public NativeArray<PooledObjectData> PooledObjects;

        /// <summary>
        /// Array of all pooled object requests.
        /// </summary>
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<TRequest> ObjectRequests;

        /// <summary>
        /// The number of requests.
        /// </summary>
        [ReadOnly]
        public NativeReference<int> ObjectRequestCount;

        /// <summary>
        /// The current frame used for age-based stealing.
        /// </summary>
        public int CurrentFrame;

        public void Execute()
        {
            int lastAvailableIndex = 0;
            int count = ObjectRequestCount.Value;

            for (int i = 0; i < count; i++)
            {
                TRequest request = ObjectRequests[i];
                //Skip invalid requests or requests that don't match our template ID
                if (!request.IsObjectPoolRequestValid || request.TemplateID != TemplateID)
                    continue;

                //Reset object index to help determine if an object is found
                request.ObjectIndex = -1;
                int poolSize = PooledObjects.Length;

                long contactPointID = request.ContactPointID;
                bool checkContactPointID = request.CheckContactPointID;

                float lowestPriority = request.Priority;
                int lowestPriorityIndex = -1;

                int oldestIndex = 0;
                float oldestTime = float.MaxValue;

                int checkedIndices = 0;
                int currentIndex = lastAvailableIndex;

                bool stolen = false;

                //Loop over all objects in the pool
                while (checkedIndices < poolSize)
                {
                    PooledObjectData obj = PooledObjects[currentIndex];

                    //Check if contact point ID matches request contact point ID
                    //This overrides everything else
                    if (checkContactPointID && obj.ContactPointID == contactPointID)
                    {
                        request.ObjectIndex = currentIndex;
                        request.IsUpdate = true;
                        break;
                    }

                    //Check if object is available
                    if (obj.IsAvailable)
                    {
                        request.ObjectIndex = currentIndex;
                        lastAvailableIndex = currentIndex;

                        //If we're not checking for a matching contact point ID, break immediately
                        if (!checkContactPointID)
                            break;
                    }

                    //Update oldest known object
                    if (obj.LastRetrievedFrame < oldestTime)
                    {
                        oldestTime = obj.LastRetrievedFrame;
                        oldestIndex = currentIndex;
                    }

                    //Update lowest priority object
                    if (obj.Priority < lowestPriority)
                    {
                        lowestPriority = obj.Priority;
                        lowestPriorityIndex = currentIndex;
                    }

                    currentIndex = (currentIndex + 1) % poolSize;
                    checkedIndices++;
                }

                //If no matching or available object was found, get one based on the stealing settings
                if (request.ObjectIndex == -1)
                {
                    if (lowestPriorityIndex > -1 && Stealing == ObjectPoolStealing.LowerPriority)
                    {
                        request.ObjectIndex = lowestPriorityIndex;
                        stolen = true;
                    }
                    else if (oldestIndex > -1 && Stealing == ObjectPoolStealing.Oldest)
                    {
                        request.ObjectIndex = oldestIndex;
                        stolen = true;
                    }
                    else
                    {
#if IMPACTCFX_DEBUG
                        ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_OBJECTPOOL, $"[{request.GetType().Name.ToUpper()}] Cannot find any available object.", false);
#endif
                    }
                }

                //If a result was found, update the object's data
                if (request.ObjectIndex > -1)
                {
                    PooledObjectData obj = PooledObjects[request.ObjectIndex];

                    obj.IsAvailable = false;
                    obj.Priority = request.Priority;
                    obj.LastRetrievedFrame = CurrentFrame;

                    //If this object was given to a previous request, invalidate that request since we have "stolen" it from them
                    if (stolen && obj.LastRequestIndex > -1)
                    {
                        TRequest lastRequest = ObjectRequests[obj.LastRequestIndex];
                        lastRequest.ObjectIndex = -1;
                        ObjectRequests[obj.LastRequestIndex] = lastRequest;
                    }

                    obj.LastRequestIndex = i;

                    //Apply object data changes
                    PooledObjects[request.ObjectIndex] = obj;
                }

                //Apply request changes
                ObjectRequests[i] = request;
            }
        }
    }
}