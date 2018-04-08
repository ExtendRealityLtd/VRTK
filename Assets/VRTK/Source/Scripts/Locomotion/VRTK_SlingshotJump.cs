// Slingshot Jump|Locomotion|20121
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    public delegate void SlingshotJumpEventHandler(object sender);

    /// <summary>
    /// Provides the ability for the SDK Camera Rig to be thrown around with a jumping motion by slingshotting based on the pull back of each valid controller.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_PlayerClimb` - A Player Climb script for dealing with the physical throwing of the play area as if throwing off an invisible climbed object.
    ///  * `VRTK_BodyPhysics` - A Body Physics script to deal with the effects of physics and gravity on the play area.
    ///
    /// **Optional Components:**
    ///  * `VRTK_BasicTeleport` - A Teleporter script to use when snapping the play area to the nearest floor when releasing from grab.
    ///  * `VRTK_HeadsetCollision` - A Headset Collision script to determine when the headset is colliding with geometry to know when to reset to a valid location.
    ///  * `VRTK_PositionRewind` - A Position Rewind script to utilise when resetting to a valid location upon ungrabbing whilst colliding with geometry.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_SlingshotJump` script on the same GameObject as the `VRTK_PlayerClimb` script.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/037_CameraRig_ClimbingFalling` shows how to set up a scene with slingshot jumping. This script just needs to be added to the PlayArea object and the requested forces and buttons set.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_SlingshotJump")]
    public class VRTK_SlingshotJump : MonoBehaviour
    {
        [Header("SlingshotJump Settings")]

        [Tooltip("How close together the button releases have to be to initiate a jump.")]
        public float releaseWindowTime = 0.5f;
        [Tooltip("Multiplier that increases the jump strength.")]
        public float velocityMultiplier = 5.0f;
        [Tooltip("The maximum velocity a jump can be.")]
        public float velocityMax = 8.0f;

        [Tooltip("The button that will initiate the slingshot move.")]
        [SerializeField]
        protected VRTK_ControllerEvents.ButtonAlias activationButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
        [Tooltip("The button that will cancel an already tensioned sling shot.")]
        [SerializeField]
        protected VRTK_ControllerEvents.ButtonAlias cancelButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The Body Physics script to deal with the physics and gravity of the play area. If the script is being applied onto an object that already has a VRTK_BodyPhysics component, this parameter can be left blank as it will be auto populated by the script at runtime.")]
        [SerializeField]
        protected VRTK_BodyPhysics bodyPhysics;
        [Tooltip("The Player Climb script to deal ability to throw the play area. If the script is being applied onto an object that already has a VRTK_PlayerClimb component, this parameter can be left blank as it will be auto populated by the script at runtime.")]
        [SerializeField]
        protected VRTK_PlayerClimb playerClimb;
        [Tooltip("The Teleporter script to deal play area teleporting. If the script is being applied onto an object that already has a VRTK_BasicTeleport component, this parameter can be left blank as it will be auto populated by the script at runtime.")]
        [SerializeField]
        protected VRTK_BasicTeleport teleporter;

        /// <summary>
        /// Emitted when a slingshot jump occurs
        /// </summary>
        public event SlingshotJumpEventHandler SlingshotJumped;

        protected Transform playArea;
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

        protected bool leftButtonReleased;
        protected bool rightButtonReleased;
        protected float countDownEndTime;

        /// <summary>
        /// The SetActivationButton method gets the button used to activate a slingshot jump.
        /// </summary>
        /// <returns>Returns the button used for slingshot activation.</returns>
        public virtual VRTK_ControllerEvents.ButtonAlias GetActivationButton()
        {
            return activationButton;
        }

        /// <summary>
        /// The SetActivationButton method sets the button used to activate a slingshot jump.
        /// </summary>
        /// <param name="button">The controller button to use to activate the jump.</param>
        public virtual void SetActivationButton(VRTK_ControllerEvents.ButtonAlias button)
        {
            InitControllerListeners(false);
            activationButton = button;
            InitControllerListeners(true);
        }

        /// <summary>
        /// The GetCancelButton method gets the button used to cancel a slingshot jump.
        /// </summary>
        /// <returns>Returns the button used to cancel a slingshot jump.</returns>
        public virtual VRTK_ControllerEvents.ButtonAlias GetCancelButton()
        {
            return cancelButton;
        }

        /// <summary>
        /// The SetCancelButton method sets the button used to cancel a slingshot jump.
        /// </summary>
        /// <param name="button">The controller button to use to cancel the jump.</param>
        public virtual void SetCancelButton(VRTK_ControllerEvents.ButtonAlias button)
        {
            InitControllerListeners(false);
            cancelButton = button;
            InitControllerListeners(true);
        }

        protected virtual void Awake()
        {
            bodyPhysics = (bodyPhysics != null ? bodyPhysics : FindObjectOfType<VRTK_BodyPhysics>());
            playerClimb = (playerClimb != null ? playerClimb : FindObjectOfType<VRTK_PlayerClimb>());
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            InitListeners(true);

            playArea = VRTK_DeviceFinder.PlayAreaTransform();
        }

        protected virtual void OnDisable()
        {
            UnAim();
            InitListeners(false);
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void LeftButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            // Check for new left aim
            if (!leftIsAiming && !IsClimbing())
            {
                leftIsAiming = true;
                leftStartAimPosition = playArea.InverseTransformPoint(leftControllerEvents.gameObject.transform.position);
            }
        }

        protected virtual void RightButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            // Check for new right aim
            if (!rightIsAiming && !IsClimbing())
            {
                rightIsAiming = true;
                rightStartAimPosition = playArea.InverseTransformPoint(rightControllerEvents.gameObject.transform.position);
            }
        }

        protected virtual void LeftButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            // Check for release states
            if (leftIsAiming)
            {
                leftReleasePosition = playArea.InverseTransformPoint(leftControllerEvents.gameObject.transform.position);
                if (!rightButtonReleased)
                {
                    countDownEndTime = Time.time + releaseWindowTime;
                }
                leftButtonReleased = true;
            }

            CheckForReset();
            CheckForJump();
        }

        protected virtual void RightButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            // Check for release states
            if (rightIsAiming)
            {
                rightReleasePosition = playArea.InverseTransformPoint(rightControllerEvents.gameObject.transform.position);
                if (!leftButtonReleased)
                {
                    countDownEndTime = Time.time + releaseWindowTime;
                }
                rightButtonReleased = true;
            }

            CheckForReset();
            CheckForJump();
        }

        protected virtual void CancelButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            UnAim();
        }

        protected virtual void CheckForReset()
        {
            if ((leftButtonReleased || rightButtonReleased) && Time.time > countDownEndTime)
            {
                UnAim();
            }
        }

        protected virtual void CheckForJump()
        {
            if (leftButtonReleased && rightButtonReleased && !bodyPhysics.IsFalling())
            {
                Vector3 leftDir = leftStartAimPosition - leftReleasePosition;
                Vector3 rightDir = rightStartAimPosition - rightReleasePosition;
                Vector3 localJumpDir = leftDir + rightDir;
                Vector3 worldJumpDir = playArea.transform.TransformVector(localJumpDir);
                Vector3 jumpVector = worldJumpDir * velocityMultiplier;

                if (jumpVector.magnitude > velocityMax)
                {
                    jumpVector = jumpVector.normalized * velocityMax;
                }

                bodyPhysics.ApplyBodyVelocity(jumpVector, true, true);

                UnAim();

                OnSlingshotJumped();
            }
        }

        protected void OnSlingshotJumped()
        {
            if (SlingshotJumped != null)
            {
                SlingshotJumped(this);
            }
        }

        protected void InitListeners(bool state)
        {
            InitTeleportListener(state);
            InitControllerListeners(state);
        }

        protected void InitTeleportListener(bool state)
        {
            teleporter = (teleporter != null ? teleporter : FindObjectOfType<VRTK_BasicTeleport>());
            if (teleporter != null)
            {
                if (state == true)
                {
                    teleporter.Teleporting += new TeleportEventHandler(OnTeleport);
                }
                else
                {
                    teleporter.Teleporting -= new TeleportEventHandler(OnTeleport);
                }
            }
        }

        protected void InitControllerListeners(bool state)
        {
            InitControllerListener(state, VRTK_DeviceFinder.GetControllerLeftHand(), ref leftControllerEvents, ref leftControllerGrab, LeftButtonPressed, LeftButtonReleased);
            InitControllerListener(state, VRTK_DeviceFinder.GetControllerRightHand(), ref rightControllerEvents, ref rightControllerGrab, RightButtonPressed, RightButtonReleased);
        }

        protected void InitControllerListener(bool state, GameObject controller, ref VRTK_ControllerEvents events, ref VRTK_InteractGrab grab,
            ControllerInteractionEventHandler triggerPressed, ControllerInteractionEventHandler triggerReleased)
        {
            if (controller != null)
            {
                events = controller.GetComponentInChildren<VRTK_ControllerEvents>();
                grab = controller.GetComponentInChildren<VRTK_InteractGrab>();

                if (events != null)
                {
                    if (state == true)
                    {
                        events.SubscribeToButtonAliasEvent(activationButton, true, triggerPressed);
                        events.SubscribeToButtonAliasEvent(activationButton, false, triggerReleased);

                        if (cancelButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                        {
                            events.SubscribeToButtonAliasEvent(cancelButton, true, CancelButtonPressed);
                        }
                    }
                    else
                    {
                        events.UnsubscribeToButtonAliasEvent(activationButton, true, triggerPressed);
                        events.UnsubscribeToButtonAliasEvent(activationButton, false, triggerReleased);

                        if (cancelButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                        {
                            events.UnsubscribeToButtonAliasEvent(cancelButton, true, CancelButtonPressed);
                        }
                    }
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

            leftButtonReleased = false;
            rightButtonReleased = false;
        }

        protected bool IsClimbing()
        {
            return (playerClimb != null && playerClimb.IsClimbing());
        }
    }
}