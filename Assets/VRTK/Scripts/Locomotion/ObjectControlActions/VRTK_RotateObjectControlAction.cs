// Rotate Object Control Action|ObjectControlActions|25020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Rotate Object Control Action script is used to rotate the controlled GameObject around the up vector when changing the axis.
    /// </summary>
    /// <remarks>
    /// The effect is a smooth rotation to simulate turning.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    ///
    /// To enable the Rotate Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Rotate` control script active.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/Object Control Actions/VRTK_RotateObjectControlAction")]
    public class VRTK_RotateObjectControlAction : VRTK_BaseObjectControlAction
    {
        [Tooltip("The maximum speed the controlled object can be rotated based on the position of the axis.")]
        public float maximumRotationSpeed = 3f;
        [Tooltip("The rotation multiplier to be applied when the modifier button is pressed.")]
        public float rotationMultiplier = 1.5f;

        protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
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