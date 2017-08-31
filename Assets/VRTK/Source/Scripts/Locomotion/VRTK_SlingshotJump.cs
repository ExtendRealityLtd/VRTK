// Slingshot Jump|Locomotion|20121
namespace VRTK
{
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
    /// <param name="e"><see cref="SlingshotJumpEventArgs"/></param>
    public delegate void SlingshotJumpEventHandler(object sender, SlingshotJumpEventArgs e);

    /// <summary>
    /// Slingshot Jump allows player jumping based on the direction and amount pulled back of each controller. This slingshots the player in defined direction and speed.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/037_CameraRig_ClimbingFalling` shows how to set up a scene with slingshot jumping. This script just needs to be added to the PlayArea object and the requested forces and buttons set.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_SlingshotJump")]
    public class VRTK_SlingshotJump : MonoBehaviour
    {
        [Header("SlingshotJump Settings")]

        [Tooltip("How close together the trigger releases have to be to initiate a jump.")]
        public float releaseWindowTime = 0.5f;
        [Tooltip("Multiplier that increases the jump strength.")]
        public float velocityMultiplier = 5.0f;
        [Tooltip("The maximum velocity a jump can be.")]
        public float velocityMax = 8.0f;

        [Tooltip("This button will cancel an already tensioned sling shot.")]
        [SerializeField]
        protected VRTK_ControllerEvents.ButtonAlias cancelButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("This button will start the sling shot move.")]
        [SerializeField]
        protected VRTK_ControllerEvents.ButtonAlias activationButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
        [Tooltip("The VRTK_BodyPhysics object used on the player. If the script is being applied onto an object that already has a VRTK_BodyPhysics component, this parameter can be left blank as it will be auto populated by the script at runtime.")]
        [SerializeField]
        protected VRTK_BodyPhysics bodyPhysics;

        /// <summary>
        /// Emitted when a slingshot jump occurs
        /// </summary>
        public event SlingshotJumpEventHandler SlingshotJumped;

        protected Transform playArea;
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
            bodyPhysics = bodyPhysics != null ? bodyPhysics : GetComponentInParent<VRTK_BodyPhysics>();
            playerClimb = GetComponent<VRTK_PlayerClimb>();

            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
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
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
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
            InitControllerListeners(state);
        }

        protected void InitTeleportListener(bool state)
        {
            VRTK_BasicTeleport teleportComponent = GetComponent<VRTK_BasicTeleport>();
            if (teleportComponent != null)
            {
                if (state == true)
                {
                    teleportComponent.Teleporting += new TeleportEventHandler(OnTeleport);
                }
                else
                {
                    teleportComponent.Teleporting -= new TeleportEventHandler(OnTeleport);
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
                events = controller.GetComponent<VRTK_ControllerEvents>();
                grab = controller.GetComponent<VRTK_InteractGrab>();

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
            return playerClimb != null && playerClimb.IsClimbing();
        }
    }
}