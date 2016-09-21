// World Pointer|Abstractions|0020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// This abstract class provides any game pointer the ability to know the state of the implemented pointer. It extends the `VRTK_DestinationMarker` to allow for destination events to be emitted when the pointer cursor collides with objects.
    /// </summary>
    /// <remarks>
    /// The World Pointer also provides a play area cursor to be displayed for all cursors that utilise this class. The play area cursor is a representation of the current calibrated play area space and is useful for visualising the potential new play area space in the game world prior to teleporting. It can also handle collisions with objects on the new play area space and prevent teleporting if there are any collisions with objects at the potential new destination.
    ///
    /// The play area collider does not work well with terrains as they are uneven and cause collisions regularly so it is recommended that handling play area collisions is not enabled when using terrains.
    /// </remarks>
    public abstract class VRTK_WorldPointer : VRTK_DestinationMarker
    {
        /// <summary>
        /// States of Pointer Visibility.
        /// </summary>
        /// <param name="On_When_Active">Only shows the pointer beam when the Pointer button on the controller is pressed.</param>
        /// <param name="Always_On">Ensures the pointer beam is always visible but pressing the Pointer button on the controller initiates the destination set event.</param>
        /// <param name="Always_Off">Ensures the pointer beam is never visible but the destination point is still set and pressing the Pointer button on the controller still initiates the destination set event.</param>
        public enum pointerVisibilityStates
        {
            On_When_Active,
            Always_On,
            Always_Off
        }

        [Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controller = null;
        [Tooltip("The material to use on the rendered version of the pointer. If no material is selected then the default `WorldPointer` material will be used.")]
        public Material pointerMaterial;
        [Tooltip("The colour of the beam when it is colliding with a valid target. It can be set to a different colour for each controller.")]
        public Color pointerHitColor = new Color(0f, 0.5f, 0f, 1f);
        [Tooltip("The colour of the beam when it is not hitting a valid target. It can be set to a different colour for each controller.")]
        public Color pointerMissColor = new Color(0.8f, 0f, 0f, 1f);
        [Tooltip("If this is enabled then the play area boundaries are displayed at the tip of the pointer beam in the current pointer colour.")]
        public bool showPlayAreaCursor = false;
        [Tooltip("Determines the size of the play area cursor and collider. If the values are left as zero then the Play Area Cursor will be sized to the calibrated Play Area space.")]
        public Vector2 playAreaCursorDimensions = Vector2.zero;
        [Tooltip("If this is ticked then if the play area cursor is colliding with any other object then the pointer colour will change to the `Pointer Miss Color` and the `WorldPointerDestinationSet` event will not be triggered, which will prevent teleporting into areas where the play area will collide.")]
        public bool handlePlayAreaCursorCollisions = false;
        [Tooltip("A string that specifies an object Tag or the name of a Script attached to an object and notifies the play area cursor to ignore collisions with the object.")]
        public string ignoreTargetWithTagOrClass;
        [Tooltip("A specified VRTK_TagOrScriptPolicyList to use to determine whether the play area cursor collisions will be acted upon. If a list is provided then the 'Ignore Target With Tag Or Class' parameter will be ignored.")]
        public VRTK_TagOrScriptPolicyList targetTagOrScriptListPolicy;
        [Tooltip("Determines when the pointer beam should be displayed.")]
        public pointerVisibilityStates pointerVisibility = pointerVisibilityStates.On_When_Active;
        [Tooltip("If this is checked then the pointer beam will be activated on first press of the pointer alias button and will stay active until the pointer alias button is pressed again. The destination set event is emitted when the beam is deactivated on the second button press.")]
        public bool holdButtonToActivate = true;
        [Tooltip("The time in seconds to delay the pointer beam being able to be active again. Useful for preventing constant teleportation.")]
        public float activateDelay = 0f;

        protected Vector3 destinationPosition;
        protected float pointerContactDistance = 0f;
        protected Transform pointerContactTarget = null;
        protected RaycastHit pointerContactRaycastHit = new RaycastHit();
        protected uint controllerIndex;
        protected bool playAreaCursorCollided = false;

        private Transform playArea;
        private GameObject playAreaCursor;
        private GameObject[] playAreaCursorBoundaries;
        private BoxCollider playAreaCursorCollider;
        private Transform headset;
        private bool isActive;
        private bool destinationSetActive;
        private float activateDelayTimer = 0f;
        private int beamEnabledState = 0;
        private VRTK_InteractableObject interactableObject = null;

        /// <summary>
        /// The setPlayAreaCursorCollision method determines whether play area collisions should be taken into consideration with the play area cursor.
        /// </summary>
        /// <param name="state">The state of whether to check for play area collisions.</param>
        public virtual void setPlayAreaCursorCollision(bool state)
        {
            if (handlePlayAreaCursorCollisions)
            {
                playAreaCursorCollided = state;
            }
        }

        /// <summary>
        /// The IsActive method is used to determine if the pointer currently active.
        /// </summary>
        /// <returns>Is true if the pointer is currently active.</returns>
        public virtual bool IsActive()
        {
            return isActive;
        }

        /// <summary>
        /// The CanActivate method checks to see if the pointer can be activated as long as the activation delay timer is zero.
        /// </summary>
        /// <returns>Is true if the pointer is able to be activated due to the activation delay timer being zero.</returns>
        public virtual bool CanActivate()
        {
            return (Time.time >= activateDelayTimer);
        }

        /// <summary>
        /// The ToggleBeam method allows the pointer beam to be toggled on or off via code at runtime. If true is passed as the state then the beam is activated, if false then the beam is deactivated.
        /// </summary>
        /// <param name="state">The state of whether to enable or disable the beam.</param>
        public virtual void ToggleBeam(bool state)
        {
            var index = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            if (state)
            {
                TurnOnBeam(index);
            }
            else
            {
                TurnOffBeam(index);
            }
        }

        protected virtual void Awake()
        {
            if (controller == null)
            {
                controller = GetComponent<VRTK_ControllerEvents>();
            }

            if (controller == null)
            {
                Debug.LogError("VRTK_WorldPointer requires a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Controller);

            headset = VRTK_DeviceFinder.HeadsetTransform();
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            playAreaCursorBoundaries = new GameObject[4];
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            controller.AliasPointerOn += new ControllerInteractionEventHandler(EnablePointerBeam);
            controller.AliasPointerOff += new ControllerInteractionEventHandler(DisablePointerBeam);
            controller.AliasPointerSet += new ControllerInteractionEventHandler(SetPointerDestination);

            var tmpMaterial = Resources.Load("WorldPointer") as Material;
            if (pointerMaterial != null)
            {
                tmpMaterial = pointerMaterial;
            }

            pointerMaterial = new Material(tmpMaterial);
            pointerMaterial.color = pointerMissColor;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DisableBeam();
            destinationSetActive = false;
            pointerContactDistance = 0f;
            pointerContactTarget = null;
            destinationPosition = Vector3.zero;

            controller.AliasPointerOn -= new ControllerInteractionEventHandler(EnablePointerBeam);
            controller.AliasPointerOff -= new ControllerInteractionEventHandler(DisablePointerBeam);
            controller.AliasPointerSet -= new ControllerInteractionEventHandler(SetPointerDestination);

            if (playAreaCursor != null)
            {
                Destroy(playAreaCursor);
            }
        }

        protected virtual void Update()
        {
            if (playAreaCursor && playAreaCursor.activeSelf)
            {
                UpdateCollider();
            }
        }

        protected virtual void InitPointer()
        {
            InitPlayAreaCursor();
        }

        protected virtual void SetPlayAreaCursorTransform(Vector3 destination)
        {
            var offset = Vector3.zero;
            if (headsetPositionCompensation)
            {
                var playAreaPos = new Vector3(playArea.transform.position.x, 0, playArea.transform.position.z);
                var headsetPos = new Vector3(headset.position.x, 0, headset.position.z);
                offset = playAreaPos - headsetPos;
            }
            playAreaCursor.transform.position = destination + offset;
        }

        protected virtual void EnablePointerBeam(object sender, ControllerInteractionEventArgs e)
        {
            TurnOnBeam(e.controllerIndex);
        }

        protected virtual void DisablePointerBeam(object sender, ControllerInteractionEventArgs e)
        {
            TurnOffBeam(e.controllerIndex);
        }

        protected virtual void SetPointerDestination(object sender, ControllerInteractionEventArgs e)
        {
            PointerSet();
        }

        protected virtual void PointerIn()
        {
            if (!enabled || !pointerContactTarget)
            {
                return;
            }

            OnDestinationMarkerEnter(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerIndex));
            StartUseAction(pointerContactTarget);
        }

        protected virtual void PointerOut()
        {
            if (!enabled || !pointerContactTarget)
            {
                return;
            }

            OnDestinationMarkerExit(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerIndex));
            StopUseAction();
        }

        private bool InvalidConstantBeam()
        {
            var equalToggleSet = controller.pointerToggleButton == controller.pointerSetButton;
            return (!holdButtonToActivate && ((equalToggleSet && beamEnabledState != 0) || (!equalToggleSet && !isActive)));
        }

        protected virtual void PointerSet()
        {
            if (!enabled || !destinationSetActive || !pointerContactTarget || !CanActivate() || InvalidConstantBeam())
            {
                return;
            }

            activateDelayTimer = Time.time + activateDelay;

            var setInteractableObject = pointerContactTarget.GetComponent<VRTK_InteractableObject>();
            if (PointerActivatesUseAction(setInteractableObject))
            {
                if (setInteractableObject.IsUsing())
                {
                    setInteractableObject.StopUsing(gameObject);
                }
                else if (!setInteractableObject.holdButtonToUse)
                {
                    setInteractableObject.StartUsing(gameObject);
                }
            }

            if (!playAreaCursorCollided && !PointerActivatesUseAction(interactableObject))
            {
                OnDestinationMarkerSet(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerIndex));
            }

            if (!isActive)
            {
                destinationSetActive = false;
            }
        }

        protected virtual void TogglePointer(bool state)
        {
            var playAreaState = (showPlayAreaCursor ? state : false);
            if (playAreaCursor)
            {
                playAreaCursor.gameObject.SetActive(playAreaState);
            }
            if (!state && PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && interactableObject.IsUsing())
            {
                interactableObject.StopUsing(gameObject);
            }
        }

        protected virtual void SetPointerMaterial()
        {
            foreach (GameObject playAreaCursorBoundary in playAreaCursorBoundaries)
            {
                playAreaCursorBoundary.GetComponent<Renderer>().material = pointerMaterial;
            }
        }

        protected void UpdatePointerMaterial(Color color)
        {
            if (playAreaCursorCollided || !ValidDestination(pointerContactTarget, destinationPosition))
            {
                color = pointerMissColor;
            }
            pointerMaterial.color = color;
            SetPointerMaterial();
        }

        protected virtual bool ValidDestination(Transform target, Vector3 destinationPosition)
        {
            bool validNavMeshLocation = false;
            if (target)
            {
                NavMeshHit hit;
                validNavMeshLocation = NavMesh.SamplePosition(destinationPosition, out hit, 0.1f, NavMesh.AllAreas);
            }
            if (navMeshCheckDistance == 0f)
            {
                validNavMeshLocation = true;
            }
            return (validNavMeshLocation && target && !(Utilities.TagOrScriptCheck(target.gameObject, invalidTagOrScriptListPolicy, invalidTargetWithTagOrClass)));
        }

        private bool PointerActivatesUseAction(VRTK_InteractableObject io)
        {
            return (io && io.pointerActivatesUseAction && io.IsValidInteractableController(controller.gameObject, io.allowedUseControllers));
        }

        private void StartUseAction(Transform target)
        {
            interactableObject = target.GetComponent<VRTK_InteractableObject>();
            bool cannotUseBecauseNotGrabbed = (interactableObject && interactableObject.useOnlyIfGrabbed && !interactableObject.IsGrabbed());

            if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && !cannotUseBecauseNotGrabbed)
            {
                interactableObject.StartUsing(gameObject);
            }
        }

        private void StopUseAction()
        {
            if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse)
            {
                interactableObject.StopUsing(gameObject);
            }
        }

        private void TurnOnBeam(uint index)
        {
            beamEnabledState++;
            if (enabled && !isActive && CanActivate())
            {
                setPlayAreaCursorCollision(false);
                controllerIndex = index;
                TogglePointer(true);
                isActive = true;
                destinationSetActive = true;
            }
        }

        private void TurnOffBeam(uint index)
        {
            if (enabled && isActive && (holdButtonToActivate || (!holdButtonToActivate && beamEnabledState >= 2)))
            {
                controllerIndex = index;
                DisableBeam();
            }
        }

        private void DisableBeam()
        {
            TogglePointer(false);
            isActive = false;
            beamEnabledState = 0;
        }

        private void DrawPlayAreaCursorBoundary(int index, float left, float right, float top, float bottom, float thickness, Vector3 localPosition)
        {
            var playAreaCursorBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playAreaCursorBoundary.name = string.Format("[{0}]WorldPointer_PlayAreaCursorBoundary_" + index, gameObject.name);
            Utilities.SetPlayerObject(playAreaCursorBoundary, VRTK_PlayerObject.ObjectTypes.Pointer);

            var width = (right - left) / 1.065f;
            var length = (top - bottom) / 1.08f;
            var height = thickness;

            playAreaCursorBoundary.transform.localScale = new Vector3(width, height, length);
            Destroy(playAreaCursorBoundary.GetComponent<BoxCollider>());
            playAreaCursorBoundary.layer = LayerMask.NameToLayer("Ignore Raycast");

            playAreaCursorBoundary.transform.parent = playAreaCursor.transform;
            playAreaCursorBoundary.transform.localPosition = localPosition;

            playAreaCursorBoundaries[index] = playAreaCursorBoundary;
        }

        private void InitPlayAreaCursor()
        {
            var btmRightInner = 0;
            var btmLeftInner = 1;
            var topLeftInner = 2;
            var topRightInner = 3;

            var btmRightOuter = 4;
            var btmLeftOuter = 5;
            var topLeftOuter = 6;
            var topRightOuter = 7;

            Vector3[] cursorDrawVertices = VRTK_SDK_Bridge.GetPlayAreaVertices(playArea.gameObject);

            if (playAreaCursorDimensions != Vector2.zero)
            {
                var customAreaPadding = VRTK_SDK_Bridge.GetPlayAreaBorderThickness(playArea.gameObject);

                cursorDrawVertices[btmRightOuter] = new Vector3(playAreaCursorDimensions.x / 2, 0f, (playAreaCursorDimensions.y / 2) * -1);
                cursorDrawVertices[btmLeftOuter] = new Vector3((playAreaCursorDimensions.x / 2) * -1, 0f, (playAreaCursorDimensions.y / 2) * -1);
                cursorDrawVertices[topLeftOuter] = new Vector3((playAreaCursorDimensions.x / 2) * -1, 0f, playAreaCursorDimensions.y / 2);
                cursorDrawVertices[topRightOuter] = new Vector3(playAreaCursorDimensions.x / 2, 0f, playAreaCursorDimensions.y / 2);

                cursorDrawVertices[btmRightInner] = cursorDrawVertices[btmRightOuter] + new Vector3(-customAreaPadding, 0f, customAreaPadding);
                cursorDrawVertices[btmLeftInner] = cursorDrawVertices[btmLeftOuter] + new Vector3(customAreaPadding, 0f, customAreaPadding);
                cursorDrawVertices[topLeftInner] = cursorDrawVertices[topLeftOuter] + new Vector3(customAreaPadding, 0f, -customAreaPadding);
                cursorDrawVertices[topRightInner] = cursorDrawVertices[topRightOuter] + new Vector3(-customAreaPadding, 0f, -customAreaPadding);
            }

            var width = cursorDrawVertices[btmRightOuter].x - cursorDrawVertices[topLeftOuter].x;
            var length = cursorDrawVertices[topLeftOuter].z - cursorDrawVertices[btmRightOuter].z;
            var height = 0.01f;

            playAreaCursor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playAreaCursor.name = string.Format("[{0}]WorldPointer_PlayAreaCursor", gameObject.name);
            Utilities.SetPlayerObject(playAreaCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
            playAreaCursor.transform.parent = null;
            playAreaCursor.transform.localScale = new Vector3(width, height, length);
            playAreaCursor.SetActive(false);

            playAreaCursor.GetComponent<Renderer>().enabled = false;

            CreateCursorCollider(playAreaCursor);

            playAreaCursor.AddComponent<Rigidbody>().isKinematic = true;

            var playAreaCursorScript = playAreaCursor.AddComponent<VRTK_PlayAreaCollider>();
            playAreaCursorScript.SetParent(gameObject);
            playAreaCursorScript.SetIgnoreTarget(ignoreTargetWithTagOrClass, targetTagOrScriptListPolicy);
            playAreaCursor.layer = LayerMask.NameToLayer("Ignore Raycast");

            var playAreaBoundaryX = playArea.transform.localScale.x / 2;
            var playAreaBoundaryZ = playArea.transform.localScale.z / 2;
            var heightOffset = 0f;

            DrawPlayAreaCursorBoundary(0, cursorDrawVertices[btmLeftOuter].x, cursorDrawVertices[btmRightOuter].x, cursorDrawVertices[btmRightInner].z, cursorDrawVertices[btmRightOuter].z, height, new Vector3(0f, heightOffset, playAreaBoundaryZ));
            DrawPlayAreaCursorBoundary(1, cursorDrawVertices[btmLeftOuter].x, cursorDrawVertices[btmLeftInner].x, cursorDrawVertices[topLeftOuter].z, cursorDrawVertices[btmLeftOuter].z, height, new Vector3(playAreaBoundaryX, heightOffset, 0f));
            DrawPlayAreaCursorBoundary(2, cursorDrawVertices[btmLeftOuter].x, cursorDrawVertices[btmRightOuter].x, cursorDrawVertices[btmRightInner].z, cursorDrawVertices[btmRightOuter].z, height, new Vector3(0f, heightOffset, -playAreaBoundaryZ));
            DrawPlayAreaCursorBoundary(3, cursorDrawVertices[btmLeftOuter].x, cursorDrawVertices[btmLeftInner].x, cursorDrawVertices[topLeftOuter].z, cursorDrawVertices[btmLeftOuter].z, height, new Vector3(-playAreaBoundaryX, heightOffset, 0f));
        }

        private void CreateCursorCollider(GameObject cursor)
        {
            playAreaCursorCollider = cursor.GetComponent<BoxCollider>();
            playAreaCursorCollider.isTrigger = true;
            playAreaCursorCollider.center = new Vector3(0f, 65f, 0f);
            playAreaCursorCollider.size = new Vector3(1f, 1f, 1f);
        }

        private void UpdateCollider()
        {
            var playAreaHeightAdjustment = 1f;
            var newBCYSize = (headset.transform.position.y - playArea.transform.position.y) * 100f;
            var newBCYCenter = (newBCYSize != 0 ? (newBCYSize / 2) + playAreaHeightAdjustment : 0);

            playAreaCursorCollider.size = new Vector3(playAreaCursorCollider.size.x, newBCYSize, playAreaCursorCollider.size.z);
            playAreaCursorCollider.center = new Vector3(playAreaCursorCollider.center.x, newBCYCenter, playAreaCursorCollider.center.z);
        }
    }

    public class VRTK_PlayAreaCollider : MonoBehaviour
    {
        private GameObject parent;
        private string ignoreTargetWithTagOrClass;
        private VRTK_TagOrScriptPolicyList targetTagOrScriptListPolicy;

        public void SetParent(GameObject setParent)
        {
            parent = setParent;
        }

        public void SetIgnoreTarget(string ignore, VRTK_TagOrScriptPolicyList list = null)
        {
            ignoreTargetWithTagOrClass = ignore;
            targetTagOrScriptListPolicy = list;
        }

        private bool ValidTarget(Collider collider)
        {
            return (!collider.GetComponent<VRTK_PlayerObject>() && !(Utilities.TagOrScriptCheck(collider.gameObject, targetTagOrScriptListPolicy, ignoreTargetWithTagOrClass)));
        }

        private void OnTriggerStay(Collider collider)
        {
            if (parent.GetComponent<VRTK_WorldPointer>().IsActive() && ValidTarget(collider))
            {
                parent.GetComponent<VRTK_WorldPointer>().setPlayAreaCursorCollision(true);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (ValidTarget(collider))
            {
                parent.GetComponent<VRTK_WorldPointer>().setPlayAreaCursorCollision(false);
            }
        }
    }
}