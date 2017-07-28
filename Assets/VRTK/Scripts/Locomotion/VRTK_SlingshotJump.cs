// Slingshot Jump|Locomotion|20120
namespace VRTK
{
    using System;
    using GrabAttachMechanics;
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="target">The GameObject of the interactable object that is being interacted with by the controller.</param>
    public struct SlingshotJumpEventArgs
    {
        public GameObject target;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="PlayerClimbEventArgs"/></param>
    public delegate void SlingshotJumpEventHandler(object sender, SlingshotJumpEventArgs e);

    /// <summary>
    /// Slingshot Jump allows player jumping based on the direction and amount pulled back of each controller. This slingshots the player in defined direction and speed.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/037_CameraRig_ClimbingFalling` shows how to set up a scene with slingshot jumping. This script just needs to be added to the PlayArea object and the requested forces and buttons set.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_SlingshotJump")]
    [RequireComponent(typeof(VRTK_BodyPhysics))]
    public class VRTK_SlingshotJump : MonoBehaviour
    {
        [Tooltip("This button will cancel an already tensioned sling shot.")]
        public VRTK_ControllerEvents.ButtonAlias cancelButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("This button will start the sling shot move.")]
        public VRTK_ControllerEvents.ButtonAlias triggerButton =  VRTK_ControllerEvents.ButtonAlias.TriggerPress;
        [Tooltip("How close together the trigger releases have to be to initiate a jump.")]
        public float releaseWindowTime = 0.5f;
        [Tooltip("Multiplier that increases the jump strength.")]
        public float velocityMultiplier = 5.0f;
        [Tooltip("The maximum velocity a jump can be.")]
        public float velocityMax = 8.0f;

        /// <summary>
        /// Emitted when a slingshot jump occurs
        /// </summary>
        public event SlingshotJumpEventHandler SlingshotJumped;

        protected Transform playArea;
        protected VRTK_BodyPhysics bodyPhysics;
        protected VRTK_PlayerClimb playerClimb;

        protected Vector3 leftStartAimPosition;
        protected Vector3 leftReleasePosition;
        protected bool leftIsAiming;

        protected Vector3 rightStartAimPosition;
        protected Vector3 rightReleasePosition;
        protected bool rightIsAiming;

        protected VRTK_ControllerEvents leftControllerEvents;
        protected VRTK_ControllerEvents rightControllerEvents;

        protected VRTK_InteractGrab leftControllerGrab;
        protected VRTK_InteractGrab rightControllerGrab;

        protected bool leftTriggerReleased;
        protected bool rightTriggerReleased;
        protected float countDownEndTime;

        protected virtual void Awake()
        {
            bodyPhysics = GetComponent<VRTK_BodyPhysics>();
            playerClimb = GetComponent<VRTK_PlayerClimb>();

            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            InitListeners(true);

            playArea = VRTK_DeviceFinder.PlayAreaTransform();

            GameObject controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            if (controllerLeftHand != null)
            {
                leftControllerEvents = controllerLeftHand.GetComponent<VRTK_ControllerEvents>();
                leftControllerGrab = controllerLeftHand.GetComponent<VRTK_InteractGrab>();
            }

            GameObject controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
            if (controllerRightHand != null)
            {
                rightControllerEvents = controllerRightHand.GetComponent<VRTK_ControllerEvents>();
                rightControllerGrab = controllerRightHand.GetComponent<VRTK_InteractGrab>();
            }
        }

        protected virtual void OnDisable()
        {
            UnAim();
            InitListeners(false);
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            // Check for cancel
            if (rightControllerEvents.IsButtonPressed(cancelButton) || rightControllerEvents.IsButtonPressed(cancelButton))
            {
                UnAim();
            }

            // Check for new left aim
            if (!leftIsAiming && leftControllerEvents.IsButtonPressed(triggerButton) && !IsClimbing())
            {
                leftIsAiming = true;
                leftStartAimPosition = playArea.InverseTransformPoint(leftControllerEvents.gameObject.transform.position);
            }

            // Check for new right aim
            if (!rightIsAiming && rightControllerEvents.IsButtonPressed(triggerButton) && !IsClimbing())
            {
                rightIsAiming = true;
                rightStartAimPosition = playArea.InverseTransformPoint(rightControllerEvents.gameObject.transform.position);
            }

            // Check for release states
            if (leftIsAiming && !leftControllerEvents.IsButtonPressed(triggerButton))
            {
                leftReleasePosition = playArea.InverseTransformPoint(leftControllerEvents.gameObject.transform.position);
                if (!rightTriggerReleased)
                {
                    countDownEndTime = Time.time + releaseWindowTime;
                }
                leftTriggerReleased = true;
            }

            if (rightIsAiming && !rightControllerEvents.IsButtonPressed(triggerButton))
            {
                rightReleasePosition = playArea.InverseTransformPoint(rightControllerEvents.gameObject.transform.position);
                if (!leftTriggerReleased)
                {
                    countDownEndTime = Time.time + releaseWindowTime;
                }
                rightTriggerReleased = true;
            }

            // Check for reset
            if ((leftTriggerReleased || rightTriggerReleased) && Time.time > countDownEndTime)
            {
                UnAim();
            }

            // Check for jump
            if (leftTriggerReleased && rightTriggerReleased && !bodyPhysics.IsFalling())
            {
                Vector3 leftDir = leftStartAimPosition-leftReleasePosition;
                Vector3 rightDir = rightStartAimPosition-rightReleasePosition;
                Vector3 localJumpDir = leftDir + rightDir;
                Vector3 worldJumpDir = playArea.transform.TransformVector(localJumpDir);
                Vector3 jumpVector = worldJumpDir * velocityMultiplier;
                
                if (jumpVector.magnitude > velocityMax)
                {
                    jumpVector = jumpVector.normalized * velocityMax;
                } 

                bodyPhysics.ApplyBodyVelocity(jumpVector, true, true);

                UnAim();

                OnSlingshotJumped(SetPlayerClimbEvent(gameObject));
            }
        }

        protected void OnSlingshotJumped(SlingshotJumpEventArgs e)
        {
            if (SlingshotJumped != null)
            {
                SlingshotJumped(this, e);
            }
        }

        protected SlingshotJumpEventArgs SetPlayerClimbEvent(GameObject target)
        {
            SlingshotJumpEventArgs e;
            e.target = target;
            return e;
        }

        protected void InitListeners(bool state)
        {
            InitTeleportListener(state);
        }

        protected void InitTeleportListener(bool state)
        {
            VRTK_BasicTeleport teleportComponent = GetComponent<VRTK_BasicTeleport>();
            if (teleportComponent!= null)
            {
                if (state != null)
                {
                    teleportComponent.Teleporting += new TeleportEventHandler(OnTeleport);
                }
                else
                {
                    teleportComponent.Teleporting -= new TeleportEventHandler(OnTeleport);
                }
            }
        }

        protected void OnTeleport(object sender, DestinationMarkerEventArgs e)
        {
            UnAim();
        }

        protected void UnAim()
        {
            leftIsAiming = false;
            rightIsAiming = false;

            leftTriggerReleased = false;
            rightTriggerReleased = false;
        }

        protected bool IsClimbing()
        {
            return playerClimb != null && playerClimb.IsClimbing();
        }
    }
}