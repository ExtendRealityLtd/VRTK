namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Zinnia.Data.Type;

    /// <summary>
    /// The basis to drive a control in a linear direction.
    /// </summary>
    public abstract class DirectionalDrive : Drive<DirectionalDriveFacade, DirectionalDrive>
    {
        /// <summary>
        /// Calculates the limits of the drive.
        /// </summary>
        /// <param name="newLimit">The maximum local space limit the drive can reach.</param>
        /// <returns>The minimum and maximum local space limit the drive can reach.</returns>
        public abstract FloatRange CalculateDriveLimits(float newLimit);

        /// <inheritdoc />
        protected override FloatRange CalculateDriveLimits(DirectionalDriveFacade facade)
        {
            return CalculateDriveLimits(facade.DriveLimit);
        }
    }
}