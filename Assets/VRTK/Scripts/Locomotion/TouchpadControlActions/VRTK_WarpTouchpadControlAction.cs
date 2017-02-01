// Warp Touchpad Control Action|TouchpadControlActions|25040
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Warp Touchpad Control Action script is used to warp the controlled GameObject a given distance when changing the touchpad axis.
    /// </summary>
    /// <remarks>
    /// The effect is a immediate snap to a new position in the given direction.
    /// </remarks>
    public class VRTK_WarpTouchpadControlAction : VRTK_BaseTouchpadControlAction
    {
        [Tooltip("The distance to warp in the facing direction.")]
        public float warpDistance = 1f;
        [Tooltip("The multiplier to be applied to the warp when the modifier button is pressed.")]
        public float warpMultiplier = 2f;
        [Tooltip("The amount of time required to pass before another warp can be carried out.")]
        public float warpDelay = 0.5f;
        [Tooltip("The height different in floor allowed to be a valid warp.")]
        public float floorHeightTolerance = 1f;
        [Tooltip("The speed for the headset to fade out and back in. Having a blink between warps can reduce nausia.")]
        public float blinkTransitionSpeed = 0.6f;

        private float warpDelayTimer = 0f;
        private Transform headset;

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
            if (warpDelayTimer < Time.timeSinceLevelLoad)
            {
                Warp(controlledGameObject, directionDevice, axisDirection, axis, modifierActive);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            headset = VRTK_DeviceFinder.HeadsetTransform();
        }

        protected virtual void Warp(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, bool modifierActive)
        {
            Vector3 objectCenter = GetObjectCenter(controlledGameObject.transform);
            Vector3 objectPosition = controlledGameObject.transform.TransformPoint(objectCenter);
            float distance = warpDistance * (modifierActive ? warpMultiplier : 1);
            int directionMultiplier = GetAxisDirection(axis);

            Vector3 targetPosition = objectPosition + (axisDirection * distance * directionMultiplier);

            float headMargin = 0.2f;
            RaycastHit warpRaycastHit;

            // direction raycast to stop near obstacles
            Vector3 raycastDirection = directionMultiplier * axisDirection;
            Vector3 startRayPosition = (controlledGameObject.transform == playArea ? headset.position : controlledGameObject.transform.position);

            if (Physics.Raycast(startRayPosition + (Vector3.up * headMargin), raycastDirection, out warpRaycastHit, distance - colliderRadius))
            {
                targetPosition = warpRaycastHit.point - (raycastDirection * colliderRadius);
            }

            // vertical raycast for height position
            if (Physics.Raycast(targetPosition + (Vector3.up * (floorHeightTolerance + headMargin)), Vector3.down, out warpRaycastHit, (floorHeightTolerance + headMargin) * 2))
            {
                targetPosition.y = warpRaycastHit.point.y + (colliderHeight / 2f);
                Vector3 finalPosition = targetPosition - objectPosition + controlledGameObject.transform.position;

                warpDelayTimer = Time.timeSinceLevelLoad + warpDelay;
                controlledGameObject.transform.position = finalPosition;

                Blink(blinkTransitionSpeed);
            }
        }
    }
}