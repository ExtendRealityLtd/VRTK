// Rotate Object Control Action|ObjectControlActions|25020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Provides the ability to rotate a GameObject through the world `y` axis in the scene by updating the Transform rotation when the corresponding Object Control axis changes.
    /// </summary>
    /// <remarks>
    ///   > The effect is a smooth rotation to simulate turning.
    ///
    /// **Required Components:**
    ///  * `VRTK_ObjectControl` - The Object Control script to listen for the axis changes on.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_RotateObjectControlAction` script on any active scene GameObject.
    ///  * Link the required Object Control script to the `Object Control Script` parameter of this script.
    ///  * Set the `Listen On Axis Change` parameter on this script to the axis change to affect with this movement type.
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