namespace VRTK.Prefabs.Locomotion.Movement.AxesToVector3
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;

    /// <summary>
    /// Caches <see cref="Vector3"/> data and emits an appropriate event when the cache is updated.
    /// </summary>
    public class Vector3Cache : MonoBehaviour
    {
        /// <summary>
        /// Emitted when the cached data is updated and has been modified to a new value.
        /// </summary>
        [DocumentedByXml]
        public AxesToVector3Facade.UnityEvent Modified = new AxesToVector3Facade.UnityEvent();
        /// <summary>
        /// Emitted when the cached data is updated but the value is unmodified.
        /// </summary>
        [DocumentedByXml]
        public AxesToVector3Facade.UnityEvent Unmodified = new AxesToVector3Facade.UnityEvent();

        /// <summary>
        /// The cached data. Emits an event when the data changes based on whether the cache has changed from it's previous value.
        /// </summary>
        public Vector3 CachedData
        {
            get
            {
                return cachedData;
            }
            set
            {
                if (value == cachedData)
                {
                    Unmodified?.Invoke(value);
                }
                else
                {
                    Modified?.Invoke(value);
                }
                cachedData = value;
            }
        }
        private Vector3 cachedData;
    }
}