// Base Snap Attach|SnapAttachMechanics
namespace VRTK.SnapAttachMechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.UI;

    /// <summary>
    /// Provides snapping and unsnapping behavior that permits stacking of identical Interactable Objects. Built on top of the basic Simple Snap Attach mechanic.
    /// </summary>
    /// <remarks>

    [ExecuteInEditMode]
    public class VRTK_StackSnapAttach : VRTK_SimpleSnapAttach
    {
        [Tooltip("How snapped objects should be scaled when dropped into the `VRTK_SnapDropZone`.")]
        public float scaleModifier = 1f;
        [Tooltip("The number of stacks where the Snap Drop Zone starts displaying numbers")]
        public int startCountDisplayAt = 2;

        protected int snapCount = 0;
        protected Text snapCountText;
        protected bool isResnapping = false;
        protected Stack<GameObject> snapStack = new Stack<GameObject>();
      
        protected override void OnEnable()
        {
            base.OnEnable();
            initializeSnapCountText();
        }

        public override void CheckCanSnap(VRTK_InteractableObject interactableObjectCheck)
        {
            if (interactableObjectCheck != null)
            {
                AddCurrentValidSnapObject(interactableObjectCheck);
                if ((!isSnapped && ValidSnapObject(interactableObjectCheck, true)) || (isSnapped && ValidStackableObject(interactableObjectCheck)))
                {
                    pairedSnapDropZone.ToggleHighlight(interactableObjectCheck, true);
                    interactableObjectCheck.SetSnapDropZoneHover(pairedSnapDropZone, true);
                    if (!willSnap)
                    {
                        pairedSnapDropZone.OnObjectEnteredSnapDropZone(pairedSnapDropZone.SetSnapDropZoneEvent(interactableObjectCheck.gameObject));
                    }
                    willSnap = true;
                    pairedSnapDropZone.ToggleHighlightColor();
                }
            }
        }

        protected override void SnapObjectToZone(VRTK_InteractableObject objectToSnap)
        {
            if ((!isSnapped && ValidSnapObject(objectToSnap, false)) || ( isSnapped && ValidStackableObject(objectToSnap)))
            {
                SnapObject(objectToSnap);
            }
        }

        protected override void SnapObject(VRTK_InteractableObject interactableObjectCheck)
        {
            Vector3 newLocalScale = GetNewLocalScale(interactableObjectCheck);

            //If the item is in a snappable position and this drop zone isn't snapped and the collider is a valid interactable object
            if (willSnap && !isSnapped && ValidSnapObject(interactableObjectCheck, false))
            {
                //Only snap it to the drop zone if it's not already in a drop zone, or if we're resnapping from a stack
                if (!interactableObjectCheck.IsInSnapDropZone() || isResnapping)
                {
                    //Turn off the drop zone highlighter
                    pairedSnapDropZone.SetHighlightObjectActive(false);


                    if (transitionInPlaceRoutine != null)
                    {
                        StopCoroutine(transitionInPlaceRoutine);
                    }

                    isSnapped = true;
                    currentSnappedObject = interactableObjectCheck;

                    //Don't increment item count if re-snapping a stack clone after removing the top interactable object
                    if (!isResnapping)
                    {
                        snapCount += 1;
                        SetSnapCountText(snapCount);
                    }
                    else
                    {
                        isResnapping = false;
                    }

                    if (gameObject.activeInHierarchy)
                    {
                        transitionInPlaceRoutine = StartCoroutine(UpdateTransformDimensions(interactableObjectCheck, pairedSnapDropZone.highlightContainer, newLocalScale, snapDuration));
                    }

                    interactableObjectCheck.ToggleSnapDropZone(pairedSnapDropZone, true);
                }
            }
            //If the item is in a snappable position and this drop zone has a snapped object, and if both interactable objects can be stacked
            else if (ValidStackableObject(interactableObjectCheck) && ValidSnapObject(interactableObjectCheck, false))
            {
                pairedSnapDropZone.SetHighlightObjectActive(false);

                if (transitionInPlaceRoutine != null)
                {
                    StopCoroutine(transitionInPlaceRoutine);
                }

                isSnapped = true;
                snapCount += 1;
                SetSnapCountText(snapCount);
                
                if (gameObject.activeInHierarchy)
                {
                    transitionInPlaceRoutine = StartCoroutine(StackObjectAndUpdateTransform(interactableObjectCheck, pairedSnapDropZone.highlightContainer, newLocalScale, snapDuration));
                }

                interactableObjectCheck.ToggleSnapDropZone(pairedSnapDropZone, true);
            }

            //Force reset isSnapped if the item is grabbed but isSnapped is still true
            isSnapped = (isSnapped && interactableObjectCheck != null && interactableObjectCheck.IsGrabbed() ? false : isSnapped);
            //Allow other objects to pass through snapped stackable interactable objects by setting isTrigger to true for all snapped object colliders
            if (isSnapped)
            {
                interactableObjectCheck.SaveColliderStates(false);
                Collider[] objectColliderStates = interactableObjectCheck.GetComponentsInChildren<Collider>();
                for (int i = 0; i < objectColliderStates.Length; i++)
                {
                    objectColliderStates[i].isTrigger = true;
                }
            }
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

            snapCount -= 1;
            SetSnapCountText(snapCount);

            if (snapStack.Count > 0)
            {
                ResnapNextInStack();
            }

            //With any next-in-stack Interactable Objects resnapped and their collider's isTrigger value set to true, turn the unsnapped Interactable Object's colliders back on.
            if (checkCanSnapObject != null)
            {
                checkCanSnapObject.LoadPreviousColliderStates();
            }

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

        protected virtual void ResnapNextInStack()
        {
            isResnapping = true;
            float savedSnapDuration = snapDuration;
            snapDuration = 0f;
            GameObject nextInStack = snapStack.Pop();
            nextInStack.SetActive(true);
            ForceSnap(nextInStack);
            snapDuration = savedSnapDuration;
        }

        protected bool ValidStackableObject(VRTK_InteractableObject interactableObjectCheck)
        {
            return (currentSnappedObject.objectID == interactableObjectCheck.objectID && currentSnappedObject.isStackable && interactableObjectCheck.isStackable);
        }

        protected virtual void initializeSnapCountText()
        {
            snapCountText = GetComponentInChildren<Text>();
            if(snapCountText == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "SnapDropZone:" + name, "Text", "the `VRTK_SnapDropZone`", " if `VRTK_SnapDropZone.stackType` is set to `StackTypes.StackIdentical`"));
            }
            SetSnapCountText(snapCount);
        }

        protected void SetSnapCountText(int count)
        {
            if (snapCountText != null)
            {
                if (count >= startCountDisplayAt)
                {
                    snapCountText.text = count.ToString();
                }
                else
                {
                    snapCountText.text = "";
                }
            }
        }

        protected override Vector3 GetNewLocalScale(VRTK_InteractableObject checkObject)
        {
            //If this object is being re-snapped to the Snap Drop Zone (after being pushed to the stack when another Interactable Object was stacked on top of it), it has already been scaled and doesn't need to be again
            if (!isResnapping)
            {
                return base.GetNewLocalScale(checkObject) * scaleModifier;
            }
            else
            {
                return checkObject.transform.localScale;
            }

            
        }

        protected virtual IEnumerator StackObjectAndUpdateTransform(VRTK_InteractableObject ioCheck, GameObject endSettings, Vector3 endScale, float duration)
        {
            yield return StartCoroutine(UpdateTransformDimensions(ioCheck, endSettings, endScale, duration));
            snapStack.Push(currentSnappedObject.gameObject);
            currentSnappedObject.gameObject.SetActive(false);
            currentSnappedObject = ioCheck;
        }
    }
}
