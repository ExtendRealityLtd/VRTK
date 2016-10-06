﻿// World Pointer|Abstractions|0020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// This abstract class provides any game pointer the ability to know the state of the implemented pointer. It extends the `VRTK_DestinationMarker` to allow for destination events to be emitted when the pointer cursor collides with objects.
    /// </summary>
    /// <remarks>
    /// The World Pointer also provides a play area cursor to be displayed for all cursors that utilise this class. The play area cursor is a representation of the current calibrated play area space and is useful for visualising the potential new play area space in the game world prior to teleporting. It can also handle collisions with objects on the new play area space and prevent teleporting if there are any collisions with objects at the potential new destination.
    ///
    /// The play area collider does not work well with terrains as they are uneven and cause collisions regularly so it is recommended that handling play area collisions is not enabled when using terrains.
    /// </remarks>
    public abstract class VRTK_WorldPointer : VRTK_DestinationMarker
    {
        /// <summary>
        /// States of Pointer Visibility.
        /// </summary>
        /// <param name="On_When_Active">Only shows the pointer beam when the Pointer button on the controller is pressed.</param>
        /// <param name="Always_On">Ensures the pointer beam is always visible but pressing the Pointer button on the controller initiates the destination set event.</param>
        /// <param name="Always_Off">Ensures the pointer beam is never visible but the destination point is still set and pressing the Pointer button on the controller still initiates the destination set event.</param>
        public enum pointerVisibilityStates
        {
            On_When_Active,
            Always_On,
            Always_Off
        }

        [Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controller = null;
        [Tooltip("The material to use on the rendered version of the pointer. If no material is selected then the default `WorldPointer` material will be used.")]
        public Material pointerMaterial;
        [Tooltip("The colour of the beam when it is colliding with a valid target. It can be set to a different colour for each controller.")]
        public Color pointerHitColor = new Color(0f, 0.5f, 0f, 1f);
        [Tooltip("The colour of the beam when it is not hitting a valid target. It can be set to a different colour for each controller.")]
        public Color pointerMissColor = new Color(0.8f, 0f, 0f, 1f);
        [Tooltip("Determines when the pointer beam should be displayed.")]
        public pointerVisibilityStates pointerVisibility = pointerVisibilityStates.On_When_Active;
        [Tooltip("If this is checked then the pointer beam will be activated on first press of the pointer alias button and will stay active until the pointer alias button is pressed again. The destination set event is emitted when the beam is deactivated on the second button press.")]
        public bool holdButtonToActivate = true;
        [Tooltip("The time in seconds to delay the pointer beam being able to be active again. Useful for preventing constant teleportation.")]
        public float activateDelay = 0f;

        protected Vector3 destinationPosition;
        protected float pointerContactDistance = 0f;
        protected Transform pointerContactTarget = null;
        protected RaycastHit pointerContactRaycastHit = new RaycastHit();
        protected uint controllerIndex;
        protected VRTK_PlayAreaCursor playAreaCursor;

        private bool isActive;
        private bool destinationSetActive;
        private float activateDelayTimer = 0f;
        private int beamEnabledState = 0;
        private VRTK_InteractableObject interactableObject = null;

        /// <summary>
        /// The IsActive method is used to determine if the pointer currently active.
        /// </summary>
        /// <returns>Is true if the pointer is currently active.</returns>
        public virtual bool IsActive()
        {
            return isActive;
        }

        /// <summary>
        /// The CanActivate method checks to see if the pointer can be activated as long as the activation delay timer is zero.
        /// </summary>
        /// <returns>Is true if the pointer is able to be activated due to the activation delay timer being zero.</returns>
        public virtual bool CanActivate()
        {
            return (Time.time >= activateDelayTimer);
        }

        /// <summary>
        /// The ToggleBeam method allows the pointer beam to be toggled on or off via code at runtime. If true is passed as the state then the beam is activated, if false then the beam is deactivated.
        /// </summary>
        /// <param name="state">The state of whether to enable or disable the beam.</param>
        public virtual void ToggleBeam(bool state)
        {
            var index = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            if (state)
            {
                TurnOnBeam(index);
            }
            else
            {
                TurnOffBeam(index);
            }
        }

        protected virtual void Awake()
        {
            if (controller == null)
            {
                controller = GetComponent<VRTK_ControllerEvents>();
            }

            if (controller == null)
            {
                Debug.LogError("VRTK_WorldPointer requires a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            controller.AliasPointerOn += new ControllerInteractionEventHandler(EnablePointerBeam);
            controller.AliasPointerOff += new ControllerInteractionEventHandler(DisablePointerBeam);
            controller.AliasPointerSet += new ControllerInteractionEventHandler(SetPointerDestination);

            var tmpMaterial = Resources.Load("WorldPointer") as Material;
            if (pointerMaterial != null)
            {
                tmpMaterial = pointerMaterial;
            }

            pointerMaterial = new Material(tmpMaterial);
            pointerMaterial.color = pointerMissColor;

            playAreaCursor = (GetComponent<VRTK_PlayAreaCursor>() ?? GetComponent<VRTK_PlayAreaCursor>());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DisableBeam();
            destinationSetActive = false;
            pointerContactDistance = 0f;
            pointerContactTarget = null;
            destinationPosition = Vector3.zero;

            controller.AliasPointerOn -= new ControllerInteractionEventHandler(EnablePointerBeam);
            controller.AliasPointerOff -= new ControllerInteractionEventHandler(DisablePointerBeam);
            controller.AliasPointerSet -= new ControllerInteractionEventHandler(SetPointerDestination);
        }

        protected virtual void Update()
        {
        }

        protected virtual void InitPointer()
        {
        }

        protected virtual void UpdateDependencies(Vector3 location)
        {
            if (playAreaCursor)
            {
                playAreaCursor.SetPlayAreaCursorTransform(location);
            }
        }

        protected virtual void EnablePointerBeam(object sender, ControllerInteractionEventArgs e)
        {
            TurnOnBeam(e.controllerIndex);
        }

        protected virtual void DisablePointerBeam(object sender, ControllerInteractionEventArgs e)
        {
            TurnOffBeam(e.controllerIndex);
        }

        protected virtual void SetPointerDestination(object sender, ControllerInteractionEventArgs e)
        {
            PointerSet();
        }

        protected virtual void PointerIn()
        {
            if (!enabled || !pointerContactTarget)
            {
                return;
            }

            OnDestinationMarkerEnter(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerIndex));
            StartUseAction(pointerContactTarget);
        }

        protected virtual void PointerOut()
        {
            if (!enabled || !pointerContactTarget)
            {
                return;
            }

            OnDestinationMarkerExit(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerIndex));
            StopUseAction();
        }

        protected virtual void PointerSet()
        {
            if (!enabled || !destinationSetActive || !pointerContactTarget || !CanActivate() || InvalidConstantBeam())
            {
                return;
            }

            activateDelayTimer = Time.time + activateDelay;

            var setInteractableObject = pointerContactTarget.GetComponent<VRTK_InteractableObject>();
            if (PointerActivatesUseAction(setInteractableObject))
            {
                if (setInteractableObject.IsUsing())
                {
                    setInteractableObject.StopUsing(gameObject);
                }
                else if (!setInteractableObject.holdButtonToUse)
                {
                    setInteractableObject.StartUsing(gameObject);
                }
            }

            if ((!playAreaCursor || !playAreaCursor.HasCollided()) && !PointerActivatesUseAction(interactableObject))
            {
                OnDestinationMarkerSet(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerIndex));
            }

            if (!isActive)
            {
                destinationSetActive = false;
            }
        }

        protected virtual void TogglePointer(bool state)
        {
            if (playAreaCursor)
            {
                playAreaCursor.SetHeadsetPositionCompensation(headsetPositionCompensation);
                playAreaCursor.ToggleState(state);
            }

            if (!state && PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && interactableObject.IsUsing())
            {
                interactableObject.StopUsing(gameObject);
            }
        }

        protected virtual void SetPointerMaterial()
        {
            if (playAreaCursor)
            {
                playAreaCursor.SetMaterial(pointerMaterial);
            }
        }

        protected void UpdatePointerMaterial(Color color)
        {
            if ((playAreaCursor && playAreaCursor.HasCollided()) || !ValidDestination(pointerContactTarget, destinationPosition))
            {
                color = pointerMissColor;
            }
            pointerMaterial.color = color;
            SetPointerMaterial();
        }

        protected virtual bool ValidDestination(Transform target, Vector3 destinationPosition)
        {
            bool validNavMeshLocation = false;
            if (target)
            {
                NavMeshHit hit;
                validNavMeshLocation = NavMesh.SamplePosition(destinationPosition, out hit, 0.1f, NavMesh.AllAreas);
            }
            if (navMeshCheckDistance == 0f)
            {
                validNavMeshLocation = true;
            }
            return (validNavMeshLocation && target && !(Utilities.TagOrScriptCheck(target.gameObject, invalidTagOrScriptListPolicy, invalidTargetWithTagOrClass)));
        }

        private bool InvalidConstantBeam()
        {
            var equalToggleSet = controller.pointerToggleButton == controller.pointerSetButton;
            return (!holdButtonToActivate && ((equalToggleSet && beamEnabledState != 0) || (!equalToggleSet && !isActive)));
        }

        private bool PointerActivatesUseAction(VRTK_InteractableObject io)
        {
            return (io && io.pointerActivatesUseAction && io.IsValidInteractableController(controller.gameObject, io.allowedUseControllers));
        }

        private void StartUseAction(Transform target)
        {
            interactableObject = target.GetComponent<VRTK_InteractableObject>();
            bool cannotUseBecauseNotGrabbed = (interactableObject && interactableObject.useOnlyIfGrabbed && !interactableObject.IsGrabbed());

            if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && !cannotUseBecauseNotGrabbed)
            {
                interactableObject.StartUsing(gameObject);
            }
        }

        private void StopUseAction()
        {
            if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse)
            {
                interactableObject.StopUsing(gameObject);
            }
        }

        private void TurnOnBeam(uint index)
        {
            beamEnabledState++;
            if (enabled && !isActive && CanActivate())
            {
                if (playAreaCursor)
                {
                    playAreaCursor.SetPlayAreaCursorCollision(false);
                }
                controllerIndex = index;
                TogglePointer(true);
                isActive = true;
                destinationSetActive = true;
            }
        }

        private void TurnOffBeam(uint index)
        {
            if (enabled && isActive && (holdButtonToActivate || (!holdButtonToActivate && beamEnabledState >= 2)))
            {
                controllerIndex = index;
                DisableBeam();
            }
        }

        private void DisableBeam()
        {
            TogglePointer(false);
            isActive = false;
            beamEnabledState = 0;
        }
    }
}