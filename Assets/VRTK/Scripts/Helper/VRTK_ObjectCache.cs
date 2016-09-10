namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    public class VRTK_ObjectCache : MonoBehaviour
    {
        public static List<VRTK_BasicTeleport> registeredTeleporters = new List<VRTK_BasicTeleport>();
        public static List<VRTK_DestinationMarker> registeredDestinationMarkers = new List<VRTK_DestinationMarker>();
        public static VRTK_HeadsetCollision registeredHeadsetCollider = null;
        public static Dictionary<uint, GameObject> trackedControllers = new Dictionary<uint, GameObject>();
    }
}