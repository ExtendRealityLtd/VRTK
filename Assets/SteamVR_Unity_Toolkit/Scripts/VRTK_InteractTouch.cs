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
    using System.Collections;

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

        public event ObjectInteractEventHandler ControllerTouchInteractableObject;
        public event ObjectInteractEventHandler ControllerUntouchInteractableObject;

        private GameObject touchedObject = null;
        private SteamVR_TrackedObject trackedController;
        private VRTK_ControllerActions controllerActions;
        private GameObject controllerRigidBody;

        public virtual void OnControllerTouchInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerTouchInteractableObject != null)
                ControllerTouchInteractableObject(this, e);
        }

        public virtual void OnControllerUntouchInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUntouchInteractableObject != null)
                ControllerUntouchInteractableObject(this, e);
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
            return (obj && (obj.GetComponent<VRTK_InteractableObject>() || obj.GetComponentInParent<VRTK_InteractableObject>()));
        }

        public void ToggleControllerRigidBody(bool state)
        {
            if (controllerRigidBody && controllerRigidBody.GetComponent<Rigidbody>())
            {
                controllerRigidBody.GetComponent<Rigidbody>().detectCollisions = state;
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

            CreateTouchCollider(this.gameObject);
            CreateControllerRigidBody();
        }

        private void OnTriggerStay(Collider collider)
        {
            if (touchedObject == null && IsObjectInteractable(collider.gameObject))
            {
                if (collider.gameObject.GetComponent<VRTK_InteractableObject>())
                {
                    touchedObject = collider.gameObject;
                }
                else
                {
                    touchedObject = collider.gameObject.GetComponentInParent<VRTK_InteractableObject>().gameObject;
                }

                OnControllerTouchInteractableObject(SetControllerInteractEvent(touchedObject));
                touchedObject.GetComponent<VRTK_InteractableObject>().ToggleHighlight(true, globalTouchHighlightColor);
                touchedObject.GetComponent<VRTK_InteractableObject>().StartTouching(this.gameObject);

                if (controllerActions.IsControllerVisible() && hideControllerOnTouch)
                {
                    Invoke("HideController", hideControllerDelay);
                }

                var rumbleAmount = touchedObject.GetComponent<VRTK_InteractableObject>().rumbleOnTouch;
                if (!rumbleAmount.Equals(Vector2.zero))
                {
                    controllerActions.TriggerHapticPulse((int)rumbleAmount.x, (ushort)rumbleAmount.y);
                }
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (IsObjectInteractable(collider.gameObject))
            {
                GameObject untouched;
                if (collider.gameObject.GetComponent<VRTK_InteractableObject>())
                {
                    untouched = collider.gameObject;
                }
                else
                {
                    untouched = collider.gameObject.GetComponentInParent<VRTK_InteractableObject>().gameObject;
                }

                OnControllerUntouchInteractableObject(SetControllerInteractEvent(untouched.gameObject));
                untouched.GetComponent<VRTK_InteractableObject>().ToggleHighlight(false);
                untouched.GetComponent<VRTK_InteractableObject>().StopTouching(this.gameObject);
            }

            if (hideControllerOnTouch)
            {
                controllerActions.ToggleControllerModel(true, touchedObject);
            }
            touchedObject = null;
        }

        private void CreateTouchCollider(GameObject obj)
        {
            SphereCollider collider = obj.AddComponent<SphereCollider>();
            collider.radius = 0.06f;
            collider.center = new Vector3(0f, -0.04f, 0f);
            collider.isTrigger = true;
        }

        private void CreateBoxCollider(GameObject obj, Vector3 center, Vector3 size)
        {
            BoxCollider bc = obj.AddComponent<BoxCollider>();
            bc.size = size;
            bc.center = center;
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
            controllerRigidBody = new GameObject(string.Format("[{0}]_RigidBody_Holder", this.gameObject.name));
            controllerRigidBody.transform.parent = this.transform;

            CreateBoxCollider(controllerRigidBody, new Vector3(0f, -0.01f, -0.098f), new Vector3(0.04f, 0.025f, 0.15f));
            CreateBoxCollider(controllerRigidBody, new Vector3(0f, -0.009f, -0.002f), new Vector3(0.05f, 0.025f, 0.04f));
            CreateBoxCollider(controllerRigidBody, new Vector3(0f, -0.024f, 0.01f), new Vector3(0.07f, 0.02f, 0.02f));
            CreateBoxCollider(controllerRigidBody, new Vector3(0f, -0.045f, 0.022f), new Vector3(0.07f, 0.02f, 0.022f));
            CreateBoxCollider(controllerRigidBody, new Vector3(0f, -0.0625f, 0.03f), new Vector3(0.065f, 0.015f, 0.025f));
            CreateBoxCollider(controllerRigidBody, new Vector3(0.045f, -0.035f, 0.005f), new Vector3(0.02f, 0.025f, 0.025f));
            CreateBoxCollider(controllerRigidBody, new Vector3(-0.045f, -0.035f, 0.005f), new Vector3(0.02f, 0.025f, 0.025f));
            Rigidbody rb = controllerRigidBody.AddComponent<Rigidbody>();

            controllerRigidBody.transform.localPosition = Vector3.zero;

            rb.useGravity = false;
            rb.mass = 100f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            ToggleControllerRigidBody(false);
        }
    }
}