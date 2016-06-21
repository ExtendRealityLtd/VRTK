using UnityEngine;
using System.Collections;
using VRTK;

public class ModelVillage_TeleportLocation : VRTK_DestinationMarker
{
    public Transform destination;

    private void Start()
    {
        GameObject.FindObjectOfType<VRTK_BasicTeleport>().InitDestinationSetListener(this.gameObject);
    }
	
    private void OnTriggerStay(Collider collider)
    {
        var controller = collider.GetComponent<VRTK_ControllerEvents>();
        if (controller && controller.usePressed)
        {
            var distance = Vector3.Distance(this.transform.position, destination.position);
            OnDestinationMarkerSet(SeDestinationMarkerEvent(distance, destination, destination.position, (uint)controller.GetComponent<SteamVR_TrackedObject>().index));
        }
    }
}
