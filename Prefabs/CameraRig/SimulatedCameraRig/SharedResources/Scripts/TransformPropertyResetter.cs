namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;

    /// <summary>
    /// Resets the saved properties of a given transform.
    /// </summary>
    public class TransformPropertyResetter : MonoBehaviour
    {
        /// <summary>
        /// The source to cache and reset.
        /// </summary>
        [Tooltip("The source to cache and reset.")]
        public Transform source;

        /// <summary>
        /// The initial local position of the <see cref="source"/>.
        /// </summary>
        protected Vector3 initialLocalPosition;
        /// <summary>
        /// The initial local rotation of the <see cref="source"/>.
        /// </summary>
        protected Quaternion initialLocalRotation;
        /// <summary>
        /// The initial local scale of the <see cref="source"/>.
        /// </summary>
        protected Vector3 initialLocalScale;
        /// <summary>
        /// Whether the initial states have been set.
        /// </summary>
        protected bool initialSet;

        /// <summary>
        /// Resets to the cached properties.
        /// </summary>
        public virtual void ResetProperties()
        {
            if (initialSet && source != null)
            {
                source.localPosition = initialLocalPosition;
                source.localRotation = initialLocalRotation;
                source.localScale = initialLocalScale;
            }
        }

        protected virtual void Awake()
        {
            if (source != null)
            {
                initialLocalPosition = source.localPosition;
                initialLocalRotation = source.localRotation;
                initialLocalScale = source.localScale;
                initialSet = true;
            }
        }
    }
}