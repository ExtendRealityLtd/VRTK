// Snap Drop Zone|Prefabs|0035
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
    /// This sets up a predefined zone where an existing interactable object can be dropped and upon dropping it snaps to the set snap drop zone transform position, rotation and scale.
    /// </summary>
    /// <remarks>
    /// The position, rotation and scale of the `SnapDropZone` Game Object will be used to determine the final position of the dropped interactable object if it is dropped within the drop zone collider volume.
    ///
    /// The provided Highlight Object Prefab is used to create the highlighting object (also within the Editor for easy placement) and by default the standard Material Color Swap highlighter is used.
    ///
    /// An alternative highlighter can also be added to the `SnapDropZone` Game Object and this new highlighter component will be used to show the interactable object position on release.
    ///
    /// The prefab is a pre-built game object that contains a default trigger collider (Sphere Collider) and a kinematic rigidbody (to ensure collisions occur).
    ///
    /// If an alternative collider is required, then the default Sphere Collider can be removed and another collider added.
    ///
    /// If the `Use Joint` Snap Type is selected then a custom Joint component is required to be added to the `SnapDropZone` Game Object and upon release the interactable object's rigidbody will be linked to this joint as the `Connected Body`.
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
        /// <param name="UseKinematic">Will set the interactable object rigidbody to `isKinematic = true`.</param>
        /// <param name="UseJoint">Will attach the interactable object's rigidbody to the provided joint as it's `Connected Body`.</param>
        /// <param name="UseParenting">Will set the SnapDropZone as the interactable object's parent and set it's rigidbody to `isKinematic = true`.</param>
        public enum SnapTypes
        {
            UseKinematic,
            UseJoint,
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
        [Tooltip("The colour to use when showing the snap zone is active.")]
        public Color highlightColor;
        [Tooltip("The highlight object will always be displayed when the snap drop zone is available even if a valid item isn't being hovered over.")]
        public bool highlightAlwaysActive = false;
        [Tooltip("A specified VRTK_PolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release.")]
        public VRTK_PolicyList validObjectListPolicy;
        [Tooltip("If this is checked then the drop zone highlight section will be displayed in the scene editor window.")]
        public bool displayDropZoneInEditor = true;
        [Tooltip("The game object to snap into the dropzone when the drop zone is enabled. The game object must be valid in any given policy list to snap.")]
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
        protected List<GameObject> currentValidSnapObjects = new List<GameObject>();
        protected GameObject currentSnappedObject = null;
        protected GameObject objectToClone = null;
        protected bool[] clonedObjectColliderStates = new bool[0];
        protected VRTK_BaseHighlighter objectHighlighter;
        protected bool willSnap = false;
        protected bool isSnapped = false;
        protected bool wasSnapped = false;
        protected bool isHighlighted = false;
        protected Coroutine transitionInPlace;
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
        /// the ForceSnap method attempts to automatically attach a valid game object to the snap drop zone.
        /// </summary>
        /// <param name="objectToSnap">The GameObject to attempt to snap.</param>
        public virtual void ForceSnap(GameObject objectToSnap)
        {
            VRTK_InteractableObject ioCheck = objectToSnap.GetComponentInParent<VRTK_InteractableObject>();
            if (ioCheck != null)
            {
                ioCheck.SaveCurrentState();
                StopCoroutine("AttemptForceSnapAtEndOfFrame");
                if (ioCheck.IsGrabbed())
                {
                    ioCheck.ForceStopInteracting();
                    StartCoroutine(AttemptForceSnapAtEndOfFrame(objectToSnap));
                }
                else
                {
                    AttemptForceSnap(objectToSnap);
                }
            }
        }

        /// <summary>
        /// The ForceUnsnap method attempts to automatically remove the current snapped game object from the snap drop zone.
        /// </summary>
        public virtual void ForceUnsnap()
        {
            if (isSnapped && currentSnappedObject != null)
            {
                VRTK_InteractableObject ioCheck = ValidSnapObject(currentSnappedObject, false);
                if (ioCheck != null)
                {
                    ioCheck.ToggleSnapDropZone(this, false);
                }
            }
        }

        /// <summary>
        /// The ValidSnappableObjectIsHovering method determines if any valid objects are currently hovering in the snap drop zone area.
        /// </summary>
        /// <returns>Returns true if a valid object is currently in the snap drop zone area.</returns>
        public virtual bool ValidSnappableObjectIsHovering()
        {
            return currentValidSnapObjects.Count > 0;
        }

        /// <summary>
        /// The IsObjectHovering method determines if the given GameObject is currently howvering (but not snapped) in the snap drop zone area.
        /// </summary>
        /// <param name="checkObject">The GameObject to check to see if it's hovering in the snap drop zone area.</param>
        /// <returns>Returns true if the given GameObject is hovering (but not snapped) in the snap drop zone area.</returns>
        public virtual bool IsObjectHovering(GameObject checkObject)
        {
            return currentValidSnapObjects.Contains(checkObject);
        }

        /// <summary>
        /// The GetHoveringObjects method returns a List of valid GameObjects that are currently hovering (but not snapped) in the snap drop zone area.
        /// </summary>
        /// <returns>The List of valid GameObjects that are hovering (but not snapped) in the snap drop zone area.</returns>
        public virtual List<GameObject> GetHoveringObjects()
        {
            return currentValidSnapObjects;
        }

        /// <summary>
        /// The GetCurrentSnappedObejct method returns the GameObject that is currently snapped in the snap drop zone area.
        /// </summary>
        /// <returns>The GameObject that is currently snapped in the snap drop zone area.</returns>
        public virtual GameObject GetCurrentSnappedObject()
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
            if (!VRTK_SharedMethods.IsEditTime() && Application.isPlaying && defaultSnappedObject != null)
            {
                ForceSnap(defaultSnappedObject);
            }
        }

        protected virtual void Update()
        {
            CheckSnappedItemExists();
            CheckPrefabUpdate();
            CreateHighlightersInEditor();
            CheckCurrentValidSnapObjectStillValid();
            //set reference to current highlightObjectPrefab
            previousPrefab = highlightObjectPrefab;
            SetObjectHighlight();
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            //if there is no current valid snapped object and the zone then attempt to highlight
            if (!isSnapped)
            {
                ToggleHighlight(collider, true);
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            //if the current valid snapped object is the collider leaving the trigger then attempt to turn off the highlighter
            if (IsObjectHovering(collider.gameObject))
            {
                ToggleHighlight(collider, false);
            }

            if (currentSnappedObject == collider.gameObject)
            {
                ForceUnsnap();
            }
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            //Do sanity check to see if there should be a snappable object
            if (!isSnapped && ValidSnapObject(collider.gameObject, true))
            {
                AddCurrentValidSnapObject(collider.gameObject);
            }

            //if the current colliding object is the valid snappable object then we can snap
            if (IsObjectHovering(collider.gameObject))
            {
                //If it isn't snapped then force the highlighter back on
                if (!isSnapped)
                {
                    ToggleHighlight(collider, true);
                }

                //Attempt to snap the object
                SnapObject(collider);
            }
        }

        protected virtual VRTK_InteractableObject ValidSnapObject(GameObject checkObject, bool grabState, bool checkGrabState = true)
        {
            var ioCheck = checkObject.GetComponentInParent<VRTK_InteractableObject>();
            return (ioCheck != null && (!checkGrabState || ioCheck.IsGrabbed() == grabState) && !VRTK_PolicyList.Check(ioCheck.gameObject, validObjectListPolicy) ? ioCheck : null);
        }

        protected virtual string ObjectPath(string name)
        {
            return HIGHLIGHT_CONTAINER_NAME + "/" + name;
        }

        protected virtual void CheckSnappedItemExists()
        {
            if (isSnapped && currentSnappedObject == null)
            {
                OnObjectUnsnappedFromDropZone(SetSnapDropZoneEvent(currentSnappedObject));
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
                highlightObject.SetActive(true);
            }
        }

        protected virtual void CreateHighlightersInEditor()
        {
            //Only run if it's in the editor
            if (VRTK_SharedMethods.IsEditTime())
            {
                //Generate the main highlight object
                GenerateHighlightObject();

                //If a joint is being used but no joint is found then throw a warning in the console
                if (snapType == SnapTypes.UseJoint && GetComponent<Joint>() == null)
                {
                    VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "SnapDropZone:" + name, "Joint", "the same", " because the `Snap Type` is set to `Use Joint`"));
                }

                //Generate the editor highlighter object with the custom material
                GenerateEditorHighlightObject();

                //Ensure the game object references are force set based on whether they exist in the path
                ForceSetObjects();

                //Show the editor highlight object if it's set.
                if (highlightEditorObject != null)
                {
                    highlightEditorObject.SetActive(displayDropZoneInEditor);
                }
            }
        }

        protected virtual void CheckCurrentValidSnapObjectStillValid()
        {
            //If there is a current valid snap object
            for (int i = 0; i < currentValidSnapObjects.Count; i++)
            {
                var currentIOCheck = currentValidSnapObjects[i].GetComponentInParent<VRTK_InteractableObject>();
                //and the interactbale object associated with it has been snapped to another zone, then unset the current valid snap object
                if (currentIOCheck != null && currentIOCheck.GetStoredSnapDropZone() != null && currentIOCheck.GetStoredSnapDropZone() != gameObject)
                {
                    RemoveCurrentValidSnapObject(currentValidSnapObjects[i]);
                    if (isHighlighted && highlightObject != null && !highlightAlwaysActive)
                    {
                        highlightObject.SetActive(false);
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

        protected virtual void SnapObject(Collider collider)
        {
            VRTK_InteractableObject ioCheck = ValidSnapObject(collider.gameObject, false);
            //If the item is in a snappable position and this drop zone isn't snapped and the collider is a valid interactable object
            if (willSnap && !isSnapped && ioCheck != null)
            {
                //Only snap it to the drop zone if it's not already in a drop zone
                if (!ioCheck.IsInSnapDropZone())
                {
                    if (highlightObject != null)
                    {
                        //Turn off the drop zone highlighter
                        highlightObject.SetActive(false);
                    }

                    Vector3 newLocalScale = GetNewLocalScale(ioCheck);
                    if (transitionInPlace != null)
                    {
                        StopCoroutine(transitionInPlace);
                    }

                    isSnapped = true;
                    currentSnappedObject = ioCheck.gameObject;
                    if (cloneNewOnUnsnap)
                    {
                        CreatePermanentClone();
                    }

                    transitionInPlace = StartCoroutine(UpdateTransformDimensions(ioCheck, highlightContainer, newLocalScale, snapDuration));

                    ioCheck.ToggleSnapDropZone(this, true);
                }
            }

            //Force reset isSnapped if the item is grabbed but isSnapped is still true
            isSnapped = (isSnapped && ioCheck && ioCheck.IsGrabbed() ? false : isSnapped);
            wasSnapped = false;
        }

        protected virtual void CreatePermanentClone()
        {
            VRTK_BaseHighlighter currentSnappedObjectHighlighter = currentSnappedObject.GetComponent<VRTK_BaseHighlighter>();
            if (currentSnappedObjectHighlighter != null)
            {
                currentSnappedObjectHighlighter.Unhighlight();
            }
            objectToClone = Instantiate(currentSnappedObject);
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
            ResetPermanentCloneColliders(currentSnappedObject);

            isSnapped = false;
            wasSnapped = true;
            currentSnappedObject = null;
            ResetSnapDropZoneJoint();

            if (transitionInPlace != null)
            {
                StopCoroutine(transitionInPlace);
            }

            if (cloneNewOnUnsnap)
            {
                ResnapPermanentClone();
            }
        }

        protected virtual Vector3 GetNewLocalScale(VRTK_InteractableObject ioCheck)
        {
            // If apply scaling is checked then use the drop zone scale to resize the object
            Vector3 newLocalScale = ioCheck.transform.localScale;
            if (applyScalingOnSnap)
            {
                ioCheck.StoreLocalScale();
                newLocalScale = Vector3.Scale(ioCheck.transform.localScale, transform.localScale);
            }
            return newLocalScale;
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
            var snapDropZoneJoint = GetComponent<Joint>();
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
            var snapDropZoneJoint = GetComponent<Joint>();
            if (snapDropZoneJoint != null)
            {
                snapDropZoneJoint.enableCollision = originalJointCollisionState;
            }
        }

        protected virtual void AddCurrentValidSnapObject(GameObject givenObject)
        {
            if (!currentValidSnapObjects.Contains(givenObject))
            {
                currentValidSnapObjects.Add(givenObject);
            }
        }

        protected virtual void RemoveCurrentValidSnapObject(GameObject givenObject)
        {
            if (currentValidSnapObjects.Contains(givenObject))
            {
                currentValidSnapObjects.Remove(givenObject);
            }
        }

        protected virtual void AttemptForceSnap(GameObject objectToSnap)
        {
            //force snap settings on
            willSnap = true;
            AddCurrentValidSnapObject(objectToSnap);
            //Force touch one of the object's colliders on this trigger collider
            OnTriggerStay(objectToSnap.GetComponentInChildren<Collider>());
        }

        protected virtual IEnumerator AttemptForceSnapAtEndOfFrame(GameObject objectToSnap)
        {
            yield return new WaitForEndOfFrame();
            AttemptForceSnap(objectToSnap);
        }

        protected virtual void ToggleHighlight(Collider collider, bool state)
        {
            VRTK_InteractableObject ioCheck = ValidSnapObject(collider.gameObject, true, state);
            if (highlightObject != null && ioCheck != null)
            {
                //Toggle the highlighter state
                highlightObject.SetActive(state);
                ioCheck.SetSnapDropZoneHover(this, state);

                willSnap = state;
                isHighlighted = state;

                if (state)
                {
                    if (!IsObjectHovering(collider.gameObject) || wasSnapped)
                    {
                        OnObjectEnteredSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
                    }
                    AddCurrentValidSnapObject(collider.gameObject);
                }
                else
                {
                    OnObjectExitedSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
                    RemoveCurrentValidSnapObject(collider.gameObject);
                }
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

            if (highlightObject != null)
            {
                highlightObject.SetActive(false);
            }
            SetContainer();
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
                foreach (Renderer renderer in highlightEditorObject.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = Resources.Load("SnapDropZoneEditorObject") as Material;
                }
                highlightEditorObject.SetActive(true);
            }
        }

        protected virtual void CleanHighlightObject(GameObject objectToClean)
        {
            //If the highlight object has any child snap zones, then force delete these
            var deleteSnapZones = objectToClean.GetComponentsInChildren<VRTK_SnapDropZone>(true);
            for (int i = 0; i < deleteSnapZones.Length; i++)
            {
                ChooseDestroyType(deleteSnapZones[i].gameObject);
            }

            //determine components that shouldn't be deleted from highlight object
            string[] validComponents = new string[] { "Transform", "MeshFilter", "MeshRenderer", "SkinnedMeshRenderer", "VRTK_GameObjectLinker" };

            //go through all of the components on the highlighted object and delete any components that aren't in the valid component list
            var components = objectToClean.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                var valid = false;

                //Loop through each valid component and check to see if this component is valid
                foreach (string validComponent in validComponents)
                {
                    //if it's a valid component then break the check
                    if (component.GetType().ToString().Contains("." + validComponent))
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
                foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
                {
                    if (!VRTK_PlayerObject.IsPlayerObject(renderer.gameObject, VRTK_PlayerObject.ObjectTypes.Highlighter))
                    {
                        renderer.enabled = false;
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