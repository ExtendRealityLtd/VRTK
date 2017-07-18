// Interact Touch|Interactions|30050
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">**OBSOLETE** The index of the controller doing the interaction.</param>
    /// <param name="controllerReference">The reference to the controller doing the interaction.</param>
    /// <param name="target">The GameObject of the interactable object that is being interacted with by the controller.</param>
    public struct ObjectInteractEventArgs
    {
        [System.Obsolete("`ObjectInteractEventArgs.controllerIndex` has been replaced with `ObjectInteractEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
        public uint controllerIndex;
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
    /// The Interact Touch script is usually applied to a Controller and provides a collider to know when the controller is touching something.
    /// </summary>
    /// <remarks>
    /// Colliders are created for the controller and by default the selected controller SDK will have a set of colliders for the given default controller of that SDK.
    ///
    /// A custom collider can be provided by the Custom Rigidbody Object parameter.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller/BasicObjectGrabbing` demonstrates the highlighting of objects that have the `VRTK_InteractableObject` script added to them to show the ability to highlight interactable objects when they are touched by the controllers.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractTouch")]
    public class VRTK_InteractTouch : MonoBehaviour
    {
        [Tooltip("An optional GameObject that contains the compound colliders to represent the touching object. If this is empty then the collider will be auto generated at runtime to match the SDK default controller.")]
        public GameObject customColliderContainer;

        /// <summary>
        /// Emitted when a valid object is touched.
        /// </summary>
        public event ObjectInteractEventHandler ControllerTouchInteractableObject;
        /// <summary>
        /// Emitted when a valid object is no longer being touched.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUntouchInteractableObject;

        protected GameObject touchedObject = null;
        protected List<Collider> touchedObjectColliders = new List<Collider>();
        protected List<Collider> touchedObjectActiveColliders = new List<Collider>();
        protected GameObject controllerCollisionDetector;
        protected bool triggerRumble;
        protected bool destroyColliderOnDisable;
        protected bool triggerIsColliding = false;
        protected bool triggerWasColliding = false;
        protected bool rigidBodyForcedActive = false;
        protected Rigidbody touchRigidBody;
        protected Object defaultColliderPrefab;
        protected VRTK_ControllerReference controllerReference
        {
            get
            {
                return VRTK_ControllerReference.GetControllerReference(gameObject);
            }
        }

        public virtual void OnControllerTouchInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerTouchInteractableObject != null)
            {
                ControllerTouchInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUntouchInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUntouchInteractableObject != null)
            {
                ControllerUntouchInteractableObject(this, e);
            }
        }

        public virtual ObjectInteractEventArgs SetControllerInteractEvent(GameObject target)
        {
            ObjectInteractEventArgs e;
#pragma warning disable 0618
            e.controllerIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
#pragma warning restore 0618
            e.controllerReference = controllerReference;
            e.target = target;
            return e;
        }

        /// <summary>
        /// The ForceTouch method will attempt to force the controller to touch the given game object. This is useful if an object that isn't being touched is required to be grabbed or used as the controller doesn't physically have to be touching it to be forced to interact with it.
        /// </summary>
        /// <param name="obj">The game object to attempt to force touch.</param>
        public virtual void ForceTouch(GameObject obj)
        {
            var objCollider = obj.GetComponentInChildren<Collider>();
            if (objCollider != null)
            {
                OnTriggerStay(objCollider);
            }
        }

        /// <summary>
        /// The GetTouchedObject method returns the current object being touched by the controller.
        /// </summary>
        /// <returns>The game object of what is currently being touched by this controller.</returns>
        public virtual GameObject GetTouchedObject()
        {
            return touchedObject;
        }

        /// <summary>
        /// The IsObjectInteractable method is used to check if a given game object is of type `VRTK_InteractableObject` and whether the object is enabled.
        /// </summary>
        /// <param name="obj">The game object to check to see if it's interactable.</param>
        /// <returns>Is true if the given object is of type `VRTK_InteractableObject`.</returns>
        public virtual bool IsObjectInteractable(GameObject obj)
        {
            if (obj != null)
            {
                var io = obj.GetComponentInParent<VRTK_InteractableObject>();
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
        /// The ToggleControllerRigidBody method toggles the controller's rigidbody's ability to detect collisions. If it is true then the controller rigidbody will collide with other collidable game objects.
        /// </summary>
        /// <param name="state">The state of whether the rigidbody is on or off. `true` toggles the rigidbody on and `false` turns it off.</param>
        /// <param name="forceToggle">Determines if the rigidbody has been forced into it's new state by another script. This can be used to override other non-force settings. Defaults to `false`</param>
        public virtual void ToggleControllerRigidBody(bool state, bool forceToggle = false)
        {
            if (controllerCollisionDetector != null && touchRigidBody != null)
            {
                touchRigidBody.isKinematic = !state;
                rigidBodyForcedActive = forceToggle;
                foreach (var collider in controllerCollisionDetector.GetComponentsInChildren<Collider>())
                {
                    collider.isTrigger = !state;
                }
            }
        }

        /// <summary>
        /// The IsRigidBodyActive method checks to see if the rigidbody on the controller object is active and can affect other rigidbodies in the scene.
        /// </summary>
        /// <returns>Is true if the rigidbody on the controller is currently active and able to affect other scene rigidbodies.</returns>
        public virtual bool IsRigidBodyActive()
        {
            return !touchRigidBody.isKinematic;
        }

        /// <summary>
        /// The IsRigidBodyForcedActive method checks to see if the rigidbody on the controller object has been forced into the active state.
        /// </summary>
        /// <returns>Is true if the rigidbody is active and has been forced into the active state.</returns>
        public virtual bool IsRigidBodyForcedActive()
        {
            return (IsRigidBodyActive() && rigidBodyForcedActive);
        }

        /// <summary>
        /// The ForceStopTouching method will stop the controller from touching an object even if the controller is physically touching the object still.
        /// </summary>
        public virtual void ForceStopTouching()
        {
            if (touchedObject != null)
            {
                StopTouching(touchedObject);
            }
        }

        /// <summary>
        /// The ControllerColliders method retrieves all of the associated colliders on the controller.
        /// </summary>
        /// <returns>An array of colliders that are associated with the controller.</returns>
        public virtual Collider[] ControllerColliders()
        {
            return (controllerCollisionDetector.GetComponents<Collider>().Length > 0 ? controllerCollisionDetector.GetComponents<Collider>() : controllerCollisionDetector.GetComponentsInChildren<Collider>());
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            destroyColliderOnDisable = false;
            SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(gameObject);
            defaultColliderPrefab = Resources.Load(VRTK_SDK_Bridge.GetControllerDefaultColliderPath(controllerHand));

            VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Controller);
            triggerRumble = false;
            CreateTouchCollider();
            CreateTouchRigidBody();
        }

        protected virtual void OnDisable()
        {
            ForceStopTouching();
            DestroyTouchCollider();
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            GameObject colliderInteractableObject = TriggerStart(collider);
            //If the new collider is not part of the existing touched object (and the object isn't being grabbed) then start touching the new object
            if (touchedObject != null && colliderInteractableObject && touchedObject != colliderInteractableObject && !touchedObject.GetComponent<VRTK_InteractableObject>().IsGrabbed())
            {
                CancelInvoke("ResetTriggerRumble");
                ResetTriggerRumble();
                ForceStopTouching();
                triggerIsColliding = true;
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            if (touchedObjectActiveColliders.Contains(collider))
            {
                touchedObjectActiveColliders.Remove(collider);
            }
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            GameObject colliderInteractableObject = TriggerStart(collider);

            if (touchedObject == null || collider.transform.IsChildOf(touchedObject.transform))
            {
                triggerIsColliding = true;
            }

            if (touchedObject == null && colliderInteractableObject && IsObjectInteractable(collider.gameObject))
            {
                touchedObject = colliderInteractableObject;
                var touchedObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();
                GameObject touchingObject = gameObject;

                //If this controller is not allowed to touch this interactable object then clean up touch and return before initiating a touch.
                if (!touchedObjectScript.IsValidInteractableController(gameObject, touchedObjectScript.allowedTouchControllers))
                {
                    CleanupEndTouch();
                    return;
                }
                StoreTouchedObjectColliders(collider);

                touchedObjectScript.ToggleHighlight(true);
                ToggleControllerVisibility(false);
                CheckRumbleController(touchedObjectScript);
                touchedObjectScript.StartTouching(touchingObject);

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

        protected virtual GameObject GetColliderInteractableObject(Collider collider)
        {
            var checkIO = collider.GetComponentInParent<VRTK_InteractableObject>();
            return (checkIO ? checkIO.gameObject : null);
        }

        protected virtual void AddActiveCollider(Collider collider)
        {
            if (touchedObject != null && !touchedObjectActiveColliders.Contains(collider) && touchedObjectColliders.Contains(collider))
            {
                touchedObjectActiveColliders.Add(collider);
            }
        }

        protected virtual void StoreTouchedObjectColliders(Collider collider)
        {
            touchedObjectColliders.Clear();
            touchedObjectActiveColliders.Clear();
            foreach (var touchedObjectCollider in touchedObject.GetComponentsInChildren<Collider>())
            {
                touchedObjectColliders.Add(touchedObjectCollider);
            }
            touchedObjectActiveColliders.Add(collider);
        }

        protected virtual void ToggleControllerVisibility(bool visible)
        {
            GameObject modelContainer = VRTK_DeviceFinder.GetModelAliasController(gameObject);
            if (touchedObject != null)
            {
                VRTK_InteractControllerAppearance[] controllerAppearanceScript = touchedObject.GetComponentsInParent<VRTK_InteractControllerAppearance>(true);
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

        protected virtual void CheckRumbleController(VRTK_InteractableObject touchedObjectScript)
        {
            if (!triggerRumble)
            {
                var doHaptics = touchedObject.GetComponentInParent<VRTK_InteractHaptics>();
                if (doHaptics != null)
                {
                    triggerRumble = true;
                    doHaptics.HapticsOnTouch(controllerReference);
                    Invoke("ResetTriggerRumble", doHaptics.durationOnTouch);
                }
            }
        }

        protected virtual void CheckStopTouching()
        {
            if (touchedObject != null)
            {
                var touchedObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();
                GameObject touchingObject = gameObject;

                //If it's being grabbed by the current touching object then it hasn't stopped being touched.
                if (touchedObjectScript != null && touchedObjectScript.GetGrabbingObject() != touchingObject)
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

        protected virtual void ResetTriggerRumble()
        {
            triggerRumble = false;
        }

        protected virtual void StopTouching(GameObject untouched)
        {
            if (IsObjectInteractable(untouched))
            {
                GameObject touchingObject = gameObject;
                var untouchedObjectScript = untouched.GetComponent<VRTK_InteractableObject>();
                untouchedObjectScript.StopTouching(touchingObject);
                if (!untouchedObjectScript.IsTouched())
                {
                    untouchedObjectScript.ToggleHighlight(false);
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
            foreach (var childTransform in GetComponentsInChildren<Transform>())
            {
                if (childTransform != transform && childTransform == customColliderContainer.transform)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void CreateTouchCollider()
        {
            if (customColliderContainer == null)
            {
                if (!defaultColliderPrefab)
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
            touchRigidBody = (GetComponent<Rigidbody>() ? GetComponent<Rigidbody>() : gameObject.AddComponent<Rigidbody>());
            touchRigidBody.isKinematic = true;
            touchRigidBody.useGravity = false;
            touchRigidBody.constraints = RigidbodyConstraints.FreezeAll;
            touchRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }
}