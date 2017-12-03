// Base Snap Attach|SnapAttachMechanics
namespace VRTK.SnapAttachMechanics
{
    using UnityEngine;
    using VRTK.Highlighters;

    /// <summary>
    /// Provides unsnapping behavior that leaves a copy of the snapped Interactable Object in place when the original object is grabbed. Built on top of the basic Simple Snap Attach mechanic.
    /// </summary>

    [ExecuteInEditMode]
    public class VRTK_CloneSnapAttach : VRTK_SimpleSnapAttach
    {
        protected GameObject objectToClone = null;
        protected bool[] clonedObjectColliderStates = new bool[0];

        protected override void OnEnable()
        {
            base.OnEnable();
            objectToClone = null;
            clonedObjectColliderStates = new bool[0];
        }

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

                    CreatePermanentClone();
                   
                    if (gameObject.activeInHierarchy)
                    {
                        transitionInPlaceRoutine = StartCoroutine(UpdateTransformDimensions(interactableObjectCheck, pairedSnapDropZone.highlightContainer, newLocalScale, snapDuration));
                    }

                    interactableObjectCheck.ToggleSnapDropZone(pairedSnapDropZone, true);
                }
            }
           
            //force reset isSnapped if the item is grabbed but isSnapped is still true
            isSnapped = (isSnapped && interactableObjectCheck != null && interactableObjectCheck.IsGrabbed() ? false : isSnapped);
            //allow other objects to pass through snapped stackable interactable objects by setting isTrigger to true for all object colliders
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

            ResnapPermanentClone();
            
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

        protected virtual void CreatePermanentClone()
        {
            VRTK_BaseHighlighter currentSnappedObjectHighlighter = currentSnappedObject.GetComponent<VRTK_BaseHighlighter>();
            if (currentSnappedObjectHighlighter != null)
            {
                currentSnappedObjectHighlighter.Unhighlight();
            }
            objectToClone = Instantiate(currentSnappedObject.gameObject);
            objectToClone.transform.position = pairedSnapDropZone.transform.position;
            objectToClone.transform.rotation = pairedSnapDropZone.transform.rotation;
            objectToClone.SetActive(false);
        }

        protected virtual void ResnapPermanentClone()
        {
            if (objectToClone != null)
            {
                float savedSnapDuration = snapDuration;
                snapDuration = 0f;
                objectToClone.SetActive(true);
                ForceSnap(objectToClone);
                snapDuration = savedSnapDuration;
            }
        }
    }
}
