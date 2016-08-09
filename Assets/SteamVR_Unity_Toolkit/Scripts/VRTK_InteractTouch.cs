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
        public bool triggerOnStaticObjects = false;

        public event ObjectInteractEventHandler ControllerTouchInteractableObject;
        public event ObjectInteractEventHandler ControllerUntouchInteractableObject;

        private GameObject touchedObject = null;
        private GameObject lastTouchedObject = null;
        private bool updatedHideControllerOnTouch = false;

        private SteamVR_TrackedObject trackedController;
        private VRTK_ControllerActions controllerActions;
        private GameObject controllerRigidBodyObject;
        private bool triggerRumble;

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
                VRTK_InteractableObject io = obj.GetComponent<VRTK_InteractableObject>();
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
            if (controllerRigidBodyObject && controllerRigidBodyObject.GetComponent<Rigidbody>())
            {
                controllerRigidBodyObject.GetComponent<Rigidbody>().detectCollisions = state;
            }
        }

        public void ForceStopTouching()
        {
            if (touchedObject != null)
            {
                StopTouching(touchedObject);
            }
        }

        private void Awake()
        {
            trackedController = GetComponent<SteamVR_TrackedObject>();
            controllerActions = GetComponent<VRTK_ControllerActions>();
        }

        private void Start()
        {
            if (GetComponent<VRTK_ControllerActions>() == null)
            {
                Debug.LogError("VRTK_InteractTouch is required to be attached to a SteamVR Controller that has the VRTK_ControllerActions script attached to it");
                return;
            }

            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Controller);
            CreateTouchCollider(gameObject);
            CreateTouchRigidBody(gameObject);
            CreateControllerRigidBody();
            triggerRumble = false;
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

        private void CreateTouchCollider(GameObject obj)
        {
            var collider = GetComponent<Collider>();
            if (collider == null)
            {
                var genCollider = obj.AddComponent<SphereCollider>();
                genCollider.radius = 0.06f;
                genCollider.center = new Vector3(0f, -0.04f, 0f);
                collider = genCollider;
            }
            collider.isTrigger = true;
        }

        private void CreateBoxCollider(GameObject obj, Vector3 center, Vector3 size)
        {
            BoxCollider bc = obj.AddComponent<BoxCollider>();
            bc.size = size;
            bc.center = center;
        }

        private void CreateTouchRigidBody(GameObject obj)
        {
            // Need a Rigidbody to interact with static objects
            if (triggerOnStaticObjects)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb==null)
                {
                    rb = obj.AddComponent<Rigidbody>();
                }

                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        private void HideController()
        {
            if (touchedObject != null)
            {
                controllerActions.ToggleControllerModel(false, touchedObject);
            }
        }

        private void CreateControllerRigidBody()
        {
            if (customRigidbodyObject != null)
            {
                controllerRigidBodyObject = customRigidbodyObject;
            }
            else
            {
                controllerRigidBodyObject = new GameObject(string.Format("[{0}]_RigidBody_Holder", gameObject.name));
                controllerRigidBodyObject.transform.parent = transform;
                controllerRigidBodyObject.transform.localPosition = Vector3.zero;

                CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.01f, -0.098f), new Vector3(0.04f, 0.025f, 0.15f));
                CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.009f, -0.002f), new Vector3(0.05f, 0.025f, 0.04f));
                CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.024f, 0.01f), new Vector3(0.07f, 0.02f, 0.02f));
                CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.045f, 0.022f), new Vector3(0.07f, 0.02f, 0.022f));
                CreateBoxCollider(controllerRigidBodyObject, new Vector3(0f, -0.0625f, 0.03f), new Vector3(0.065f, 0.015f, 0.025f));
                CreateBoxCollider(controllerRigidBodyObject, new Vector3(0.045f, -0.035f, 0.005f), new Vector3(0.02f, 0.025f, 0.025f));
                CreateBoxCollider(controllerRigidBodyObject, new Vector3(-0.045f, -0.035f, 0.005f), new Vector3(0.02f, 0.025f, 0.025f));

                var createRB = controllerRigidBodyObject.AddComponent<Rigidbody>();
                createRB.mass = 100f;
            }

            var controllerRB = controllerRigidBodyObject.GetComponent<Rigidbody>();

            controllerRB.useGravity = false;
            controllerRB.isKinematic = false;
            controllerRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            controllerRB.constraints = RigidbodyConstraints.FreezeAll;

            ToggleControllerRigidBody(false);
        }
    }
}