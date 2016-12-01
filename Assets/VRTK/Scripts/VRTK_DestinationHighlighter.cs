using VRTK;

namespace VRTK
{
    using UnityEngine;

    [RequireComponent(typeof(VRTK_SimplePointer), typeof(VRTK_ControllerEvents))]
    public class VRTK_DestinationHighlighter : MonoBehaviour
    {
        private Material originalMaterial;
        private Renderer lastHighlightedObject;

        private VRTK_ControllerEvents events;
        private VRTK_SimplePointer pointer;

        private void Start()
        {
            pointer = GetComponent<VRTK_SimplePointer>();
            if (pointer == null)
            {
                Debug.LogError("VRTK_DestinationHighlighter is required to be attached to a Controller that has the VRTK_SimplePointer script attached to it");
                return;
            }

            events = GetComponent<VRTK_ControllerEvents>();
            if (events == null)
            {
                Debug.LogError("VRTK_DestinationHighlighter is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            pointer.DestinationMarkerEnter += new DestinationMarkerEventHandler(OnDestinationMarkerEnter);
            pointer.DestinationMarkerExit += new DestinationMarkerEventHandler(OnDestinationMarkerExit);

            events.TouchpadReleased += new ControllerInteractionEventHandler(OnTouchpadRelease);
        }

        private void OnTouchpadRelease(object sender, ControllerInteractionEventArgs e)
        {
            Restore();
        }

        private void OnDestinationMarkerEnter(object sender, DestinationMarkerEventArgs e)
        {
            var io = e.target.GetComponent<VRTK_InteractableObject>();
            if (lastHighlightedObject == null && io != null && io.highlightOnTarget)
            {
                lastHighlightedObject = io.GetComponent<Renderer>();
                if (lastHighlightedObject != null && io.highlightOnTargetMaterial != null)
                {
                    originalMaterial = lastHighlightedObject.material;
                    lastHighlightedObject.material = io.highlightOnTargetMaterial;
                }
            } 
        }

        private void OnDestinationMarkerExit(object sender, DestinationMarkerEventArgs e)
        {
            Restore();
        }

        private void Restore()
        {
            if (lastHighlightedObject != null && originalMaterial != null)
            {
                lastHighlightedObject.material = originalMaterial;
                lastHighlightedObject = null;
            }
        }

    }
}