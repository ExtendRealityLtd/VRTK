namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.MemberChangeMethod;

    /// <summary>
    /// Resets the saved properties of a given transform.
    /// </summary>
    public class TransformPropertyResetter : MonoBehaviour
    {
        /// <summary>
        /// The source to cache and reset.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Source { get; set; }

        /// <summary>
        /// The initial local position of the <see cref="Source"/>.
        /// </summary>
        protected Vector3? initialLocalPosition;
        /// <summary>
        /// The initial local rotation of the <see cref="Source"/>.
        /// </summary>
        protected Quaternion initialLocalRotation;
        /// <summary>
        /// The initial local scale of the <see cref="Source"/>.
        /// </summary>
        protected Vector3 initialLocalScale;

        /// <summary>
        /// Resets to the cached properties.
        /// </summary>
        public virtual void ResetProperties()
        {
            if (Source == null || initialLocalPosition == null)
            {
                return;
            }
            Source.transform.localPosition = (Vector3)initialLocalPosition;
            Source.transform.localRotation = initialLocalRotation;
            Source.transform.localScale = initialLocalScale;
        }

        protected virtual void Awake()
        {
            CacheSourceTransformData();
        }

        /// <summary>
        /// Caches the initial state of the <see cref="Source"/> transform data.
        /// </summary>
        protected virtual void CacheSourceTransformData()
        {
            if (Source == null)
            {
                initialLocalPosition = null;
                return;
            }

            initialLocalPosition = Source.transform.localPosition;
            initialLocalRotation = Source.transform.localRotation;
            initialLocalScale = Source.transform.localScale;
        }

        /// <summary>
        /// Called after <see cref="Source"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Source))]
        protected virtual void OnAfterSourceChange()
        {
            CacheSourceTransformData();
        }
    }
}