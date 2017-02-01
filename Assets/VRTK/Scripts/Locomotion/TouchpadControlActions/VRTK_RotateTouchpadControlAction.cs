// Rotate Touchpad Control Action|TouchpadControlActions|25020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Rotate Touchpad Control Action script is used to rotate the controlled GameObject around the up vector when changing the touchpad axis.
    /// </summary>
    /// <remarks>
    /// The effect is a smooth rotation to simulate turning.
    /// </remarks>
    public class VRTK_RotateTouchpadControlAction : VRTK_BaseTouchpadControlAction
    {
        [Tooltip("The maximum speed the controlled object can be rotated based on the position of the touchpad axis.")]
        public float maximumRotationSpeed = 3f;
        [Tooltip("The rotation multiplier to be applied when the modifier button is pressed.")]
        public float rotationMultiplier = 1.5f;

        /// <summary>
        /// The ProcessFixedUpdate method is run for every FixedUpdate on the Touchpad Control script.
        /// </summary>
        /// <param name="controlledGameObject">The GameObject that is going to be affected.</param>
        /// <param name="directionDevice">The device that is used for the direction.</param>
        /// <param name="axisDirection">The axis that is being affected from the touchpad.</param>
        /// <param name="axis">The value of the current touchpad touch point based across the axis direction.</param>
        /// <param name="deadzone">The value of the deadzone based across the axis direction.</param>
        /// <param name="currentlyFalling">Whether the controlled GameObject is currently falling.</param>
        /// <param name="modifierActive">Whether the modifier button is pressed.</param>
        public override void ProcessFixedUpdate(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
        {
            float angle = Rotate(axis, modifierActive);
            if (angle != 0f)
            {
                RotateAroundPlayer(controlledGameObject, angle);
            }
        }

        protected virtual float Rotate(float axis, bool modifierActive)
        {
            return axis * maximumRotationSpeed * Time.deltaTime * (modifierActive ? rotationMultiplier : 1) * 10;
        }
    }
}