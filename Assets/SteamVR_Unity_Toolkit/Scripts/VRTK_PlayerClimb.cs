namespace VRTK
{
    using UnityEngine;
    using System.Collections;

        public uint controllerIndex;
        public GameObject target;


    public class VRTK_PlayerClimb : MonoBehaviour
    {







        {
            PlayerClimbEventArgs e;
            e.controllerIndex = controllerIndex;
            return e;
        }

        private void Start()
        {
            var controllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();
            InitControllerListeners(controllerManager.left);
            InitControllerListeners(controllerManager.right);




        }



        private void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {

        }

        private void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
        }

        {

        {
        }

        {
        }





        private void Update()
        {

        }


        private void InitControllerListeners(GameObject controller)
        {
            if (controller)
            {
                var grabbingController = controller.GetComponent<VRTK_InteractGrab>();
                if (grabbingController)
                {
                    grabbingController.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
                    grabbingController.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUngrabObject);
                }
            }
        }
    }
}
