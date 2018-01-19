namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTKExample_OptionTilePointerSelector : MonoBehaviour
    {
        public VRTK_DestinationMarker pointer;

        protected virtual void OnEnable()
        {
            pointer = (pointer == null ? GetComponent<VRTK_DestinationMarker>() : pointer);

            if (pointer != null)
            {
                pointer.DestinationMarkerEnter += DestinationMarkerEnter;
                pointer.DestinationMarkerExit += DestinationMarkerExit;
                pointer.DestinationMarkerSet += DestinationMarkerSet;
            }
            else
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTKExample_OptionTileSelector", "VRTK_DestinationMarker", "the Controller Alias"));
            }
        }

        protected virtual void OnDisable()
        {
            if (pointer != null)
            {
                pointer.DestinationMarkerEnter -= DestinationMarkerEnter;
                pointer.DestinationMarkerExit -= DestinationMarkerExit;
                pointer.DestinationMarkerSet -= DestinationMarkerSet;
            }
        }

        protected virtual void DestinationMarkerEnter(object sender, DestinationMarkerEventArgs e)
        {
            if (e.target != null)
            {
                VRTKExample_OptionTile optionTile = e.target.GetComponent<VRTKExample_OptionTile>();
                if (optionTile != null)
                {
                    optionTile.Highlight();
                }
            }
        }

        protected virtual void DestinationMarkerExit(object sender, DestinationMarkerEventArgs e)
        {
            if (e.target != null)
            {
                VRTKExample_OptionTile optionTile = e.target.GetComponent<VRTKExample_OptionTile>();
                if (optionTile != null)
                {
                    optionTile.Unhighlight();
                }
            }
        }

        protected virtual void DestinationMarkerSet(object sender, DestinationMarkerEventArgs e)
        {
            if (e.target != null)
            {
                VRTKExample_OptionTile optionTile = e.target.GetComponent<VRTKExample_OptionTile>();
                if (optionTile != null)
                {
                    optionTile.Activate();
                }
            }
        }
    }
}