﻿//====================================================================================
//
// Purpose: Provide abstraction into projecting a raycast into the game world.
// As this is an abstract class, it should never be used on it's own.
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public abstract class VRTK_WorldPointer : VRTK_DestinationMarker
    {
        public enum pointerVisibilityStates
        {
            On_When_Active,
            Always_On,
            Always_Off
        }

        public VRTK_ControllerEvents controller = null;
        public Material pointerMaterial;
        public Color pointerHitColor = new Color(0f, 0.5f, 0f, 1f);
        public Color pointerMissColor = new Color(0.8f, 0f, 0f, 1f);
        public bool showPlayAreaCursor = false;
        public Vector2 playAreaCursorDimensions = Vector2.zero;
        public bool handlePlayAreaCursorCollisions = false;
        public string ignoreTargetWithTagOrClass;
        public pointerVisibilityStates pointerVisibility = pointerVisibilityStates.On_When_Active;

        public float activateDelay = 0f;

        protected Vector3 destinationPosition;
        protected float pointerContactDistance = 0f;
        protected Transform pointerContactTarget = null;
        protected uint controllerIndex;

        protected bool playAreaCursorCollided = false;

        private SteamVR_PlayArea playArea;
        private GameObject playAreaCursor;
        private GameObject[] playAreaCursorBoundaries;
        private BoxCollider playAreaCursorCollider;
        private Transform headset;
        private bool isActive;
        private bool destinationSetActive;
        private bool eventsRegistered = false;

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

            Utilities.SetPlayerObject(this.gameObject, VRTK_PlayerObject.ObjectTypes.Controller);

            //Setup controller event listeners
            controller.AliasPointerOn += new ControllerInteractionEventHandler(EnablePointerBeam);
            controller.AliasPointerOff += new ControllerInteractionEventHandler(DisablePointerBeam);
            controller.AliasPointerSet += new ControllerInteractionEventHandler(SetPointerDestination);

            eventsRegistered = true;

            headset = DeviceFinder.HeadsetTransform();

            playArea = GameObject.FindObjectOfType<SteamVR_PlayArea>();
            playAreaCursorBoundaries = new GameObject[4];

            var tmpMaterial = Resources.Load("WorldPointer") as Material;
            if (pointerMaterial != null)
            {
                tmpMaterial = pointerMaterial;
            }

            pointerMaterial = new Material(tmpMaterial);
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

        protected virtual void OnDestroy()
        {
            if (eventsRegistered)
            {
                controller.AliasPointerOn -= EnablePointerBeam;
                controller.AliasPointerOff -= DisablePointerBeam;
                controller.AliasPointerSet -= SetPointerDestination;
            }

            if (playAreaCursor != null)
            {
                Destroy(playAreaCursor);
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
            if (this.enabled && !isActive && activateDelayTimer <= 0)
            {
                setPlayAreaCursorCollision(false);
                controllerIndex = e.controllerIndex;
                TogglePointer(true);
                isActive = true;
                destinationSetActive = true;
            }
        }

        protected virtual void DisablePointerBeam(object sender, ControllerInteractionEventArgs e)
        {
            if (isActive)
            {
                controllerIndex = e.controllerIndex;
                TogglePointer(false);
                isActive = false;
            }
        }

        protected virtual void SetPointerDestination(object sender, ControllerInteractionEventArgs e)
        {
            PointerSet();
        }

        protected virtual void PointerIn()
        {
            if (!this.enabled || !pointerContactTarget)
            {
                return;
            }

            OnDestinationMarkerEnter(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, destinationPosition, controllerIndex));

            var interactableObject = pointerContactTarget.GetComponent<VRTK_InteractableObject>();
            if (interactableObject && interactableObject.pointerActivatesUseAction && interactableObject.holdButtonToUse)
            {
                interactableObject.StartUsing(this.gameObject);
            }
        }

        protected virtual void PointerOut()
        {
            if (!this.enabled || !pointerContactTarget)
            {
                return;
            }

            OnDestinationMarkerExit(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, destinationPosition, controllerIndex));

            var interactableObject = pointerContactTarget.GetComponent<VRTK_InteractableObject>();
            if (interactableObject && interactableObject.pointerActivatesUseAction && interactableObject.holdButtonToUse)
            {
                interactableObject.StopUsing(this.gameObject);
            }
        }

        protected virtual void PointerSet()
        {
            if (!this.enabled || !destinationSetActive || !pointerContactTarget || activateDelayTimer > 0)
            {
                return;
            }

            activateDelayTimer = activateDelay;

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
                OnDestinationMarkerSet(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, destinationPosition, controllerIndex));
            }

            if (!isActive)
            {
                destinationSetActive = false;
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

        /// <summary>
        /// Determines if a teleport location is valid given a list of names (eg tags or component names) 
        /// and an appropriate lookup function (eg Transform.CompareTag).
        /// </summary>
        /// <param name="invalidTagsOrComponentNames"></param>
        /// <param name="function"></param>
        /// <returns>true is the teleport destination is valid</returns>
        public static bool IsDestinationStringValid(List<string> invalidTagsOrComponentNames, Func<string, bool> function)
        {
            var isValid = true;
            foreach (var tag in invalidTagsOrComponentNames)
            {
                if (function(tag))
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }

        /// <summary>
        /// Checks that the given transform is a valid teleport location based on the disallowed tags and 
        /// component types provided. A null target is also implicitly considered an invalid teleportation target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="invalidTags"></param>
        /// <param name="invalidComponentNames"></param>
        /// <returns>true if teleportation to this target is allowed</returns>
        public static bool IsValidTagOrComponentDestination(Transform target, List<string> invalidTags, List<string> invalidComponentNames)
        {
            bool validTag = false;
            bool validComponent = false;
            if (target != null)
            {
                validTag = IsDestinationStringValid(invalidTags, target.CompareTag);
                validComponent = IsDestinationStringValid(invalidComponentNames, x => target.GetComponent(x) != null);
            }

            return (validTag && validComponent);
        }

        protected virtual bool ValidDestination(Transform target)
        {
            bool validNavMeshLocation = false;
            if (target)
            {
                NavMeshHit hit;
                validNavMeshLocation = NavMesh.SamplePosition(target.position, out hit, 1.0f, NavMesh.AllAreas);
            }
            if (!checkNavMesh)
            {
                validNavMeshLocation = true;
            }

            var validTagOrComponent = IsValidTagOrComponentDestination(target, ignoreTargetWithTags, ignoreTargetWithComponents);

            return (validNavMeshLocation && validTagOrComponent);
        }

        private void DrawPlayAreaCursorBoundary(int index, float left, float right, float top, float bottom, float thickness, Vector3 localPosition)
        {
            var playAreaCursorBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playAreaCursorBoundary.name = string.Format("[{0}]WorldPointer_PlayAreaCursorBoundary_" + index, this.gameObject.name);
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
            playAreaCursor.name = string.Format("[{0}]WorldPointer_PlayAreaCursor", this.gameObject.name);
            Utilities.SetPlayerObject(playAreaCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
            playAreaCursor.transform.parent = null;
            playAreaCursor.transform.localScale = new Vector3(width, height, length);
            playAreaCursor.SetActive(false);

            playAreaCursor.GetComponent<Renderer>().enabled = false;

            CreateCursorCollider(playAreaCursor);

            playAreaCursor.AddComponent<Rigidbody>().isKinematic = true;

            var playAreaCursorScript = playAreaCursor.AddComponent<VRTK_PlayAreaCollider>();
            playAreaCursorScript.SetParent(this.gameObject);
            playAreaCursorScript.SetIgnoreTarget(ignoreTargetWithTagOrClass);
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

        public void SetParent(GameObject setParent)
        {
            parent = setParent;
        }

        public void SetIgnoreTarget(string ignore)
        {
            ignoreTargetWithTagOrClass = ignore;
        }

        private bool ValidTarget(Collider collider)
        {
            return (!collider.GetComponent<VRTK_PlayerObject>() & collider.tag != ignoreTargetWithTagOrClass && collider.GetComponent(ignoreTargetWithTagOrClass) == null);
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