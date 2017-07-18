// Warp Object Control Action|ObjectControlActions|25040
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Warp Object Control Action script is used to warp the controlled GameObject a given distance when changing the axis.
    /// </summary>
    /// <remarks>
    /// The effect is a immediate snap to a new position in the given direction.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    ///
    /// To enable the Warp Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Warp` control script active.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/Object Control Actions/VRTK_WarpObjectControlAction")]
    public class VRTK_WarpObjectControlAction : VRTK_BaseObjectControlAction
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

        protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
        {
            if (warpDelayTimer < Time.timeSinceLevelLoad && axis != 0f)
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