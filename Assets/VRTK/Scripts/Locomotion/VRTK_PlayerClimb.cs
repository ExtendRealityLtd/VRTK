// Player Climb|Locomotion|20120
namespace VRTK
{
    using GrabAttachMechanics;
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">**OBSOLETE** The index of the controller doing the interaction.</param>
    /// <param name="controllerReference">The reference to the controller doing the interaction.</param>
    /// <param name="target">The GameObject of the interactable object that is being interacted with by the controller.</param>
    public struct PlayerClimbEventArgs
    {
        [System.Obsolete("`PlayerClimbEventArgs.controllerIndex` has been replaced with `PlayerClimbEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
        public uint controllerIndex;
        public VRTK_ControllerReference controllerReference;
        public GameObject target;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="PlayerClimbEventArgs"/></param>
    public delegate void PlayerClimbEventHandler(object sender, PlayerClimbEventArgs e);

    /// <summary>
    /// The Player Climb allows player movement based on grabbing of `VRTK_InteractableObject` objects that have a `Climbable` grab attach script. Because it works by grabbing, each controller should have a `VRTK_InteractGrab` and `VRTK_InteractTouch` component attached.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/037_CameraRig_ClimbingFalling` shows how to set up a scene with player climbing. There are many different examples showing how the same system can be used in unique ways.
    /// </example>
    [RequireComponent(typeof(VRTK_BodyPhysics))]
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_PlayerClimb")]
    public class VRTK_PlayerClimb : MonoBehaviour
    {
        [Header("Climb Settings")]

        [Tooltip("Will scale movement up and down based on the player transform's scale.")]
        public bool usePlayerScale = true;

        [Header("Custom Settings")]

        [Tooltip("The VRTK Body Physics script to use for dealing with climbing and falling. If this is left blank then the script will need to be applied to the same GameObject.")]
        public VRTK_BodyPhysics bodyPhysics;
        [Tooltip("The VRTK Teleport script to use when snapping to nearest floor on release. If this is left blank then a Teleport script will need to be applied to the same GameObject.")]
        public VRTK_BasicTeleport teleporter;
        [Tooltip("The VRTK Headset Collision script to use for determining if the user is climbing inside a collidable object. If this is left blank then the script will need to be applied to the same GameObject.")]
        public VRTK_HeadsetCollision headsetCollision;
        [Tooltip("The VRTK Position Rewind script to use for dealing resetting invalid positions. If this is left blank then the script will need to be applied to the same GameObject.")]
        public VRTK_PositionRewind positionRewind;

        /// <summary>
        /// Emitted when player climbing has started.
        /// </summary>
        public event PlayerClimbEventHandler PlayerClimbStarted;
        /// <summary>
        /// Emitted when player climbing has ended.
        /// </summary>
        public event PlayerClimbEventHandler PlayerClimbEnded;

        protected Transform playArea;
        protected Vector3 startControllerScaledLocalPosition;
        protected Vector3 startGrabPointLocalPosition;
        protected Vector3 startPlayAreaWorldOffset;
        protected GameObject grabbingController;
        protected GameObject climbingObject;
        protected Quaternion climbingObjectLastRotation;
        protected bool isClimbing;
        protected bool useGrabbedObjectRotation;

        protected virtual void Awake()
        {
            bodyPhysics = (bodyPhysics != null ? bodyPhysics : GetComponentInChildren<VRTK_BodyPhysics>());
            teleporter = (teleporter != null ? teleporter : GetComponentInChildren<VRTK_BasicTeleport>());
            headsetCollision = (headsetCollision != null ? headsetCollision : GetComponentInChildren<VRTK_HeadsetCollision>());
            positionRewind = (positionRewind != null ? positionRewind : GetComponentInChildren<VRTK_PositionRewind>());

            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            InitListeners(true);
        }

        protected virtual void OnDisable()
        {
            Ungrab(false, null, climbingObject);
            InitListeners(false);
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            if (isClimbing)
            {
                Vector3 controllerLocalOffset = GetScaledLocalPosition(grabbingController.transform) - startControllerScaledLocalPosition;
                Vector3 grabPointWorldPosition = climbingObject.transform.TransformPoint(startGrabPointLocalPosition);
                playArea.position = grabPointWorldPosition + startPlayAreaWorldOffset - controllerLocalOffset;

                if (useGrabbedObjectRotation)
                {
                    Vector3 lastRotationVec = climbingObjectLastRotation * Vector3.forward;
                    Vector3 currentObectRotationVec = climbingObject.transform.rotation * Vector3.forward;
                    Vector3 axis = Vector3.Cross(lastRotationVec, currentObectRotationVec);
                    float angle = Vector3.Angle(lastRotationVec, currentObectRotationVec);

                    playArea.RotateAround(grabPointWorldPosition, axis, angle);
                    climbingObjectLastRotation = climbingObject.transform.rotation;
                }

                if (positionRewind != null && !IsHeadsetColliding())
                {
                    positionRewind.SetLastGoodPosition();
                }
            }
        }

        protected virtual void OnPlayerClimbStarted(PlayerClimbEventArgs e)
        {
            if (PlayerClimbStarted != null)
            {
                PlayerClimbStarted(this, e);
            }
        }

        protected virtual void OnPlayerClimbEnded(PlayerClimbEventArgs e)
        {
            if (PlayerClimbEnded != null)
            {
                PlayerClimbEnded(this, e);
            }
        }

        protected virtual PlayerClimbEventArgs SetPlayerClimbEvent(VRTK_ControllerReference controllerReference, GameObject target)
        {
            PlayerClimbEventArgs e;
#pragma warning disable 0618
            e.controllerIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
#pragma warning restore 0618
            e.controllerReference = controllerReference;
            e.target = target;
            return e;
        }

        protected virtual void InitListeners(bool state)
        {
            InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), state);
            InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), state);

