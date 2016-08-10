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
                OnDestinationMarkerSet(SetDestinationMarkerEvent(distance, destination, destination.position, (uint)controller.GetComponent<SteamVR_TrackedObject>().index));
            }
            lastUsePressedState = controller.usePressed;
        }
    }
}