// Base Pointer|Pointers|10020
namespace VRTK
{
    using UnityEngine;
#if UNITY_5_5_OR_NEWER
    using UnityEngine.AI;
#endif

    /// <summary>
    /// This abstract class provides any game pointer the ability to know the state of the implemented pointer.
    /// </summary>
    /// <remarks>
    /// It extends the `VRTK_DestinationMarker` to allow for destination events to be emitted when the pointer cursor collides with objects.
    ///
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BasePointer : VRTK_DestinationMarker
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

        [Header("Base Pointer Settings", order = 2)]

        [Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controller = null;
        [Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
        public Transform pointerOriginTransform = null;
        [Tooltip("The material to use on the rendered version of the pointer. If no material is selected then the default `WorldPointer` material will be used.")]
        public Material pointerMaterial;
        [Tooltip("The colour of the beam when it is colliding with a valid target. It can be set to a different colour for each controller.")]
        public Color pointerHitColor = new Color(0f, 0.5f, 0f, 1f);
        [Tooltip("The colour of the beam when it is not hitting a valid target. It can be set to a different colour for each controller.")]
        public Color pointerMissColor = new Color(0.8f, 0f, 0f, 1f);
        [Tooltip("If this is checked then the pointer beam will be activated on first press of the pointer alias button and will stay active until the pointer alias button is pressed again. The destination set event is emitted when the beam is deactivated on the second button press.")]
        public bool holdButtonToActivate = true;
        [Tooltip("If this is checked then the pointer will be an extension of the controller and able to interact with Interactable Objects.")]
        public bool interactWithObjects = false;
        [Tooltip("If `Interact With Objects` is checked and this is checked then when an object is grabbed with the pointer touching it, the object will attach to the pointer tip and not snap to the controller.")]
        public bool grabToPointerTip = false;
        [Tooltip("The time in seconds to delay the pointer beam being able to be active again. Useful for preventing constant teleportation.")]
        public float activateDelay = 0f;
        [Tooltip("Determines when the pointer beam should be displayed.")]
        public pointerVisibilityStates pointerVisibility = pointerVisibilityStates.On_When_Active;
        [Tooltip("The layers to ignore when raycasting.")]
        public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;

        protected Vector3 destinationPosition;
        protected float pointerContactDistance = 0f;
        protected Transform pointerContactTarget = null;
        protected RaycastHit pointerContactRaycastHit = new RaycastHit();
        protected uint controllerIndex;
        protected VRTK_PlayAreaCursor playAreaCursor;
        protected Color currentPointerColor;
        protected GameObject objectInteractor;
        protected GameObject objectInteractorAttachPoint;

        private bool isActive;
        private bool destinationSetActive;
        private float activateDelayTimer = 0f;
        private int beamEnabledState = 0;
        private VRTK_InteractableObject interactableObject = null;
        private Rigidbody savedAttachPoint;
        private bool attachedToInteractorAttachPoint = false;
        private float savedBeamLength = 0f;
        private VRTK_InteractGrab controllerGrabScript;

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
            var index = (controller ? VRTK_DeviceFinder.GetControllerIndex(controller.gameObject) : uint.MaxValue);
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
            VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            pointerOriginTransform = (pointerOriginTransform == null ? VRTK_SDK_Bridge.GenerateControllerPointerOrigin(gameObject) : pointerOriginTransform);

            AttemptSetController();

            var tmpMaterial = Resources.Load("WorldPointer") as Material;
            if (pointerMaterial != null)
            {
                tmpMaterial = pointerMaterial;
            }

            pointerMaterial = new Material(tmpMaterial);
            pointerMaterial.color = pointerMissColor;

            var fetchedPlayAreaCursor = GetComponent<VRTK_PlayAreaCursor>();
            playAreaCursor = (fetchedPlayAreaCursor ?? fetchedPlayAreaCursor);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DisableBeam();
            Destroy(objectInteractor);
            destinationSetActive = false;
            pointerContactDistance = 0f;
            pointerContactTarget = null;
            destinationPosition = Vector3.zero;

            AliasRegistration(false);
            controllerGrabScript = null;
        }

        protected virtual void Start()
        {
            SetupController();
        }

        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {
            if (interactWithObjects && objectInteractor && objectInteractor.activeInHierarchy)
            {
                UpdateObjectInteractor();
            }
        }

        protected virtual void AliasRegistration(bool state)
        {
            if (controller)
            {
                if (state)
                {
                    controller.AliasPointerOn += new ControllerInteractionEventHandler(EnablePointerBeam);
                    controller.AliasPointerOff += new ControllerInteractionEventHandler(DisablePointerBeam);
                    controller.AliasPointerSet += new ControllerInteractionEventHandler(SetPointerDestination);
                }
                else
                {
                    controller.AliasPointerOn -= new ControllerInteractionEventHandler(EnablePointerBeam);
                    controller.AliasPointerOff -= new ControllerInteractionEventHandler(DisablePointerBeam);
                    controller.AliasPointerSet -= new ControllerInteractionEventHandler(SetPointerDestination);
                }
            }
        }

        protected virtual Vector3 GetOriginPosition()
        {
            return (pointerOriginTransform ? pointerOriginTransform.position : transform.position);
        }

        protected virtual Vector3 GetOriginLocalPosition()
        {
            return (pointerOriginTransform ? pointerOriginTransform.localPosition : Vector3.zero);
        }

        protected virtual Vector3 GetOriginForward()
        {
            return (pointerOriginTransform ? pointerOriginTransform.forward : transform.forward);
        }

        protected virtual Quaternion GetOriginRotation()
        {
            return (pointerOriginTransform ? pointerOriginTransform.rotation : transform.rotation);
        }

        protected virtual Quaternion GetOriginLocalRotation()
        {
            return (pointerOriginTransform ? pointerOriginTransform.localRotation : Quaternion.identity);
        }

        protected virtual void UpdateObjectInteractor()
        {
            objectInteractor.transform.position = destinationPosition;
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
                    setInteractableObject.StopUsing(controller.gameObject);
                    interactableObject.usingState = 0;
                }
                else if (!setInteractableObject.holdButtonToUse)
                {
                    setInteractableObject.StartUsing(controller.gameObject);
                    interactableObject.usingState++;
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
            ToggleObjectInteraction(state);

            if (playAreaCursor)
            {
                playAreaCursor.SetHeadsetPositionCompensation(headsetPositionCompensation);
                playAreaCursor.ToggleState(state);
            }

            if (!state && PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && interactableObject.IsUsing())
            {
                interactableObject.StopUsing(controller.gameObject);
            }
        }

        protected virtual void ToggleObjectInteraction(bool state)
        {
            if (interactWithObjects)
            {
                if (state && grabToPointerTip && controllerGrabScript)
                {
                    savedAttachPoint = controllerGrabScript.controllerAttachPoint;
                    controllerGrabScript.controllerAttachPoint = objectInteractorAttachPoint.GetComponent<Rigidbody>();
                    attachedToInteractorAttachPoint = true;
                }

                if (!state && grabToPointerTip && controllerGrabScript)
                {
                    if (attachedToInteractorAttachPoint)
                    {
                        controllerGrabScript.ForceRelease(true);
                    }
                    controllerGrabScript.controllerAttachPoint = savedAttachPoint;
                    savedAttachPoint = null;
                    attachedToInteractorAttachPoint = false;
                    savedBeamLength = 0f;
                }

                if (objectInteractor)
                {
                    objectInteractor.SetActive(state);
                }
            }
        }

        protected virtual void ChangeMaterialColor(GameObject obj, Color color)
        {
            foreach (Renderer mr in obj.GetComponentsInChildren<Renderer>())
            {
                if (mr.material)
                {
                    mr.material.EnableKeyword("_EMISSION");

                    if (mr.material.HasProperty("_Color"))
                    {
                        mr.material.color = color;
                    }

                    if (mr.material.HasProperty("_EmissionColor"))
                    {
                        mr.material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, 50));
                    }
                }
            }
        }

        protected virtual void SetPointerMaterial(Color color)
        {
            if (playAreaCursor)
            {
                playAreaCursor.SetMaterialColor(color);
            }
        }

        protected void UpdatePointerMaterial(Color color)
        {
            if ((playAreaCursor && playAreaCursor.HasCollided()) || !ValidDestination(pointerContactTarget, destinationPosition))
            {
                color = pointerMissColor;
            }
            currentPointerColor = color;
            SetPointerMaterial(color);
        }

        protected virtual bool ValidDestination(Transform target, Vector3 destinationPosition)
        {
            bool validNavMeshLocation = false;
            if (target)
            {
                NavMeshHit hit;
                validNavMeshLocation = NavMesh.SamplePosition(destinationPosition, out hit, navMeshCheckDistance, NavMesh.AllAreas);
            }
            if (navMeshCheckDistance == 0f)
            {
                validNavMeshLocation = true;
            }
            return (validNavMeshLocation && target && !(VRTK_PolicyList.Check(target.gameObject, invalidListPolicy)));
        }

        protected virtual void CreateObjectInteractor()
        {
            objectInteractor = new GameObject(string.Format("[{0}]BasePointer_ObjectInteractor_Holder", gameObject.name));
            objectInteractor.transform.SetParent(controller.transform);
            objectInteractor.transform.localPosition = Vector3.zero;
            objectInteractor.layer = LayerMask.NameToLayer("Ignore Raycast");
            VRTK_PlayerObject.SetPlayerObject(objectInteractor, VRTK_PlayerObject.ObjectTypes.Pointer);

            var objectInteractorCollider = new GameObject(string.Format("[{0}]BasePointer_ObjectInteractor_Collider", gameObject.name));
            objectInteractorCollider.transform.SetParent(objectInteractor.transform);
            objectInteractorCollider.transform.localPosition = Vector3.zero;
            objectInteractorCollider.layer = LayerMask.NameToLayer("Ignore Raycast");
            var tmpCollider = objectInteractorCollider.AddComponent<SphereCollider>();
            tmpCollider.isTrigger = true;
            VRTK_PlayerObject.SetPlayerObject(objectInteractorCollider, VRTK_PlayerObject.ObjectTypes.Pointer);

            if (grabToPointerTip)
            {
                objectInteractorAttachPoint = new GameObject(string.Format("[{0}]BasePointer_ObjectInteractor_AttachPoint", gameObject.name));
                objectInteractorAttachPoint.transform.SetParent(objectInteractor.transform);
                objectInteractorAttachPoint.transform.localPosition = Vector3.zero;
                objectInteractorAttachPoint.layer = LayerMask.NameToLayer("Ignore Raycast");
                var objectInteratorRigidBody = objectInteractorAttachPoint.AddComponent<Rigidbody>();
                objectInteratorRigidBody.isKinematic = true;
                objectInteratorRigidBody.freezeRotation = true;
                objectInteratorRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                VRTK_PlayerObject.SetPlayerObject(objectInteractorAttachPoint, VRTK_PlayerObject.ObjectTypes.Pointer);
            }

            var objectInteractorScale = 0.025f;
            objectInteractor.transform.localScale = new Vector3(objectInteractorScale, objectInteractorScale, objectInteractorScale);
            objectInteractor.SetActive(false);
        }

        protected virtual float OverrideBeamLength(float currentLength)
        {
            if (!controllerGrabScript || !controllerGrabScript.GetGrabbedObject())
            {
                savedBeamLength = 0f;
            }

            if (interactWithObjects && grabToPointerTip && attachedToInteractorAttachPoint && controllerGrabScript && controllerGrabScript.GetGrabbedObject())
            {
                savedBeamLength = (savedBeamLength == 0f ? currentLength : savedBeamLength);
                return savedBeamLength;
            }
            return currentLength;
        }

        private void SetupController()
        {
            if (controller == null)
            {
                controller = GetComponent<VRTK_ControllerEvents>();
                AttemptSetController();
            }

            if (controller == null)
            {
                Debug.LogError("VRTK_BasePointer requires a Controller that has the VRTK_ControllerEvents script attached to it");
            }
        }

        private void AttemptSetController()
        {
            if (controller)
            {
                AliasRegistration(true);

                controllerGrabScript = controller.GetComponent<VRTK_InteractGrab>();

                if (interactWithObjects)
                {
                    CreateObjectInteractor();
                }
            }
        }

        private bool InvalidConstantBeam()
        {
            var equalToggleSet = controller.pointerToggleButton == controller.pointerSetButton;
            return (!holdButtonToActivate && ((equalToggleSet && beamEnabledState != 0) || (!equalToggleSet && !isActive)));
        }

        private bool PointerActivatesUseAction(VRTK_InteractableObject io)
        {
            return (io && io.pointerActivatesUseAction && io.IsValidInteractableController(gameObject, io.allowedUseControllers));
        }

        private void StartUseAction(Transform target)
        {
            interactableObject = target.GetComponent<VRTK_InteractableObject>();
            bool cannotUseBecauseNotGrabbed = (interactableObject && interactableObject.useOnlyIfGrabbed && !interactableObject.IsGrabbed());

            if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && !cannotUseBecauseNotGrabbed && interactableObject.usingState == 0)
            {
                interactableObject.StartUsing(controller.gameObject);
                interactableObject.usingState++;
            }
        }

        private void StopUseAction()
        {
            if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse)
            {
                interactableObject.StopUsing(controller.gameObject);
                interactableObject.usingState = 0;
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