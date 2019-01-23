namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into any DirectionalDrive prefab.
    /// </summary>
    public class DirectionalDriveFacade : DriveFacade<DirectionalDrive, DirectionalDriveFacade>
    {
        #region Limit Settings
        [Header("Limit Settings"), Tooltip("The world space limit of the drive direction along the specified axis."), SerializeField]
        private float _driveLimit = 1f;
        /// <summary>
        /// The world space limit of the drive direction along the specified axis.
        /// </summary>
        public float DriveLimit
        {
            get { return _driveLimit; }
            set
            {
                _driveLimit = value;
                drive.CalculateDriveLimits(_driveLimit);
            }
        }
        #endregion

        #region Gizmo Settings
        /// <summary>
        /// The size of the gizmo cube to draw at the limits of the drive.
        /// </summary>
        [Header("Gizmo Settings"), Tooltip("The size of the gizmo cube to draw at the limits of the drive."), InternalSetting, SerializeField]
        protected Vector3 gizmoCubeSize = Vector3.one * 0.015f;
        #endregion

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 origin = Vector3.zero;
            Vector3 direction = (DriveAxis.GetAxisDirection(true) * (DriveLimit * 0.5f));
            Vector3 from = origin - direction;
            Vector3 to = origin + direction;
            Gizmos.DrawLine(from, to);
            Gizmos.DrawCube(from, gizmoCubeSize);
            Gizmos.DrawCube(to, gizmoCubeSize);
        }
    }
}