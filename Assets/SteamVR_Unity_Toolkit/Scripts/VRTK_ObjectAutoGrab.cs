namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_ObjectAutoGrab : MonoBehaviour
    {
        public GameObject objectToGrab;
        public bool cloneGrabbedObject;

        private VRTK_InteractGrab controller;

        private IEnumerator Start()
        {
            controller = GetComponent<VRTK_InteractGrab>();

            if (!controller)
            {
                Debug.LogError("The VRTK_InteractGrab script is required to be attached to the controller along with this script.");
            }

            if (!objectToGrab || !objectToGrab.GetComponent<VRTK_InteractableObject>())
            {
                Debug.LogError("The objectToGrab Game Object must have the VRTK_InteractableObject script applied to it.");
            }

            while (controller.controllerAttachPoint == null)
            {
                yield return true;
            }

            var grabbableObject = objectToGrab;
            if (cloneGrabbedObject)
            {
                grabbableObject = Instantiate(objectToGrab);
            }
            controller.GetComponent<VRTK_InteractTouch>().ForceTouch(grabbableObject);
            controller.AttemptGrab();
        }
    }
}