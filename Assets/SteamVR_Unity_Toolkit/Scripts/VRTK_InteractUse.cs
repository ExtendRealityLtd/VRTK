//====================================================================================
//
// Purpose: Provide ability to use an interactable object when it is being touched
//
// This script must be attached to a Controller within the [CameraRig] Prefab
//
// The VRTK_ControllerEvents and VRTK_InteractTouch scripts must also be
// attached to the Controller
//
// Press the default 'Trigger' button on the controller to use an object
// Released the default 'Trigger' button on the controller to stop using an object
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;

    [RequireComponent(typeof(VRTK_InteractTouch)), RequireComponent(typeof(VRTK_ControllerEvents))]
    public class VRTK_InteractUse : MonoBehaviour
    {
        public bool hideControllerOnUse = false;
        public float hideControllerDelay = 0f;

        public event ObjectInteractEventHandler ControllerUseInteractableObject;
        public event ObjectInteractEventHandler ControllerUnuseInteractableObject;

        private GameObject usingObject = null;
        private VRTK_InteractTouch interactTouch;
        private VRTK_ControllerActions controllerActions;
        private bool updatedHideControllerOnUse = false;

        public virtual void OnControllerUseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUseInteractableObject != null)
            {
                ControllerUseInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUnuseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUnuseInteractableObject != null)
            {
                ControllerUnuseInteractableObject(this, e);
            }
        }

        public GameObject GetUsingObject()
        {
            return usingObject;
        }

        public void ForceStopUsing()
        {
            if (usingObject != null)
            {
                StopUsing();
            }
        }

        public void ForceResetUsing()
        {
            if (usingObject != null)
            {
                UnuseInteractedObject(false);
            }
        }

        private void Awake()
        {
            if (GetComponent<VRTK_InteractTouch>() == null)
            {
                Debug.LogError("VRTK_InteractUse is required to be attached to a SteamVR Controller that has the VRTK_InteractTouch script attached to it");
                return;
            }

            interactTouch = GetComponent<VRTK_InteractTouch>();
            controllerActions = GetComponent<VRTK_ControllerActions>();
        }

        private void OnEnable()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_InteractUse is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            GetComponent<VRTK_ControllerEvents>().AliasUseOn += new ControllerInteractionEventHandler(DoStartUseObject);
            GetComponent<VRTK_ControllerEvents>().AliasUseOff += new ControllerInteractionEventHandler(DoStopUseObject);
        }

        private void OnDisable()
        {
            ForceStopUsing();
            GetComponent<VRTK_ControllerEvents>().AliasUseOn -= new ControllerInteractionEventHandler(DoStartUseObject);
            GetComponent<VRTK_ControllerEvents>().AliasUseOff -= new ControllerInteractionEventHandler(DoStopUseObject);
        }

        private bool IsObjectUsable(GameObject obj)
        {
            return (interactTouch.IsObjectInteractable(obj) && obj.GetComponent<VRTK_InteractableObject>().isUsable);
        }

        private bool IsObjectHoldOnUse(GameObject obj)
        {
            return (obj && obj.GetComponent<VRTK_InteractableObject>() && obj.GetComponent<VRTK_InteractableObject>().holdButtonToUse);
        }

        private int GetObjectUsingState(GameObject obj)
        {
            if (obj && obj.GetComponent<VRTK_InteractableObject>())
            {
                return obj.GetComponent<VRTK_InteractableObject>().UsingState;
            }
            return 0;
        }

        private void SetObjectUsingState(GameObject obj, int value)
        {
            if (obj && obj.GetComponent<VRTK_InteractableObject>())
            {
                obj.GetComponent<VRTK_InteractableObject>().UsingState = value;
            }
        }

        private void UseInteractedObject(GameObject touchedObject)
        {
            if ((usingObject == null || usingObject != touchedObject) && IsObjectUsable(touchedObject))
            {
                usingObject = touchedObject;
                var usingObjectScript = usingObject.GetComponent<VRTK_InteractableObject>();

                if (!usingObjectScript.IsValidInteractableController(gameObject, usingObjectScript.allowedUseControllers))
                {
                    usingObject = null;
                    return;
                }

                updatedHideControllerOnUse = usingObjectScript.CheckHideMode(hideControllerOnUse, usingObjectScript.hideControllerOnUse);
                OnControllerUseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
                usingObjectScript.StartUsing(gameObject);

                if (updatedHideControllerOnUse)
                {
                    Invoke("HideController", hideControllerDelay);
                }

                usingObjectScript.ToggleHighlight(false);

                var rumbleAmount = usingObjectScript.rumbleOnUse;
                if (!rumbleAmount.Equals(Vector2.zero))
                {
                    controllerActions.TriggerHapticPulse((ushort)rumbleAmount.y, rumbleAmount.x, 0.05f);
                }
            }
        }

        private void HideController()
        {
            if (usingObject != null)
            {
                controllerActions.ToggleControllerModel(false, usingObject);
            }
        }

        private void UnuseInteractedObject(bool completeStop)
        {
            if (usingObject != null)
            {
                OnControllerUnuseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
                if (completeStop)
                {
                    usingObject.GetComponent<VRTK_InteractableObject>().StopUsing(gameObject);
                }
                if (updatedHideControllerOnUse)
                {
                    controllerActions.ToggleControllerModel(true, usingObject);
                }
                if (completeStop)
                {
                    usingObject.GetComponent<VRTK_InteractableObject>().ToggleHighlight(false);
                }
                usingObject = null;
            }
        }

        private GameObject GetFromGrab()
        {
            if (GetComponent<VRTK_InteractGrab>())
            {
                return GetComponent<VRTK_InteractGrab>().GetGrabbedObject();
            }
            return null;
        }

        private void StopUsing()
        {
            SetObjectUsingState(usingObject, 0);
            UnuseInteractedObject(true);
        }

        private void DoStartUseObject(object sender, ControllerInteractionEventArgs e)
        {
            GameObject touchedObject = interactTouch.GetTouchedObject();
            if (touchedObject == null)
            {
                touchedObject = GetFromGrab();
            }

            if (touchedObject != null && interactTouch.IsObjectInteractable(touchedObject))
            {
                var interactableObjectScript = touchedObject.GetComponent<VRTK_InteractableObject>();

                if (interactableObjectScript.useOnlyIfGrabbed && !interactableObjectScript.IsGrabbed())
                {
                    return;
                }

                UseInteractedObject(touchedObject);
                if (usingObject && !IsObjectHoldOnUse(usingObject))
                {
                    SetObjectUsingState(usingObject, GetObjectUsingState(usingObject) + 1);
                }
            }
        }

        private void DoStopUseObject(object sender, ControllerInteractionEventArgs e)
        {
            if (IsObjectHoldOnUse(usingObject) || GetObjectUsingState(usingObject) >= 2)
            {
                StopUsing();
            }
        }
    }
}