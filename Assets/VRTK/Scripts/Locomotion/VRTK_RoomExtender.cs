// Room Extender|Scripts|0150
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// This script allows the playArea to move with the user. The `[CameraRig]` is only moved when at the edge of a defined circle. Aims to create a virtually bigger play area. To use this add this script to the `[CameraRig`] prefab.
    /// </summary>
    /// <remarks>
    /// There is an additional script `VRTK_RoomExtender_PlayAreaGizmo` which can be attached to the `[CameraRig`] to visualize the extended playArea within the Editor.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/028_CameraRig_RoomExtender` shows how the RoomExtender script is controlled by a VRTK_RoomExtender_Controller Example script located at both controllers. Pressing the `Touchpad` on the controller activates the Room Extender. The Additional Movement Multiplier is changed based on the touch distance to the centre of the touchpad.
    /// </example>
    public class VRTK_RoomExtender : MonoBehaviour
    {
        /// <summary>
        /// Movement methods.
        /// </summary>
        /// <param name="Nonlinear">Moves the head with a non-linear drift movement.</param>
        /// <param name="LinearDirect">Moves the headset in a direct linear movement.</param>
        public enum MovementFunction
        {
            Nonlinear,
            LinearDirect
        }

        [Tooltip("This determines the type of movement used by the extender.")]
        public MovementFunction movementFunction = MovementFunction.LinearDirect;
        [Tooltip("This is the a public variable to enable the additional movement. This can be used in other scripts to toggle the `[CameraRig]` movement.")]
        public bool additionalMovementEnabled = true;
        [Tooltip("This configures the controls of the RoomExtender. If this is true then the touchpad needs to be pressed to enable it. If this is false then it is disabled by pressing the touchpad.")]
        public bool additionalMovementEnabledOnButtonPress = true;
        [Tooltip("This is the factor by which movement at the edge of the circle is amplified. 0 is no movement of the `[CameraRig]`. Higher values simulate a bigger play area but may be too uncomfortable.")]
        [Range(0, 10)]
        public float additionalMovementMultiplier = 1.0f;
        [Tooltip("This is the size of the circle in which the playArea is not moved and everything is normal. If it is to low it becomes uncomfortable when crouching.")]
        [Range(0, 5)]
        public float headZoneRadius = 0.25f;
        [Tooltip("This transform visualises the circle around the user where the `[CameraRig]` is not moved. In the demo scene this is a cylinder at floor level. Remember to turn of collisions.")]
        public Transform debugTransform;

        [HideInInspector]
        public Vector3 relativeMovementOfCameraRig = new Vector3();

        protected Transform movementTransform;
        protected Transform cameraRig;
        protected Vector3 headCirclePosition;
        protected Vector3 lastPosition;
        protected Vector3 lastMovement;

        private void Start()
        {
            if (movementTransform == null)
            {
                if (VRTK.VRTK_DeviceFinder.HeadsetTransform() != null)
                {
                    movementTransform = VRTK.VRTK_DeviceFinder.HeadsetTransform();
                }
                else
                {
                    Debug.LogWarning("The VRTK_RoomExtender script needs a movementTransform to work.");
                }
            }
            cameraRig = VRTK_DeviceFinder.PlayAreaTransform();
            additionalMovementEnabled = !additionalMovementEnabledOnButtonPress;
            if (debugTransform)
            {
                debugTransform.localScale = new Vector3(headZoneRadius * 2, 0.01f, headZoneRadius * 2);
            }
            MoveHeadCircleNonLinearDrift();
            lastPosition = movementTransform.localPosition;
        }

        private void Update()
        {
            switch (movementFunction)
            {
                case MovementFunction.Nonlinear:
                    MoveHeadCircleNonLinearDrift();
                    break;
                case MovementFunction.LinearDirect:
                    MoveHeadCircle();
                    break;
                default:
                    break;
            }
        }

        private void Move(Vector3 movement)
        {
            headCirclePosition += movement;
            if (debugTransform)
            {
                debugTransform.localPosition = headCirclePosition;
            }
            if (additionalMovementEnabled)
            {
                cameraRig.localPosition += movement * additionalMovementMultiplier;
                relativeMovementOfCameraRig += movement * additionalMovementMultiplier;
            }
        }

        private void MoveHeadCircle()
        {
            //Get the movement of the head relative to the headCircle.
            var circleCenterToHead = new Vector3(movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);

            //Get the direction of the head movement.
            UpdateLastMovement();

            //Checks if the head is outside of the head cirlce and moves the head circle and play area in the movementDirection.
            if (circleCenterToHead.sqrMagnitude > headZoneRadius * headZoneRadius && lastMovement != Vector3.zero)
            {
                //Just move like the headset moved
                Move(lastMovement);
            }
        }

        private void MoveHeadCircleNonLinearDrift()
        {
            var movement = new Vector3(movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);
            if (movement.sqrMagnitude > headZoneRadius * headZoneRadius)
            {
                var deltaMovement = movement.normalized * (movement.magnitude - headZoneRadius);
                Move(deltaMovement);
            }
        }

        private void UpdateLastMovement()
        {
            //Save the last movement
            lastMovement = movementTransform.localPosition - lastPosition;
            lastMovement.y = 0;
            lastPosition = movementTransform.localPosition;
        }
    }
}