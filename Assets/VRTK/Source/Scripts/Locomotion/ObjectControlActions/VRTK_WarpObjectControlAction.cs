// Warp Object Control Action|ObjectControlActions|25040
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Provides the ability to move a GameObject around by warping it across the `x/z` plane in the scene by updating the Transform position in defined steps when the corresponding Object Control axis changes.
    /// </summary>
    /// <remarks>
    ///   > The effect is a immediate snap to a new position in the given direction.
    ///
    /// **Required Components:**
    ///  * `VRTK_ObjectControl` - The Object Control script to listen for the axis changes on.
    ///
    /// **Optional Components:**
    ///  * `VRTK_BodyPhysics` - The Body Physics script to utilise when checking for potential collisions on movement.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_WarpObjectControlAction` script on any active scene GameObject.
    ///  * Link the required Object Control script to the `Object Control Script` parameter of this script.
    ///  * Set the `Listen On Axis Change` parameter on this script to the axis change to affect with this movement type.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    ///
    /// To enable the Warp Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Warp` control script active.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/Object Control Actions/VRTK_WarpObjectControlAction")]
    public class VRTK_WarpObjectControlAction : VRTK_BaseObjectControlAction
    {
        [Header("Warp Settings")]

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

        [Header("Custom Settings")]

        [Tooltip("An optional Body Physics script to check for potential collisions in the moving direction. If any potential collision is found then the move will not take place. This can help reduce collision tunnelling.")]
        public VRTK_BodyPhysics bodyPhysics;

        protected float warpDelayTimer = 0f;
        protected Transform headset;

        protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
        {
            if (warpDelayTimer < Time.time && axis != 0f)
            {
                Warp(controlledGameObject, directionDevice, axisDirection, axis, modifierActive);
            }
        }

        protected override void OnEnable()
        {
            internalBodyPhysics = bodyPhysics;
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

                warpDelayTimer = Time.time + warpDelay;
                if (CanMove(bodyPhysics, controlledGameObject.transform.position, finalPosition))
                {
                    controlledGameObject.transform.position = finalPosition;
                    Blink(blinkTransitionSpeed);
                }
            }
        }
    }
}