            InitTeleportListener(state);
        }

        protected virtual void InitTeleportListener(bool state)
        {
            if (teleporter != null)
            {
                if (state)
                {
                    teleporter.Teleporting += new TeleportEventHandler(OnTeleport);
                }
                else
                {
                    teleporter.Teleporting -= new TeleportEventHandler(OnTeleport);
                }
            }
        }

        protected virtual void OnTeleport(object sender, DestinationMarkerEventArgs e)
        {
            if (isClimbing)
            {
                Ungrab(false, e.controllerReference, e.target.gameObject);
            }
        }

        protected virtual Vector3 GetScaledLocalPosition(Transform objTransform)
        {
            if (usePlayerScale)
            {
                return playArea.localRotation * Vector3.Scale(objTransform.localPosition, playArea.localScale);
            }

            return playArea.localRotation * objTransform.localPosition;
        }

        protected virtual void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (IsClimbableObject(e.target))
            {
                var controller = ((VRTK_InteractGrab)sender).gameObject;
                var actualController = VRTK_DeviceFinder.GetActualController(controller);
                Grab(actualController, e.controllerReference, e.target);
            }
        }

        protected virtual void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            var controller = ((VRTK_InteractGrab)sender).gameObject;
            var actualController = VRTK_DeviceFinder.GetActualController(controller);
            if (e.target && IsClimbableObject(e.target) && IsActiveClimbingController(actualController))
            {
                Ungrab(true, e.controllerReference, e.target);
            }
        }

        protected virtual void Grab(GameObject currentGrabbingController, VRTK_ControllerReference controllerReference, GameObject target)
        {
            bodyPhysics.ResetFalling();
            bodyPhysics.TogglePreventSnapToFloor(true);
            bodyPhysics.enableBodyCollisions = false;
            bodyPhysics.ToggleOnGround(false);

            isClimbing = true;
            climbingObject = target;
            grabbingController = currentGrabbingController;
            startControllerScaledLocalPosition = GetScaledLocalPosition(grabbingController.transform);
            startGrabPointLocalPosition = climbingObject.transform.InverseTransformPoint(grabbingController.transform.position);
            startPlayAreaWorldOffset = playArea.transform.position - grabbingController.transform.position;
            climbingObjectLastRotation = climbingObject.transform.rotation;
            useGrabbedObjectRotation = climbingObject.GetComponent<VRTK_ClimbableGrabAttach>().useObjectRotation;

            OnPlayerClimbStarted(SetPlayerClimbEvent(controllerReference, climbingObject));
        }

        protected virtual void Ungrab(bool carryMomentum, VRTK_ControllerReference controllerReference, GameObject target)
        {
            isClimbing = false;
            if (positionRewind != null && IsHeadsetColliding())
            {
                positionRewind.RewindPosition();
            }
            if (IsBodyColliding() && !IsHeadsetColliding())
            {
                bodyPhysics.ForceSnapToFloor();
            }

            bodyPhysics.enableBodyCollisions = true;

            if (carryMomentum)
            {
                Vector3 velocity = Vector3.zero;

                if (VRTK_ControllerReference.IsValid(controllerReference))
                {
                    velocity = -VRTK_DeviceFinder.GetControllerVelocity(controllerReference);
                    if (usePlayerScale)
                    {
                        velocity = playArea.TransformVector(velocity);
                    }
                    else
                    {
                        velocity = playArea.TransformDirection(velocity);
                    }
                }

                bodyPhysics.ApplyBodyVelocity(velocity, true, true);
            }

            grabbingController = null;
            climbingObject = null;

            OnPlayerClimbEnded(SetPlayerClimbEvent(controllerReference, target));
        }

        protected virtual bool IsActiveClimbingController(GameObject controller)
        {
            return (controller == grabbingController);
        }

        protected virtual bool IsClimbableObject(GameObject obj)
        {
            var interactObject = obj.GetComponent<VRTK_InteractableObject>();
            return (interactObject && interactObject.grabAttachMechanicScript && interactObject.grabAttachMechanicScript.IsClimbable());
        }

        protected virtual void InitControllerListeners(GameObject controller, bool state)
        {
            if (controller)
            {
                var grabScript = controller.GetComponent<VRTK_InteractGrab>();
                if (grabScript)
                {
                    if (state)
                    {
                        grabScript.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
                        grabScript.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUngrabObject);
                    }
                    else
                    {
                        grabScript.ControllerGrabInteractableObject -= new ObjectInteractEventHandler(OnGrabObject);
                        grabScript.ControllerUngrabInteractableObject -= new ObjectInteractEventHandler(OnUngrabObject);
                    }
                }
            }
        }

        protected virtual bool IsBodyColliding()
        {
            return (bodyPhysics != null && bodyPhysics.GetCurrentCollidingObject() != null);
        }

        protected virtual bool IsHeadsetColliding()
        {
            return (headsetCollision != null && headsetCollision.IsColliding());
        }
    }
}