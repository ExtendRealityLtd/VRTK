// Object Auto Grab|Interactions|30110
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
    /// It is possible to automatically grab an Interactable Object to a specific controller by applying the Object Auto Grab script to the controller that the object should be grabbed by default.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/026_Controller_ForceHoldObject` shows how to automatically grab a sword to each controller and also prevents the swords from being dropped so they are permanently attached to the user's controllers.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_ObjectAutoGrab")]
    public class VRTK_ObjectAutoGrab : MonoBehaviour
    {
        [Tooltip("A game object (either within the scene or a prefab) that will be grabbed by the controller on game start.")]
        public VRTK_InteractableObject objectToGrab;
        [Tooltip("If the `Object To Grab` is a prefab then this needs to be checked, if the `Object To Grab` already exists in the scene then this needs to be unchecked.")]
        public bool objectIsPrefab;
        [Tooltip("If this is checked then the Object To Grab will be cloned into a new object and attached to the controller leaving the existing object in the scene. This is required if the same object is to be grabbed to both controllers as a single object cannot be grabbed by different controllers at the same time. It is also required to clone a grabbed object if it is a prefab as it needs to exist within the scene to be grabbed.")]
        public bool cloneGrabbedObject;
        [Tooltip("If `Clone Grabbed Object` is checked and this is checked, then whenever this script is disabled and re-enabled, it will always create a new clone of the object to grab. If this is false then the original cloned object will attempt to be grabbed again. If the original cloned object no longer exists then a new clone will be created.")]
        public bool alwaysCloneOnEnable;

        [Header("Custom Settings")]

        [Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractTouch interactTouch;
        [Tooltip("The Interact Grab to listen for grab actions on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractGrab interactGrab;

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
        /// The ClearPreviousClone method resets the previous cloned object to null to ensure when the script is re-enabled that a new cloned object is created, rather than the original clone being grabbed again.
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
                    OnObjectAutoGrabCompleted();
                }
            }
            objectToGrab.disableWhenIdle = grabbableObjectDisableState;
            grabbableObject.disableWhenIdle = grabbableObjectDisableState;
        }
    }
}