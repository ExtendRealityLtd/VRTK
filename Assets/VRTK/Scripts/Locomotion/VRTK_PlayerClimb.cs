// Player Climb|Locomotion|20120
namespace VRTK
{
    using GrabAttachMechanics;
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">The index of the controller doing the interaction.</param>
    /// <param name="target">The GameObject of the interactable object that is being interacted with by the controller.</param>
    public struct PlayerClimbEventArgs
    {
        public uint controllerIndex;
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
    public class VRTK_PlayerClimb : MonoBehaviour
    {
        [Tooltip("Will scale movement up and down based on the player transform's scale.")]
        public bool usePlayerScale = true;

        /// <summary>
        /// Emitted when player climbing has started.
        /// </summary>
        public event PlayerClimbEventHandler PlayerClimbStarted;
        /// <summary>
        /// Emitted when player climbing has ended.
        /// </summary>
        public event PlayerClimbEventHandler PlayerClimbEnded;

        private Transform playArea;
        private Vector3 startControllerScaledLocalPosition;
        private Vector3 startGrabPointLocalPosition;
        private Vector3 startPlayAreaWorldOffset;
        private GameObject grabbingController;
        private GameObject climbingObject;
        private Quaternion climbingObjectLastRotation;
        private VRTK_BodyPhysics bodyPhysics;
        private bool isClimbing;
        private bool useGrabbedObjectRotation;

        protected virtual void Awake()
        {
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            bodyPhysics = GetComponent<VRTK_BodyPhysics>();
        }

        protected virtual void OnEnable()
        {
            InitListeners(true);
        }

        protected virtual void OnDisable()
        {
            Ungrab(false, 0, climbingObject);
            InitListeners(false);
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
            }
        }

        private void OnPlayerClimbStarted(PlayerClimbEventArgs e)
        {
            if (PlayerClimbStarted != null)
            {
                PlayerClimbStarted(this, e);
            }
        }

        private void OnPlayerClimbEnded(PlayerClimbEventArgs e)
        {
            if (PlayerClimbEnded != null)
            {
                PlayerClimbEnded(this, e);
            }
        }

        private PlayerClimbEventArgs SetPlayerClimbEvent(uint controllerIndex, GameObject target)
        {
            PlayerClimbEventArgs e;
            e.controllerIndex = controllerIndex;
            e.target = target;
            return e;
        }

        private void InitListeners(bool state)
        {
            InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), state);
            InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), state);

            InitTeleportListener(state);
        }

        private void InitTeleportListener(bool state)
        {
            var teleportComponent = GetComponent<VRTK_BasicTeleport>();
            if (teleportComponent)
            {
                if (state)
                {
                    teleportComponent.Teleporting += new TeleportEventHandler(OnTeleport);
                }
                else
                {
                    teleportComponent.Teleporting -= new TeleportEventHandler(OnTeleport);
                }
            }
        }

        private void OnTeleport(object sender, DestinationMarkerEventArgs e)
        {
            Ungrab(false, e.controllerIndex, e.target.gameObject);
        }

        private Vector3 GetScaledLocalPosition(Transform objTransform)
        {
            if (usePlayerScale)
            {
                return playArea.localRotation * Vector3.Scale(objTransform.localPosition, playArea.localScale);
            }

            return playArea.localRotation * objTransform.localPosition;
        }

        private void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (IsClimbableObject(e.target))
            {
                var controller = ((VRTK_InteractGrab)sender).gameObject;
                var actualController = VRTK_DeviceFinder.GetActualController(controller);
                Grab(actualController, e.controllerIndex, e.target);
            }
        }

        private void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            var controller = ((VRTK_InteractGrab)sender).gameObject;
            var actualController = VRTK_DeviceFinder.GetActualController(controller);
            if (e.target && IsClimbableObject(e.target) && IsActiveClimbingController(actualController))
            {
                Ungrab(true, e.controllerIndex, e.target);
            }
        }

        private void Grab(GameObject currentGrabbingController, uint controllerIndex, GameObject target)
        {
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

            OnPlayerClimbStarted(SetPlayerClimbEvent(controllerIndex, climbingObject));
        }

        private void Ungrab(bool carryMomentum, uint controllerIndex, GameObject target)
        {
            bodyPhysics.TogglePreventSnapToFloor(false);
            bodyPhysics.enableBodyCollisions = true;

            if (carryMomentum)
            {
                Vector3 velocity = Vector3.zero;
                var device = VRTK_DeviceFinder.GetControllerByIndex(controllerIndex, false);

                if (device)
                {
                    velocity = -VRTK_DeviceFinder.GetControllerVelocity(device);
                    if (usePlayerScale)
                    {
                        velocity = Vector3.Scale(velocity, playArea.localScale);
                    }
                }

                bodyPhysics.ApplyBodyVelocity(velocity, true, true);
            }

            isClimbing = false;
            grabbingController = null;
            climbingObject = null;

            OnPlayerClimbEnded(SetPlayerClimbEvent(controllerIndex, target));
        }

        private bool IsActiveClimbingController(GameObject controller)
        {
            return (controller == grabbingController);
        }

        private bool IsClimbableObject(GameObject obj)
        {
            var interactObject = obj.GetComponent<VRTK_InteractableObject>();
            return (interactObject && interactObject.grabAttachMechanicScript && interactObject.grabAttachMechanicScript.IsClimbable());
        }

        private void InitControllerListeners(GameObject controller, bool state)
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
    }
}