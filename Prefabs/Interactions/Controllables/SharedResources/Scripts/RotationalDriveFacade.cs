namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Malimbe.MemberChangeMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into any RotationalDrive prefab.
    /// </summary>
    public class RotationalDriveFacade : DriveFacade<RotationalDrive, RotationalDriveFacade>
    {
        #region Limit Settings
        /// <summary>
        /// The rotational angle limits for the drive.
        /// </summary>
        [Serialized]
        [field: Header("Limit Settings"), DocumentedByXml]
        public FloatRange DriveLimit { get; set; } = new FloatRange(-180f, 180f);
        #endregion

        #region Hinge Settings
        /// <summary>
        /// The location of the hinge within the local position of the drive.
        /// </summary>
        [Serialized]
        [field: Header("Hinge Settings"), DocumentedByXml]
        public Vector3 HingeLocation { get; set; }
        #endregion

        #region Gizmo Settings
        /// <summary>
        /// The distance of the gizmo hinge location line.
        /// </summary>
        [Serialized]
        [field: Header("Gizmo Settings"), DocumentedByXml, Restricted(RestrictedAttribute.Restrictions.Muted)]
        public float GizmoLineDistance { get; set; } = 0.2f;
        /// <summary>
        /// The radius of the gizmo hinge location end sphere.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted(RestrictedAttribute.Restrictions.Muted)]
        public float GizmoSphereRadius { get; set; } = 0.015f;
        #endregion

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 origin = HingeLocation;
            Vector3 direction = DriveAxis.GetAxisDirection(true) * (GizmoLineDistance * 0.5f);
            Vector3 from = origin - direction;
            Vector3 to = origin + direction;
            Gizmos.DrawLine(from, to);
            Gizmos.DrawSphere(from, GizmoSphereRadius);
            Gizmos.DrawSphere(to, GizmoSphereRadius);
        }

        /// <summary>
        /// Called after <see cref="HingeLocation"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(HingeLocation))]
        protected virtual void OnAfterHingeLocationChange()
        {
            Drive.SetUp();
        }
    }
}