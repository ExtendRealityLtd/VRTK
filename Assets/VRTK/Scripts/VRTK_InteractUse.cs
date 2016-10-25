// Interact Use|Scripts|0190
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interact Use script is attached to a Controller object within the `[CameraRig]` prefab and the Controller object requires the `VRTK_ControllerEvents` script to be attached as it uses this for listening to the controller button events for using and stop using interactable game objects. It listens for the `AliasUseOn` and `AliasUseOff` events to determine when an object should be used and should stop using.
    /// </summary>
    /// <remarks>
    /// The Controller object also requires the `VRTK_InteractTouch` script to be attached to it as this is used to determine when an interactable object is being touched. Only valid touched objects can be used.
    ///
    /// An object can be used if the Controller touches a game object which contains the `VRTK_InteractableObject` script and has the flag `isUsable` set to `true`.
    ///
    /// If a valid interactable object is usable then pressing the set `Use` button on the Controller (default is `Trigger`) will call the `StartUsing` method on the touched interactable object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/006_Controller_UsingADoor` simulates using a door object to open and close it. It also has a cube on the floor that can be grabbed to show how interactable objects can be usable or grabbable.
    ///
    /// `VRTK/Examples/008_Controller_UsingAGrabbedObject` which shows that objects can be grabbed with one button and used with another (e.g. firing a gun).
    /// </example>
    [RequireComponent(typeof(VRTK_InteractTouch)), RequireComponent(typeof(VRTK_ControllerEvents))]
    public class VRTK_InteractUse : MonoBehaviour
    {
        [Tooltip("Hides the controller model when a valid use action starts.")]
        public bool hideControllerOnUse = false;
        [Tooltip("The amount of seconds to wait before hiding the controller on use.")]
        public float hideControllerDelay = 0f;

        /// <summary>
        /// Emitted when a valid object starts being used.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUseInteractableObject;
        /// <summary>
        /// Emitted when a valid object stops being used.
        /// </summary>
        public event ObjectInteractEventHandler ControllerUnuseInteractableObject;

        private GameObject usingObject = null;
        private VRTK_InteractTouch interactTouch;
        private VRTK_ControllerActions controllerActions;
        private bool updatedHideControllerOnUse = false;
        private bool currentControllerHideState = false;

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

        /// <summary>
        /// The GetUsingObject method returns the current object being used by the controller.
        /// </summary>
        /// <returns>The game object of what is currently being used by this controller.</returns>
        public GameObject GetUsingObject()
        {
            return usingObject;
        }

        /// <summary>
        /// The ForceStopUsing method will force the controller to stop using the currently touched object and will also stop the object's using action.
        /// </summary>
        public void ForceStopUsing()
        {
            if (usingObject != null)
            {
                StopUsing();
            }
        }

        /// <summary>
        /// The ForceResetUsing will force the controller to stop using the currently touched object but the object will continue with it's existing using action.
        /// </summary>
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
                Debug.LogError("VRTK_InteractUse is required to be attached to a Controller that has the VRTK_InteractTouch script attached to it");
                return;
            }

            interactTouch = GetComponent<VRTK_InteractTouch>();
            controllerActions = GetComponent<VRTK_ControllerActions>();
        }

        private void OnEnable()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_InteractUse is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
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
                return obj.GetComponent<VRTK_InteractableObject>().usingState;
            }
            return 0;
        }

        private void SetObjectUsingState(GameObject obj, int value)
        {
            if (obj && obj.GetComponent<VRTK_InteractableObject>())
            {
                obj.GetComponent<VRTK_InteractableObject>().usingState = value;
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
                currentControllerHideState = controllerActions.IsControllerVisible();
                OnControllerUseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
                usingObjectScript.StartUsing(gameObject);

                if (updatedHideControllerOnUse)
                {
                    Invoke("HideController", hideControllerDelay);
                }
                else
                {
                    controllerActions.ToggleControllerModel(true, usingObject);
                }

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

        private bool ControllerGrabVisibilityState()
        {
            var interactGrab = GetComponent<VRTK_InteractGrab>();
            if (interactGrab)
            {
                return interactGrab.GetControllerVisibilityState();
            }
            return currentControllerHideState;
        }

        private void UnuseInteractedObject(bool completeStop)
        {
            if (usingObject != null)
            {
                OnControllerUnuseInteractableObject(interactTouch.SetControllerInteractEvent(usingObject));
                var usingObjectCheck = usingObject.GetComponent<VRTK_InteractableObject>();
                if (usingObjectCheck && completeStop)
                {
                    usingObjectCheck.StopUsing(gameObject);
                }
                var toggleControllerState = (usingObjectCheck && usingObjectCheck.IsGrabbed(gameObject) ? ControllerGrabVisibilityState() : true);
                controllerActions.ToggleControllerModel(toggleControllerState, usingObject);
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