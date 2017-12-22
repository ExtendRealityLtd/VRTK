// Base Snap Attach|SnapAttachMechanics
namespace VRTK.SnapAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// Provides basic snapping functionality and serves as the default mechanic for a `VRTK_SnapDropZone'.
    /// </summary>
    /// <remarks>

    [ExecuteInEditMode]
    public class VRTK_SimpleSnapAttach : VRTK_BaseSnapAttach
    {

        protected override void SnapObject(VRTK_InteractableObject interactableObjectCheck)
        {
            Vector3 newLocalScale = GetNewLocalScale(interactableObjectCheck);

            //If the item is in a snappable position and this drop zone isn't snapped and the collider is a valid interactable object
            if (willSnap && !isSnapped && ValidSnapObject(interactableObjectCheck, false))
            {
                //Only snap it to the drop zone if it's not already in a drop zone, or if we're resnapping from a stack
                if (!interactableObjectCheck.IsInSnapDropZone())
                {
                    //Turn off the drop zone highlighter
                    pairedSnapDropZone.SetHighlightObjectActive(false);
                    
                    if (transitionInPlaceRoutine != null)
                    {
                        StopCoroutine(transitionInPlaceRoutine);
                    }

                    isSnapped = true;
                    currentSnappedObject = interactableObjectCheck;

                    if (gameObject.activeInHierarchy)
                    {
                        transitionInPlaceRoutine = StartCoroutine(UpdateTransformDimensions(interactableObjectCheck, pairedSnapDropZone.highlightContainer, newLocalScale, snapDuration));
                    }

                    interactableObjectCheck.ToggleSnapDropZone(pairedSnapDropZone, true);
                }
            }
           
            //force reset isSnapped if the item is grabbed but isSnapped is still true
            isSnapped = (isSnapped && interactableObjectCheck != null && interactableObjectCheck.IsGrabbed() ? false : isSnapped);
            willSnap = !isSnapped;
            wasSnapped = false;
        }

        public override void UnsnapObject()
        {
            if (currentSnappedObject != null)
            {
                RemoveCurrentValidSnapObject(currentSnappedObject);
            }

            isSnapped = false;
            wasSnapped = true;
            VRTK_InteractableObject checkCanSnapObject = currentSnappedObject;
            currentSnappedObject = null;
            ResetSnapDropZoneJoint();

            if (transitionInPlaceRoutine != null)
            {
                StopCoroutine(transitionInPlaceRoutine);
            }

            //With any cloned or next-in-stack Interactable Objects resnapped and their collider's isTrigger value set to true, turn the unsnapped Interactable Object's colliders back on.
            checkCanSnapObject.LoadPreviousColliderStates();

            if (checkCanSnapRoutine != null)
            {
                StopCoroutine(checkCanSnapRoutine);
            }

            if (gameObject.activeInHierarchy)
            {
                checkCanSnapRoutine = StartCoroutine(CheckCanSnapObjectAtEndOfFrame(checkCanSnapObject));
            }

            checkCanSnapObject = null;
        }
    }
}
