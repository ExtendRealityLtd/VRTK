// Snap Drop Zone|Prefabs|0080
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Highlighters;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="snappedObject">The interactable object that is dealing with the snap drop zone.</param>
    public struct SnapDropZoneEventArgs
    {
        public GameObject snappedObject;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="SnapDropZoneEventArgs"/></param>
    public delegate void SnapDropZoneEventHandler(object sender, SnapDropZoneEventArgs e);

    /// <summary>
    /// Provides a predefined zone where a valid interactable object can be dropped and upon dropping it snaps to the set snap drop zone transform position, rotation and scale.
    /// </summary>
    /// <remarks>
    /// **Prefab Usage:**
    ///  * Place the `VRTK/Prefabs/SnapDropZone/SnapDropZone` prefab into the scene hierarchy.
    ///  * Provide the SnapDropZone with an optional `Highlight Object Prefab` to generate an object outline in the scene that determines the final position, rotation and scale of the snapped object.
    ///  * If no `VRTK_BaseHighlighter` derivative is applied to the SnapDropZone then the default MaterialColorSwap Highlighter will be used.
    ///  * The collision zone that activates the SnapDropZone is a `Sphere Collider` by default but can be amended or replaced on the SnapDropZone GameObject.
    ///  * If the `Use Joint` Snap Type is selected then a custom Joint component is required to be added to the `SnapDropZone` Game Object and upon release the interactable object's rigidbody will be linked to this joint as the `Connected Body`.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/041_Controller_ObjectSnappingToDropZones` uses the `VRTK_SnapDropZone` prefab to set up pre-determined snap zones for a range of objects and demonstrates how only objects of certain types can be snapped into certain areas.
    /// </example>
    [ExecuteInEditMode]
    public class VRTK_SnapDropZone : MonoBehaviour
    {
        /// <summary>
        /// The types of snap on release available.
        /// </summary>
        public enum SnapTypes
        {
            /// <summary>
            /// Will set the interactable object rigidbody to `isKinematic = true`.
            /// </summary>
            UseKinematic,
            /// <summary>
            /// Will attach the interactable object's rigidbody to the provided joint as it's `Connected Body`.
            /// </summary>
            UseJoint,
            /// <summary>
            /// Will set the SnapDropZone as the interactable object's parent and set it's rigidbody to `isKinematic = true`.
            /// </summary>
            UseParenting
        }

        [Tooltip("A game object that is used to draw the highlighted destination for within the drop zone. This object will also be created in the Editor for easy placement.")]
        public GameObject highlightObjectPrefab;
        [Tooltip("The Snap Type to apply when a valid interactable object is dropped within the snap zone.")]
        public SnapTypes snapType = SnapTypes.UseKinematic;
        [Tooltip("The amount of time it takes for the object being snapped to move into the new snapped position, rotation and scale.")]
        public float snapDuration = 0f;
        [Tooltip("If this is checked then the scaled size of the snap drop zone will be applied to the object that is snapped to it.")]
        public bool applyScalingOnSnap = false;
        [Tooltip("If this is checked then when the snapped object is unsnapped from the drop zone, a clone of the unsnapped object will be snapped back into the drop zone.")]
        public bool cloneNewOnUnsnap = false;
        [Tooltip("The colour to use when showing the snap zone is active. This is used as the highlight colour when no object is hovering but `Highlight Always Active` is true.")]
        public Color highlightColor = Color.clear;
        [Tooltip("The colour to use when showing the snap zone is active and a valid object is hovering. If this is `Color.clear` then the `Highlight Color` will be used.")]
        public Color validHighlightColor = Color.clear;
        [Tooltip("The highlight object will always be displayed when the snap drop zone is available even if a valid item isn't being hovered over.")]
        public bool highlightAlwaysActive = false;
        [Tooltip("A specified VRTK_PolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release.")]
        public VRTK_PolicyList validObjectListPolicy;
        [Tooltip("If this is checked then the drop zone highlight section will be displayed in the scene editor window.")]
        public bool displayDropZoneInEditor = true;

        [Tooltip("The Interactable Object to snap into the dropzone when the drop zone is enabled. The Interactable Object must be valid in any given policy list to snap.")]
        public VRTK_InteractableObject defaultSnappedInteractableObject;

        [Header("Obsolete Settings")]

        [System.Obsolete("`VRTK_SnapDropZone.defaultSnappedObject` has been replaced with the `VRTK_SnapDropZone.defaultSnappedInteractableObject`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public GameObject defaultSnappedObject;

        /// <summary>
        /// Emitted when a valid interactable object enters the snap drop zone trigger collider.
        /// </summary>
        public event SnapDropZoneEventHandler ObjectEnteredSnapDropZone;
        /// <summary>
        /// Emitted when a valid interactable object exists the snap drop zone trigger collider.
        /// </summary>
        public event SnapDropZoneEventHandler ObjectExitedSnapDropZone;
        /// <summary>
        /// Emitted when an interactable object is successfully snapped into a drop zone.
        /// </summary>
        public event SnapDropZoneEventHandler ObjectSnappedToDropZone;
        /// <summary>
        /// Emitted when an interactable object is removed from a snapped drop zone.
        /// </summary>
        public event SnapDropZoneEventHandler ObjectUnsnappedFromDropZone;

        protected GameObject previousPrefab;
        protected GameObject highlightContainer;
        protected GameObject highlightObject;
        protected GameObject highlightEditorObject = null;

        protected List<VRTK_InteractableObject> currentValidSnapInteractableObjects = new List<VRTK_InteractableObject>();
        protected VRTK_InteractableObject currentSnappedObject = null;
        protected GameObject objectToClone = null;
        protected bool[] clonedObjectColliderStates = new bool[0];

        protected bool willSnap = false;
        protected bool isSnapped = false;
        protected bool wasSnapped = false;
        protected bool isHighlighted = false;

        protected VRTK_BaseHighlighter objectHighlighter;
        protected Coroutine transitionInPlaceRoutine;
        protected Coroutine attemptTransitionAtEndOfFrameRoutine;
        protected Coroutine checkCanSnapRoutine;
        protected bool originalJointCollisionState = false;

        protected const string HIGHLIGHT_CONTAINER_NAME = "HighlightContainer";
        protected const string HIGHLIGHT_OBJECT_NAME = "HighlightObject";
        protected const string HIGHLIGHT_EDITOR_OBJECT_NAME = "EditorHighlightObject";

        public virtual void OnObjectEnteredSnapDropZone(SnapDropZoneEventArgs e)
        {
            if (ObjectEnteredSnapDropZone != null)
            {
                ObjectEnteredSnapDropZone(this, e);
            }
        }

        public virtual void OnObjectExitedSnapDropZone(SnapDropZoneEventArgs e)
        {
            if (ObjectExitedSnapDropZone != null)
            {
                ObjectExitedSnapDropZone(this, e);
            }
        }

        public virtual void OnObjectSnappedToDropZone(SnapDropZoneEventArgs e)
        {
            if (ObjectSnappedToDropZone != null)
            {
                ObjectSnappedToDropZone(this, e);
            }
        }

        public virtual void OnObjectUnsnappedFromDropZone(SnapDropZoneEventArgs e)
        {
            UnsnapObject();
            if (ObjectUnsnappedFromDropZone != null)
            {
                ObjectUnsnappedFromDropZone(this, e);
            }
        }

        public virtual SnapDropZoneEventArgs SetSnapDropZoneEvent(GameObject interactableObject)
        {
            SnapDropZoneEventArgs e;
            e.snappedObject = interactableObject;
            return e;
        }

        /// <summary>
        /// The InitaliseHighlightObject method sets up the highlight object based on the given Highlight Object Prefab.
        /// </summary>
        /// <param name="removeOldObject">If this is set to true then it attempts to delete the old highlight object if it exists. Defaults to `false`</param>
        public virtual void InitaliseHighlightObject(bool removeOldObject = false)
        {
            //force delete previous created highlight object
            if (removeOldObject)
            {
                DeleteHighlightObject();
            }
            //Always remove editor highlight object at runtime
            ChooseDestroyType(transform.Find(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME)));
            highlightEditorObject = null;

            GenerateObjects();
        }

        /// <summary>
        /// the ForceSnap method attempts to automatically attach a valid GameObject to the snap drop zone.
        /// </summary>
        /// <param name="objectToSnap">The GameObject to attempt to snap.</param>
        public virtual void ForceSnap(GameObject objectToSnap)
        {
            ForceSnap(objectToSnap.GetComponentInParent<VRTK_InteractableObject>());
        }

        /// <summary>
        /// the ForceSnap method attempts to automatically attach a valid Interactable Object to the snap drop zone.
        /// </summary>
        /// <param name="objectToSnap">The Interactable Object to attempt to snap.</param>
        protected virtual void ForceSnap(VRTK_InteractableObject interactableObjectToSnap)
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

        /// <summary>
        /// The ForceUnsnap method attempts to automatically remove the current snapped game object from the snap drop zone.
        /// </summary>
        public virtual void ForceUnsnap()
        {
            if (isSnapped && ValidSnapObject(currentSnappedObject, false))
            {
                currentSnappedObject.ToggleSnapDropZone(this, false);
            }
        }

        /// <summary>
        /// The ValidSnappableObjectIsHovering method determines if any valid objects are currently hovering in the snap drop zone area.
        /// </summary>
        /// <returns>Returns true if a valid object is currently in the snap drop zone area.</returns>
        public virtual bool ValidSnappableObjectIsHovering()
        {
            for (int i = 0; i < currentValidSnapInteractableObjects.Count; i++)
            {
                if (currentValidSnapInteractableObjects[i].IsGrabbed())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// The IsObjectHovering method determines if the given GameObject is currently howvering (but not snapped) in the snap drop zone area.
        /// </summary>
        /// <param name="checkObject">The GameObject to check to see if it's hovering in the snap drop zone area.</param>
        /// <returns>Returns true if the given GameObject is hovering (but not snapped) in the snap drop zone area.</returns>
        public virtual bool IsObjectHovering(GameObject checkObject)
        {
            VRTK_InteractableObject interactableObjectToCheck = checkObject.GetComponentInParent<VRTK_InteractableObject>();
            return (interactableObjectToCheck != null ? currentValidSnapInteractableObjects.Contains(interactableObjectToCheck) : false);
        }

        /// <summary>
        /// The IsInteractableObjectHovering method determines if the given Interactable Object script is currently howvering (but not snapped) in the snap drop zone area.
        /// </summary>
        /// <param name="checkObject">The Interactable Object script to check to see if it's hovering in the snap drop zone area.</param>
        /// <returns>Returns true if the given Interactable Object script is hovering (but not snapped) in the snap drop zone area.</returns>
        public virtual bool IsInteractableObjectHovering(VRTK_InteractableObject checkObject)
        {
            return (checkObject != null ? currentValidSnapInteractableObjects.Contains(checkObject) : false);
        }

        /// <summary>
        /// The GetHoveringObjects method returns a List of valid GameObjects that are currently hovering (but not snapped) in the snap drop zone area.
        /// </summary>
        /// <returns>The List of valid GameObjects that are hovering (but not snapped) in the snap drop zone area.</returns>
        public virtual List<GameObject> GetHoveringObjects()
        {
            List<GameObject> returnList = new List<GameObject>();
            for (int i = 0; i < currentValidSnapInteractableObjects.Count; i++)
            {
                VRTK_SharedMethods.AddListValue(returnList, currentValidSnapInteractableObjects[i].gameObject);
            }
            return returnList;
        }

        /// <summary>
        /// The GetHoveringInteractableObjects method returns a List of valid Interactable Object scripts that are currently hovering (but not snapped) in the snap drop zone area.
        /// </summary>
        /// <returns>The List of valid Interactable Object scripts that are hovering (but not snapped) in the snap drop zone area.</returns>
        public virtual List<VRTK_InteractableObject> GetHoveringInteractableObjects()
        {
            return currentValidSnapInteractableObjects;
        }

        /// <summary>
        /// The GetCurrentSnappedObejct method returns the GameObject that is currently snapped in the snap drop zone area.
        /// </summary>
        /// <returns>The GameObject that is currently snapped in the snap drop zone area.</returns>
        public virtual GameObject GetCurrentSnappedObject()
        {
            return (currentSnappedObject != null ? currentSnappedObject.gameObject : null);
        }

        /// <summary>
        /// The GetCurrentSnappedInteractableObject method returns the Interactable Object script that is currently snapped in the snap drop zone area.
        /// </summary>
        /// <returns>The Interactable Object script that is currently snapped in the snap drop zone area.</returns>
        public virtual VRTK_InteractableObject GetCurrentSnappedInteractableObject()
        {
            return currentSnappedObject;
        }

        protected virtual void Awake()
        {
            if (Application.isPlaying)
            {
                InitaliseHighlightObject();
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (objectHighlighter != null)
            {
                objectHighlighter.Unhighlight();
            }
        }

        protected virtual void OnEnable()
        {
            currentValidSnapInteractableObjects.Clear();
            currentSnappedObject = null;
            objectToClone = null;
            clonedObjectColliderStates = new bool[0];
            willSnap = false;
            isSnapped = false;
            wasSnapped = false;
            isHighlighted = false;

#pragma warning disable 618
            if (defaultSnappedObject != null && defaultSnappedInteractableObject == null)
            {
                defaultSnappedInteractableObject = defaultSnappedObject.GetComponentInParent<VRTK_InteractableObject>();
            }
#pragma warning restore 618

            DisableHighlightShadows();
            if (!VRTK_SharedMethods.IsEditTime() && Application.isPlaying && defaultSnappedInteractableObject != null)
            {
                ForceSnap(defaultSnappedInteractableObject);
            }
        }

        protected virtual void OnDisable()
        {
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
            SetHighlightObjectActive(false);
            UnregisterAllUngrabEvents();
        }

        protected virtual void Update()
        {
            CheckSnappedItemExists();
            CheckPrefabUpdate();
            CreateHighlightersInEditor();
            CheckCurrentValidSnapObjectStillValid();
            previousPrefab = highlightObjectPrefab;
            SetObjectHighlight();
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            CheckCanSnap(collider.GetComponentInParent<VRTK_InteractableObject>());
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            CheckCanUnsnap(collider.GetComponentInParent<VRTK_InteractableObject>());
        }

        protected virtual void CheckCanSnap(VRTK_InteractableObject interactableObjectCheck)
        {
            if (interactableObjectCheck != null && ValidSnapObject(interactableObjectCheck, true))
            {
                AddCurrentValidSnapObject(interactableObjectCheck);
                if (!isSnapped)
                {
                    ToggleHighlight(interactableObjectCheck, true);
                    interactableObjectCheck.SetSnapDropZoneHover(this, true);
                    if (!willSnap)
                    {
                        OnObjectEnteredSnapDropZone(SetSnapDropZoneEvent(interactableObjectCheck.gameObject));
                    }
                    willSnap = true;
                    ToggleHighlightColor();
                }
            }
        }

        protected virtual void CheckCanUnsnap(VRTK_InteractableObject interactableObjectCheck)
        {
            if (interactableObjectCheck != null && currentValidSnapInteractableObjects.Contains(interactableObjectCheck) && ValidUnsnap(interactableObjectCheck))
            {
                if (isSnapped && currentSnappedObject == interactableObjectCheck)
                {
                    ForceUnsnap();
                }

                RemoveCurrentValidSnapObject(interactableObjectCheck);

                if (!ValidSnappableObjectIsHovering())
                {
                    ToggleHighlight(interactableObjectCheck, false);
                    willSnap = false;
                }

                interactableObjectCheck.SetSnapDropZoneHover(this, false);

                if (ValidSnapObject(interactableObjectCheck, true))
                {
                    ToggleHighlightColor();
                    OnObjectExitedSnapDropZone(SetSnapDropZoneEvent(interactableObjectCheck.gameObject));
                }
            }
        }

        protected virtual bool ValidUnsnap(VRTK_InteractableObject interactableObjectCheck)
        {
            return (interactableObjectCheck.IsGrabbed() || ((snapType != SnapTypes.UseJoint || !float.IsInfinity(GetComponent<Joint>().breakForce)) && interactableObjectCheck.validDrop == VRTK_InteractableObject.ValidDropTypes.DropAnywhere));
        }

        protected virtual void SnapObjectToZone(VRTK_InteractableObject objectToSnap)
        {
            if (!isSnapped && ValidSnapObject(objectToSnap, false))
            {
                SnapObject(objectToSnap);
            }
        }

        protected virtual void UnregisterAllUngrabEvents()
        {
            for (int i = 0; i < currentValidSnapInteractableObjects.Count; i++)
            {
                currentValidSnapInteractableObjects[i].InteractableObjectGrabbed -= InteractableObjectGrabbed;
                currentValidSnapInteractableObjects[i].InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
            }
        }

        protected virtual bool ValidSnapObject(VRTK_InteractableObject interactableObjectCheck, bool grabState, bool checkGrabState = true)
        {
            return (interactableObjectCheck != null && (!checkGrabState || interactableObjectCheck.IsGrabbed() == grabState) && !VRTK_PolicyList.Check(interactableObjectCheck.gameObject, validObjectListPolicy));
        }

        protected virtual string ObjectPath(string name)
        {
            return HIGHLIGHT_CONTAINER_NAME + "/" + name;
        }

        protected virtual void CheckSnappedItemExists()
        {
            if (isSnapped && (currentSnappedObject == null || !currentSnappedObject.gameObject.activeInHierarchy))
            {
                ForceUnsnap();
                OnObjectUnsnappedFromDropZone(SetSnapDropZoneEvent((currentSnappedObject != null ? currentSnappedObject.gameObject : null)));
            }
        }

        protected virtual void CheckPrefabUpdate()
        {
            //If the highlightObjectPrefab has changed then delete the highlight object in preparation to create a new one
            if (previousPrefab != null && previousPrefab != highlightObjectPrefab)
            {
                DeleteHighlightObject();
            }
        }

        protected virtual void SetObjectHighlight()
        {
            if (highlightAlwaysActive && !isSnapped && !isHighlighted)
            {
                SetHighlightObjectActive(true);
                ToggleHighlightColor();
            }

            if (!highlightAlwaysActive && isHighlighted && !ValidSnappableObjectIsHovering())
            {
                SetHighlightObjectActive(false);
            }
        }

        protected virtual void ToggleHighlightColor()
        {
            if (Application.isPlaying && highlightAlwaysActive && !isSnapped && objectHighlighter != null)
            {
                objectHighlighter.Highlight((willSnap && validHighlightColor != Color.clear ? validHighlightColor : highlightColor));
            }
        }

        protected virtual void CreateHighlightersInEditor()
        {
            if (VRTK_SharedMethods.IsEditTime())
            {
                GenerateHighlightObject();

                if (snapType == SnapTypes.UseJoint && GetComponent<Joint>() == null)
                {
                    VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "SnapDropZone:" + name, "Joint", "the same", " because the `Snap Type` is set to `Use Joint`"));
                }

                GenerateEditorHighlightObject();
                ForceSetObjects();
                if (highlightEditorObject != null)
                {
                    highlightEditorObject.SetActive(displayDropZoneInEditor);
                }
            }
        }

        protected virtual void CheckCurrentValidSnapObjectStillValid()
        {
            for (int i = 0; i < currentValidSnapInteractableObjects.Count; i++)
            {
                VRTK_InteractableObject interactableObjectCheck = currentValidSnapInteractableObjects[i];
                //if the interactable object associated with it has been snapped to another zone, then unset the current valid snap object
                if (interactableObjectCheck != null && interactableObjectCheck.GetStoredSnapDropZone() != null && interactableObjectCheck.GetStoredSnapDropZone() != this)
                {
                    RemoveCurrentValidSnapObject(interactableObjectCheck);
                    if (isHighlighted && highlightObject != null && !highlightAlwaysActive)
                    {
                        SetHighlightObjectActive(false);
                    }
                }
            }
        }

        protected virtual void ForceSetObjects()
        {
            if (highlightEditorObject == null)
            {
                Transform forceFindHighlightEditorObject = transform.Find(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME));
                highlightEditorObject = (forceFindHighlightEditorObject ? forceFindHighlightEditorObject.gameObject : null);
            }

            if (highlightObject == null)
            {
                Transform forceFindHighlightObject = transform.Find(ObjectPath(HIGHLIGHT_OBJECT_NAME));
                highlightObject = (forceFindHighlightObject ? forceFindHighlightObject.gameObject : null);
            }

            if (highlightContainer == null)
            {
                Transform forceFindHighlightContainer = transform.Find(HIGHLIGHT_CONTAINER_NAME);
                highlightContainer = (forceFindHighlightContainer ? forceFindHighlightContainer.gameObject : null);
            }
        }

        protected virtual void GenerateContainer()
        {
            if (highlightContainer == null || transform.Find(HIGHLIGHT_CONTAINER_NAME) == null)
            {
                highlightContainer = new GameObject(HIGHLIGHT_CONTAINER_NAME);
                highlightContainer.transform.SetParent(transform);
                highlightContainer.transform.localPosition = Vector3.zero;
                highlightContainer.transform.localRotation = Quaternion.identity;
                highlightContainer.transform.localScale = Vector3.one;
            }
        }

        protected virtual void DisableHighlightShadows()
        {
            if (highlightObject != null)
            {
                Renderer[] foundRenderers = highlightObject.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < foundRenderers.Length; i++)
                {
                    foundRenderers[i].receiveShadows = false;
                    foundRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
        }

        protected virtual void SetContainer()
        {
            Transform findContainer = transform.Find(HIGHLIGHT_CONTAINER_NAME);
            if (findContainer != null)
            {
                highlightContainer = findContainer.gameObject;
            }
        }

        protected virtual void GenerateObjects()
        {
            GenerateHighlightObject();
            if (highlightObject != null && objectHighlighter == null)
            {
                InitialiseHighlighter();
            }
        }

        protected virtual void SnapObject(VRTK_InteractableObject interactableObjectCheck)
        {
            //If the item is in a snappable position and this drop zone isn't snapped and the collider is a valid interactable object
            if (willSnap && !isSnapped && ValidSnapObject(interactableObjectCheck, false))
            {
                //Only snap it to the drop zone if it's not already in a drop zone
                if (!interactableObjectCheck.IsInSnapDropZone())
                {
                    if (highlightObject != null)
                    {
                        //Turn off the drop zone highlighter
                        SetHighlightObjectActive(false);
                    }

                    Vector3 newLocalScale = GetNewLocalScale(interactableObjectCheck);
                    if (transitionInPlaceRoutine != null)
                    {
                        StopCoroutine(transitionInPlaceRoutine);
                    }

                    isSnapped = true;
                    currentSnappedObject = interactableObjectCheck;
                    if (cloneNewOnUnsnap)
                    {
                        CreatePermanentClone();
                    }

                    if (gameObject.activeInHierarchy)
                    {
                        transitionInPlaceRoutine = StartCoroutine(UpdateTransformDimensions(interactableObjectCheck, highlightContainer, newLocalScale, snapDuration));
                    }

                    interactableObjectCheck.ToggleSnapDropZone(this, true);
                }
            }

            //Force reset isSnapped if the item is grabbed but isSnapped is still true
            isSnapped = (isSnapped && interactableObjectCheck != null && interactableObjectCheck.IsGrabbed() ? false : isSnapped);
            willSnap = !isSnapped;
            wasSnapped = false;
        }

        protected virtual void CreatePermanentClone()
        {
            VRTK_BaseHighlighter currentSnappedObjectHighlighter = currentSnappedObject.GetComponent<VRTK_BaseHighlighter>();
            if (currentSnappedObjectHighlighter != null)
            {
                currentSnappedObjectHighlighter.Unhighlight();
            }
            objectToClone = Instantiate(currentSnappedObject.gameObject);
            objectToClone.transform.position = highlightContainer.transform.position;
            objectToClone.transform.rotation = highlightContainer.transform.rotation;
            Collider[] clonedObjectStates = currentSnappedObject.GetComponentsInChildren<Collider>();
            clonedObjectColliderStates = new bool[clonedObjectStates.Length];
            for (int i = 0; i < clonedObjectStates.Length; i++)
            {
                Collider clonedObjectColliderState = clonedObjectStates[i];
                clonedObjectColliderStates[i] = clonedObjectColliderState.isTrigger;
                clonedObjectColliderState.isTrigger = true;
            }
            objectToClone.SetActive(false);
        }

        protected virtual void ResetPermanentCloneColliders(GameObject objectToReset)
        {
            if (objectToReset != null && clonedObjectColliderStates.Length > 0)
            {
                Collider[] clonedObjectStates = objectToReset.GetComponentsInChildren<Collider>();
                for (int i = 0; i < clonedObjectStates.Length; i++)
                {
                    Collider clonedObjectColliderState = clonedObjectStates[i];
                    if (clonedObjectColliderStates.Length > i)
                    {
                        clonedObjectColliderState.isTrigger = clonedObjectColliderStates[i];
                    }
                }
            }
        }

        protected virtual void ResnapPermanentClone()
        {
            if (objectToClone != null)
            {
                float savedSnapDuration = snapDuration;
                snapDuration = 0f;
                objectToClone.SetActive(true);
                ResetPermanentCloneColliders(objectToClone);
                ForceSnap(objectToClone);
                snapDuration = savedSnapDuration;
            }
        }

        protected virtual void UnsnapObject()
        {
            if (currentSnappedObject != null)
            {
                ResetPermanentCloneColliders(currentSnappedObject.gameObject);
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

            if (cloneNewOnUnsnap)
            {
                ResnapPermanentClone();
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

        protected virtual Vector3 GetNewLocalScale(VRTK_InteractableObject checkObject)
        {
            // If apply scaling is checked then use the drop zone scale to resize the object
            Vector3 newLocalScale = checkObject.transform.localScale;
            if (applyScalingOnSnap)
            {
                checkObject.StoreLocalScale();
                newLocalScale = Vector3.Scale(checkObject.transform.localScale, transform.localScale);
            }
            return newLocalScale;
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
            bool storedKinematicState = ioCheck.isKinematic;
            ioCheck.isKinematic = true;
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                if (ioTransform != null && endSettings != null)
                {
                    ioTransform.position = Vector3.Lerp(startPosition, endSettings.transform.position, (elapsedTime / duration));
                    ioTransform.rotation = Quaternion.Lerp(startRotation, endSettings.transform.rotation, (elapsedTime / duration));
                    ioTransform.localScale = Vector3.Lerp(startScale, endScale, (elapsedTime / duration));
                }
                yield return null;
            }

            //Force all to the last setting in case anything has moved during the transition
            ioTransform.position = endSettings.transform.position;
            ioTransform.rotation = endSettings.transform.rotation;
            ioTransform.localScale = endScale;

            ioCheck.isKinematic = storedKinematicState;
            SetDropSnapType(ioCheck);
        }

        protected virtual void SetDropSnapType(VRTK_InteractableObject ioCheck)
        {
            switch (snapType)
            {
                case SnapTypes.UseKinematic:
                    ioCheck.SaveCurrentState();
                    ioCheck.isKinematic = true;
                    break;
                case SnapTypes.UseParenting:
                    ioCheck.SaveCurrentState();
                    ioCheck.isKinematic = true;
                    ioCheck.transform.SetParent(transform);
                    break;
                case SnapTypes.UseJoint:
                    SetSnapDropZoneJoint(ioCheck.GetComponent<Rigidbody>());
                    break;
            }
            OnObjectSnappedToDropZone(SetSnapDropZoneEvent(ioCheck.gameObject));
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

        protected virtual void AddCurrentValidSnapObject(VRTK_InteractableObject givenObject)
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

        protected virtual void RemoveCurrentValidSnapObject(VRTK_InteractableObject givenObject)
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

        protected virtual void AttemptForceSnap(VRTK_InteractableObject objectToSnap)
        {
            //force snap settings on
            willSnap = true;
            //Force touch one of the object's colliders on this trigger collider
            SnapObjectToZone(objectToSnap);
        }

        protected virtual IEnumerator AttemptForceSnapAtEndOfFrame(VRTK_InteractableObject objectToSnap)
        {
            yield return new WaitForEndOfFrame();
            objectToSnap.SaveCurrentState();
            AttemptForceSnap(objectToSnap);
        }

        protected virtual void ToggleHighlight(VRTK_InteractableObject checkObject, bool state)
        {
            if (highlightObject != null && ValidSnapObject(checkObject, true, state))
            {
                //Toggle the highlighter state
                SetHighlightObjectActive(state);
            }
        }

        protected virtual void CopyObject(GameObject objectBlueprint, ref GameObject clonedObject, string givenName)
        {
            GenerateContainer();
            Vector3 saveScale = transform.localScale;
            transform.localScale = Vector3.one;

            clonedObject = Instantiate(objectBlueprint, highlightContainer.transform) as GameObject;
            clonedObject.name = givenName;

            //default position of new highlight object
            clonedObject.transform.localPosition = Vector3.zero;
            clonedObject.transform.localRotation = Quaternion.identity;

            transform.localScale = saveScale;
            CleanHighlightObject(clonedObject);
        }

        protected virtual void GenerateHighlightObject()
        {
            //If there is a given highlight prefab and no existing highlight object then create a new highlight object
            if (highlightObjectPrefab != null && highlightObject == null && transform.Find(ObjectPath(HIGHLIGHT_OBJECT_NAME)) == null)
            {
                CopyObject(highlightObjectPrefab, ref highlightObject, HIGHLIGHT_OBJECT_NAME);
            }

            //if highlight object exists but not in the variable then force grab it
            Transform checkForChild = transform.Find(ObjectPath(HIGHLIGHT_OBJECT_NAME));
            if (checkForChild != null && highlightObject == null)
            {
                highlightObject = checkForChild.gameObject;
            }

            //if no highlight object prefab is set but a highlight object is found then destroy the highlight object
            if (highlightObjectPrefab == null && highlightObject != null)
            {
                DeleteHighlightObject();
            }

            DisableHighlightShadows();
            SetHighlightObjectActive(false);
            SetContainer();
        }

        protected virtual void SetHighlightObjectActive(bool state)
        {
            if (highlightObject != null)
            {
                highlightObject.SetActive(state);
                isHighlighted = state;
            }
        }

        protected virtual void DeleteHighlightObject()
        {
            ChooseDestroyType(transform.Find(HIGHLIGHT_CONTAINER_NAME));
            highlightContainer = null;
            highlightObject = null;
            objectHighlighter = null;
        }

        protected virtual void GenerateEditorHighlightObject()
        {
            if (highlightObject != null && highlightEditorObject == null && transform.Find(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME)) == null)
            {
                CopyObject(highlightObject, ref highlightEditorObject, HIGHLIGHT_EDITOR_OBJECT_NAME);
                Renderer[] renderers = highlightEditorObject.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material = Resources.Load("SnapDropZoneEditorObject") as Material;
                }
                highlightEditorObject.SetActive(true);
            }
        }

        protected virtual void CleanHighlightObject(GameObject objectToClean)
        {
            //If the highlight object has any child snap zones, then force delete these
            VRTK_SnapDropZone[] deleteSnapZones = objectToClean.GetComponentsInChildren<VRTK_SnapDropZone>(true);
            for (int i = 0; i < deleteSnapZones.Length; i++)
            {
                ChooseDestroyType(deleteSnapZones[i].gameObject);
            }

            //determine components that shouldn't be deleted from highlight object
            string[] validComponents = new string[] { "Transform", "MeshFilter", "MeshRenderer", "SkinnedMeshRenderer", "VRTK_GameObjectLinker" };

            //First clean out the joints cause RigidBodys depends on them.
            Joint[] joints = objectToClean.GetComponentsInChildren<Joint>(true);
            for (int i = 0; i < joints.Length; i++)
            {
                ChooseDestroyType(joints[i]);
            }

            //Go through all of the components on the highlighted object and delete any components that aren't in the valid component list
            Component[] components = objectToClean.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                bool valid = false;

                //Loop through each valid component and check to see if this component is valid
                for (int j = 0; j < validComponents.Length; j++)
                {
                    //if it's a valid component then break the check
                    if (component.GetType().ToString().Contains("." + validComponents[j]))
                    {
                        valid = true;
                        break;
                    }
                }

                //if this is a valid component then just continue to the next component
                if (valid)
                {
                    continue;
                }

                //If not a valid component then delete it
                ChooseDestroyType(component);
            }
        }

        protected virtual void InitialiseHighlighter()
        {
            VRTK_BaseHighlighter existingHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(gameObject);
            //If no highlighter is found on the GameObject then create the default one
            if (existingHighlighter == null)
            {
                highlightObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
            }
            else
            {
                VRTK_SharedMethods.CloneComponent(existingHighlighter, highlightObject);
            }

            //Initialise highlighter and set highlight colour
            objectHighlighter = highlightObject.GetComponent<VRTK_BaseHighlighter>();
            objectHighlighter.unhighlightOnDisable = false;
            objectHighlighter.Initialise(highlightColor);
            objectHighlighter.Highlight(highlightColor);

            //if the object highlighter is using a cloned object then disable the created highlight object's renderers
            if (objectHighlighter.UsesClonedObject())
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (!VRTK_PlayerObject.IsPlayerObject(renderers[i].gameObject, VRTK_PlayerObject.ObjectTypes.Highlighter))
                    {
                        renderers[i].enabled = false;
                    }
                }
            }
        }

        protected virtual void ChooseDestroyType(Transform deleteTransform)
        {
            if (deleteTransform != null)
            {
                ChooseDestroyType(deleteTransform.gameObject);
            }
        }

        protected virtual void ChooseDestroyType(GameObject deleteObject)
        {
            if (VRTK_SharedMethods.IsEditTime())
            {
                if (deleteObject != null)
                {
                    DestroyImmediate(deleteObject);
                }
            }
            else
            {
                if (deleteObject != null)
                {
                    Destroy(deleteObject);
                }
            }
        }

        protected virtual void ChooseDestroyType(Component deleteComponent)
        {
            if (VRTK_SharedMethods.IsEditTime())
            {
                if (deleteComponent != null)
                {
                    DestroyImmediate(deleteComponent);
                }
            }
            else
            {
                if (deleteComponent != null)
                {
                    Destroy(deleteComponent);
                }
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (highlightObject != null && !displayDropZoneInEditor)
            {
                Vector3 boxSize = VRTK_SharedMethods.GetBounds(highlightObject.transform).size * 1.05f;
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(highlightObject.transform.position, boxSize);
            }
        }
    }
}
