namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_ObjectAutoGrab : MonoBehaviour
    {
        public VRTK_InteractableObject objectToGrab;
        public bool cloneGrabbedObject;

        private VRTK_InteractGrab controller;

        private IEnumerator Start()
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

            VRTK_InteractableObject grabbableObject = objectToGrab;
            if (cloneGrabbedObject)
            {
                grabbableObject = Instantiate(objectToGrab);
            }
            controller.GetComponent<VRTK_InteractTouch>().ForceStopTouching();
            controller.GetComponent<VRTK_InteractTouch>().ForceTouch(grabbableObject.gameObject);
            controller.AttemptGrab();
        }
    }
}