namespace VRTK.Prefabs.Locomotion.Movement.AxesToVector3
{
    using UnityEngine;

    /// <summary>
    /// Caches <see cref="Vector3"/> data and emits an appropriate event when the cache is updated.
    /// </summary>
    public class Vector3Cache : MonoBehaviour
    {
        /// <summary>
        /// Emitted when the cached data is updated and has been modified to a new value.
        /// </summary>
        public AxesToVector3Facade.UnityEvent Modified = new AxesToVector3Facade.UnityEvent();
        /// <summary>
        /// Emitted when the cached data is updated but the value is unmodified.
        /// </summary>
        public AxesToVector3Facade.UnityEvent Unmodified = new AxesToVector3Facade.UnityEvent();

        /// <summary>
        /// The cached data.
        /// </summary>
        public Vector3 CachedData
        {
            get;
            protected set;
        }

        public virtual void CacheData(Vector3 data)
        {
            if (data == CachedData)
            {
                Unmodified?.Invoke(data);
            }
            else
            {
                Modified?.Invoke(data);
            }
            CachedData = data;
        }
    }
}