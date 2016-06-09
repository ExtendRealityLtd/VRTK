namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_ObjectAutoGrab : MonoBehaviour
    {
        public GameObject objectToGrab;
        public bool cloneGrabbedObject;

        private VRTK_InteractGrab controller;
        private float initGrabCooldown;
        private bool initGrab;

        private void Start()
        {
            controller = this.GetComponent<VRTK_InteractGrab>();
            initGrab = false;
            initGrabCooldown = 0.5f;

            if (!controller)
            {
                Debug.LogError("The VRTK_InteractGrab script is required to be attached to the controller along with this script.");
            }

            if (!objectToGrab || !objectToGrab.GetComponent<VRTK_InteractableObject>())
            {
                Debug.LogError("The objectToGrab Game Object must have the VRTK_InteractableObject script applied to it.");
            }
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

        private void Update()
        {
            //Give the SteamVR controllers a bit of time to initialise before grabbing
            if (initGrabCooldown <= 0 && !initGrab)
            {
                initGrab = true;
                InitAutoGrab();
            }
            else
            {
                initGrabCooldown -= Time.deltaTime;
            }
        }
    }
}