// Snap Drop Zone|Prefabs|0080
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Highlighters;
    using VRTK.SnapAttachMechanics;

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
        [HideInInspector]
        [System.Obsolete("`VRTK_SnapDropZone.snapDuration` has been moved to `VRTK_BaseSnapAttach.snapDuration`. This parameter will be removed in a future version of VRTK.")]
        public float snapDuration = 0f;
        [Tooltip("If this is checked then the scaled size  of the snap drop zone will be applied to the object that is snapped to it.")]
        [HideInInspector]
        [System.Obsolete("`VRTK_SnapDropZone.applyScalingOnSnap` has been moved to `VRTK_BaseSnapAttach.applyScalingOnSnap`. This parameter will be removed in a future version of VRTK.")]
        public bool applyScalingOnSnap = false;
        [Tooltip("If this is checked then when the snapped object is unsnapped from the drop zone, a clone of the unsnapped object will be snapped back into the drop zone.")]
        [System.Obsolete("`VRTK_SnapDropZone.cloneNewOnUnsnap` is obselete and will be removed in a future version of VRTK. The same functionality is now enabled by setting VRTK_SnapDropZone.snapAttachMechanicScript to `VRTK_CloneSnapAttach`")]
        [HideInInspector]
        public bool cloneNewOnUnsnap = false;
        [Tooltip("The colour to use when showing the snap zone is active. This is used as the highlight colour when no object is hovering but `Highlight Always Active` is true.")]
        public Color highlightColor = Color.clear;
        [Tooltip("The colour to use when showing the snap zone is active and a valid object is hovering. If this is `Color.clear` then the `Highlight Color` will be used.")]
        public Color validHighlightColor = Color.clear;
        [Tooltip("The highlight object will always be displayed when the snap drop zone is available even if a valid item isn't being hovered over.")]
        public bool highlightAlwaysActive = false;
        [Tooltip("A specified VRTK_PolicyList to use to determine which interactable objects will be snapped to the snap drop zone on release.")]
        [HideInInspector]
        [System.Obsolete("`VRTK_SnapDropZone.validObjectListPolicy` has been moved to `VRTK_BaseSnapAttach.validObjectListPolicy`. This parameter will be removed in a future version of VRTK.")]
        public VRTK_PolicyList validObjectListPolicy;
        [Tooltip("If this is checked then the drop zone highlight section will be displayed in the scene editor window.")]
        public bool displayDropZoneInEditor = true;
        [Tooltip("This determines how the grabbed Interactable Object will attach to a `VRTK_SnapDropZone`. If one isn't provided then the first Snap Attach script on the GameObject will be used, if one is not found then a Basic Snap Grab Attach script will be created at runtime.")]
        public VRTK_BaseSnapAttach snapAttachMechanicScript;
        [System.Obsolete("`VRTK_SnapDropZone.defaultSnappedObject` has been replaced with the `VRTK_SnapDropZone.defaultSnappedInteractableObject`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public GameObject defaultSnappedObject;
        [System.Obsolete("`VRTK_SnapDropZone.defaultSnappedInteractableObject` has been moved to `VRTK_BaseSnapAttach.defaultSnappedInteractableObject`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public VRTK_InteractableObject defaultSnappedInteractableObject;

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
        public GameObject highlightContainer { get; private set;}
        protected GameObject highlightObject;
        protected GameObject highlightEditorObject = null;

        protected bool isHighlighted = false;

        protected VRTK_BaseHighlighter objectHighlighter;

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
            snapAttachMechanicScript.UnsnapObject();
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
            //Force delete previous created highlight object
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
        /// The ValidSnappableObjectIsHovering method determines if any valid objects are currently hovering in the snap drop zone area.
        /// </summary>
        /// <returns>Returns true if a valid object is currently in the snap drop zone area.</returns>
        public virtual bool ValidSnappableObjectIsHovering()
        {
            for (int i = 0; i < snapAttachMechanicScript.GetHoveringInteractableObjects().Count; i++)
            {
                if (snapAttachMechanicScript.GetHoveringInteractableObjects()[i].IsGrabbed())
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
            return (interactableObjectToCheck != null ? snapAttachMechanicScript.GetHoveringInteractableObjects().Contains(interactableObjectToCheck) : false);
        }

        /// <summary>
        /// The IsInteractableObjectHovering method determines if the given Interactable Object script is currently howvering (but not snapped) in the snap drop zone area.
        /// </summary>
        /// <param name="checkObject">The Interactable Object script to check to see if it's hovering in the snap drop zone area.</param>
        /// <returns>Returns true if the given Interactable Object script is hovering (but not snapped) in the snap drop zone area.</returns>
        public virtual bool IsInteractableObjectHovering(VRTK_InteractableObject checkObject)
        {
            return (checkObject != null ? snapAttachMechanicScript.GetHoveringInteractableObjects().Contains(checkObject) : false);
        }

        /// <summary>
        /// The GetHoveringObjects method returns a List of valid GameObjects that are currently hovering (but not snapped) in the snap drop zone area.
        /// </summary>
        /// <returns>The List of valid GameObjects that are hovering (but not snapped) in the snap drop zone area.</returns>
        public virtual List<GameObject> GetHoveringObjects()
        {
            List<GameObject> returnList = new List<GameObject>();
            for (int i = 0; i < snapAttachMechanicScript.GetHoveringInteractableObjects().Count; i++)
            {
                VRTK_SharedMethods.AddListValue(returnList, snapAttachMechanicScript.GetHoveringInteractableObjects()[i].gameObject);
            }
            return returnList;
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
            isHighlighted = false;
            DisableHighlightShadows();
            AttemptSetSnapMechanic();
        }

        protected virtual void OnDisable()
        {
            SetHighlightObjectActive(false);
        }

        protected virtual void Update()
        {
            AttemptSetSnapMechanic();
            snapAttachMechanicScript.CheckSnappedItemExists();
            CheckPrefabUpdate();
            CreateHighlightersInEditor();
            CheckCurrentValidSnapObjectStillValid();
            previousPrefab = highlightObjectPrefab;
            SetObjectHighlight();
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            snapAttachMechanicScript.CheckCanSnap(collider.GetComponentInParent<VRTK_InteractableObject>());
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            snapAttachMechanicScript.CheckCanUnsnap(collider.GetComponentInParent<VRTK_InteractableObject>());
        }

        /// <summary>
        /// The AttemptSetSnapMechanic method looks for and assigns the first Snap Attach Mechanic component it finds if the 'snapAttachMechanicScript' property hasn't been assigned already.
        /// </summary>
        protected virtual void AttemptSetSnapMechanic()
        {
            if (snapAttachMechanicScript == null)
            {
                VRTK_BaseSnapAttach setSnapMechanic = GetComponentInChildren<VRTK_BaseSnapAttach>();
                if (setSnapMechanic == null)
                {
#pragma warning disable 618
                    if (cloneNewOnUnsnap == true)
                    {
                        setSnapMechanic = gameObject.AddComponent<VRTK_CloneSnapAttach>();
                    }
#pragma warning restore 618
                    else
                    {
                        setSnapMechanic = gameObject.AddComponent<VRTK_SimpleSnapAttach>();
                    }
                }
                snapAttachMechanicScript = setSnapMechanic;
            }
        }

        protected virtual string ObjectPath(string name)
        {
            return HIGHLIGHT_CONTAINER_NAME + "/" + name;
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
            if (highlightAlwaysActive && !snapAttachMechanicScript.IsSnapped() && !isHighlighted)
            {
                SetHighlightObjectActive(true);
                ToggleHighlightColor();
            }

            if (!highlightAlwaysActive && isHighlighted && !ValidSnappableObjectIsHovering())
            {
                SetHighlightObjectActive(false);
            }
        }

        public virtual void ToggleHighlightColor()
        {
            if (Application.isPlaying && highlightAlwaysActive && !snapAttachMechanicScript.IsSnapped() && objectHighlighter != null)
            {
                objectHighlighter.Highlight((snapAttachMechanicScript.WillSnap() && validHighlightColor != Color.clear ? validHighlightColor : highlightColor));
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

        /// <summary>
        /// The CheckCurrentValidSnapObjectStillValid method checks to see if any of the Interactable Objects previously previously hovering around this Snap Drop Zone are now snapped to another zone, and then updates the list of valid snap objects accordingly.
        /// </summary>
        protected virtual void CheckCurrentValidSnapObjectStillValid()
        {
            for (int i = 0; i < snapAttachMechanicScript.GetHoveringInteractableObjects().Count; i++)
            {
                VRTK_InteractableObject interactableObjectCheck = snapAttachMechanicScript.GetHoveringInteractableObjects()[i];
                //If this particular Interactable Object has been snapped to another zone, then remove it from the list of valid snappable objects for this Snap Drop Zone and deactivate the highlighter
                if (interactableObjectCheck != null && interactableObjectCheck.GetStoredSnapDropZone() != null && interactableObjectCheck.GetStoredSnapDropZone() != this)
                {
                    snapAttachMechanicScript.RemoveCurrentValidSnapObject(interactableObjectCheck);
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

        public virtual void ToggleHighlight(VRTK_InteractableObject checkObject, bool state)
        {
            if (highlightObject != null && snapAttachMechanicScript.ValidSnapObject(checkObject, true, state))
            {
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

            //Default position of new highlight object
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

            //If highlight object exists but not in the variable then force grab it
            Transform checkForChild = transform.Find(ObjectPath(HIGHLIGHT_OBJECT_NAME));
            if (checkForChild != null && highlightObject == null)
            {
                highlightObject = checkForChild.gameObject;
            }

            //If no highlight object prefab is set but a highlight object is found then destroy the highlight object
            if (highlightObjectPrefab == null && highlightObject != null)
            {
                DeleteHighlightObject();
            }

            DisableHighlightShadows();
            SetHighlightObjectActive(false);
            SetContainer();
        }

        public virtual void SetHighlightObjectActive(bool state)
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

            //Determine components that shouldn't be deleted from highlight object
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
                    //If it's a valid component then break the check
                    if (component.GetType().ToString().Contains("." + validComponents[j]))
                    {
                        valid = true;
                        break;
                    }
                }

                //If this is a valid component then just continue to the next component
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
