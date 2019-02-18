namespace VRTK.Prefabs.Helpers.TrackedCollider
{
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the TrackedCollider prefab.
    /// </summary>
    public class TrackedColliderFacade : MonoBehaviour
    {
        #region Tracking Settings
        [Header("Tracking Settings"), Tooltip("The source to track."), SerializeField]
        private GameObject source;
        /// <summary>
        /// The source to track.
        /// </summary>
        public GameObject Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                internalSetup.SetSource(source);
            }
        }
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected TrackedColliderInternalSetup internalSetup;
        #endregion

        /// <summary>
        /// Snaps the tracked collider directly to the source current position.
        /// </summary>
        public virtual void SnapToSource()
        {
            internalSetup.SnapToSource();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            internalSetup.SetSource(Source);
        }
    }
}