// Base Snap Attach|SnapAttachMechanics
namespace VRTK.SnapAttachMechanics
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a base that all mechanics for snapping to a `VRTK_SnapDropZone' can inherit from.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///   > This is an abstract class that is to be inherited to a concrete class that provides grab attach functionality, therefore this script should not be directly used.
    /// </remarks>

    [ExecuteInEditMode]
    public abstract class VRTK_BaseSnapAttach : MonoBehaviour
    {
        /// <summary>
        /// The types of object stacking available.
        /// </summary>
        public enum ScaleTypes
        {
            /// <summary>
            /// The snapped Interactable Object will not be scaled.
            /// </summary>
            None,
            /// <summary>
            /// The snapped Interactable Object will be given the same scale as the Snap Drop Zone.
            /// </summary>
            MatchScale,
            /// <summary>
            /// The snapped Interactable Object will be scaled to fit within the bounds of the Snap Drop Zone highlight object.
            /// </summary>
            MatchBounds
        }

        [Tooltip("The amount of time it takes for the object being snapped to move into the new snapped position, rotation and scale.")]
        public float snapDuration = 0f;
        [Tooltip("How snapped objects should be scaled when dropped into the `VRTK_SnapDropZone`.")]
        public ScaleTypes scaleType = ScaleTypes.None;
        [Tooltip("A specified VRTK_PolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release.")]
        public VRTK_PolicyList validObjectListPolicy;
        [Tooltip("The Interactable Object to snap into the dropzone when the drop zone is enabled. The Interactable Object must be valid in any given policy list to snap.")]
        public VRTK_InteractableObject defaultSnappedInteractableObject;

        protected VRTK_SnapDropZone pairedSnapDropZone = null;
        protected List<VRTK_InteractableObject> currentValidSnapInteractableObjects = new List<VRTK_InteractableObject>();
        protected VRTK_InteractableObject currentSnappedObject = null;
        
        protected bool willSnap = false;
        protected bool isSnapped = false;
        protected bool wasSnapped = false;

        protected Coroutine transitionInPlaceRoutine;
        protected Coroutine attemptTransitionAtEndOfFrameRoutine;
        protected Coroutine checkCanSnapRoutine;
        protected bool originalJointCollisionState = false;

        /// <summary>
        /// The GetHoveringInteractableObjects method returns a List of valid Interactable Object scripts that are currently hovering (but not snapped) in the snap drop zone area.
        /// </summary>
        /// <returns>The List of valid Interactable Object scripts that are hovering (but not snapped) in the snap drop zone area.</returns>
        public virtual List<VRTK_InteractableObject> GetHoveringInteractableObjects()
        {
            return currentValidSnapInteractableObjects;
        }

        /// <summary>
        /// The GetCurrentSnappedObejct method returns the GameObject that is currently snapped in the Snap Drop Zone area.
        /// </summary>
        /// <returns>The GameObject that is currently snapped in the Snap Drop Zone area.</returns>
        public virtual GameObject GetCurrentSnappedObject()
        {
            return (currentSnappedObject != null ? currentSnappedObject.gameObject : null);
        }

        /// <summary>
        /// The GetCurrentSnappedInteractableObject method returns the Interactable Object script that is currently snapped in the Snap Drop Zone area.
        /// </summary>
        /// <returns>The Interactable Object script that is currently snapped in the snap Drop Zone Area.</returns>
        public virtual VRTK_InteractableObject GetCurrentSnappedInteractableObject()
        {
            return currentSnappedObject;
        }

        protected virtual void OnEnable()
        {
            pairedSnapDropZone = gameObject.GetComponentInParent<VRTK_SnapDropZone>();
            currentValidSnapInteractableObjects.Clear();
            currentSnappedObject = null;
            willSnap = false;
            isSnapped = false;
            wasSnapped = false;

            ///[Obsolete] Move deprecated properties over gracefully
#pragma warning disable 618
            if (pairedSnapDropZone.snapDuration != 0f && snapDuration == 0f)
            {
                snapDuration = pairedSnapDropZone.snapDuration;
                //Wipe the deprecated value after moving it over so that this code only runs once, even if snapDuration is later intentionally set to 0f
                pairedSnapDropZone.snapDuration = 0f;
            }

            if (pairedSnapDropZone.applyScalingOnSnap != false && scaleType == ScaleTypes.None)
            {
                scaleType = ScaleTypes.MatchScale;
                //Wipe the deprecated value after moving it over so that this code only runs once, even if scaleType is later changed from MatchScale
                pairedSnapDropZone.applyScalingOnSnap = false;
            }

            if (pairedSnapDropZone.validObjectListPolicy != null && validObjectListPolicy == null)
            {
                validObjectListPolicy = pairedSnapDropZone.validObjectListPolicy;
            }

            if (defaultSnappedInteractableObject == null)
            {
                //When handling deprecation for the already obselete defaultSnappedObject property
                if (pairedSnapDropZone.defaultSnappedObject != null)
                {
                    defaultSnappedInteractableObject = pairedSnapDropZone.defaultSnappedObject.GetComponentInParent<VRTK_InteractableObject>();
                    if (defaultSnappedInteractableObject != null)
                    {
                        pairedSnapDropZone.defaultSnappedObject = null;
                    }
                }
                //When handling deprecation for the defaultSnappedInteractableObject property for those users who have been on the 3.3.0 alpha branch
                if (pairedSnapDropZone.defaultSnappedInteractableObject != null)
                {
                    defaultSnappedInteractableObject = pairedSnapDropZone.defaultSnappedInteractableObject.GetComponentInParent<VRTK_InteractableObject>();
                    if (defaultSnappedInteractableObject != null) 
                    {
                        pairedSnapDropZone.defaultSnappedInteractableObject = null;    
                    }
                } 
            }
#pragma warning restore 618

            if (!VRTK_SharedMethods.IsEditTime() && Application.isPlaying && defaultSnappedInteractableObject != null)
            {
                ForceSnap(defaultSnappedInteractableObject);
            }
        }

        protected virtual void OnDisable()
        {
            //Stop any active animation or snapping coroutines
            if (transitionInPlaceRoutine != null)
            {
                StopCoroutine(transitionInPlaceRoutine);
            }

            if (attemptTransitionAtEndOfFrameRoutine != null)
            {
                StopCoroutine(attemptTransitionAtEndOfFrameRoutine);
            }

            if (checkCanSnapRoutine != null)
            {
                StopCoroutine(checkCanSnapRoutine);
            }

            ForceUnsnap();
            UnregisterAllGrabEvents();
        }

        /// <summary>
        /// The IsSnapped method returns true if an Interactable Object is currently snapped to the Snap Drop Zone paired with this attach script
        /// </summary>
        /// <returns>Whetehr an Interactable Object is currently snapped to the Snap Drop Zone paired with this attach script.</returns>
        public virtual bool IsSnapped()
        {
            return isSnapped;
        }

        /// <summary>
        /// The CheckCanSnap method tests to see if an Interactable Object can be snapped to the Snap Drop Zone paired with this attach script, and then triggers a snap event.
        /// </summary>
        /// /// <param name="interactableObjectCheck">The Interactable Object to test.</param>
        public virtual void CheckCanSnap(VRTK_InteractableObject interactableObjectCheck)
        {
            if (interactableObjectCheck != null)
            {
                AddCurrentValidSnapObject(interactableObjectCheck);
                if (!isSnapped && ValidSnapObject(interactableObjectCheck, true))
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

        /// <summary>
        /// The ForceSnap method attempts to automatically attach a valid GameObject to the snap drop zone.
        /// </summary>
        /// <param name="objectToSnap">The GameObject to attempt to snap.</param>
        public virtual void ForceSnap(GameObject objectToSnap)
        {
            ForceSnap(objectToSnap.GetComponentInParent<VRTK_InteractableObject>());
        }

        /// <summary>
        /// The ForceSnap method attempts to automatically attach a valid Interactable Object to the snap drop zone.
        /// </summary>
        /// <param name="interactableObjectToSnap">The Interactable Object to attempt to snap.</param>
        public virtual void ForceSnap(VRTK_InteractableObject interactableObjectToSnap)
        {
            if (interactableObjectToSnap != null)
            {
                if (attemptTransitionAtEndOfFrameRoutine != null)
                {
                    StopCoroutine(attemptTransitionAtEndOfFrameRoutine);
                }

                if (checkCanSnapRoutine != null)
                {
                    StopCoroutine(checkCanSnapRoutine);
                }

                if (interactableObjectToSnap.IsGrabbed())
                {
                    interactableObjectToSnap.ForceStopInteracting();
                }

                if (gameObject.activeInHierarchy)
                {
                    attemptTransitionAtEndOfFrameRoutine = StartCoroutine(AttemptForceSnapAtEndOfFrame(interactableObjectToSnap));
                }
            }
        }

        protected virtual IEnumerator AttemptForceSnapAtEndOfFrame(VRTK_InteractableObject objectToSnap)
        {
            yield return new WaitForEndOfFrame();
            objectToSnap.SaveCurrentState();
            AttemptForceSnap(objectToSnap);
        }

        protected virtual void AttemptForceSnap(VRTK_InteractableObject objectToSnap)
        {
            //Force snap settings on
            willSnap = true;
            //Force touch one of the Interactable Objects's colliders on this trigger collider
            SnapObjectToZone(objectToSnap);
        }

        protected virtual void SnapObjectToZone(VRTK_InteractableObject objectToSnap)
        {
            //Snap if the Interactable Object if not already snapped and if both valid and not currently grabbed
            if (!isSnapped && ValidSnapObject(objectToSnap, false))
            {
                SnapObject(objectToSnap);
            }
        }

        /// <summary>
        /// The SnapObject method implements the logic for snapping Interactable Objects to the paired Snap Drop Zone.
        /// </summary>
        protected virtual void SnapObject(VRTK_InteractableObject interactableObjectCheck)
        {
            
        }

        /// <summary>
        /// The CheckCanUnsnap method tests to see if an Interactable Object is available to be unsnapped, and if so, runs through the unsnap logic.
        /// </summary>
        public virtual void CheckCanUnsnap(VRTK_InteractableObject interactableObjectCheck)
        {
            if (interactableObjectCheck != null && currentValidSnapInteractableObjects.Contains(interactableObjectCheck) && ValidUnsnap(interactableObjectCheck))
            {
                if (isSnapped && currentSnappedObject == interactableObjectCheck)
                {
                    ForceUnsnap();
                }

                RemoveCurrentValidSnapObject(interactableObjectCheck);

                if (!pairedSnapDropZone.ValidSnappableObjectIsHovering())
                {
                    pairedSnapDropZone.ToggleHighlight(interactableObjectCheck, false);
                    willSnap = false;
                }

                interactableObjectCheck.SetSnapDropZoneHover(pairedSnapDropZone, false);

                if (ValidSnapObject(interactableObjectCheck, true))
                {
                    pairedSnapDropZone.ToggleHighlightColor();
                    pairedSnapDropZone.OnObjectExitedSnapDropZone(pairedSnapDropZone.SetSnapDropZoneEvent(interactableObjectCheck.gameObject));
                }
            }
        }

        /// <summary>
        /// The ForceUnsnap method attempts to automatically remove the current snapped game object from the snap drop zone.
        /// </summary>
        public virtual void ForceUnsnap()
        {
            if (isSnapped && ValidSnapObject(currentSnappedObject, false))
            {
                currentSnappedObject.ToggleSnapDropZone(pairedSnapDropZone, false);
            }
        }

        /// <summary>
        /// The UnsnapObject method handles the core logic for unsnapping an Interactable Object from SnapDropZone associated with this snap attach script.
        /// </summary>
        public virtual void UnsnapObject()
        {
            
        }

        /// <summary>
        /// The ValidSnapObject method tests to see if an Interactable Object is a valid snappable object based on its grab state and this component's `VRTK_PolicyList`.
        /// </summary>
        /// <param name="interactableObjectToSnap">The Interactable Object to check.</param>
        /// <param name="grabState">The desired Interactable Object grabbed state to test for.</param>
        /// <param name="checkGrabState">Whether the grab state affects snap validity.</param>
        /// <returns>Whether an Interactable Object is currently snapped to the Snap Drop Zone.</returns>
        public virtual bool ValidSnapObject(VRTK_InteractableObject interactableObjectCheck, bool grabState, bool checkGrabState = true)
        {
            return (interactableObjectCheck != null && (!checkGrabState || interactableObjectCheck.IsGrabbed() == grabState) && !VRTK_PolicyList.Check(interactableObjectCheck.gameObject, validObjectListPolicy));
        }

        /// <summary>
        /// The ValidUnsnap method tests to see if an Interactable Object is permitted to be unsnapped.
        /// </summary>
        /// <param name="interactableObjectToSnap">The Interactable Object to check.</param>
        /// <returns>Whether an Interactable Object is permitted to be unsnapped.</returns>
        protected virtual bool ValidUnsnap(VRTK_InteractableObject interactableObjectCheck)
        {
            return (interactableObjectCheck.IsGrabbed() || ((pairedSnapDropZone.snapType != VRTK_SnapDropZone.SnapTypes.UseJoint || !float.IsInfinity(GetComponent<Joint>().breakForce)) && interactableObjectCheck.validDrop == VRTK_InteractableObject.ValidDropTypes.DropAnywhere));
        }

        protected virtual Vector3 GetNewLocalScale(VRTK_InteractableObject checkObject)
        {

            Vector3 newLocalScale = checkObject.transform.localScale;

            if (scaleType == ScaleTypes.MatchBounds)
            {
                //Save and wipe object position and rotation so that we can get a proper set of bounds
                Vector3 savedGrabbedPosition = checkObject.transform.position;
                Quaternion savedGrabbedRotation = checkObject.transform.rotation;
                checkObject.transform.position = pairedSnapDropZone.highlightContainer.transform.position;
                checkObject.transform.rotation = pairedSnapDropZone.highlightContainer.transform.rotation;

                checkObject.StoreLocalScale();
                BoxCollider slotCollider = gameObject.GetComponentInChildren<Renderer>().gameObject.AddComponent<BoxCollider>();
                Bounds slotBounds = slotCollider.bounds;

                MeshCollider objectCollider = checkObject.GetComponentInChildren<MeshCollider>();
                if (objectCollider != null)
                {
                    Bounds objectBounds = objectCollider.bounds;

                    //Make sure the largest dimension of the snapped object fits within the smallest bound of the Snap Drop Zone, assuming that the zone has the same x, y, and z dimensions
                    float smallestSlotBound = Mathf.Min(slotBounds.size.x, slotBounds.size.y);
                    float largestObjectBound = Mathf.Max(objectBounds.size.x, objectBounds.size.y, objectBounds.size.z);
                    float scaleMultiplier = smallestSlotBound / largestObjectBound;

                    //Calculate the object bounds accounting for all all colliders
                    Vector3 min = Vector3.positiveInfinity;
                    Vector3 max = Vector3.negativeInfinity;
                    foreach (Collider collider in checkObject.GetComponents<Collider>())
                    {
                        Bounds bounds = collider.bounds;
                        min = Vector3.Min(min, bounds.min);
                        max = Vector3.Max(max, bounds.max);
                    }

                    newLocalScale = new Vector3(checkObject.transform.localScale.x * scaleMultiplier, checkObject.transform.localScale.y * scaleMultiplier, checkObject.transform.localScale.z * scaleMultiplier);

                    //Scale it down by a small amount to prevent z-fighting
                    newLocalScale = newLocalScale * .99f;
                    Destroy(slotCollider);
                }
                //Re-apply saved position and rotation to the grabbed interactable object
                checkObject.transform.position = savedGrabbedPosition;
                checkObject.transform.rotation = savedGrabbedRotation;
            }
            else if (scaleType == ScaleTypes.MatchScale)
            {
                checkObject.StoreLocalScale();
                newLocalScale = Vector3.Scale(checkObject.transform.localScale, transform.localScale);
            }

            return newLocalScale;
        }

        protected virtual Vector3 getLocalPositionOffset(VRTK_InteractableObject checkObject, Vector3 endScale)
        {
            Vector3 newPositionOffset = checkObject.transform.position;

            //Save and wipe object position, rotation and scale so that we can get a proper set of bounds
            Vector3 savedGrabbedPosition = checkObject.transform.position;
            Quaternion savedGrabbedRotation = checkObject.transform.rotation;
            Vector3 savedGrabbedScale = checkObject.transform.localScale;
            checkObject.transform.position = pairedSnapDropZone.highlightContainer.transform.position;
            checkObject.transform.rotation = pairedSnapDropZone.highlightContainer.transform.rotation;
            checkObject.transform.localScale = endScale;

            MeshCollider objectCollider = checkObject.GetComponentInChildren<MeshCollider>();
            if (objectCollider != null)
            {
                //Create a new collider just for measuring the bounds of the Snap Drop Zone
                BoxCollider slotCollider = gameObject.GetComponentInChildren<Renderer>().gameObject.AddComponent<BoxCollider>();
                Bounds slotBounds = slotCollider.bounds;

                Bounds objectBounds = objectCollider.bounds;
                newPositionOffset = slotBounds.center - objectBounds.center;
                Destroy(slotCollider);
            }
            else
            {
                VRTK_Logger.Warn("VRTK_BaseSnapAttach.getLocalPositionOffset could not fine a mesh collider on Interactable Object " + checkObject.name);
                newPositionOffset = Vector3.zero;
            }
            //Re-apply saved position and rotation to the grabbed interactable object
            checkObject.transform.position = savedGrabbedPosition;
            checkObject.transform.rotation = savedGrabbedRotation;
            checkObject.transform.localScale = savedGrabbedScale;

            return newPositionOffset;
        }

        public virtual void CheckSnappedItemExists()
        {
            if (isSnapped && (currentSnappedObject == null || !currentSnappedObject.gameObject.activeInHierarchy))
            {
                ForceUnsnap();
                pairedSnapDropZone.OnObjectUnsnappedFromDropZone(pairedSnapDropZone.SetSnapDropZoneEvent((currentSnappedObject != null ? currentSnappedObject.gameObject : null)));
            }
        }

        public virtual void AddCurrentValidSnapObject(VRTK_InteractableObject givenObject)
        {
            if (givenObject != null)
            {
                if (VRTK_SharedMethods.AddListValue(currentValidSnapInteractableObjects, givenObject, true))
                {
                    givenObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
                    givenObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
                }
            }
        }

        public virtual void RemoveCurrentValidSnapObject(VRTK_InteractableObject givenObject)
        {
            if (givenObject != null)
            {
                if (currentValidSnapInteractableObjects.Remove(givenObject))
                {
                    givenObject.InteractableObjectGrabbed -= InteractableObjectGrabbed;
                    givenObject.InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
                }
            }
        }

        protected virtual IEnumerator CheckCanSnapObjectAtEndOfFrame(VRTK_InteractableObject interactableObjectCheck)
        {
            yield return new WaitForEndOfFrame();
            CheckCanSnap(interactableObjectCheck);
        }

        protected virtual IEnumerator UpdateTransformDimensions(VRTK_InteractableObject ioCheck, GameObject endSettings, Vector3 endScale, float duration)
        {
            float elapsedTime = 0f;
            Transform ioTransform = ioCheck.transform;
            Vector3 startPosition = ioTransform.position;
            Quaternion startRotation = ioTransform.rotation;
            Vector3 startScale = ioTransform.localScale;
            Vector3 correctedPosition = endSettings.transform.position;
            bool storedKinematicState = ioCheck.isKinematic;
            ioCheck.isKinematic = true;

            if (scaleType == ScaleTypes.MatchBounds)
            {
                correctedPosition = endSettings.transform.position + getLocalPositionOffset(ioCheck, endScale);
            }

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                if (ioTransform != null && endSettings != null)
                {
                    ioTransform.position = Vector3.Lerp(startPosition, correctedPosition, (elapsedTime / duration));
                    ioTransform.rotation = Quaternion.Lerp(startRotation, endSettings.transform.rotation, (elapsedTime / duration));
                    ioTransform.localScale = Vector3.Lerp(startScale, endScale, (elapsedTime / duration));
                }
                yield return null;
            }

            //Force all to the last setting in case anything has moved during the transition
            ioTransform.position = correctedPosition;
            ioTransform.rotation = endSettings.transform.rotation;
            ioTransform.localScale = endScale;

            ioCheck.isKinematic = storedKinematicState;
            SetDropSnapType(ioCheck);
        }

        protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject grabbedInteractableObject = sender as VRTK_InteractableObject;
            if (!grabbedInteractableObject.IsInSnapDropZone())
            {
                CheckCanSnap(grabbedInteractableObject);
            }
        }

        protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            VRTK_InteractableObject releasedInteractableObject = sender as VRTK_InteractableObject;

            if (attemptTransitionAtEndOfFrameRoutine != null)
            {
                StopCoroutine(attemptTransitionAtEndOfFrameRoutine);
            }
            attemptTransitionAtEndOfFrameRoutine = StartCoroutine(AttemptForceSnapAtEndOfFrame(releasedInteractableObject));
        }

        protected virtual void UnregisterAllGrabEvents()
        {
            for (int i = 0; i < GetHoveringInteractableObjects().Count; i++)
            {
                GetHoveringInteractableObjects()[i].InteractableObjectGrabbed -= InteractableObjectGrabbed;
                GetHoveringInteractableObjects()[i].InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
            }
        }

        protected virtual void SetDropSnapType(VRTK_InteractableObject ioCheck)
        {
            switch (pairedSnapDropZone.snapType)
            {
                case VRTK_SnapDropZone.SnapTypes.UseKinematic:
                    ioCheck.SaveCurrentState();
                    ioCheck.isKinematic = true;
                    break;
                case VRTK_SnapDropZone.SnapTypes.UseParenting:
                    ioCheck.SaveCurrentState();
                    ioCheck.isKinematic = true;
                    ioCheck.transform.SetParent(transform);
                    break;
                case VRTK_SnapDropZone.SnapTypes.UseJoint:
                    SetSnapDropZoneJoint(ioCheck.GetComponent<Rigidbody>());
                    break;
            }
            pairedSnapDropZone.OnObjectSnappedToDropZone(pairedSnapDropZone.SetSnapDropZoneEvent(ioCheck.gameObject));
        }

        protected virtual void SetSnapDropZoneJoint(Rigidbody snapTo)
        {
            Joint snapDropZoneJoint = GetComponent<Joint>();
            if (snapDropZoneJoint == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "SnapDropZone:" + name, "Joint", "the same", " because the `Snap Type` is set to `Use Joint`"));
                return;
            }
            if (snapTo == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_SnapDropZone", "Rigidbody", "the `VRTK_InteractableObject`"));
                return;
            }

            snapDropZoneJoint.connectedBody = snapTo;
            originalJointCollisionState = snapDropZoneJoint.enableCollision;
            //need to set this to true otherwise highlighting doesn't work again on grab
            snapDropZoneJoint.enableCollision = true;
        }

        protected virtual void ResetSnapDropZoneJoint()
        {
            Joint snapDropZoneJoint = GetComponent<Joint>();
            if (snapDropZoneJoint != null)
            {
                snapDropZoneJoint.enableCollision = originalJointCollisionState;
            }
        }

        public bool WillSnap () {
            return willSnap;
        }
    }
}
