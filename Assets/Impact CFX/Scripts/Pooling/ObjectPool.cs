using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ImpactCFX.Pooling
{
    /// <summary>
    /// Base for an object pool.
    /// </summary>
    /// <typeparam name="T">The type of object to be pooled.</typeparam>
    public abstract class ObjectPool<T> : MonoBehaviour where T : IPoolable
    {
        /// <summary>
        /// The unique identifier for the pool
        /// </summary>
        public int PoolID { get; private set; }

        /// <summary>
        /// The number of objects in the pool.
        /// </summary>
        public int PoolSize { get; private set; }

        /// <summary>
        /// How objects should be "stolen" if there are no available objects in the pool.
        /// </summary>
        public ObjectPoolStealing Stealing { get; private set; }

        protected T[] pooledObjects;
        protected NativeArray<PooledObjectData> pooledObjectData;

        private void Awake()
        {
            SceneManager.sceneLoaded += sceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= sceneLoaded;

            if (pooledObjectData.IsCreated)
                pooledObjectData.Dispose();
        }

        /// <summary>
        /// Initializes the pool.
        /// </summary>
        /// <param name="poolID">A unique identifier for the pool.</param>
        /// <param name="objectPoolConfig">Size and stealing configuration.</param>
        public void Initialize(int poolID, ObjectPoolConfig objectPoolConfig)
        {
            Initialize(poolID, objectPoolConfig.PoolSize, objectPoolConfig.Stealing);
        }

        /// <summary>
        /// Initializes the pool. 
        /// </summary>
        /// <param name="poolID">A unique identifier for the pool.</param>
        /// <param name="poolSize">The number of objects to create for the pool.</param>
        /// <param name="stealing">How objects are "stolen" if there are no available objects in the pool.</param>
        public virtual void Initialize(int poolID, int poolSize, ObjectPoolStealing stealing)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_REGISTER, $"Initializing pool '{name}' with ID {poolID} with {poolSize} objects.", true);
#endif
            PoolID = poolID;
            PoolSize = poolSize;
            Stealing = stealing;

            pooledObjects = new T[poolSize];

            for (int i = 0; i < pooledObjects.Length; i++)
            {
                pooledObjects[i] = createPooledObjectInstance(i);
            }

            pooledObjectData = new NativeArray<PooledObjectData>(poolSize, Allocator.Persistent);
        }

        /// <summary>
        /// Creates a new instance of a pooled object.
        /// </summary>
        /// <param name="index">The index of the object in the pool's array.</param>
        protected abstract T createPooledObjectInstance(int index);

        /// <summary>
        /// Retrieves the object at the given index from the pool.
        /// </summary>
        /// <param name="index">The index of the object to retrieve.</param>
        /// <param name="priority">The new priority of the object.</param>
        /// <param name="contactPointID">The new contact point ID this object will be associated with.</param>
        public virtual T RetrieveObject(int index, float priority, long contactPointID)
        {
            T a = pooledObjects[index];
            a.RetrieveFromPool(priority, contactPointID);

            return a;
        }

        /// <summary>
        /// Returns all of this pool's objects to the pool.
        /// </summary>
        public void ReturnAllObjectsToPool()
        {
            foreach (var item in pooledObjects)
            {
                item.ReturnToPool();
            }
        }

        /// <summary>
        /// Completely disposes and destroys this pool and all of all pooled data and objects
        /// </summary>
        public void Destroy()
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogClear($"Destroying pool '{name}' with ID {PoolID}.");
#endif
            foreach (var item in pooledObjects)
            {
                if (item != null && !item.Equals(null))
                    item.Destroy();
            }

            Destroy(this.gameObject);
        }

        public void UpdatePooledObjects()
        {
            for (int i = 0; i < PoolSize; i++)
            {
                T obj = pooledObjects[i];

                if (obj.NeedsUpdate)
                    obj.UpdatePooledObject();

                pooledObjectData[i] = new PooledObjectData()
                {
                    IsAvailable = obj.IsAvailable(),
                    Priority = obj.Priority,
                    LastRetrievedFrame = obj.LastRetrievedFrame,
                    ContactPointID = obj.ContactPointID,
                    LastRequestIndex = -1
                };
            }
        }

        /// <summary>
        /// Gets a pooled object data array in a form suitable for jobs.
        /// </summary>
        public NativeArray<PooledObjectData> GetPooledObjectDataArray()
        {
            return pooledObjectData;
        }

        /// <summary>
        /// Re-instantiates any objects that have been destroyed.
        /// This can happen if a pooled object is made a child of an object that gets destroyed on scene load/unload.
        /// </summary>
        public void ReinstantiateMissingObjects()
        {
            //Scan through pool to detect missing objects
            for (int i = 0; i < pooledObjects.Length; i++)
            {
                //Create new instances if needed
                //This can happen if a pooled object is made a child of an object that gets destroyed on scene load/unload.
                if (pooledObjects[i] == null || pooledObjects[i].Equals(null))
                {
                    pooledObjects[i] = createPooledObjectInstance(i);
                }
            }
        }

        private void sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            ReinstantiateMissingObjects();
        }
    }
}