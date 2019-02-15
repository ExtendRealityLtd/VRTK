namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;

    /// <summary>
    /// Denotes a world axis for the drive to operate on.
    /// </summary>
    public static class DriveAxis
    {
        /// <summary>
        /// The axis to operate the drive on.
        /// </summary>
        public enum Axis
        {
            /// <summary>
            /// The world space X Axis.
            /// </summary>
            XAxis,
            /// <summary>
            /// The world space Y Axis.
            /// </summary>
            YAxis,
            /// <summary>
            /// The world space Z Axis.
            /// </summary>
            ZAxis
        }

        /// <summary>
        /// Gets the axis direction for the given <see cref="DriveAxis"/>.
        /// </summary>
        /// <param name="axis">The desired world axis.</param>
        /// <param name="negativeDirection">Whether to get the negative axis direction.</param>
        /// <returns>The direction of the drive axis.</returns>
        public static Vector3 GetAxisDirection(this Axis axis, bool negativeDirection = false)
        {
            Vector3 axisDirection = Vector3.zero;
            switch (axis)
            {
                case Axis.XAxis:
                    axisDirection = negativeDirection ? Vector3.left : Vector3.right;
                    break;
                case Axis.YAxis:
                    axisDirection = negativeDirection ? Vector3.down : Vector3.up;
                    break;
                case Axis.ZAxis:
                    axisDirection = negativeDirection ? Vector3.back : Vector3.forward;
                    break;
            }
            return axisDirection;
        }
    }
}