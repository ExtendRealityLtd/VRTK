using VRTK;

namespace VRTK
{
    using UnityEngine;

    public class VRTK_DestinationHighlighter : MonoBehaviour
    {
        private Material originalMaterial;
        private Renderer lastHighlightedObject;

        private void Start()
        {
            if (GetComponent<VRTK_SimplePointer>() == null)
            {
                Debug.LogError("VRTK_DestinationHighLighter is required to be attached to a Controller that has the VRTK_SimplePointer script attached to it");
                return;
            }

            GetComponent<VRTK_SimplePointer>().DestinationMarkerEnter += new DestinationMarkerEventHandler(OnDestinationMarkerEnter);
            GetComponent<VRTK_SimplePointer>().DestinationMarkerExit += new DestinationMarkerEventHandler(OnDestinationMarkerExit);
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
            if (lastHighlightedObject != null && originalMaterial != null)
            {
                lastHighlightedObject.material = originalMaterial;
                lastHighlightedObject = null;
            }
        }

    }
}