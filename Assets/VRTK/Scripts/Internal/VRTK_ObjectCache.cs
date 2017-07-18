namespace VRTK
{
    using System.Collections.Generic;

    public static class VRTK_ObjectCache
    {
        public static List<VRTK_BasicTeleport> registeredTeleporters = new List<VRTK_BasicTeleport>();
        public static List<VRTK_DestinationMarker> registeredDestinationMarkers = new List<VRTK_DestinationMarker>();
        public static VRTK_HeadsetCollision registeredHeadsetCollider = null;
        public static VRTK_HeadsetControllerAware registeredHeadsetControllerAwareness = null;
    }
}