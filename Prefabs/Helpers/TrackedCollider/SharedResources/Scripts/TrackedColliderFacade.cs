namespace VRTK.Prefabs.Helpers.TrackedCollider
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.MemberChangeMethod;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the TrackedCollider prefab.
    /// </summary>
    public class TrackedColliderFacade : MonoBehaviour
    {
        #region Tracking Settings
        /// <summary>
        /// The source to track.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Tracking Settings"), DocumentedByXml]
        public GameObject Source { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public TrackedColliderConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Snaps the tracked collider directly to the source current position.
        /// </summary>
        public virtual void SnapToSource()
        {
            Configuration.SnapToSource();
        }

        /// <summary>
        /// Called after <see cref="Source"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Source))]
        protected virtual void OnAfterSourceChange()
        {
            Configuration.SetSource(Source);
        }
    }
}