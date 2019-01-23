namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Zinnia.Data.Type;

    /// <summary>
    /// The basis to drive a control in a rotational angle.
    /// </summary>
    public abstract class RotationalDrive : Drive<RotationalDriveFacade, RotationalDrive>
    {
        /// <summary>
        /// The total number of degrees in a circle.
        /// </summary>
        protected const float circleDegrees = 360f;
        /// <summary>

        /// The angle range that defines the upper right quadrant of a circle.
        /// </summary>
        protected static readonly FloatRange circleUpperRightQuadrant = new FloatRange(-1f, 90f);
        /// <summary>
        /// The angle range that defines the upper left quadrant of a circle.
        /// </summary>
        protected static readonly FloatRange circleUpperLeftQuadrant = new FloatRange(270f, 360f);

        /// <summary>
        /// The representation of the rotation in meaningful values and not limited to 0f to 360f.
        /// </summary>
        protected float pseudoRotation;
        /// <summary>
        /// The multiplier used to determine how many complete revolutions the drive has performed.
        /// </summary>
        protected float rotationMultiplier;
        /// <summary>
        /// The previous actual rotational value of the drive.
        /// </summary>
        protected Vector3 previousActualRotation = Vector3.one * float.MaxValue;
        /// <summary>
        /// The current actual rotational value of the drive.
        /// </summary>
        protected Vector3 currentActualRotation = Vector3.zero;

        /// <summary>
        /// Calculates the limits of the drive.
        /// </summary>
        /// <param name="newLimit">The new limits the drive can reach.</param>
        /// <returns>The minimum and maximum local space limit the drive can reach.</returns>
        public abstract FloatRange CalculateDriveLimits(FloatRange newLimit);
        /// <summary>
        /// Calculates the location of the rotational hinge for the drive.
        /// </summary>
        /// <param name="newHingeLocation">The new local space for the hinge point.</param>
        public abstract void CalculateHingeLocation(Vector3 newHingeLocation);

        /// <inheritdoc />
        protected override FloatRange CalculateDriveLimits(RotationalDriveFacade facade)
        {
            return CalculateDriveLimits(facade.DriveLimit);
        }

        /// <inheritdoc />
        protected override float CalculateValue(DriveAxis.Axis driveAxis, FloatRange limits)
        {
            if (!isActiveAndEnabled)
            {
                return 0f;
            }

            return Mathf.Clamp(pseudoRotation, limits.minimum, limits.maximum);
        }
    }
}