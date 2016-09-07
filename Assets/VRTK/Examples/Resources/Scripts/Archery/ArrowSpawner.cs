﻿
namespace VRTK.Example
{
    using UnityEngine;

    public class ArrowSpawner : MonoBehaviour
    {
        public GameObject arrowPrefab;
        public float spawnDelay = 1f;

        private float spawnDelayTimer = 0f;
        private BowAim bow;

        private void Start ()
        {
            spawnDelayTimer = 0f;
        }

        private void OnTriggerStay (Collider collider)
        {
            VRTK_InteractGrab grabbingController = (collider.gameObject.GetComponent<VRTK_InteractGrab>() ? collider.gameObject.GetComponent<VRTK_InteractGrab>() : collider.gameObject.GetComponentInParent<VRTK_InteractGrab>());
            if (CanGrab(grabbingController) && NoArrowNotched(grabbingController.gameObject) && Time.time >= spawnDelayTimer) {
                GameObject newArrow = Instantiate(arrowPrefab);
                newArrow.name = "ArrowClone";
                grabbingController.gameObject.GetComponent<VRTK_InteractTouch>().ForceTouch(newArrow);
                grabbingController.AttemptGrab();
                spawnDelayTimer = Time.time + spawnDelay;
            }
        }

        private bool CanGrab (VRTK_InteractGrab grabbingController)
        {
            return (grabbingController && grabbingController.GetGrabbedObject() == null && grabbingController.gameObject.GetComponent<VRTK_ControllerEvents>().grabPressed);
        }

        private bool NoArrowNotched (GameObject controller)
        {
            if (VRTK_SDK_Bridge.IsControllerLeftHand(controller)) {
                bow = VRTK_SDK_Bridge.GetControllerRightHand().GetComponentInChildren<BowAim>();
            }
            else if (VRTK_SDK_Bridge.IsControllerRightHand(controller)) {
                bow = VRTK_SDK_Bridge.GetControllerLeftHand().GetComponentInChildren<BowAim>();
            }
            return (bow == null || !bow.HasArrow());
        }
    }
}
