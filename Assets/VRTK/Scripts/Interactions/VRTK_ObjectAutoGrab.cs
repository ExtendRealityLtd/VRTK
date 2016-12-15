// Object Auto Grab|Interactions|30090
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// It is possible to automatically grab an Interactable Object to a specific controller by applying the Object Auto Grab script to the controller that the object should be grabbed by default.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/026_Controller_ForceHoldObject` shows how to automatically grab a sword to each controller and also prevents the swords from being dropped so they are permanently attached to the user's controllers.
    /// </example>
    [RequireComponent(typeof(VRTK_InteractGrab))]
    public class VRTK_ObjectAutoGrab : MonoBehaviour
    {
        [Tooltip("A game object (either within the scene or a prefab) that will be grabbed by the controller on game start.")]
        public VRTK_InteractableObject objectToGrab;
        [Tooltip("If this is checked then the Object To Grab will be cloned into a new object and attached to the controller leaving the existing object in the scene. This is required if the same object is to be grabbed to both controllers as a single object cannot be grabbed by different controllers at the same time. It is also required to clone a grabbed object if it is a prefab as it needs to exist within the scene to be grabbed.")]
        public bool cloneGrabbedObject;

        private void OnEnable()
        {
            StartCoroutine(AutoGrab());
        }

        private IEnumerator AutoGrab()
        {
            yield return new WaitForEndOfFrame();

            var controllerGrab = GetComponent<VRTK_InteractGrab>();
            var controllerTouch = GetComponent<VRTK_InteractTouch>();

            if (!objectToGrab)
            {
                Debug.LogError("You have to assign an object that should be grabbed.");
                yield break;
            }

            while (controllerGrab.controllerAttachPoint == null)
            {
                yield return true;
            }

            VRTK_InteractableObject grabbableObject = objectToGrab;
            VRTK_InteractableObject previousClonedObject = null;

            if (!controllerGrab.GetGrabbedObject())
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
                    if (grabbableObject.grabAttachMechanicScript && grabbableObject.grabAttachMechanicScript.IsKinematic())
                    {
                        grabbableObject.isKinematic = true;
                    }

                    grabbableObject.transform.position = transform.position;
                    controllerTouch.ForceStopTouching();
                    controllerTouch.ForceTouch(grabbableObject.gameObject);
                    controllerGrab.AttemptGrab();
                }
            }
        }
    }
}