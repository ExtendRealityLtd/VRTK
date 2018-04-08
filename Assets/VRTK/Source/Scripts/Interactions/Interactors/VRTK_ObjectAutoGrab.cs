// Object Auto Grab|Interactors|30080
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    public delegate void ObjectAutoGrabEventHandler(object sender);

    /// <summary>
    /// Attempt to automatically grab a specified Interactable Object.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_InteractTouch` - The touch component to determine when a valid touch has taken place to denote a use interaction can occur. This must be applied on the same GameObject as this script if one is not provided via the `Interact Touch` parameter.
    ///  * `VRTK_InteractGrab` - The grab component to determine when a valid grab has taken place. This must be applied on the same GameObject as this script if one is not provided via the `Interact Grab` parameter.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_ObjectAutoGrab` script on either:
    ///    * The GameObject that contains the Interact Touch and Interact Grab scripts.
    ///    * Any other scene GameObject and provide a valid `VRTK_InteractTouch` component to the `Interact Touch` parameter and a valid `VRTK_InteractGrab` component to the `Interact Grab` parameter of this script.
    /// * Assign the Interactable Object to auto grab to the `Object To Grab` parameter on this script.
    ///   * If this Interactable Object is a prefab then the `Object Is Prefab` parameter on this script must be checked.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/026_Controller_ForceHoldObject` shows how to automatically grab a sword to each controller and also prevents the swords from being dropped so they are permanently attached to the user's controllers.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactors/VRTK_ObjectAutoGrab")]
    public class VRTK_ObjectAutoGrab : MonoBehaviour
    {
        [Tooltip("The Interactable Object that will be grabbed by the Interact Grab.")]
        public VRTK_InteractableObject objectToGrab;
        [Tooltip("If the `Object To Grab` is a prefab then this needs to be checked, if the `Object To Grab` already exists in the scene then this needs to be unchecked.")]
        public bool objectIsPrefab;
        [Tooltip("If this is checked then the `Object To Grab` will be cloned into a new Interactable Object and grabbed by the Interact Grab leaving the existing Interactable Object in the scene. This is required if the same Interactable Object is to be grabbed to multiple instances of Interact Grab. It is also required to clone a grabbed Interactable Object if it is a prefab as it needs to exist within the scene to be grabbed.")]
        public bool cloneGrabbedObject;
        [Tooltip("If `Clone Grabbed Object` is checked and this is checked, then whenever this script is disabled and re-enabled, it will always create a new clone of the Interactable Object to grab. If this is unchecked then the original cloned Interactable Object will attempt to be grabbed again. If the original cloned object no longer exists then a new clone will be created.")]
        public bool alwaysCloneOnEnable;
        [Tooltip("If this is checked then the `Object To Grab` will attempt to be secondary grabbed as well as primary grabbed.")]
        public bool attemptSecondaryGrab;

        [Header("Custom Settings")]

        [Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractTouch interactTouch;
        [Tooltip("The Interact Grab to listen for grab actions on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractGrab interactGrab;
        [Tooltip("The secondary controller Interact Touch to listen for touches on. If this field is left blank then it will be looked up on the opposite controller script alias at runtime.")]
        public VRTK_InteractTouch secondaryInteractTouch;
        [Tooltip("The secondary controller Interact Grab to listen for grab actions on. If this field is left blank then it will be looked up on the opposite controller script alias at runtime.")]
        public VRTK_InteractGrab secondaryInteractGrab;

        /// <summary>
        /// Emitted when the object auto grab has completed successfully.
        /// </summary>
        public event ObjectAutoGrabEventHandler ObjectAutoGrabCompleted;

        protected VRTK_InteractableObject previousClonedObject = null;
        protected Coroutine autoGrabRoutine;

        public virtual void OnObjectAutoGrabCompleted()
        {
            if (ObjectAutoGrabCompleted != null)
            {
                ObjectAutoGrabCompleted(this);
            }
        }

        /// <summary>
        /// The ClearPreviousClone method resets the previous cloned Interactable Object to null to ensure when the script is re-enabled that a new cloned Interactable Object is created, rather than the original clone being grabbed again.
        /// </summary>
        public virtual void ClearPreviousClone()
        {
            previousClonedObject = null;
        }

        protected virtual void OnEnable()
        {
            //Must always clone if the object is a prefab
            if (objectIsPrefab)
            {
                cloneGrabbedObject = true;
            }

            autoGrabRoutine = StartCoroutine(AutoGrab());
        }

        protected virtual void OnDisable()
        {
            if (autoGrabRoutine != null)
            {
                StopCoroutine(autoGrabRoutine);
            }
        }

        protected virtual IEnumerator AutoGrab()
        {
            yield return new WaitForEndOfFrame();

            interactTouch = (interactTouch != null ? interactTouch : GetComponentInParent<VRTK_InteractTouch>());
            interactGrab = (interactGrab != null ? interactGrab : GetComponentInParent<VRTK_InteractGrab>());

            if (interactTouch == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_ObjectAutoGrab", "VRTK_InteractTouch", "interactTouch", "the same or parent"));
            }

            if (interactGrab == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_ObjectAutoGrab", "VRTK_InteractGrab", "interactGrab", "the same or parent"));
            }

            if (objectToGrab == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.NOT_DEFINED, "objectToGrab"));
                yield break;
            }

            while (interactGrab.controllerAttachPoint == null)
            {
                yield return true;
            }

            bool grabbableObjectDisableState = objectToGrab.disableWhenIdle;

            if (objectIsPrefab)
            {
                objectToGrab.disableWhenIdle = false;
            }

            VRTK_InteractableObject grabbableObject = objectToGrab;
            if (alwaysCloneOnEnable)
            {
                ClearPreviousClone();
            }

            if (!interactGrab.GetGrabbedObject())
            {
                if (cloneGrabbedObject)
                {
                    if (previousClonedObject == null)
                    {
                        grabbableObject = Instantiate(objectToGrab);
                        previousClonedObject = grabbableObject;
                    }
                    else
                    {
                        grabbableObject = previousClonedObject;
                    }
                }

                if (grabbableObject.isGrabbable && !grabbableObject.IsGrabbed())
                {
                    grabbableObject.transform.position = transform.position;
                    interactTouch.ForceStopTouching();
                    interactTouch.ForceTouch(grabbableObject.gameObject);
                    interactGrab.AttemptGrab();
                    AttemptSecondaryGrab(grabbableObject);
                    OnObjectAutoGrabCompleted();
                }
            }
            objectToGrab.disableWhenIdle = grabbableObjectDisableState;
            grabbableObject.disableWhenIdle = grabbableObjectDisableState;
        }

        protected virtual void AttemptSecondaryGrab(VRTK_InteractableObject grabbableObject)
        {
            if (attemptSecondaryGrab)
            {
                SDK_BaseController.ControllerHand currentHand = VRTK_DeviceFinder.GetControllerHand(interactTouch.gameObject);
                VRTK_ControllerReference oppositeControllerReference = VRTK_ControllerReference.GetControllerReference(VRTK_DeviceFinder.GetOppositeHand(currentHand));
                if (VRTK_ControllerReference.IsValid(oppositeControllerReference))
                {
                    secondaryInteractTouch = (secondaryInteractTouch == null ? oppositeControllerReference.scriptAlias.GetComponentInChildren<VRTK_InteractTouch>() : secondaryInteractTouch);
                    secondaryInteractGrab = (secondaryInteractGrab == null ? oppositeControllerReference.scriptAlias.GetComponentInChildren<VRTK_InteractGrab>() : secondaryInteractGrab);
                    secondaryInteractTouch.ForceStopTouching();
                    secondaryInteractTouch.ForceTouch(grabbableObject.gameObject);
                    secondaryInteractGrab.AttemptGrab();
                }
            }
        }
    }
}