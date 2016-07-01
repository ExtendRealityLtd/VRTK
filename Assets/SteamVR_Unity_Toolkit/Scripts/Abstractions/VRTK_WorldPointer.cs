//====================================================================================
//
// Purpose: Provide abstraction into projecting a raycast into the game world.
// As this is an abstract class, it should never be used on it's own.
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public abstract class VRTK_WorldPointer : VRTK_DestinationMarker
    {
        public enum pointerVisibilityStates
        {
            On_When_Active,
            Always_On,
            Always_Off
        }

        public VRTK_ControllerEvents controller = null;
        public Color pointerHitColor = new Color(0f, 0.5f, 0f, 1f);
        public Color pointerMissColor = new Color(0.8f, 0f, 0f, 1f);
        public bool showPlayAreaCursor = false;
        public Vector2 playAreaCursorDimensions = Vector2.zero;
        public bool handlePlayAreaCursorCollisions = false;
        public pointerVisibilityStates pointerVisibility = pointerVisibilityStates.On_When_Active;

        public float activateDelay = 0f;

        protected Vector3 destinationPosition;
        protected float pointerContactDistance = 0f;
        protected Transform pointerContactTarget = null;
        protected uint controllerIndex;

        protected Material pointerMaterial;
        protected bool playAreaCursorCollided = false;

        private SteamVR_PlayArea playArea;
        private GameObject playAreaCursor;
        private GameObject[] playAreaCursorBoundaries;
        private BoxCollider playAreaCursorCollider;
        private Transform headset;
        private bool isActive;

        private float activateDelayTimer = 0f;

        public virtual void setPlayAreaCursorCollision(bool state)
        {
            if (handlePlayAreaCursorCollisions)
            {
                playAreaCursorCollided = state;
            }
        }

        public virtual bool IsActive()
        {
            return isActive;
        }

        public virtual bool CanActivate()
        {
            return (activateDelayTimer <= 0);
        }

        protected virtual void Start()
        {

            if (controller == null)
            {
                controller = this.GetComponent<VRTK_ControllerEvents>();
            }

            if (controller == null)
            {
                Debug.LogError("VRTK_WorldPointer requires a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            this.name = "PlayerObject_" + this.name;

            //Setup controller event listeners
            controller.AliasPointerOn += new ControllerInteractionEventHandler(EnablePointerBeam);
            controller.AliasPointerOff += new ControllerInteractionEventHandler(DisablePointerBeam);

            headset = DeviceFinder.HeadsetTransform();

            playArea = GameObject.FindObjectOfType<SteamVR_PlayArea>();
            playAreaCursorBoundaries = new GameObject[4];

            pointerMaterial = new Material(Shader.Find("Unlit/TransparentColor"));
            pointerMaterial.color = pointerMissColor;
        }

        protected virtual void Update()
        {
            if (activateDelayTimer > 0)
            {
                activateDelayTimer -= Time.deltaTime;
            }

            if (playAreaCursor.activeSelf)
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
            var playAreaPos = new Vector3(playArea.transform.position.x, 0, playArea.transform.position.z);
            var headsetPos = new Vector3(headset.position.x, 0, headset.position.z);
            var offset = playAreaPos - headsetPos;
            playAreaCursor.transform.position = destination + offset;
        }

        protected virtual void EnablePointerBeam(object sender, ControllerInteractionEventArgs e)
        {
            if (!isActive && activateDelayTimer <= 0)
            {
                setPlayAreaCursorCollision(false);
                controllerIndex = e.controllerIndex;
                TogglePointer(true);
                isActive = true;
            }
        }

        protected virtual void DisablePointerBeam(object sender, ControllerInteractionEventArgs e)
        {
            if (isActive && activateDelayTimer <= 0)
            {
                activateDelayTimer = activateDelay;
                controllerIndex = e.controllerIndex;
                TogglePointer(false);
                isActive = false;
            }
        }

        protected virtual void PointerIn()
        {
            if (!pointerContactTarget)
            {
                return;
            }

            OnDestinationMarkerEnter(SeDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, destinationPosition, controllerIndex));

            var interactableObject = pointerContactTarget.GetComponent<VRTK_InteractableObject>();
            if (interactableObject && interactableObject.pointerActivatesUseAction && interactableObject.holdButtonToUse)
            {
                interactableObject.StartUsing(this.gameObject);
            }
        }

        protected virtual void PointerOut()
        {
            if (!pointerContactTarget)
            {
                return;
            }

            OnDestinationMarkerExit(SeDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, destinationPosition, controllerIndex));

            var interactableObject = pointerContactTarget.GetComponent<VRTK_InteractableObject>();
            if (interactableObject && interactableObject.pointerActivatesUseAction && interactableObject.holdButtonToUse)
            {
                interactableObject.StopUsing(this.gameObject);
            }
        }

        protected virtual void PointerSet()
        {
            if (!isActive || !pointerContactTarget)
            {
                return;
            }

            var interactableObject = pointerContactTarget.GetComponent<VRTK_InteractableObject>();
            if (interactableObject && interactableObject.pointerActivatesUseAction)
            {
                if (interactableObject.IsUsing())
                {
                    interactableObject.StopUsing(this.gameObject);
                }
                else if (!interactableObject.holdButtonToUse)
                {
                    interactableObject.StartUsing(this.gameObject);
                }
            }

            if (!playAreaCursorCollided && (interactableObject == null || !interactableObject.pointerActivatesUseAction))
            {
                OnDestinationMarkerSet(SeDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, destinationPosition, controllerIndex));
            }
        }

        protected virtual void TogglePointer(bool state)
        {
            var playAreaState = (showPlayAreaCursor ? state : false);
            playAreaCursor.gameObject.SetActive(playAreaState);
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
            if (playAreaCursorCollided || !ValidDestination(pointerContactTarget))
            {
                color = pointerMissColor;
            }
            pointerMaterial.color = color;
            SetPointerMaterial();
        }

        protected virtual bool ValidDestination(Transform target)
        {
            return (target && target.tag != invalidTargetWithTagOrClass && target.GetComponent(invalidTargetWithTagOrClass) == null);
        }

        private void DrawPlayAreaCursorBoundary(int index, float left, float right, float top, float bottom, float thickness, Vector3 localPosition)
        {
            var playAreaCursorBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playAreaCursorBoundary.name = string.Format("[{0}]PlayerObject_WorldPointer_PlayAreaCursorBoundary_" + index, this.gameObject.name);

            var width = (right - left) / 1.065f;
            var length = (top - bottom) / 1.08f;
            var height = thickness;

            playAreaCursorBoundary.transform.localScale = new Vector3(width, height, length);
            Destroy(playAreaCursorBoundary.GetComponent<BoxCollider>());
            playAreaCursorBoundary.layer = 2;

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

            Vector3[] cursorDrawVertices = playArea.vertices;

            if (playAreaCursorDimensions != Vector2.zero)
            {
                var customAreaPadding = playArea.borderThickness;

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
            playAreaCursor.name = string.Format("[{0}]PlayerObject_WorldPointer_PlayAreaCursor", this.gameObject.name);
            playAreaCursor.transform.parent = null;
            playAreaCursor.transform.localScale = new Vector3(width, height, length);
            playAreaCursor.SetActive(false);

            playAreaCursor.GetComponent<Renderer>().enabled = false;

            CreateCursorCollider(playAreaCursor);

            playAreaCursor.AddComponent<Rigidbody>().isKinematic = true;

            var playAreaCursorScript = playAreaCursor.AddComponent<VRTK_PlayAreaCollider>();
            playAreaCursorScript.SetParent(this.gameObject);
            playAreaCursor.layer = 2;

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

        public void SetParent(GameObject setParent)
        {
            parent = setParent;
        }

        private void OnTriggerStay(Collider collider)
        {
            if (parent.GetComponent<VRTK_WorldPointer>().IsActive() && !collider.name.Contains("PlayerObject_"))
            {
                parent.GetComponent<VRTK_WorldPointer>().setPlayAreaCursorCollision(true);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!collider.name.Contains("PlayerObject_"))
            {
                parent.GetComponent<VRTK_WorldPointer>().setPlayAreaCursorCollision(false);
            }
        }
    }
}