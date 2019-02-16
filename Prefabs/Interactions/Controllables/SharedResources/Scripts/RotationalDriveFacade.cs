namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into any RotationalDrive prefab.
    /// </summary>
    public class RotationalDriveFacade : DriveFacade<RotationalDrive, RotationalDriveFacade>
    {
        #region Limit Settings
        [Header("Limit Settings"), Tooltip("The rotational angle limits for the drive."), SerializeField]
        private FloatRange _driveLimit = new FloatRange(-180f, 180f);
        /// <summary>
        /// The rotational angle limits for the drive.
        /// </summary>
        public FloatRange DriveLimit
        {
            get { return _driveLimit; }
            set
            {
                _driveLimit = value;
            }
        }
        #endregion

        #region Hinge Settings
        [Header("Hinge Settings"), Tooltip("The location of the hinge within the local position of the drive."), SerializeField]
        private Vector3 _hingeLocation = Vector3.zero;
        /// <summary>
        /// The location of the hinge within the local position of the drive.
        /// </summary>
        public Vector3 HingeLocation
        {
            get { return _hingeLocation; }
            set
            {
                _hingeLocation = value;
                drive.CalculateHingeLocation(_hingeLocation);
            }
        }
        #endregion

        #region Gizmo Settings
        /// <summary>
        /// The distance of the gizmo hinge location line.
        /// </summary>
        [Header("Gizmo Settings"), Tooltip("The distance of the gizmo hinge location line."), InternalSetting, SerializeField]
        protected float gizmoLineDistance = 0.2f;
        /// <summary>
        /// The radius of the gizmo hinge location end sphere.
        /// </summary>
        [Tooltip("The radius of the gizmo hinge location end sphere."), InternalSetting, SerializeField]
        protected float gizmoSphereRadius = 0.015f;
        #endregion

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 origin = HingeLocation;
            Vector3 direction = (DriveAxis.GetAxisDirection(true) * (gizmoLineDistance * 0.5f));
            Vector3 from = origin - direction;
            Vector3 to = origin + direction;
            Gizmos.DrawLine(from, to);
            Gizmos.DrawSphere(from, gizmoSphereRadius);
            Gizmos.DrawSphere(to, gizmoSphereRadius);
        }
    }
}