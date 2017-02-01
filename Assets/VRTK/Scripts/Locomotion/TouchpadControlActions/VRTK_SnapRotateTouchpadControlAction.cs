// Snap Rotate Touchpad Control Action|TouchpadControlActions|25030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Snap Rotate Touchpad Control Action script is used to snap rotate the controlled GameObject around the up vector when changing the touchpad axis.
    /// </summary>
    /// <remarks>
    /// The effect is a immediate snap rotation to quickly face in a new direction.
    /// </remarks>
    public class VRTK_SnapRotateTouchpadControlAction : VRTK_BaseTouchpadControlAction
    {
        [Tooltip("The angle to rotate for each snap.")]
        public float anglePerSnap = 30f;
        [Tooltip("The snap angle multiplier to be applied when the modifier button is pressed.")]
        public float angleMultiplier = 1.5f;
        [Tooltip("The amount of time required to pass before another snap rotation can be carried out.")]
        public float snapDelay = 0.5f;
        [Tooltip("The speed for the headset to fade out and back in. Having a blink between rotations can reduce nausia.")]
        public float blinkTransitionSpeed = 0.6f;

        private float snapDelayTimer = 0f;

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
            if (snapDelayTimer < Time.timeSinceLevelLoad)
            {
                float angle = Rotate(axis, modifierActive);
                if (angle != 0f)
                {
                    Blink(blinkTransitionSpeed);
                    RotateAroundPlayer(controlledGameObject, angle);
                }
            }
        }

        protected virtual float Rotate(float axis, bool modifierActive)
        {
            snapDelayTimer = Time.timeSinceLevelLoad + snapDelay;
            int directionMultiplier = GetAxisDirection(axis);
            return (anglePerSnap * (modifierActive ? angleMultiplier : 1)) * directionMultiplier;
        }
    }
}