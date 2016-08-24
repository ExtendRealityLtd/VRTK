namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_ObjectAutoGrab : MonoBehaviour
    {
        public VRTK_InteractableObject objectToGrab;
        public bool cloneGrabbedObject;

        private VRTK_InteractGrab controller;
        private VRTK_InteractableObject grabbableObject;
        private VRTK_InteractableObject previousClonedObject;

        private void OnEnable()
        {
            StartCoroutine(AutoGrab());
        }

        private IEnumerator AutoGrab()
        {
            controller = GetComponent<VRTK_InteractGrab>();
            if (!controller)
            {
                Debug.LogError("The VRTK_InteractGrab script is required to be attached to the controller along with this script.");
                yield break;
            }

            if (!objectToGrab)
            {
                Debug.LogError("You have to assign an object that should be grabbed.");
                yield break;
            }

            while (controller.controllerAttachPoint == null)
            {
                yield return true;
            }

            grabbableObject = objectToGrab;
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
                if (grabbableObject.AttachIsKinematicObject())
                {
                    grabbableObject.ToggleKinematic(true);
                }

                grabbableObject.transform.position = transform.position;
                controller.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
                controller.GetComponent<VRTK_InteractTouch>().ForceTouch(grabbableObject.gameObject);
                controller.AttemptGrab();
            }
        }
    }
}