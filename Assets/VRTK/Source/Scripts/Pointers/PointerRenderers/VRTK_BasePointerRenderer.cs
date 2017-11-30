// Base Pointer Renderer|PointerRenderers|10010
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
    using UnityEngine.AI;
#endif

    /// <summary>
    /// Provides a base that all pointer renderers can inherit from.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///   > This is an abstract class that is to be inherited to a concrete class that provides pointer renderer functionality, therefore this script should not be directly used.
    /// </remarks>
    public abstract class VRTK_BasePointerRenderer : MonoBehaviour
    {
        /// <summary>
        /// States of Pointer Visibility.
        /// </summary>
        public enum VisibilityStates
        {
            /// <summary>
            /// Only shows the object when the pointer is active.
            /// </summary>
            OnWhenActive,
            /// <summary>
            /// Ensures the object is always.
            /// </summary>
            AlwaysOn,
            /// <summary>
            /// Ensures the object beam is never visible.
            /// </summary>
            AlwaysOff
        }

        [Serializable]
        public sealed class PointerOriginSmoothingSettings
        {
            [Tooltip("Whether or not to smooth the position of the pointer origin when positioning the pointer tip.")]
            public bool smoothsPosition;
            [Tooltip("The maximum allowed distance between the unsmoothed pointer origin and the smoothed pointer origin per frame to use for smoothing.")]
            public float maxAllowedPerFrameDistanceDifference = 0.003f;

            [Tooltip("Whether or not to smooth the rotation of the pointer origin when positioning the pointer tip.")]
            public bool smoothsRotation;
            [Tooltip("The maximum allowed angle between the unsmoothed pointer origin and the smoothed pointer origin per frame to use for smoothing.")]
            public float maxAllowedPerFrameAngleDifference = 1.5f;
        }

        [Header("Renderer Supplement Settings")]

        [Tooltip("An optional Play Area Cursor generator to add to the destination position of the pointer tip.")]
        public VRTK_PlayAreaCursor playareaCursor;
        [Tooltip("A custom VRTK_PointerDirectionIndicator to use to determine the rotation given to the destination set event.")]
        public VRTK_PointerDirectionIndicator directionIndicator;

        [Header("General Renderer Settings")]

        [Tooltip("A custom raycaster to use for the pointer's raycasts to ignore.")]
        public VRTK_CustomRaycast customRaycast;
        [Tooltip("Specifies the smoothing to be applied to the pointer origin when positioning the pointer tip.")]
        public PointerOriginSmoothingSettings pointerOriginSmoothingSettings = new PointerOriginSmoothingSettings();

        [Header("General Appearance Settings")]

        [Tooltip("The colour to change the pointer materials when the pointer collides with a valid object. Set to `Color.clear` to bypass changing material colour on valid collision.")]
        public Color validCollisionColor = Color.green;
        [Tooltip("The colour to change the pointer materials when the pointer is not colliding with anything or with an invalid object. Set to `Color.clear` to bypass changing material colour on invalid collision.")]
        public Color invalidCollisionColor = Color.red;

        [Tooltip("Determines when the main tracer of the pointer renderer will be visible.")]
        public VisibilityStates tracerVisibility = VisibilityStates.OnWhenActive;
        [Tooltip("Determines when the cursor/tip of the pointer renderer will be visible.")]
        public VisibilityStates cursorVisibility = VisibilityStates.OnWhenActive;

        protected const float BEAM_ADJUST_OFFSET = 0.0001f;

        protected VRTK_Pointer controllingPointer;
        protected RaycastHit destinationHit = new RaycastHit();
        protected Material defaultMaterial;
        protected Color previousColor;
        protected Color currentColor;

        protected VRTK_PolicyList invalidListPolicy;
        protected VRTK_NavMeshData navMeshData;
        protected bool headsetPositionCompensation;

        protected GameObject objectInteractor;
        protected GameObject objectInteractorAttachPoint;
        protected GameObject pointerOriginTransformFollowGameObject;
        protected VRTK_TransformFollow pointerOriginTransformFollow;
        protected VRTK_InteractGrab controllerGrabScript;
        protected Rigidbody savedAttachPoint;
        protected bool attachedToInteractorAttachPoint = false;
        protected float savedBeamLength = 0f;
        protected HashSet<GameObject> makeRendererVisible = new HashSet<GameObject>();

        protected bool tracerVisible;
        protected bool cursorVisible;
        protected LayerMask defaultIgnoreLayer = Physics.IgnoreRaycastLayer;

        protected SDK_BaseController.ControllerHand cachedAttachedHand = SDK_BaseController.ControllerHand.None;
        protected Transform cachedPointerAttachPoint = null;

        /// <summary>
        /// The GetPointerObjects returns an array of the auto generated GameObjects associated with the pointer.
        /// </summary>
        /// <returns>An array of pointer auto generated GameObjects.</returns>
        public abstract GameObject[] GetPointerObjects();

        /// <summary>
        /// The InitalizePointer method is used to set up the state of the pointer renderer.
        /// </summary>
        /// <param name="givenPointer">The VRTK_Pointer that is controlling the pointer renderer.</param>
        /// <param name="givenInvalidListPolicy">The VRTK_PolicyList for managing valid and invalid pointer locations.</param>
        /// <param name="givenNavMeshCheckDistance">The given distance from a nav mesh that the pointer can be to be valid.</param>
        /// <param name="givenHeadsetPositionCompensation">Determines whether the play area cursor will take the headset position within the play area into account when being displayed.</param>
        [Obsolete("`VRTK_BasePointerRenderer.InitalizePointer(givenPointer, givenInvalidListPolicy, givenNavMeshCheckDistance, givenHeadsetPositionCompensation)` has been replaced with the method `VRTK_BasePointerRenderer.InitalizePointer(givenPointer, givenInvalidListPolicy, givenNavMeshData, givenHeadsetPositionCompensation)`. This method will be removed in a future version of VRTK.")]
        public virtual void InitalizePointer(VRTK_Pointer givenPointer, VRTK_PolicyList givenInvalidListPolicy, float givenNavMeshCheckDistance, bool givenHeadsetPositionCompensation)
        {
            VRTK_NavMeshData givenData = gameObject.AddComponent<VRTK_NavMeshData>();
            givenData.distanceLimit = givenNavMeshCheckDistance;
            InitalizePointer(givenPointer, givenInvalidListPolicy, givenData, givenHeadsetPositionCompensation);
        }

        /// <summary>
        /// The InitalizePointer method is used to set up the state of the pointer renderer.
        /// </summary>
        /// <param name="givenPointer">The VRTK_Pointer that is controlling the pointer renderer.</param>
        /// <param name="givenInvalidListPolicy">The VRTK_PolicyList for managing valid and invalid pointer locations.</param>
        /// <param name="givenNavMeshData">The NavMeshData object that contains the Nav Mesh restriction options.</param>
        /// <param name="givenHeadsetPositionCompensation">Determines whether the play area cursor will take the headset position within the play area into account when being displayed.</param>
        public virtual void InitalizePointer(VRTK_Pointer givenPointer, VRTK_PolicyList givenInvalidListPolicy, VRTK_NavMeshData givenNavMeshData, bool givenHeadsetPositionCompensation)
        {
            controllingPointer = givenPointer;
            invalidListPolicy = givenInvalidListPolicy;
            navMeshData = givenNavMeshData;
            headsetPositionCompensation = givenHeadsetPositionCompensation;

            if (controllingPointer != null && controllingPointer.interactWithObjects && controllingPointer.controllerEvents != null && objectInteractor == null)
            {
                controllerGrabScript = controllingPointer.controllerEvents.GetComponentInChildren<VRTK_InteractGrab>();
                CreateObjectInteractor();
            }
            SetupDirectionIndicator();
        }

        /// <summary>
        /// The ResetPointerObjects method is used to destroy any existing pointer objects and recreate them at runtime.
        /// </summary>
        public virtual void ResetPointerObjects()
        {
            DestroyPointerObjects();
            CreatePointerObjects();
        }

        /// <summary>
        /// The Toggle Method is used to enable or disable the pointer renderer.
        /// </summary>
        /// <param name="pointerState">The activation state of the pointer.</param>
        /// <param name="actualState">The actual state of the activation button press.</param>
        public virtual void Toggle(bool pointerState, bool actualState)
        {
            if (pointerState)
            {
                destinationHit = new RaycastHit();
            }
            else if (controllingPointer != null)
            {
                controllingPointer.ResetActivationTimer();
                PointerExit(destinationHit);
            }

            ToggleInteraction(pointerState);
            ToggleRenderer(pointerState, actualState);
        }

        /// <summary>
        /// The ToggleInteraction method is used to enable or disable the controller extension interactions.
        /// </summary>
        /// <param name="state">If true then the object interactor will be enabled.</param>
        public virtual void ToggleInteraction(bool state)
        {
            ToggleObjectInteraction(state);
        }

        /// <summary>
        /// The UpdateRenderer method is used to run an Update routine on the pointer.
        /// </summary>
        public virtual void UpdateRenderer()
        {
            if (playareaCursor != null)
            {
                playareaCursor.SetHeadsetPositionCompensation(headsetPositionCompensation);
                playareaCursor.ToggleState(IsCursorVisible());
            }

            if (directionIndicator != null)
            {
                UpdateDirectionIndicator();
            }
        }

        /// <summary>
        /// The GetDestinationHit method is used to get the RaycastHit of the pointer destination.
        /// </summary>
        /// <returns>The RaycastHit containing the information where the pointer is hitting.</returns>
        public virtual RaycastHit GetDestinationHit()
        {
            return destinationHit;
        }

        /// <summary>
        /// The ValidPlayArea method is used to determine if there is a valid play area and if it has had any collisions.
        /// </summary>
        /// <returns>Returns true if there is a valid play area and no collisions. Returns false if there is no valid play area or there is but with a collision detected.</returns>
        public virtual bool ValidPlayArea()
        {
            return (playareaCursor == null || !playareaCursor.IsActive() || !playareaCursor.HasCollided());
        }

        /// <summary>
        /// The IsVisible method determines if the pointer renderer is at all visible by checking the state of the tracer and the cursor.
        /// </summary>
        /// <returns>Returns true if either the tracer or cursor renderers are visible. Returns false if none are visible.</returns>
        public virtual bool IsVisible()
        {
            return (IsTracerVisible() || IsCursorVisible());
        }

        /// <summary>
        /// The IsTracerVisible method determines if the pointer tracer renderer is visible.
        /// </summary>
        /// <returns>Returns true if the tracer renderers are visible.</returns>
        public virtual bool IsTracerVisible()
        {
            return (tracerVisibility == VisibilityStates.AlwaysOn || tracerVisible);
        }

        /// <summary>
        /// The IsCursorVisible method determines if the pointer cursor renderer is visible.
        /// </summary>
        /// <returns>Returns true if the cursor renderers are visible.</returns>
        public virtual bool IsCursorVisible()
        {
            return (cursorVisibility == VisibilityStates.AlwaysOn || cursorVisible);
        }

        /// <summary>
        /// The IsValidCollision method determines if the pointer is currently in it's valid collision state.
        /// </summary>
        /// <returns>Returns true if the pointer is in a valid collision, returns false if the pointer is in an invalid collision state.</returns>
        public virtual bool IsValidCollision()
        {
            return (currentColor != invalidCollisionColor);
        }

        /// <summary>
        /// The GetObjectInteractor method returns the auto generated GameObject that acts as the controller extension for interacting with objects.
        /// </summary>
        /// <returns>The auto generated object interactor GameObject.</returns>
        /// <returns></returns>
        public virtual GameObject GetObjectInteractor()
        {
            return objectInteractor;
        }

        protected abstract void CreatePointerObjects();
        protected abstract void DestroyPointerObjects();
        protected abstract void ToggleRenderer(bool pointerState, bool actualState);

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            cachedPointerAttachPoint = null;
            cachedAttachedHand = SDK_BaseController.ControllerHand.None;
            defaultMaterial = Resources.Load("WorldPointer") as Material;
            makeRendererVisible.Clear();
            CreatePointerOriginTransformFollow();
            CreatePointerObjects();
        }

        protected virtual void OnDisable()
        {
            DestroyPointerObjects();
            if (objectInteractor != null)
            {
                Destroy(objectInteractor);
            }
            controllerGrabScript = null;
            Destroy(pointerOriginTransformFollowGameObject);
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnValidate()
        {
            pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference = Mathf.Max(0.0001f, pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference);
            pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference = Mathf.Max(0.0001f, pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference);
        }

        protected virtual void FixedUpdate()
        {
            if (controllingPointer != null && controllingPointer.interactWithObjects && objectInteractor != null && objectInteractor.activeInHierarchy)
            {
                UpdateObjectInteractor();
            }

            UpdatePointerOriginTransformFollow();
        }

        protected virtual void ToggleObjectInteraction(bool state)
        {
            if (controllingPointer != null && controllingPointer.interactWithObjects)
            {
                if (state && controllingPointer.grabToPointerTip && controllerGrabScript != null && objectInteractorAttachPoint != null)
                {
                    savedAttachPoint = controllerGrabScript.controllerAttachPoint;
                    controllerGrabScript.controllerAttachPoint = objectInteractorAttachPoint.GetComponent<Rigidbody>();
                    attachedToInteractorAttachPoint = true;
                }

                if (!state && controllingPointer.grabToPointerTip && controllerGrabScript != null)
                {
                    if (attachedToInteractorAttachPoint)
                    {
                        controllerGrabScript.ForceRelease(true);
                    }
                    controllerGrabScript.controllerAttachPoint = savedAttachPoint;
                    savedAttachPoint = null;
                    attachedToInteractorAttachPoint = false;
                    savedBeamLength = 0f;
                }

                if (objectInteractor != null)
                {
                    objectInteractor.SetActive(state);
                }
            }
        }

        protected virtual void UpdateObjectInteractor()
        {
            objectInteractor.transform.position = destinationHit.point;
        }

        protected virtual VRTK_ControllerReference GetControllerReference(GameObject reference = null)
        {
            reference = (reference == null && controllingPointer != null && controllingPointer.controllerEvents != null ? controllingPointer.controllerEvents.gameObject : reference);
            return VRTK_ControllerReference.GetControllerReference(reference);
        }

        protected virtual Transform GetPointerOriginTransform()
        {
            VRTK_ControllerReference controllerReference = GetControllerReference((controllingPointer != null ? controllingPointer.attachedTo : null));
            if (VRTK_ControllerReference.IsValid(controllerReference) && (cachedAttachedHand != controllerReference.hand || cachedPointerAttachPoint == null))
            {
                cachedPointerAttachPoint = controllerReference.model.transform.Find(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.AttachPoint, controllerReference.hand));
                cachedAttachedHand = controllerReference.hand;
                pointerOriginTransformFollow.gameObject.SetActive(false);
            }
            return (cachedPointerAttachPoint != null ? cachedPointerAttachPoint : transform);
        }

        protected virtual void UpdatePointerOriginTransformFollow()
        {
            pointerOriginTransformFollow.gameObject.SetActive((controllingPointer != null));
            if (controllingPointer != null)
            {
                pointerOriginTransformFollow.gameObjectToFollow = (controllingPointer.customOrigin == null ? GetPointerOriginTransform() : controllingPointer.customOrigin).gameObject;
                pointerOriginTransformFollow.enabled = controllingPointer != null;
                pointerOriginTransformFollowGameObject.SetActive(controllingPointer != null);

                pointerOriginTransformFollow.smoothsPosition = pointerOriginSmoothingSettings.smoothsPosition;
                pointerOriginTransformFollow.maxAllowedPerFrameDistanceDifference = pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference;
                pointerOriginTransformFollow.smoothsRotation = pointerOriginSmoothingSettings.smoothsRotation;
                pointerOriginTransformFollow.maxAllowedPerFrameAngleDifference = pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference;
            }
        }

        protected Transform GetOrigin(bool smoothed = true)
        {
            return (smoothed ? pointerOriginTransformFollow.gameObjectToChange.transform : (controllingPointer.customOrigin == null ? GetPointerOriginTransform() : controllingPointer.customOrigin));
        }

        protected virtual void PointerEnter(RaycastHit givenHit)
        {
            controllingPointer.PointerEnter(givenHit);
        }

        protected virtual void PointerExit(RaycastHit givenHit)
        {
            controllingPointer.PointerExit(givenHit);
        }

        protected virtual bool ValidDestination()
        {
            bool validNavMeshLocation = false;
            if (navMeshData != null)
            {
                if (destinationHit.transform != null)
                {
                    NavMeshHit hit;
                    validNavMeshLocation = NavMesh.SamplePosition(destinationHit.point, out hit, navMeshData.distanceLimit, navMeshData.validAreas);
                }
            }
            else
            {
                validNavMeshLocation = true;
            }
            return (validNavMeshLocation && destinationHit.collider != null && !(VRTK_PolicyList.Check(destinationHit.collider.gameObject, invalidListPolicy)));
        }

        protected virtual void ToggleElement(GameObject givenObject, bool pointerState, bool actualState, VisibilityStates givenVisibility, ref bool currentVisible)
        {
            if (givenObject != null)
            {
                currentVisible = (givenVisibility == VisibilityStates.AlwaysOn ? true : pointerState);

                givenObject.SetActive(currentVisible);

                if (givenVisibility == VisibilityStates.AlwaysOff)
                {
                    currentVisible = false;
                    ToggleRendererVisibility(givenObject, false);
                }
                else
                {
                    if (actualState && givenVisibility != VisibilityStates.AlwaysOn)
                    {
                        ToggleRendererVisibility(givenObject, false);
                        AddVisibleRenderer(givenObject);
                    }
                    else
                    {
                        ToggleRendererVisibility(givenObject, true);
                    }
                }
            }
        }

        protected virtual void AddVisibleRenderer(GameObject givenObject)
        {
            makeRendererVisible.Add(givenObject);
        }

        protected virtual void MakeRenderersVisible()
        {
            foreach (GameObject currentRenderer in makeRendererVisible)
            {
                ToggleRendererVisibility(currentRenderer, true);
            }
            makeRendererVisible.Clear();
        }

        protected virtual void ToggleRendererVisibility(GameObject givenObject, bool state)
        {
            if (givenObject != null)
            {
                Renderer[] renderers = givenObject.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].enabled = state;
                }
            }
        }

        protected virtual void SetupMaterialRenderer(GameObject givenObject)
        {
            if (givenObject != null)
            {
                MeshRenderer pointerRenderer = givenObject.GetComponent<MeshRenderer>();
                pointerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                pointerRenderer.receiveShadows = false;
                pointerRenderer.material = defaultMaterial;
            }
        }

        protected virtual void ChangeColor(Color givenColor)
        {
            previousColor = currentColor;
            if ((playareaCursor != null && playareaCursor.IsActive() && playareaCursor.HasCollided()) || !ValidDestination() || (controllingPointer != null && !controllingPointer.CanSelect()))
            {
                givenColor = invalidCollisionColor;
            }

            if (givenColor != Color.clear)
            {
                currentColor = givenColor;
                ChangeMaterial(givenColor);
            }

            if (previousColor != currentColor)
            {
                EmitStateEvent();
            }
        }

        protected virtual void EmitStateEvent()
        {
            if (controllingPointer != null)
            {
                if (IsValidCollision())
                {
                    controllingPointer.OnPointerStateValid();
                }
                else
                {
                    controllingPointer.OnPointerStateInvalid();
                }
            }
        }

        protected virtual void ChangeMaterial(Color givenColor)
        {
            if (playareaCursor != null)
            {
                playareaCursor.SetMaterialColor(givenColor, IsValidCollision());
            }

            if (directionIndicator != null)
            {
                directionIndicator.SetMaterialColor(givenColor, IsValidCollision());
            }
        }

        protected virtual void ChangeMaterialColor(GameObject givenObject, Color givenColor)
        {
            if (givenObject != null)
            {
                Renderer[] foundRenderers = givenObject.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < foundRenderers.Length; i++)
                {
                    Renderer foundRenderer = foundRenderers[i];
                    if (foundRenderer.material != null)
                    {
                        foundRenderer.material.EnableKeyword("_EMISSION");

                        if (foundRenderer.material.HasProperty("_Color"))
                        {
                            foundRenderer.material.color = givenColor;
                        }

                        if (foundRenderer.material.HasProperty("_EmissionColor"))
                        {
                            foundRenderer.material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(givenColor, 50));
                        }
                    }
                }
            }
        }

        protected virtual void CreateObjectInteractor()
        {
            objectInteractor = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, gameObject.name, "BasePointerRenderer_ObjectInteractor_Container"));
            objectInteractor.transform.SetParent(controllingPointer.controllerEvents.transform);
            objectInteractor.transform.localPosition = Vector3.zero;
            objectInteractor.layer = LayerMask.NameToLayer("Ignore Raycast");
            VRTK_PlayerObject.SetPlayerObject(objectInteractor, VRTK_PlayerObject.ObjectTypes.Pointer);

            GameObject objectInteractorCollider = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, gameObject.name, "BasePointerRenderer_ObjectInteractor_Collider"));
            objectInteractorCollider.transform.SetParent(objectInteractor.transform);
            objectInteractorCollider.transform.localPosition = Vector3.zero;
            objectInteractorCollider.layer = LayerMask.NameToLayer("Ignore Raycast");
            SphereCollider tmpCollider = objectInteractorCollider.AddComponent<SphereCollider>();
            tmpCollider.isTrigger = true;
            VRTK_PlayerObject.SetPlayerObject(objectInteractorCollider, VRTK_PlayerObject.ObjectTypes.Pointer);

            if (controllingPointer.grabToPointerTip)
            {
                objectInteractorAttachPoint = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, gameObject.name, "BasePointerRenderer_ObjectInteractor_AttachPoint"));
                objectInteractorAttachPoint.transform.SetParent(objectInteractor.transform);
                objectInteractorAttachPoint.transform.localPosition = Vector3.zero;
                objectInteractorAttachPoint.layer = LayerMask.NameToLayer("Ignore Raycast");
                Rigidbody objectInteratorRigidBody = objectInteractorAttachPoint.AddComponent<Rigidbody>();
                objectInteratorRigidBody.isKinematic = true;
                objectInteratorRigidBody.freezeRotation = true;
                objectInteratorRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                VRTK_PlayerObject.SetPlayerObject(objectInteractorAttachPoint, VRTK_PlayerObject.ObjectTypes.Pointer);
            }

            ScaleObjectInteractor(Vector3.one);
            objectInteractor.SetActive(false);
        }

        protected virtual void ScaleObjectInteractor(Vector3 scaleAmount)
        {
            if (objectInteractor != null)
            {
                VRTK_SharedMethods.SetGlobalScale(objectInteractor.transform, scaleAmount);
            }
        }

        protected virtual void CreatePointerOriginTransformFollow()
        {
            pointerOriginTransformFollowGameObject = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, gameObject.name, "BasePointerRenderer_Origin_Smoothed"));
            pointerOriginTransformFollow = pointerOriginTransformFollowGameObject.AddComponent<VRTK_TransformFollow>();
            pointerOriginTransformFollow.enabled = false;
            pointerOriginTransformFollow.moment = VRTK_TransformFollow.FollowMoment.OnFixedUpdate;
            pointerOriginTransformFollow.followsScale = false;
        }

        protected virtual float OverrideBeamLength(float currentLength)
        {
            if (controllerGrabScript == null || !controllerGrabScript.GetGrabbedObject())
            {
                savedBeamLength = 0f;
            }

            if (controllingPointer != null && controllingPointer.interactWithObjects && controllingPointer.grabToPointerTip && attachedToInteractorAttachPoint && controllerGrabScript != null && controllerGrabScript.GetGrabbedObject())
            {
                savedBeamLength = (savedBeamLength == 0f ? currentLength : savedBeamLength);
                return savedBeamLength;
            }
            return currentLength;
        }

        protected virtual void UpdateDependencies(Vector3 location)
        {
            if (playareaCursor != null)
            {
                playareaCursor.SetPlayAreaCursorTransform(location);
            }
        }

        protected virtual void SetupDirectionIndicator()
        {
            if (directionIndicator != null && controllingPointer != null && controllingPointer.controllerEvents != null)
            {
                directionIndicator.Initialize(controllingPointer.controllerEvents);
            }
        }

        protected virtual void UpdateDirectionIndicator()
        {
            RaycastHit destinationHit = GetDestinationHit();
            directionIndicator.SetPosition((ShowDirectionIndicator() && destinationHit.collider != null), destinationHit.point);
        }

        protected virtual bool ShowDirectionIndicator()
        {
            switch (directionIndicator.indicatorVisibility)
            {
                case VRTK_PointerDirectionIndicator.VisibilityState.OnWhenPointerActive:
                    return controllingPointer.IsPointerActive();
                case VRTK_PointerDirectionIndicator.VisibilityState.AlwaysOnWithPointerCursor:
                    return (IsCursorVisible() || controllingPointer.IsPointerActive());
            }
            return false;
        }
    }
}