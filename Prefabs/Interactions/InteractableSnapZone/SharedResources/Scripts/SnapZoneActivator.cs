namespace VRTK.Prefabs.Interactions.InteractableSnapZone
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Collision;

    public class SnapZoneActivator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public SnapZoneFacade Facade { get; protected set; }
        /// <summary>
        /// The <see cref="CollisionTracker"/> used for tracking collisions on the zone.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public CollisionTracker CollisionTracker { get; protected set; }
        /// <summary>
        /// The <see cref="Collider"/> that is used to determine the collidable area for the zone.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Collider Collider { get; set; }
        #endregion
    }
}