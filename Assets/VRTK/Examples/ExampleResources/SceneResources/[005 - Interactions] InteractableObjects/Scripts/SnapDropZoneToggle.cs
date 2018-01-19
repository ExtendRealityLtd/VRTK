namespace VRTK.Examples
{
    using UnityEngine;

    public class SnapDropZoneToggle : MonoBehaviour
    {
        public VRTK_SnapDropZone toggleZoneA;
        public VRTK_SnapDropZone toggleZoneB;

        protected virtual void OnEnable()
        {
            toggleZoneA.ObjectEnteredSnapDropZone += ToggleZoneA;
            toggleZoneA.ObjectSnappedToDropZone += ToggleZoneA;
            toggleZoneA.ObjectExitedSnapDropZone += UntoggleZoneA;
            toggleZoneA.ObjectUnsnappedFromDropZone += UntoggleZoneA;

            toggleZoneB.ObjectEnteredSnapDropZone += ToggleZoneB;
            toggleZoneB.ObjectSnappedToDropZone += ToggleZoneB;
            toggleZoneB.ObjectExitedSnapDropZone += UntoggleZoneB;
            toggleZoneB.ObjectUnsnappedFromDropZone += UntoggleZoneB;
        }

        protected virtual void OnDisable()
        {
            toggleZoneA.ObjectEnteredSnapDropZone -= ToggleZoneA;
            toggleZoneA.ObjectSnappedToDropZone -= ToggleZoneA;
            toggleZoneA.ObjectExitedSnapDropZone -= UntoggleZoneA;
            toggleZoneA.ObjectUnsnappedFromDropZone -= UntoggleZoneA;

            toggleZoneB.ObjectEnteredSnapDropZone -= ToggleZoneB;
            toggleZoneB.ObjectSnappedToDropZone -= ToggleZoneB;
            toggleZoneB.ObjectExitedSnapDropZone -= UntoggleZoneB;
            toggleZoneB.ObjectUnsnappedFromDropZone -= UntoggleZoneB;
        }

        protected virtual void ToggleZoneA(object sender, SnapDropZoneEventArgs e)
        {
            if (toggleZoneB.GetCurrentSnappedObject() == null)
            {
                toggleZoneB.gameObject.SetActive(false);
            }
        }

        protected virtual void UntoggleZoneA(object sender, SnapDropZoneEventArgs e)
        {
            if (toggleZoneA.GetCurrentSnappedObject() == null)
            {
                toggleZoneB.gameObject.SetActive(true);
            }
        }

        protected virtual void ToggleZoneB(object sender, SnapDropZoneEventArgs e)
        {
            if (toggleZoneA.GetCurrentSnappedObject() == null)
            {
                toggleZoneA.gameObject.SetActive(false);
            }
        }

        protected virtual void UntoggleZoneB(object sender, SnapDropZoneEventArgs e)
        {
            if (toggleZoneB.GetCurrentSnappedObject() == null)
            {
                toggleZoneA.gameObject.SetActive(true);
            }
        }
    }
}