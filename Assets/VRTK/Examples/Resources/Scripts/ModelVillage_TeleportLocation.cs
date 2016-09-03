using UnityEngine;
using VRTK;

public class ModelVillage_TeleportLocation : VRTK_DestinationMarker
{
    public Transform destination;
    private bool lastUsePressedState = false;

    private void OnTriggerStay(Collider collider)
    {
        var controller = (collider.GetComponent<VRTK_ControllerEvents>() ? collider.GetComponent<VRTK_ControllerEvents>() : collider.GetComponentInParent<VRTK_ControllerEvents>());
        if (controller)
        {
            if (lastUsePressedState == true && !controller.usePressed)
            {
                var distance = Vector3.Distance(transform.position, destination.position);
                var controllerIndex = VRTK_DeviceFinder.GetControllerIndex(controller.gameObject);
                OnDestinationMarkerSet(SetDestinationMarkerEvent(distance, destination, destination.position, Vector3.zero, controllerIndex));
            }
            lastUsePressedState = controller.usePressed;
        }
    }
}