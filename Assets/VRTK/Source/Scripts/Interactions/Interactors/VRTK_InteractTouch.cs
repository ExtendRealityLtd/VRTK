// Interact Touch|Interactors|30020
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerReference">The reference to the controller doing the interaction.</param>
    /// <param name="target">The GameObject of the Interactable Object that is being interacted with.</param>
    public struct ObjectInteractEventArgs
    {
        public VRTK_ControllerReference controllerReference;
        public GameObject target;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ObjectInteractEventArgs"/></param>
    public delegate void ObjectInteractEventHandler(object sender, ObjectInteractEventArgs e);

    /// <summary>
    /// Determines if a GameObject can initiate a touch with an Interactable Object.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `Rigidbody` - A Unity kinematic Rigidbody to determine when collisions happen between the Interact Touch GameObject and other valid colliders.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_InteractTouch` script on the controller script alias GameObject of the controller to track (e.g. Right Controller Script Alias).
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller/BasicObjectGrabbing` demonstrates the highlighting of objects that have the `VRTK_InteractableObject` script added to them to show the ability to highlight interactable objects when they are touched by the controllers.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactors/VRTK_InteractTouch")]
    public class VRTK_InteractTouch : MonoBehaviour
    {
        [Tooltip("An optional GameObject that contains the compound colliders to represent the touching object. If this is empty then the collider will be auto generated at runtime to match the SDK default controller.")]
        public GameObject customColliderContainer;

        /// <summary>
        /// Emitted when the touch of a valid object has started.
        /// </summary>
        public event ObjectInteractEventHandler ControllerStartTouchInteractableObject;
        /// <summary>
        /// Emitted when a valid object is touched.
        /// </summary>
        public event ObjectInteractEventHandler ControllerTouchInteractableObject;
        /// <summary>
        /// Emitted when the untouch of a valid object has started.
        /// </summary>
        public event ObjectInteractEventHandler ControllerStartUntouchInteractableObject;
        /// <summary>
        /// Emitted when a valid object is no longer being touched.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUntouchInteractableObject;
        /// <summary>
        /// Emitted when the controller rigidbody is activated.
        /// </summary>
        public event ObjectInteractEventHandler ControllerRigidbodyActivated;
        /// <summary>
        /// Emitted when the controller rigidbody is deactivated.
        /// </summary>
        public event ObjectInteractEventHandler ControllerRigidbodyDeactivated;

        protected GameObject touchedObject = null;
        protected List<Collider> touchedObjectColliders = new List<Collider>();
        protected List<Collider> touchedObjectActiveColliders = new List<Collider>();
        protected GameObject controllerCollisionDetector;
        protected bool destroyColliderOnDisable;
        protected bool triggerIsColliding = false;
        protected bool triggerWasColliding = false;
        protected bool rigidBodyForcedActive = false;
        protected Rigidbody touchRigidBody;
        protected VRTK_TrackedController trackedController;

        protected VRTK_ControllerReference controllerReference
        {
            get
            {
                return VRTK_ControllerReference.GetControllerReference(gameObject);
            }
        }

        public virtual void OnControllerStartTouchInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerStartTouchInteractableObject != null)
            {
                ControllerStartTouchInteractableObject(this, e);
            }
        }

        public virtual void OnControllerTouchInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerTouchInteractableObject != null)
            {
                ControllerTouchInteractableObject(this, e);
            }
        }

        public virtual void OnControllerStartUntouchInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerStartUntouchInteractableObject != null)
            {
                ControllerStartUntouchInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUntouchInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUntouchInteractableObject != null)
            {
                ControllerUntouchInteractableObject(this, e);
            }
        }

        public virtual void OnControllerRigidbodyActivated(ObjectInteractEventArgs e)
        {
            if (ControllerRigidbodyActivated != null)
            {
                ControllerRigidbodyActivated(this, e);
            }
        }

        public virtual void OnControllerRigidbodyDeactivated(ObjectInteractEventArgs e)
        {
            if (ControllerRigidbodyDeactivated != null)
            {
                ControllerRigidbodyDeactivated(this, e);
            }
        }

        public virtual ObjectInteractEventArgs SetControllerInteractEvent(GameObject target)
        {
            ObjectInteractEventArgs e;
            e.controllerReference = controllerReference;
            e.target = target;
            return e;
        }

        /// <summary>
        /// The ForceTouch method will attempt to force the Interact Touch onto the given GameObject.
        /// </summary>
        /// <param name="obj">The GameObject to attempt to force touch.</param>
        public virtual void ForceTouch(GameObject obj)
        {
            Collider objCollider = (obj != null ? obj.GetComponentInChildren<Collider>() : null);
            if (objCollider != null)
            {
                OnTriggerStay(objCollider);
            }
        }

        /// <summary>
        /// The GetTouchedObject method returns the current GameObject being touched by the Interact Touch.
        /// </summary>
        /// <returns>The GameObject of what is currently being touched by this Interact Touch.</returns>
        public virtual GameObject GetTouchedObject()
        {
            return touchedObject;
        }

        /// <summary>
        /// The IsObjectInteractable method is used to check if a given GameObject is a valid Interactable Object.
        /// </summary>
        /// <param name="obj">The GameObject to check to see if it's a valid Interactable Object.</param>
        /// <returns>Returns `true` if the given GameObjectis a valid Interactable Object.</returns>
        public virtual bool IsObjectInteractable(GameObject obj)
        {
            if (obj != null)
            {
                VRTK_InteractableObject io = obj.GetComponentInParent<VRTK_InteractableObject>();
                if (io != null)
                {
                    if (io.disableWhenIdle && !io.enabled)
                    {
                        return true;
                    }
                    return io.enabled;
                }
            }
            return false;
        }

        /// <summary>
        /// The ToggleControllerRigidBody method toggles the Interact Touch rigidbody's ability to detect collisions. If it is true then the controller rigidbody will collide with other collidable GameObjects.
        /// </summary>
        /// <param name="state">The state of whether the rigidbody is on or off. `true` toggles the rigidbody on and `false` turns it off.</param>
        /// <param name="forceToggle">Determines if the rigidbody has been forced into it's new state by another script. This can be used to override other non-force settings. Defaults to `false`</param>
        public virtual void ToggleControllerRigidBody(bool state, bool forceToggle = false)
        {
            if (controllerCollisionDetector != null && touchRigidBody != null)
            {
                touchRigidBody.isKinematic = !state;
                rigidBodyForcedActive = forceToggle;
                Collider[] foundColliders = controllerCollisionDetector.GetComponentsInChildren<Collider>();
                for (int i = 0; i < foundColliders.Length; i++)
                {
                    foundColliders[i].isTrigger = !state;
                }
                EmitControllerRigidbodyEvent(state);
            }
        }

        /// <summary>
        /// The IsRigidBodyActive method checks to see if the rigidbody on the Interact Touch is active and can affect other rigidbodies in the scene.
        /// </summary>
        /// <returns>Returns `true` if the rigidbody on the Interact Touch is currently active and able to affect other scene rigidbodies.</returns>
        public virtual bool IsRigidBodyActive()
        {
            return !touchRigidBody.isKinematic;
        }

        /// <summary>
        /// The IsRigidBodyForcedActive method checks to see if the rigidbody on the Interact Touch has been forced into the active state.
        /// </summary>
        /// <returns>Returns `true` if the rigidbody is active and has been forced into the active state.</returns>
        public virtual bool IsRigidBodyForcedActive()
        {
            return (IsRigidBodyActive() && rigidBodyForcedActive);
        }

        /// <summary>
        /// The ForceStopTouching method will stop the Interact Touch from touching an Interactable Object even if the Interact Touch is physically touching the Interactable Object.
        /// </summary>
        public virtual void ForceStopTouching()
        {
            if (touchedObject != null)
            {
                StopTouching(touchedObject);
            }
        }

        /// <summary>
        /// The ControllerColliders method retrieves all of the associated colliders on the Interact Touch.
        /// </summary>
        /// <returns>An array of colliders that are associated with the controller.</returns>
        public virtual Collider[] ControllerColliders()
        {
            return (controllerCollisionDetector != null ? controllerCollisionDetector.GetComponentsInChildren<Collider>() : new Collider[0]);
        }

        /// <summary>
        /// The GetControllerType method is a shortcut to retrieve the current controller type the Interact Touch is attached to.
        /// </summary>
        /// <returns>The type of controller that the Interact Touch is attached to.</returns>
        public virtual SDK_BaseController.ControllerType GetControllerType()
        {
            return (trackedController != null ? trackedController.GetControllerType() : SDK_BaseController.ControllerType.Undefined);
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            destroyColliderOnDisable = false;
            controllerCollisionDetector = (customColliderContainer != null ? customColliderContainer : controllerCollisionDetector);
            VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Controller);
            CreateTouchRigidBody();
            trackedController = GetComponentInParent<VRTK_TrackedController>();
            if (trackedController != null)
            {
                trackedController.ControllerModelAvailable += DoControllerModelAvailable;
            }
        }

        protected virtual void OnDisable()
        {
            ForceStopTouching();
            DestroyTouchCollider();
            if (trackedController != null)
            {
                trackedController.ControllerModelAvailable -= DoControllerModelAvailable;
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            GameObject colliderInteractableObject = TriggerStart(collider);
            VRTK_InteractableObject touchedObjectScript = (touchedObject != null ? touchedObject.GetComponent<VRTK_InteractableObject>() : null);
            //If the new collider is not part of the existing touched object (and the object isn't being grabbed) then start touching the new object
            if (touchedObject != null && colliderInteractableObject != null && touchedObject != colliderInteractableObject && touchedObjectScript != null && !touchedObjectScript.IsGrabbed())
            {
                ForceStopTouching();
                triggerIsColliding = true;
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            touchedObjectActiveColliders.Remove(collider);
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            GameObject colliderInteractableObject = TriggerStart(collider);

            if (touchedObject == null || collider.transform.IsChildOf(touchedObject.transform))
            {
                triggerIsColliding = true;
            }

            if (touchedObject == null && colliderInteractableObject != null && IsObjectInteractable(collider.gameObject))
            {
                touchedObject = colliderInteractableObject;
                VRTK_InteractableObject touchedObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();

                //If this controller is not allowed to touch this interactable object then clean up touch and return before initiating a touch.
                if (touchedObjectScript != null && !touchedObjectScript.IsValidInteractableController(gameObject, touchedObjectScript.allowedTouchControllers))
                {
                    CleanupEndTouch();
                    return;
                }
                OnControllerStartTouchInteractableObject(SetControllerInteractEvent(touchedObject));
                StoreTouchedObjectColliders(collider);

                ToggleControllerVisibility(false);
                touchedObjectScript.StartTouching(this);

                OnControllerTouchInteractableObject(SetControllerInteractEvent(touchedObject));
            }
        }

        protected virtual void FixedUpdate()
        {
            if (!triggerIsColliding && !triggerWasColliding)
            {
                CheckStopTouching();
            }
            triggerWasColliding = triggerIsColliding;
            triggerIsColliding = false;
        }

        protected virtual void LateUpdate()
        {
            if (touchedObjectActiveColliders.Count == 0)
            {
                CheckStopTouching();
            }
        }

        protected virtual void DoControllerModelAvailable(object sender, VRTKTrackedControllerEventArgs e)
        {
            CreateTouchCollider();
        }

        protected virtual GameObject GetColliderInteractableObject(Collider collider)
        {
            VRTK_InteractableObject checkIO = collider.GetComponentInParent<VRTK_InteractableObject>();
            return (checkIO != null ? checkIO.gameObject : null);
        }

        protected virtual void AddActiveCollider(Collider collider)
        {
            if (touchedObject != null && touchedObjectColliders.Contains(collider))
            {
                VRTK_SharedMethods.AddListValue(touchedObjectActiveColliders, collider, true);
            }
        }

        protected virtual void StoreTouchedObjectColliders(Collider collider)
        {
            touchedObjectColliders.Clear();
            touchedObjectActiveColliders.Clear();
            Collider[] touchedObjectChildColliders = touchedObject.GetComponentsInChildren<Collider>();
            for (int i = 0; i < touchedObjectChildColliders.Length; i++)
            {
                VRTK_SharedMethods.AddListValue(touchedObjectColliders, touchedObjectChildColliders[i], true);
            }
            VRTK_SharedMethods.AddListValue(touchedObjectActiveColliders, collider, true);
        }

        protected virtual void ToggleControllerVisibility(bool visible)
        {
            GameObject modelContainer = VRTK_DeviceFinder.GetModelAliasController(gameObject);
            if (touchedObject != null)
            {
                ///[Obsolete]
#pragma warning disable 0618
                VRTK_InteractControllerAppearance[] controllerAppearanceScript = touchedObject.GetComponentsInParent<VRTK_InteractControllerAppearance>(true);
#pragma warning restore 0618
                if (controllerAppearanceScript.Length > 0)
                {
                    controllerAppearanceScript[0].ToggleControllerOnTouch(visible, modelContainer, touchedObject);
                }
            }
            else if (visible)
            {
                VRTK_ObjectAppearance.SetRendererVisible(modelContainer, touchedObject);
            }
        }

        protected virtual void CheckStopTouching()
        {
            if (touchedObject != null)
            {
                VRTK_InteractableObject touchedObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();

                //If it's being grabbed by the current touching object then it hasn't stopped being touched.
                if (touchedObjectScript != null && touchedObjectScript.GetGrabbingObject() != gameObject)
                {
                    StopTouching(touchedObject);
                }
            }
        }

        protected virtual GameObject TriggerStart(Collider collider)
        {
            if (IsSnapDropZone(collider))
            {
                return null;
            }

            AddActiveCollider(collider);

            return GetColliderInteractableObject(collider);
        }

        protected virtual bool IsSnapDropZone(Collider collider)
        {
            if (collider.GetComponent<VRTK_SnapDropZone>())
            {
                return true;
            }
            return false;
        }

        protected virtual void StopTouching(GameObject untouched)
        {
            OnControllerStartUntouchInteractableObject(SetControllerInteractEvent(untouched));
            if (IsObjectInteractable(untouched))
            {
                VRTK_InteractableObject untouchedObjectScript = (untouched != null ? untouched.GetComponent<VRTK_InteractableObject>() : null);
                if (untouchedObjectScript != null)
                {
                    untouchedObjectScript.StopTouching(this);
                }
            }

            ToggleControllerVisibility(true);
            OnControllerUntouchInteractableObject(SetControllerInteractEvent(untouched));
            CleanupEndTouch();
        }

        protected virtual void CleanupEndTouch()
        {
            touchedObject = null;
            touchedObjectActiveColliders.Clear();
            touchedObjectColliders.Clear();
        }

        protected virtual void DestroyTouchCollider()
        {
            if (destroyColliderOnDisable)
            {
                Destroy(controllerCollisionDetector);
            }
        }

        protected virtual bool CustomRigidBodyIsChild()
        {
            Transform[] childTransforms = GetComponentsInChildren<Transform>();
            for (int i = 0; i < childTransforms.Length; i++)
            {
                Transform childTransform = childTransforms[i];
                if (childTransform != transform && childTransform == customColliderContainer.transform)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void CreateTouchCollider()
        {
            SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(gameObject);
            Object defaultColliderPrefab = Resources.Load(VRTK_SDK_Bridge.GetControllerDefaultColliderPath(controllerHand));

            if (customColliderContainer == null)
            {
                if (defaultColliderPrefab == null)
                {
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "default collider prefab", "Controller SDK"));
                    return;
                }
                controllerCollisionDetector = Instantiate(defaultColliderPrefab, transform.position, transform.rotation) as GameObject;
                controllerCollisionDetector.transform.SetParent(transform);
                controllerCollisionDetector.transform.localScale = transform.localScale;
                controllerCollisionDetector.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "Controller", "CollidersContainer");
                destroyColliderOnDisable = true;
            }
            else
            {
                if (CustomRigidBodyIsChild())
                {
                    controllerCollisionDetector = customColliderContainer;
                    destroyColliderOnDisable = false;
                }
                else
                {
                    controllerCollisionDetector = Instantiate(customColliderContainer, transform.position, transform.rotation) as GameObject;
                    controllerCollisionDetector.transform.SetParent(transform);
                    controllerCollisionDetector.transform.localScale = transform.localScale;
                    destroyColliderOnDisable = true;
                }
            }
            controllerCollisionDetector.AddComponent<VRTK_PlayerObject>().objectType = VRTK_PlayerObject.ObjectTypes.Collider;
        }

        protected virtual void CreateTouchRigidBody()
        {
            touchRigidBody = (GetComponent<Rigidbody>() != null ? GetComponent<Rigidbody>() : gameObject.AddComponent<Rigidbody>());
            touchRigidBody.isKinematic = true;
            touchRigidBody.useGravity = false;
            touchRigidBody.constraints = RigidbodyConstraints.FreezeAll;
            touchRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        protected virtual void EmitControllerRigidbodyEvent(bool state)
        {
            if (state)
            {
                OnControllerRigidbodyActivated(SetControllerInteractEvent(null));
            }
            else
            {
                OnControllerRigidbodyDeactivated(SetControllerInteractEvent(null));
            }
        }
    }
}