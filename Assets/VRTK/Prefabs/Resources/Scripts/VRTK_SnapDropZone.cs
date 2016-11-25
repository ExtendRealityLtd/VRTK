// Snap Drop Zone|Prefabs|0035
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
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
    [RequireComponent(typeof(Rigidbody))]
    public class VRTK_SnapDropZone : MonoBehaviour
    {
        /// <summary>
        /// The types of snap on release available.
        /// </summary>
        /// <param name="Use_Kinematic">Will set the interactable object rigidbody to `isKinematic = true`.</param>
        /// <param name="Use_Joint">Will attach the interactable object's rigidbody to the provided joint as it's `Connected Body`.</param>
        /// <param name="Use_Parenting">Will set the SnapDropZone as the interactable object's parent and set it's rigidbody to `isKinematic = true`.</param>
        public enum SnapTypes
        {
            Use_Kinematic,
            Use_Joint,
            Use_Parenting
        }

        [Tooltip("A game object that is used to draw the highlighted destination for within the drop zone. This object will also be created in the Editor for easy placement.")]
        public GameObject highlightObjectPrefab;
        [Tooltip("The Snap Type to apply when a valid interactable object is dropped within the snap zone.")]
        public SnapTypes snapType = SnapTypes.Use_Kinematic;
        [Tooltip("The amount of time it takes for the object being snapped to move into the new snapped position, rotation and scale.")]
        public float snapDuration = 0f;
        [Tooltip("If this is checked then the scaled size of the snap drop zone will be applied to the object that is snapped to it.")]
        public bool applyScalingOnSnap = false;
        [Tooltip("The colour to use when showing the snap zone is active.")]
        public Color highlightColor;
        [Tooltip("The highlight object will always be displayed when the snap drop zone is available even if a valid item isn't being hovered over.")]
        public bool highlightAlwaysActive = false;
        [Tooltip("A string that specifies an object Tag or the name of a Script attached to an object and notifies the snap drop zone that this is a valid object for snapping on release.")]
        public string validObjectWithTagOrClass;
        [Tooltip("A specified VRTK_TagOrScriptPolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release. If a list is provided then the 'Valid Object With Tag Or Class' parameter will be ignored.")]
        public VRTK_TagOrScriptPolicyList validObjectTagOrScriptListPolicy;
        [Tooltip("If this is checked then the drop zone highlight section will be displayed in the scene editor window.")]
        public bool displayDropZoneInEditor = true;

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

        private GameObject previousPrefab;
        private GameObject highlightContainer;
        private GameObject highlightObject;
        private GameObject highlightEditorObject = null;
        private GameObject currentValidSnapObject = null;
        private GameObject currentSnappedObject = null;
        private VRTK_BaseHighlighter objectHighlighter;
        private bool willSnap = false;
        private bool isSnapped = false;
        private bool isHighlighted = false;
        private Coroutine transitionInPlace;
        private bool originalJointCollisionState = false;

        private const string HIGHLIGHT_CONTAINER_NAME = "HighlightContainer";
        private const string HIGHLIGHT_OBJECT_NAME = "HighlightObject";
        private const string HIGHLIGHT_EDITOR_OBJECT_NAME = "EditorHighlightObject";

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

        public SnapDropZoneEventArgs SetSnapDropZoneEvent(GameObject interactableObject)
        {
            SnapDropZoneEventArgs e;
            e.snappedObject = interactableObject;
            return e;
        }

        /// <summary>
        /// The InitaliseHighlightObject method sets up the highlight object based on the given Highlight Object Prefab.
        /// </summary>
        /// <param name="removeOldObject">If this is set to true then it attempts to delete the old highlight object if it exists. Defaults to `false`</param>
        public void InitaliseHighlightObject(bool removeOldObject = false)
        {
            //force delete previous created highlight object
            if (removeOldObject)
            {
                DeleteHighlightObject();
            }
            //Always remove editor highlight object at runtime
            ChooseDestroyType(transform.FindChild(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME)));
            highlightEditorObject = null;

            GenerateObjects();
        }

        /// <summary>
        /// the ForceSnap method attempts to automatically attach a valid game object to the snap drop zone.
        /// </summary>
        /// <param name="objectToSnap">The GameObject to attempt to snap.</param>
        public void ForceSnap(GameObject objectToSnap)
        {
            var ioCheck = objectToSnap.GetComponentInParent<VRTK_InteractableObject>();
            if (ioCheck)
            {
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
        public void ForceUnsnap()
        {
            if (isSnapped && currentSnappedObject)
            {
                var ioCheck = ValidSnapObject(currentSnappedObject, false);
                ioCheck.ToggleSnapDropZone(this, false);
            }
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                InitaliseHighlightObject();
            }
        }

        private void OnApplicationQuit()
        {
            if (objectHighlighter)
            {
                objectHighlighter.Unhighlight();
            }
        }

        private void Update()
        {
            //If the highlightObjectPrefab has changed then delete the highlight object in preparation to create a new one
            if (previousPrefab != null && previousPrefab != highlightObjectPrefab)
            {
                DeleteHighlightObject();
            }

            CreateHighlightersInEditor();
            CheckCurrentValidSnapObjectStillValid();

            //set reference to current highlightObjectPrefab
            previousPrefab = highlightObjectPrefab;

            if (highlightAlwaysActive && !isSnapped && !isHighlighted)
            {
                highlightObject.SetActive(true);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            //if there is no current valid snappable object and the zone isn't being snapped then attempt to highlight
            if (!isSnapped && currentValidSnapObject == null)
            {
                ToggleHighlight(collider, true);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            //if the current valid snapped object is the collider leaving the trigger then attempt to turn off the highlighter
            if (currentValidSnapObject == collider.gameObject)
            {
                ToggleHighlight(collider, false);
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            //Do sanity check to see if there should be a snappable object
            if (!isSnapped && currentValidSnapObject == null && ValidSnapObject(collider.gameObject, true))
            {
                currentValidSnapObject = collider.gameObject;
            }

            //if the current colliding object is the valid snappable object then we can snap
            if (currentValidSnapObject == collider.gameObject)
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

        private VRTK_InteractableObject ValidSnapObject(GameObject checkObject, bool grabState)
        {
            var ioCheck = checkObject.GetComponentInParent<VRTK_InteractableObject>();
            return (ioCheck && ioCheck.IsGrabbed() == grabState && !Utilities.TagOrScriptCheck(checkObject, validObjectTagOrScriptListPolicy, validObjectWithTagOrClass, true) ? ioCheck : null);
        }

        private string ObjectPath(string name)
        {
            return HIGHLIGHT_CONTAINER_NAME + "/" + name;
        }

        private void CreateHighlightersInEditor()
        {
            //Only run if it's in the editor
            if (Utilities.IsEditTime())
            {
                //Generate the main highlight object
                GenerateHighlightObject();

                //If a joint is being used but no joint is found then throw a warning in the console
                if (snapType == SnapTypes.Use_Joint && GetComponent<Joint>() == null)
                {
                    Debug.LogWarning("A Joint Component is required on the SnapDropZone GameObject called [" + name + "] because the Snap Type is set to `Use Joint`.");
                }

                //Generate the editor highlighter object with the custom material
                GenerateEditorHighlightObject();

                //Ensure the game object references are force set based on whether they exist in the path
                ForceSetObjects();

                //Show the editor highlight object if it's set.
                if (highlightEditorObject)
                {
                    highlightEditorObject.SetActive(displayDropZoneInEditor);
                }
            }
        }

        private void CheckCurrentValidSnapObjectStillValid()
        {
            //If there is a current valid snap object
            if (currentValidSnapObject)
            {
                var currentIOCheck = currentValidSnapObject.GetComponentInParent<VRTK_InteractableObject>();
                //and the interactbale object associated with it has been snapped to another zone, then unset the current valid snap object
                if (currentIOCheck && currentIOCheck.GetStoredSnapDropZone() != null && currentIOCheck.GetStoredSnapDropZone() != gameObject)
                {
                    currentValidSnapObject = null;
                    if (isHighlighted && highlightObject)
                    {
                        highlightObject.SetActive(false);
                    }
                }
            }
        }

        private void ForceSetObjects()
        {
            if (!highlightEditorObject)
            {
                var forceFindHighlightEditorObject = transform.FindChild(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME));
                highlightEditorObject = (forceFindHighlightEditorObject ? forceFindHighlightEditorObject.gameObject : null);
            }

            if (!highlightObject)
            {
                var forceFindHighlightObject = transform.FindChild(ObjectPath(HIGHLIGHT_OBJECT_NAME));
                highlightObject = (forceFindHighlightObject ? forceFindHighlightObject.gameObject : null);
            }

            if (!highlightContainer)
            {
                var forceFindHighlightContainer = transform.FindChild(HIGHLIGHT_CONTAINER_NAME);
                highlightContainer = (forceFindHighlightContainer ? forceFindHighlightContainer.gameObject : null);
            }
        }

        private void GenerateContainer()
        {
            if (!highlightContainer || !transform.FindChild(HIGHLIGHT_CONTAINER_NAME))
            {
                highlightContainer = new GameObject(HIGHLIGHT_CONTAINER_NAME);
                highlightContainer.transform.SetParent(transform);
                highlightContainer.transform.localPosition = Vector3.zero;
                highlightContainer.transform.localRotation = Quaternion.identity;
                highlightContainer.transform.localScale = Vector3.one;
            }
        }

        private void SetContainer()
        {
            var findContainer = transform.FindChild(HIGHLIGHT_CONTAINER_NAME);
            if (findContainer)
            {
                highlightContainer = findContainer.gameObject;
            }
        }

        private void GenerateObjects()
        {
            GenerateHighlightObject();
            if (highlightObject && objectHighlighter == null)
            {
                InitialiseHighlighter();
            }
        }

        private void SnapObject(Collider collider)
        {
            var ioCheck = ValidSnapObject(collider.gameObject, false);
            //If the item is in a snappable position and this drop zone isn't snapped and the collider is a valid interactable object
            if (willSnap && !isSnapped && ioCheck)
            {
                //Only snap it to the drop zone if it's not already in a drop zone
                if (!ioCheck.IsInSnapDropZone())
                {
                    //Turn off the drop zone highlighter
                    highlightObject.SetActive(false);

                    var newLocalScale = GetNewLocalScale(ioCheck);
                    if (transitionInPlace != null)
                    {
                        StopCoroutine(transitionInPlace);
                    }

                    isSnapped = true;
                    currentSnappedObject = ioCheck.gameObject;

                    transitionInPlace = StartCoroutine(UpdateTransformDimensions(ioCheck, highlightContainer.transform.position, highlightContainer.transform.rotation, newLocalScale, snapDuration));

                    ioCheck.ToggleSnapDropZone(this, true);
                }
            }

            //Force reset isSnapped if the item is grabbed but isSnapped is still true
            isSnapped = (isSnapped && ioCheck && ioCheck.IsGrabbed() ? false : isSnapped);
        }

        private void UnsnapObject()
        {
            isSnapped = false;
            currentSnappedObject = null;
            ResetSnapDropZoneJoint();
            if (transitionInPlace != null)
            {
                StopCoroutine(transitionInPlace);
            }
        }

        private Vector3 GetNewLocalScale(VRTK_InteractableObject ioCheck)
        {
            // If apply scaling is checked then use the drop zone scale to resize the object
            var newLocalScale = ioCheck.transform.localScale;
            if (applyScalingOnSnap)
            {
                ioCheck.StoreLocalScale();
                newLocalScale = Vector3.Scale(ioCheck.transform.localScale, transform.localScale);
            }
            return newLocalScale;
        }

        private IEnumerator UpdateTransformDimensions(VRTK_InteractableObject ioCheck, Vector3 endPosition, Quaternion endRotation, Vector3 endScale, float duration)
        {
            var elapsedTime = 0f;
            var ioTransform = ioCheck.transform;
            var startPosition = ioTransform.position;
            var startRotation = ioTransform.rotation;
            var startScale = ioTransform.localScale;
            var storedKinematicState = ioCheck.IsKinematic();
            ioCheck.ToggleKinematic(true);

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                ioTransform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / duration));
                ioTransform.rotation = Quaternion.Lerp(startRotation, endRotation, (elapsedTime / duration));
                ioTransform.localScale = Vector3.Lerp(startScale, endScale, (elapsedTime / duration));
                yield return null;
            }

            //Force all to the last setting in case anything has moved during the transition
            ioTransform.position = endPosition;
            ioTransform.rotation = endRotation;
            ioTransform.localScale = endScale;

            ioCheck.ToggleKinematic(storedKinematicState);
            SetDropSnapType(ioCheck);
        }

        private void SetDropSnapType(VRTK_InteractableObject ioCheck)
        {
            switch (snapType)
            {
                case SnapTypes.Use_Kinematic:
                    ioCheck.SaveCurrentState();
                    ioCheck.ToggleKinematic(true);
                    break;
                case SnapTypes.Use_Parenting:
                    ioCheck.SaveCurrentState();
                    ioCheck.ToggleKinematic(true);
                    ioCheck.transform.SetParent(transform);
                    break;
                case SnapTypes.Use_Joint:
                    SetSnapDropZoneJoint(ioCheck.GetComponent<Rigidbody>());
                    break;
            }
            OnObjectSnappedToDropZone(SetSnapDropZoneEvent(ioCheck.gameObject));
        }

        private void SetSnapDropZoneJoint(Rigidbody snapTo)
        {
            var snapDropZoneJoint = GetComponent<Joint>();
            if (snapDropZoneJoint == null)
            {
                Debug.LogError("No Joint Component was found on the SnapDropZone GameObject yet the Snap Type is set to `Use Joint`. Please manually add a joint to the SnapDropZone GameObject.");
                return;
            }
            if (snapTo == null)
            {
                Debug.LogError("No Rigidbody was found on the Interactbale Object.");
                return;
            }

            snapDropZoneJoint.connectedBody = snapTo;
            originalJointCollisionState = snapDropZoneJoint.enableCollision;
            //need to set this to true otherwise highlighting doesn't work again on grab
            snapDropZoneJoint.enableCollision = true;
        }

        private void ResetSnapDropZoneJoint()
        {
            var snapDropZoneJoint = GetComponent<Joint>();
            if (snapDropZoneJoint)
            {
                snapDropZoneJoint.enableCollision = originalJointCollisionState;
            }
        }

        private void AttemptForceSnap(GameObject objectToSnap)
        {
            //force snap settings on
            willSnap = true;
            currentValidSnapObject = objectToSnap;
            //Force touch one of the object's colliders on this trigger collider
            OnTriggerStay(objectToSnap.GetComponentInChildren<Collider>());
        }

        private IEnumerator AttemptForceSnapAtEndOfFrame(GameObject objectToSnap)
        {
            yield return new WaitForEndOfFrame();
            AttemptForceSnap(objectToSnap);
        }

        private void ToggleHighlight(Collider collider, bool state)
        {
            var ioCheck = ValidSnapObject(collider.gameObject, true);
            if (highlightObject && ioCheck)
            {
                //Turn on the highlighter
                highlightObject.SetActive(state);
                ioCheck.SetSnapDropZoneHover(state);

                willSnap = state;
                isHighlighted = state;

                if (state)
                {
                    OnObjectEnteredSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
                    currentValidSnapObject = collider.gameObject;
                }
                else
                {
                    OnObjectExitedSnapDropZone(SetSnapDropZoneEvent(collider.gameObject));
                    currentValidSnapObject = null;
                }
            }
        }

        private void CopyObject(GameObject objectBlueprint, ref GameObject clonedObject, string name)
        {
            GenerateContainer();
            var saveScale = transform.localScale;
            transform.localScale = Vector3.one;

            clonedObject = Instantiate(objectBlueprint, highlightContainer.transform) as GameObject;
            clonedObject.name = name;

            //default position of new highlight object
            clonedObject.transform.localPosition = Vector3.zero;
            clonedObject.transform.localRotation = Quaternion.identity;

            transform.localScale = saveScale;
            CleanHighlightObject(clonedObject);
        }

        private void GenerateHighlightObject()
        {
            //If there is a given highlight prefab and no existing highlight object then create a new highlight object
            if (highlightObjectPrefab && !highlightObject && !transform.FindChild(ObjectPath(HIGHLIGHT_OBJECT_NAME)))
            {
                CopyObject(highlightObjectPrefab, ref highlightObject, HIGHLIGHT_OBJECT_NAME);
            }

            //if highlight object exists but not in the variable then force grab it
            var checkForChild = transform.FindChild(ObjectPath(HIGHLIGHT_OBJECT_NAME));
            if (checkForChild && !highlightObject)
            {
                highlightObject = checkForChild.gameObject;
            }

            //if no highlight object prefab is set but a highlight object is found then destroy the highlight object
            if (!highlightObjectPrefab && highlightObject)
            {
                DeleteHighlightObject();
            }

            if (highlightObject)
            {
                highlightObject.SetActive(false);
            }
            SetContainer();
        }

        private void DeleteHighlightObject()
        {
            ChooseDestroyType(transform.FindChild(HIGHLIGHT_CONTAINER_NAME));
            highlightContainer = null;
            highlightObject = null;
            objectHighlighter = null;
        }

        private void GenerateEditorHighlightObject()
        {
            if (highlightObject && !highlightEditorObject && !transform.FindChild(ObjectPath(HIGHLIGHT_EDITOR_OBJECT_NAME)))
            {
                CopyObject(highlightObject, ref highlightEditorObject, HIGHLIGHT_EDITOR_OBJECT_NAME);
                foreach (var renderer in highlightEditorObject.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = Resources.Load("SnapDropZoneEditorObject") as Material;
                }
                highlightEditorObject.SetActive(true);
            }
        }

        private void CleanHighlightObject(GameObject objectToClean)
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
                var component = components[i];
                var valid = false;

                //Loop through each valid component and check to see if this component is valid
                foreach (var validComponent in validComponents)
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

        private void InitialiseHighlighter()
        {
            var existingHighlighter = Utilities.GetActiveHighlighter(gameObject);
            //If no highlighter is found on the GameObject then create the default one
            if (existingHighlighter == null)
            {
                highlightObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
            }
            else
            {
                Utilities.CloneComponent(existingHighlighter, highlightObject);
            }

            //Initialise highlighter and set highlight colour
            objectHighlighter = highlightObject.GetComponent<VRTK_BaseHighlighter>();
            objectHighlighter.Initialise(highlightColor);
            objectHighlighter.Highlight(highlightColor);

            //if the object highlighter is using a cloned object then disable the created highlight object's renderers
            if (objectHighlighter.UsesClonedObject())
            {
                foreach (var renderer in GetComponentsInChildren<Renderer>(true))
                {
                    var check = renderer.GetComponent<VRTK_PlayerObject>();
                    if (!check || check.objectType != VRTK_PlayerObject.ObjectTypes.Highlighter)
                    {
                        renderer.enabled = false;
                    }
                }
            }
        }

        private void ChooseDestroyType(Transform deleteTransform)
        {
            if (deleteTransform)
            {
                ChooseDestroyType(deleteTransform.gameObject);
            }
        }

        private void ChooseDestroyType(GameObject deleteObject)
        {
            if (Utilities.IsEditTime())
            {
                if (deleteObject)
                {
                    DestroyImmediate(deleteObject);
                }
            }
            else
            {
                if (deleteObject)
                {
                    Destroy(deleteObject);
                }
            }
        }

        private void ChooseDestroyType(Component deleteComponent)
        {
            if (Utilities.IsEditTime())
            {
                if (deleteComponent)
                {
                    DestroyImmediate(deleteComponent);
                }
            }
            else
            {
                if (deleteComponent)
                {
                    Destroy(deleteComponent);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (highlightObject && !displayDropZoneInEditor)
            {
                var boxSize = Utilities.GetBounds(highlightObject.transform).size * 1.05f;
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(highlightObject.transform.position, boxSize);
            }
        }
    }
}