// Interact Touch|Scripts|0170
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">The index of the controller doing the interaction.</param>
    /// <param name="target">The GameObject of the interactable object that is being interacted with by the controller.</param>
    public struct ObjectInteractEventArgs
    {
        public uint controllerIndex;
        public GameObject target;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ObjectInteractEventArgs"/></param>
    public delegate void ObjectInteractEventHandler(object sender, ObjectInteractEventArgs e);

    /// <summary>
    /// The Interact Touch script is attached to a Controller object within the `[CameraRig]` prefab.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/005_Controller/BasicObjectGrabbing` demonstrates the highlighting of objects that have the `VRTK_InteractableObject` script added to them to show the ability to highlight interactable objects when they are touched by the controllers.
    /// </example>
    [RequireComponent(typeof(VRTK_ControllerActions))]
    public class VRTK_InteractTouch : MonoBehaviour
    {
        [Tooltip("Hides the controller model when a valid touch occurs.")]
        public bool hideControllerOnTouch = false;
        [Tooltip("The amount of seconds to wait before hiding the controller on touch.")]
        public float hideControllerDelay = 0f;
        [Tooltip("If the interactable object can be highlighted when it's touched but no local colour is set then this global colour is used.")]
        public Color globalTouchHighlightColor = Color.clear;
        [Tooltip("If a custom rigidbody and collider for the rigidbody are required, then a gameobject containing a rigidbody and collider can be passed into this parameter. If this is empty then the rigidbody and collider will be auto generated at runtime to match the HTC Vive default controller.")]
        public GameObject customRigidbodyObject;

        /// <summary>
        /// Emitted when a valid object is touched.
        /// </summary>
        public event ObjectInteractEventHandler ControllerTouchInteractableObject;
        /// <summary>
        /// Emitted when a valid object is no longer being touched.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUntouchInteractableObject;

        private GameObject touchedObject = null;
        private List<Collider> touchedObjectColliders = new List<Collider>();
        private List<Collider> touchedObjectActiveColliders = new List<Collider>();

        private bool updatedHideControllerOnTouch = false;
        private VRTK_ControllerEvents controllerEvents;
        private VRTK_ControllerActions controllerActions;
        private GameObject controllerCollisionDetector;
        private bool triggerRumble;
        private bool destroyColliderOnDisable;
        private Rigidbody touchRigidBody;
        private Object defaultColliderPrefab;
        private VRTK_ControllerEvents.ButtonAlias originalGrabAlias;
        private VRTK_ControllerEvents.ButtonAlias originalUseAlias;

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

        public ObjectInteractEventArgs SetControllerInteractEvent(GameObject target)
        {
            ObjectInteractEventArgs e;
            e.controllerIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            e.target = target;
            return e;
        }

        /// <summary>
        /// The ForceTouch method will attempt to force the controller to touch the given game object. This is useful if an object that isn't being touched is required to be grabbed or used as the controller doesn't physically have to be touching it to be forced to interact with it.
        /// </summary>
        /// <param name="obj">The game object to attempt to force touch.</param>
        public void ForceTouch(GameObject obj)
        {
            if (obj.GetComponent<Collider>())
            {
                OnTriggerStay(obj.GetComponent<Collider>());
            }
            else if (obj.GetComponentInChildren<Collider>())
            {
                OnTriggerStay(obj.GetComponentInChildren<Collider>());
            }
        }

        /// <summary>
        /// The GetTouchedObject method returns the current object being touched by the controller.
        /// </summary>
        /// <returns>The game object of what is currently being touched by this controller.</returns>
        public GameObject GetTouchedObject()
        {
            return touchedObject;
        }

        /// <summary>
        /// The IsObjectInteractable method is used to check if a given game object is of type `VRTK_InteractableObject` and whether the object is enabled.
        /// </summary>
        /// <param name="obj">The game object to check to see if it's interactable.</param>
        /// <returns>Is true if the given object is of type `VRTK_InteractableObject`.</returns>
        public bool IsObjectInteractable(GameObject obj)
        {
            if (obj)
            {
                var io = obj.GetComponent<VRTK_InteractableObject>();
                if (io)
                {
                    return io.enabled;
                }
                else
                {
                    io = obj.GetComponentInParent<VRTK_InteractableObject>();
                    if (io)
                    {
                        return io.enabled;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// The ToggleControllerRigidBody method toggles the controller's rigidbody's ability to detect collisions. If it is true then the controller rigidbody will collide with other collidable game objects.
        /// </summary>
        /// <param name="state">The state of whether the rigidbody is on or off. `true` toggles the rigidbody on and `false` turns it off.</param>
        public void ToggleControllerRigidBody(bool state)
        {
            if (controllerCollisionDetector && touchRigidBody)
            {
                touchRigidBody.isKinematic = !state;
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
        public bool IsRigidBodyActive()
        {
            return !touchRigidBody.isKinematic;
        }

        /// <summary>
        /// The ForceStopTouching method will stop the controller from touching an object even if the controller is physically touching the object still.
        /// </summary>
        public void ForceStopTouching()
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
        public Collider[] ControllerColliders()
        {
            return (controllerCollisionDetector.GetComponents<Collider>().Length > 0 ? controllerCollisionDetector.GetComponents<Collider>() : controllerCollisionDetector.GetComponentsInChildren<Collider>());
        }

        private void Awake()
        {
            controllerEvents = GetComponent<VRTK_ControllerEvents>();
            controllerActions = GetComponent<VRTK_ControllerActions>();
            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Controller);
            destroyColliderOnDisable = false;
            defaultColliderPrefab = Resources.Load("ControllerColliders/HTCVive");
        }

        private void OnEnable()
        {
            triggerRumble = false;
            CreateTouchCollider();
            CreateTouchRigidBody();
            originalGrabAlias = VRTK_ControllerEvents.ButtonAlias.Undefined;
            originalUseAlias = VRTK_ControllerEvents.ButtonAlias.Undefined;
        }

        private void OnDisable()
        {
            ForceStopTouching();
            DestroyTouchCollider();
        }

        private GameObject GetColliderInteractableObject(Collider collider)
        {
            GameObject found = null;
            if (collider.gameObject.GetComponent<VRTK_InteractableObject>())
            {
                found = collider.gameObject;
            }
            else if (collider.transform.parent && collider.gameObject.GetComponentInParent<VRTK_InteractableObject>())
            {
                found = collider.gameObject.GetComponentInParent<VRTK_InteractableObject>().gameObject;
            }
            return found;
        }

        private void AddActiveCollider(Collider collider)
        {
            if (touchedObject != null && !touchedObjectActiveColliders.Contains(collider) && touchedObjectColliders.Contains(collider))
            {
                touchedObjectActiveColliders.Add(collider);
            }
        }

        private void StoreTouchedObjectColliders(Collider collider)
        {
            touchedObjectColliders.Clear();
            touchedObjectActiveColliders.Clear();
            foreach (var touchedObjectCollider in touchedObject.GetComponentsInChildren<Collider>())
            {
                touchedObjectColliders.Add(touchedObjectCollider);
            }
            touchedObjectActiveColliders.Add(collider);
        }

        private void CheckHideController(VRTK_InteractableObject touchedObjectScript)
        {
            updatedHideControllerOnTouch = touchedObjectScript.CheckHideMode(hideControllerOnTouch, touchedObjectScript.hideControllerOnTouch);
            if (controllerActions.IsControllerVisible() && updatedHideControllerOnTouch)
            {
                Invoke("HideController", hideControllerDelay);
            }
        }

        private void CheckRumbleController(VRTK_InteractableObject touchedObjectScript)
        {
            var rumbleAmount = touchedObjectScript.rumbleOnTouch;
            if (!rumbleAmount.Equals(Vector2.zero) && !triggerRumble)
            {
                triggerRumble = true;
                controllerActions.TriggerHapticPulse((ushort)rumbleAmount.y, rumbleAmount.x, 0.05f);
                Invoke("ResetTriggerRumble", rumbleAmount.x);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            AddActiveCollider(collider);

            var colliderInteractableObject = GetColliderInteractableObject(collider);
            //If the new collider is not part of the existing touched object (and the object isn't being grabbed) then start touching the new object
            if (touchedObject != null && touchedObject != colliderInteractableObject && !touchedObject.GetComponent<VRTK_InteractableObject>().IsGrabbed())
            {
                CancelInvoke("ResetTriggerRumble");
                ResetTriggerRumble();
                ForceStopTouching();
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            AddActiveCollider(collider);

            var colliderInteractableObject = GetColliderInteractableObject(collider);
            if (touchedObject == null && IsObjectInteractable(collider.gameObject))
            {
                touchedObject = colliderInteractableObject;
                var touchedObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();

                //If this controller is not allowed to touch this interactable object then clean up touch and return before initiating a touch.
                if (!touchedObjectScript.IsValidInteractableController(gameObject, touchedObjectScript.allowedTouchControllers))
                {
                    CleanupEndTouch();
                    return;
                }

                StoreTouchedObjectColliders(collider);
                CheckButtonOverrides(touchedObjectScript);

                touchedObjectScript.ToggleHighlight(true, globalTouchHighlightColor);

                OnControllerTouchInteractableObject(SetControllerInteractEvent(touchedObject));
                touchedObjectScript.StartTouching(gameObject);

                CheckHideController(touchedObjectScript);
                CheckRumbleController(touchedObjectScript);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (touchedObjectActiveColliders.Contains(collider))
            {
                touchedObjectActiveColliders.Remove(collider);
            }
        }

        private void LateUpdate()
        {
            if (touchedObject != null && touchedObjectActiveColliders.Count == 0)
            {
                StopTouching(touchedObject);
            }
        }

        private void CheckButtonOverrides(VRTK_InteractableObject touchedObjectScript)
        {
            if (touchedObjectScript.grabOverrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                originalGrabAlias = controllerEvents.grabToggleButton;
                controllerEvents.grabToggleButton = touchedObjectScript.grabOverrideButton;
            }

            if (touchedObjectScript.useOverrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                originalUseAlias = controllerEvents.useToggleButton;
                controllerEvents.useToggleButton = touchedObjectScript.useOverrideButton;
            }
        }

        private void ResetButtonOverrides()
        {
            if (originalGrabAlias != VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                controllerEvents.grabToggleButton = originalGrabAlias;
                originalGrabAlias = VRTK_ControllerEvents.ButtonAlias.Undefined;
            }
            if (originalUseAlias != VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                controllerEvents.useToggleButton = originalUseAlias;
                originalUseAlias = VRTK_ControllerEvents.ButtonAlias.Undefined;
            }
        }

        private void ResetTriggerRumble()
        {
            triggerRumble = false;
        }

        private void StopTouching(GameObject untouched)
        {
            if (IsObjectInteractable(untouched))
            {
                ResetButtonOverrides();
                OnControllerUntouchInteractableObject(SetControllerInteractEvent(untouched.gameObject));

                var untouchedObjectScript = untouched.GetComponent<VRTK_InteractableObject>();
                untouchedObjectScript.StopTouching(gameObject);

                if (!untouchedObjectScript.IsTouched())
                {
                    untouchedObjectScript.ToggleHighlight(false);
                }
            }

            if (updatedHideControllerOnTouch)
            {
                controllerActions.ToggleControllerModel(true, touchedObject);
            }

            CleanupEndTouch();
        }

        private void CleanupEndTouch()
        {
            touchedObject = null;
            touchedObjectActiveColliders.Clear();
            touchedObjectColliders.Clear();
        }

        private void DestroyTouchCollider()
        {
            if (destroyColliderOnDisable)
            {
                Destroy(controllerCollisionDetector);
            }
        }

        private bool CustomRigidBodyIsChild()
        {
            foreach (var childTransform in GetComponentsInChildren<Transform>())
            {
                if (childTransform != transform && childTransform == customRigidbodyObject.transform)
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateTouchCollider()
        {
            if (customRigidbodyObject == null)
            {
                controllerCollisionDetector = Instantiate(defaultColliderPrefab, transform.position, transform.rotation) as GameObject;
                controllerCollisionDetector.transform.SetParent(transform);
                controllerCollisionDetector.transform.localScale = transform.localScale;
                controllerCollisionDetector.name = "ControllerColliders";
                destroyColliderOnDisable = true;
            }
            else
            {
                if (CustomRigidBodyIsChild())
                {
                    controllerCollisionDetector = customRigidbodyObject;
                    destroyColliderOnDisable = false;
                }
                else
                {
                    controllerCollisionDetector = Instantiate(customRigidbodyObject, transform.position, transform.rotation) as GameObject;
                    controllerCollisionDetector.transform.SetParent(transform);
                    controllerCollisionDetector.transform.localScale = transform.localScale;
                    destroyColliderOnDisable = true;
                }
            }
        }

        private void CreateTouchRigidBody()
        {
            touchRigidBody = gameObject.GetComponent<Rigidbody>();
            if (touchRigidBody == null)
            {
                touchRigidBody = gameObject.AddComponent<Rigidbody>();
                touchRigidBody.isKinematic = true;
                touchRigidBody.useGravity = false;
                touchRigidBody.constraints = RigidbodyConstraints.FreezeAll;
                touchRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }

        private void HideController()
        {
            if (touchedObject != null)
            {
                controllerActions.ToggleControllerModel(false, touchedObject);
            }
        }
    }
}