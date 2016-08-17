//====================================================================================
//
// Purpose: Provide basic touch detection of controller to interactable objects
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;

    public struct ObjectInteractEventArgs
    {
        public uint controllerIndex;
        public GameObject target;
    }

    public delegate void ObjectInteractEventHandler(object sender, ObjectInteractEventArgs e);

    [RequireComponent(typeof(VRTK_ControllerActions))]
    public class VRTK_InteractTouch : MonoBehaviour
    {
        public bool hideControllerOnTouch = false;
        public float hideControllerDelay = 0f;
        public Color globalTouchHighlightColor = Color.clear;
        public GameObject customRigidbodyObject;

        public event ObjectInteractEventHandler ControllerTouchInteractableObject;
        public event ObjectInteractEventHandler ControllerUntouchInteractableObject;

        private GameObject touchedObject = null;
        private GameObject lastTouchedObject = null;
        private bool updatedHideControllerOnTouch = false;

        private SteamVR_TrackedObject trackedController;
        private VRTK_ControllerActions controllerActions;
        private GameObject controllerCollisionDetector;
        private bool triggerRumble;
        private bool destroyColliderOnDisable;
        private Rigidbody touchRigidBody;
        private Object defaultColliderPrefab;

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
            e.controllerIndex = (uint)trackedController.index;
            e.target = target;
            return e;
        }

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

        public GameObject GetTouchedObject()
        {
            return touchedObject;
        }

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

        public void ToggleControllerRigidBody(bool state)
        {
            if (controllerCollisionDetector && touchRigidBody)
            {
                touchRigidBody.isKinematic = !state;
                foreach (var collider in controllerCollisionDetector.GetComponents<Collider>())
                {
                    collider.isTrigger = !state;
                }

                foreach (var collider in controllerCollisionDetector.GetComponentsInChildren<Collider>())
                {
                    collider.isTrigger = !state;
                }
            }
        }

        public bool IsRigidBodyActive()
        {
            return !touchRigidBody.isKinematic;
        }

        public void ForceStopTouching()
        {
            if (touchedObject != null)
            {
                StopTouching(touchedObject);
            }
        }

        public Collider[] ControllerColliders()
        {
            return (controllerCollisionDetector.GetComponents<Collider>().Length > 0 ? controllerCollisionDetector.GetComponents<Collider>() : controllerCollisionDetector.GetComponentsInChildren<Collider>());
        }

        private void Awake()
        {
            trackedController = GetComponent<SteamVR_TrackedObject>();
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
            else
            {
                found = collider.gameObject.GetComponentInParent<VRTK_InteractableObject>().gameObject;
            }
            return found;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (IsObjectInteractable(collider.gameObject) && (touchedObject == null || !touchedObject.GetComponent<VRTK_InteractableObject>().IsGrabbed()))
            {
                lastTouchedObject = GetColliderInteractableObject(collider);
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            if (!enabled)
            {
                return;
            }

            if (touchedObject != null && touchedObject != lastTouchedObject && !touchedObject.GetComponent<VRTK_InteractableObject>().IsGrabbed())
            {
                CancelInvoke("ResetTriggerRumble");
                ResetTriggerRumble();
                ForceStopTouching();
            }

            if (touchedObject == null && IsObjectInteractable(collider.gameObject))
            {
                touchedObject = GetColliderInteractableObject(collider);
                lastTouchedObject = touchedObject;

                var touchedObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();

                if (!touchedObjectScript.IsValidInteractableController(gameObject, touchedObjectScript.allowedTouchControllers))
                {
                    touchedObject = null;
                    return;
                }

                updatedHideControllerOnTouch = touchedObjectScript.CheckHideMode(hideControllerOnTouch, touchedObjectScript.hideControllerOnTouch);
                OnControllerTouchInteractableObject(SetControllerInteractEvent(touchedObject));
                touchedObjectScript.ToggleHighlight(true, globalTouchHighlightColor);
                touchedObjectScript.StartTouching(gameObject);

                if (controllerActions.IsControllerVisible() && updatedHideControllerOnTouch)
                {
                    Invoke("HideController", hideControllerDelay);
                }

                var rumbleAmount = touchedObjectScript.rumbleOnTouch;
                if (!rumbleAmount.Equals(Vector2.zero) && !triggerRumble)
                {
                    triggerRumble = true;
                    controllerActions.TriggerHapticPulse((ushort)rumbleAmount.y, rumbleAmount.x, 0.05f);
                    Invoke("ResetTriggerRumble", rumbleAmount.x);
                }
            }
        }

        private void ResetTriggerRumble()
        {
            triggerRumble = false;
        }

        private bool IsColliderChildOfTouchedObject(GameObject collider)
        {
            if (touchedObject != null && collider.GetComponentInParent<VRTK_InteractableObject>() && collider.GetComponentInParent<VRTK_InteractableObject>().gameObject == touchedObject)
            {
                return true;
            }
            return false;
        }

        private void OnTriggerExit(Collider collider)
        {
            if (touchedObject != null && (touchedObject == collider.gameObject || IsColliderChildOfTouchedObject(collider.gameObject)))
            {
                StopTouching(collider.gameObject);
            }
        }

        private void StopTouching(GameObject obj)
        {
            if (IsObjectInteractable(obj))
            {
                GameObject untouched;
                if (obj.GetComponent<VRTK_InteractableObject>())
                {
                    untouched = obj;
                }
                else
                {
                    untouched = obj.GetComponentInParent<VRTK_InteractableObject>().gameObject;
                }

                OnControllerUntouchInteractableObject(SetControllerInteractEvent(untouched.gameObject));
                untouched.GetComponent<VRTK_InteractableObject>().ToggleHighlight(false);
                untouched.GetComponent<VRTK_InteractableObject>().StopTouching(gameObject);
            }

            if (updatedHideControllerOnTouch)
            {
                controllerActions.ToggleControllerModel(true, touchedObject);
            }
            touchedObject = null;
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
                if (childTransform == customRigidbodyObject.transform)
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