namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    public sealed class VRTK_ObjectCache : ScriptableObject
    {
        public static List<VRTK_BasicTeleport> registeredTeleporters = new List<VRTK_BasicTeleport>();
        public static List<VRTK_DestinationMarker> registeredDestinationMarkers = new List<VRTK_DestinationMarker>();
        public static VRTK_HeadsetCollision registeredHeadsetCollider = null;
        public static VRTK_HeadsetControllerAware registeredHeadsetControllerAwareness = null;
    }
}