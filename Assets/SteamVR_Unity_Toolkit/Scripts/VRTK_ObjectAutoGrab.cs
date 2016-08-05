namespace VRTK
{
    using UnityEngine;

    public class VRTK_ObjectAutoGrab : MonoBehaviour
    {
        public GameObject objectToGrab;
        public bool cloneGrabbedObject;

        private VRTK_InteractGrab controller;

        private void Start()
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
            Invoke("InitAutoGrab", 0.5f);
        }

        private void InitAutoGrab()
        {
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