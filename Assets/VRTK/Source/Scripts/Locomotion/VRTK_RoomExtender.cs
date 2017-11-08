// Room Extender|Locomotion|20130
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Multiplies each real world step within the play area to enable further distances to be travelled in the virtual world.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_RoomExtender` script on any active scene GameObject.
    ///
    /// **Script Dependencies:**
    ///  * The Controller Events script on the controller Script Alias to determine when the touchpad is pressed.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/028_CameraRig_RoomExtender` shows how the RoomExtender script is controlled by a VRTK_RoomExtender_Controller Example script located at both controllers. Pressing the `Touchpad` on the controller activates the Room Extender. The Additional Movement Multiplier is changed based on the touch distance to the centre of the touchpad.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_RoomExtender")]
    [System.Obsolete("`VRTK_RoomExtender` has been replaced with `VRTK_StepMultiplier`. This script will be removed in a future version of VRTK.")]
    public class VRTK_RoomExtender : MonoBehaviour
    {
        /// <summary>
        /// Movement methods.
        /// </summary>
        public enum MovementFunction
        {
            /// <summary>
            /// Moves the head with a non-linear drift movement.
            /// </summary>
            Nonlinear,
            /// <summary>
            /// Moves the headset in a direct linear movement.
            /// </summary>
            LinearDirect
        }

        [Tooltip("This determines the type of movement used by the extender.")]
        public MovementFunction movementFunction = MovementFunction.LinearDirect;
        [Tooltip("Enables the additional movement.")]
        public bool additionalMovementEnabled = true;
        [Tooltip("If this is checked then the touchpad needs to be pressed to enable it. If this is unchecked then it is disabled by pressing the touchpad.")]
        public bool additionalMovementEnabledOnButtonPress = true;
        [Tooltip("This is the factor by which movement at the edge of the circle is amplified. `0` is no movement of the play area. Higher values simulate a bigger play area but may be too uncomfortable.")]
        [Range(0, 10)]
        public float additionalMovementMultiplier = 1.0f;
        [Tooltip("This is the size of the circle in which the play area is not moved and everything is normal. If it is to low it becomes uncomfortable when crouching.")]
        [Range(0, 5)]
        public float headZoneRadius = 0.25f;
        [Tooltip("This transform visualises the circle around the user where the play area is not moved. In the demo scene this is a cylinder at floor level. Remember to turn of collisions.")]
        public Transform debugTransform;

        [HideInInspector]
        public Vector3 relativeMovementOfCameraRig = new Vector3();

        protected Transform movementTransform;
        protected Transform playArea;
        protected Vector3 headCirclePosition;
        protected Vector3 lastPosition;
        protected Vector3 lastMovement;

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            movementTransform = VRTK_DeviceFinder.HeadsetTransform();
            if (movementTransform == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_RoomExtender", "Headset Transform"));
            }
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            additionalMovementEnabled = !additionalMovementEnabledOnButtonPress;
            if (debugTransform != null)
            {
                debugTransform.localScale = new Vector3(headZoneRadius * 2, 0.01f, headZoneRadius * 2);
            }
            MoveHeadCircleNonLinearDrift();
            lastPosition = movementTransform.localPosition;
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
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

        protected virtual void Move(Vector3 movement)
        {
            headCirclePosition += movement;
            if (debugTransform != null)
            {
                debugTransform.localPosition = new Vector3(headCirclePosition.x, debugTransform.localPosition.y, headCirclePosition.z);
            }
            if (additionalMovementEnabled)
            {
                playArea.localPosition += movement * additionalMovementMultiplier;
                relativeMovementOfCameraRig += movement * additionalMovementMultiplier;
            }
        }

        protected virtual void MoveHeadCircle()
        {
            //Get the movement of the head relative to the headCircle.
            Vector3 circleCenterToHead = new Vector3(movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);

            //Get the direction of the head movement.
            UpdateLastMovement();

            //Checks if the head is outside of the head cirlce and moves the head circle and play area in the movementDirection.
            if (circleCenterToHead.sqrMagnitude > headZoneRadius * headZoneRadius && lastMovement != Vector3.zero)
            {
                //Just move like the headset moved
                Move(lastMovement);
            }
        }

        protected virtual void MoveHeadCircleNonLinearDrift()
        {
            Vector3 movement = new Vector3(movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);
            if (movement.sqrMagnitude > headZoneRadius * headZoneRadius)
            {
                Vector3 deltaMovement = movement.normalized * (movement.magnitude - headZoneRadius);
                Move(deltaMovement);
            }
        }

        protected virtual void UpdateLastMovement()
        {
            //Save the last movement
            lastMovement = movementTransform.localPosition - lastPosition;
            lastMovement.y = 0;
            lastPosition = movementTransform.localPosition;
        }
    }
}