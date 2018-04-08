// Snap Rotate Object Control Action|ObjectControlActions|25030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Provides the ability to snap rotate a GameObject through the world `y` axis in the scene by updating the Transform rotation in defined steps when the corresponding Object Control axis changes.
    /// </summary>
    /// <remarks>
    ///   > The effect is a immediate snap rotation to quickly face in a new direction.
    ///
    /// **Required Components:**
    ///  * `VRTK_ObjectControl` - The Object Control script to listen for the axis changes on.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_SnapRotateObjectControlAction` script on any active scene GameObject.
    ///  * Link the required Object Control script to the `Object Control Script` parameter of this script.
    ///  * Set the `Listen On Axis Change` parameter on this script to the axis change to affect with this movement type.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    ///
    /// To enable the Snap Rotate Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Snap Rotate` control script active.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/Object Control Actions/VRTK_SnapRotateObjectControlAction")]
    public class VRTK_SnapRotateObjectControlAction : VRTK_BaseObjectControlAction
    {
        [Tooltip("The angle to rotate for each snap.")]
        public float anglePerSnap = 30f;
        [Tooltip("The snap angle multiplier to be applied when the modifier button is pressed.")]
        public float angleMultiplier = 1.5f;
        [Tooltip("The amount of time required to pass before another snap rotation can be carried out.")]
        public float snapDelay = 0.5f;
        [Tooltip("The speed for the headset to fade out and back in. Having a blink between rotations can reduce nausia.")]
        public float blinkTransitionSpeed = 0.6f;
        [Range(-1f, 1f)]
        [Tooltip("The threshold the listened axis needs to exceed before the action occurs. This can be used to limit the snap rotate to a single axis direction (e.g. pull down to flip rotate). The threshold is ignored if it is 0.")]
        public float axisThreshold = 0f;

        protected float snapDelayTimer = 0f;

        protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
        {
            CheckForPlayerBeforeRotation(controlledGameObject);

            if (snapDelayTimer < Time.time && ValidThreshold(axis))
            {
                float angle = Rotate(axis, modifierActive);
                if (angle != 0f)
                {
                    Blink(blinkTransitionSpeed);
                    RotateAroundPlayer(controlledGameObject, angle);
                }
            }

            CheckForPlayerAfterRotation(controlledGameObject);
        }

        protected virtual bool ValidThreshold(float axis)
        {
            return (axisThreshold == 0f || ((axisThreshold > 0f && axis >= axisThreshold) || (axisThreshold < 0f && axis <= axisThreshold)));
        }

        protected virtual float Rotate(float axis, bool modifierActive)
        {
            snapDelayTimer = Time.time + snapDelay;
            int directionMultiplier = GetAxisDirection(axis);
            return (anglePerSnap * (modifierActive ? angleMultiplier : 1)) * directionMultiplier;
        }
    }
}