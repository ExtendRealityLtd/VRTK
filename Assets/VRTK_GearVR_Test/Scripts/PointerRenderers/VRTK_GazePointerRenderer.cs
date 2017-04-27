// Gaze Pointer Renderer|PointerRenderers|10040
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// The Gaze Pointer Renderer project a coloured cursor in the look direction or a custom cursor for teleporting.
    /// </summary>
    /// <remarks>
    /// It can be used to use only the gaze both for interacting and teleporting.
    /// </remarks>
    public class VRTK_GazePointerRenderer : VRTK_BasePointerRenderer
    {
        [Header("Gaze Pointer Appearance Settings")]

        [Tooltip("The maximum length the pointer tracer can reach.")]
        public float maximumLength = 100f;
        [Tooltip("The scale factor to scale the pointer cursor object by.")]
        public float cursorScale = 0.05f;
        [Tooltip("The scale factor to scale the teleport cursor object by.")]
        public float teleportCursorScale = 0.5f;
        [Tooltip("The scale factor to scale the teleport cursor height by, RELATIVE to Teleport Cursor Scale.")]
        public float teleportCursorVerticalScaling = 1f;
        [Tooltip("The cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.")]
        public bool cursorMatchTargetRotation = false;
        [Tooltip("Rescale the cursor proportionally to the distance from the tracer origin.")]
        public bool cursorDistanceRescale = false;
        [Tooltip("The maximum scale the cursor is allowed to reach. This is only used when rescaling the cursor proportionally to the distance from the tracer origin.")]
        public Vector3 maximumCursorScale = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        [Header("Gaze Pointer Custom Appearance Settings")]

        [Tooltip("A custom game object to use as the appearance for the pointer cursor. If this is empty then a Sphere primitive will be created and used.")]
        public GameObject customCursor;
        [Tooltip("A custom game object can be applied here to appear only if the location is valid.")]
        public GameObject validLocationObject = null;

        protected GameObject actualContainer;
        protected GameObject actualCursor;
        protected GameObject actualValidLocationObject = null;

        protected Vector3 cursorOriginalScale = Vector3.one;

        /// <summary>
        /// The UpdateRenderer method is used to run an Update routine on the pointer.
        /// </summary>
        public override void UpdateRenderer()
        {
            if ((controllingPointer && controllingPointer.IsPointerActive()) || IsVisible())
            {
                float tracerLength = CastRayForward();
                SetPointerAppearance(tracerLength);
                MakeRenderersVisible();
            }
            base.UpdateRenderer();
        }

        protected override void OnEnable()
        {
            base.OnEnable(); 
            // workaround for enabling the pointer when activateOnEnable is set
            // TODO: check this
            if (controllingPointer && controllingPointer.enabled)
            {
                controllingPointer.enabled = false;
                controllingPointer.enabled = true;
            }
        }
        protected override void ToggleRenderer(bool pointerState, bool actualState)
        {
            ToggleElement(actualCursor, pointerState, actualState, cursorVisibility, ref cursorVisible);
            if (actualValidLocationObject)
            {
                bool validLocation = ValidDestination() && IsValidCollision();
                actualValidLocationObject.SetActive(pointerState && validLocation);
                //actualValidLocationObject.SetActive(validLocation);
                if (validLocation)
                {
                    ToggleRendererVisibility(actualCursor, false);
                }
            }
        }

        protected override void CreatePointerObjects()
        {
            actualContainer = new GameObject(string.Format("[{0}]GazePointerRenderer_Container", gameObject.name));
            actualContainer.transform.localPosition = Vector3.zero;
            VRTK_PlayerObject.SetPlayerObject(actualContainer, VRTK_PlayerObject.ObjectTypes.Pointer);

            CreateCursor();
            CreateValidLocationObject();
            Toggle(false, false);
            if (controllingPointer)
            {
                controllingPointer.ResetActivationTimer(true);
                controllingPointer.ResetSelectionTimer(true);
            }
        }

        protected override void DestroyPointerObjects()
        {
            if (actualContainer != null)
            {
                Destroy(actualContainer);
            }
        }

        protected override void ChangeMaterial(Color givenColor)
        {
            base.ChangeMaterial(givenColor);
            ChangeMaterialColor(actualCursor, givenColor);
        }

        protected override void UpdateObjectInteractor()
        {
            base.UpdateObjectInteractor();
            //if the object interactor is too far from the pointer tip then set it to the pointer tip position to prevent glitching.
            if (objectInteractor && actualCursor && Vector3.Distance(objectInteractor.transform.position, actualCursor.transform.position) > 0f)
            {
                objectInteractor.transform.position = actualCursor.transform.position;
            }
        }


        protected virtual void CreateValidLocationObject()
        {
            if (validLocationObject != null)
            {
                actualValidLocationObject = Instantiate(validLocationObject);
                actualValidLocationObject.name = string.Format("[{0}]GazePointerRenderer_ValidLocation", gameObject.name);
                actualValidLocationObject.transform.localScale = new Vector3(teleportCursorScale, teleportCursorScale * teleportCursorVerticalScaling, teleportCursorScale);
                actualValidLocationObject.transform.SetParent(actualContainer.transform);
                actualValidLocationObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                actualValidLocationObject.SetActive(false);
            }
        }

        protected virtual void CreateCursor()
        {
            if (customCursor)
            {
                actualCursor = Instantiate(customCursor);
            }
            else
            {
                actualCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                actualCursor.transform.localScale = Vector3.one * cursorScale;
                actualCursor.GetComponent<Collider>().isTrigger = true;
                actualCursor.AddComponent<Rigidbody>().isKinematic = true;
                actualCursor.layer = LayerMask.NameToLayer("Ignore Raycast");

                SetupMaterialRenderer(actualCursor);
            }

            cursorOriginalScale = actualCursor.transform.localScale;
            actualCursor.transform.name = string.Format("[{0}]GazePointerRenderer_Cursor", gameObject.name);
            actualCursor.transform.SetParent(actualContainer.transform);
            VRTK_PlayerObject.SetPlayerObject(actualCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
        }

        protected virtual void CheckRayMiss(bool rayHit, RaycastHit pointerCollidedWith)
        {
            if (!rayHit || (destinationHit.collider && destinationHit.collider != pointerCollidedWith.collider))
            {
                if (destinationHit.collider != null)
                {
                    PointerExit(destinationHit);
                }

                destinationHit = new RaycastHit();
                ChangeColor(invalidCollisionColor);
            }
        }

        protected virtual void CheckRayHit(bool rayHit, RaycastHit pointerCollidedWith)
        {
            if (rayHit)
            {
                PointerEnter(pointerCollidedWith);

                destinationHit = pointerCollidedWith;
                ChangeColor(validCollisionColor);

                bool validInteraction = destinationHit.collider.GetComponent<VRTK_InteractableObject>()
                    || destinationHit.collider.GetComponent<VRTK_UICanvas>();
                var uiSelectable = destinationHit.collider.GetComponent<Selectable>();
                if (uiSelectable)
                {
                    validInteraction = validInteraction || uiSelectable.interactable;
                }
                ChangeMaterialColor(actualCursor, validInteraction ? validCollisionColor : invalidCollisionColor);
            }
        }

        protected virtual float CastRayForward()
        {
            Transform origin = GetOrigin();
            Ray pointerRaycast = new Ray(origin.position, origin.forward);
            RaycastHit pointerCollidedWith;
            bool rayHit = VRTK_CustomRaycast.Raycast(customRaycast, pointerRaycast, out pointerCollidedWith, layersToIgnore, maximumLength);

            CheckRayMiss(rayHit, pointerCollidedWith);
            CheckRayHit(rayHit, pointerCollidedWith);

            float actualLength = maximumLength;
            if (rayHit && pointerCollidedWith.distance < maximumLength)
            {
                actualLength = pointerCollidedWith.distance;
            }

            return OverrideBeamLength(actualLength);
        }

        protected virtual void SetPointerAppearance(float tracerLength)
        {
            if (actualContainer)
            {
                actualCursor.transform.localScale = Vector3.one * cursorScale;
                actualCursor.transform.localPosition = new Vector3(0f, 0f, tracerLength);

                Transform origin = GetOrigin();
                actualContainer.transform.position = origin.position;
                actualContainer.transform.rotation = origin.rotation;

                ScaleObjectInteractor(actualCursor.transform.localScale * 1.05f);

                if (destinationHit.transform)
                {
                    if (cursorMatchTargetRotation)
                    {
                        actualCursor.transform.forward = -destinationHit.normal;
                    }

                    if (actualValidLocationObject)
                    {
                        if (cursorMatchTargetRotation)
                        {
                            actualValidLocationObject.transform.up = destinationHit.normal;
                        }
                        else
                        {
                            actualValidLocationObject.transform.up = Vector3.up;
                        }
                        actualValidLocationObject.transform.localPosition = actualCursor.transform.localPosition;
                    }
                    if (cursorDistanceRescale)
                    {
                        float collisionDistance = Vector3.Distance(destinationHit.point, origin.position);
                        actualCursor.transform.localScale = Vector3.Min(cursorOriginalScale * collisionDistance, maximumCursorScale);
                        if (actualValidLocationObject)
                        {
                            float scaledRadius = teleportCursorScale * collisionDistance;
                            actualValidLocationObject.transform.localScale = new Vector3(scaledRadius, scaledRadius * teleportCursorVerticalScaling, scaledRadius);
                        }
                    }
                }
                else
                {
                    if (actualValidLocationObject)
                    {
                        actualValidLocationObject.SetActive(false);
                    }
                    if (cursorMatchTargetRotation)
                    {
                        actualCursor.transform.forward = origin.forward;
                    }
                    if (cursorDistanceRescale)
                    {
                        actualCursor.transform.localScale = Vector3.Min(cursorOriginalScale * tracerLength, maximumCursorScale);
                    }
                }

                ToggleRenderer(controllingPointer.IsPointerActive(), false);
                UpdateDependencies(actualCursor.transform.position);
            }
        }
    }
}