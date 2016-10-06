﻿// Play Area Cursor|Scripts|0055
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Play Area Cursor is used in conjunction with a World Pointer script and displays a representation of the play area where the pointer cursor hits.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/012_Controller_PointerWithAreaCollision` shows how a Bezier Pointer with the Play Area Cursor and Collision Detection enabled can be used to traverse a game area but not allow teleporting into areas where the walls or other objects would fall into the play area space enabling the user to enter walls.
    /// </example>
    public class VRTK_PlayAreaCursor : MonoBehaviour
    {
        [Tooltip("Determines the size of the play area cursor and collider. If the values are left as zero then the Play Area Cursor will be sized to the calibrated Play Area space.")]
        public Vector2 playAreaCursorDimensions = Vector2.zero;
        [Tooltip("If this is ticked then if the play area cursor is colliding with any other object then the pointer colour will change to the `Pointer Miss Color` and the `WorldPointerDestinationSet` event will not be triggered, which will prevent teleporting into areas where the play area will collide.")]
        public bool handlePlayAreaCursorCollisions = false;
        [Tooltip("A string that specifies an object Tag or the name of a Script attached to an object and notifies the play area cursor to ignore collisions with the object.")]
        public string ignoreTargetWithTagOrClass;
        [Tooltip("A specified VRTK_TagOrScriptPolicyList to use to determine whether the play area cursor collisions will be acted upon. If a list is provided then the 'Ignore Target With Tag Or Class' parameter will be ignored.")]
        public VRTK_TagOrScriptPolicyList targetTagOrScriptListPolicy;

        private bool headsetPositionCompensation;
        private bool playAreaCursorCollided = false;
        private Transform playArea;
        private GameObject playAreaCursor;
        private GameObject[] playAreaCursorBoundaries;
        private BoxCollider playAreaCursorCollider;
        private Transform headset;

        /// <summary>
        /// The HasCollided method returns the state of whether the play area cursor has currently collided with another valid object.
        /// </summary>
        /// <returns>A bool to determine the state of collision. `true` if the play area is colliding with a valid object and `false` if not.</returns>
        public virtual bool HasCollided()
        {
            return playAreaCursorCollided;
        }

        /// <summary>
        /// The SetHeadsetPositionCompensation method determines whether the offset position of the headset from the centre of the play area should be taken into consideration when setting the destination marker. If `true` then it will take the offset position into consideration.
        /// </summary>
        /// <param name="state">The state of whether to take the position of the headset within the play area into account when setting the destination marker.</param>
        public virtual void SetHeadsetPositionCompensation(bool state)
        {
            headsetPositionCompensation = state;
        }

        /// <summary>
        /// The SetPlayAreaCursorCollision method determines whether play area collisions should be taken into consideration with the play area cursor.
        /// </summary>
        /// <param name="state">The state of whether to check for play area collisions.</param>
        public virtual void SetPlayAreaCursorCollision(bool state)
        {
            playAreaCursorCollided = false;
            state = (!enabled ? false : state);
            if (handlePlayAreaCursorCollisions)
            {
                playAreaCursorCollided = state;
            }
        }

        /// <summary>
        /// The SetMaterialColor method sets the current material colour on the play area cursor.
        /// </summary>
        /// <param name="color">The colour to update the play area cursor material to.</param>
        public virtual void SetMaterialColor(Color color)
        {
            foreach (GameObject playAreaCursorBoundary in playAreaCursorBoundaries)
            {
                var paRenderer = playAreaCursorBoundary.GetComponent<Renderer>();

                if (paRenderer && paRenderer.material && paRenderer.material.HasProperty("_Color"))
                {
                    paRenderer.material.color = color;
                }
            }
        }

        /// <summary>
        /// The SetPlayAreaCursorTransform method is used to update the position of the play area cursor in world space to the given location.
        /// </summary>
        /// <param name="location">The location where to draw the play area cursor.</param>
        public virtual void SetPlayAreaCursorTransform(Vector3 location)
        {
            var offset = Vector3.zero;
            if (headsetPositionCompensation)
            {
                var playAreaPos = new Vector3(playArea.transform.position.x, 0f, playArea.transform.position.z);
                var headsetPos = new Vector3(headset.position.x, 0f, headset.position.z);
                offset = playAreaPos - headsetPos;
            }
            playAreaCursor.transform.position = location + offset;
        }

        /// <summary>
        /// The ToggleState method enables or disables the visibility of the play area cursor.
        /// </summary>
        /// <param name="state">The state of whether to show or hide the play area cursor.</param>
        public virtual void ToggleState(bool state)
        {
            state = (!enabled ? false : state);
            playAreaCursor.SetActive(state);
        }

        protected virtual void Awake()
        {
            if (!GetComponent<VRTK_WorldPointer>())
            {
                Debug.LogError("VRTK_PlayAreaCursor requires a VRTK_WorldPointer script attached to the same object.");
                return;
            }

            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);

            headset = VRTK_DeviceFinder.HeadsetTransform();
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            playAreaCursorBoundaries = new GameObject[4];
            InitPlayAreaCursor();
        }

        protected virtual void Destroy()
        {
            if (playAreaCursor != null)
            {
                Destroy(playAreaCursor);
            }
        }

        protected virtual void Update()
        {
            if (enabled && playAreaCursor && playAreaCursor.activeInHierarchy)
            {
                UpdateCollider();
            }
        }

        private void DrawPlayAreaCursorBoundary(int index, float left, float right, float top, float bottom, float thickness, Vector3 localPosition)
        {
            var playAreaCursorBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playAreaCursorBoundary.name = string.Format("[{0}]PlayAreaCursorBoundary_" + index, gameObject.name);
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
            playAreaCursor.name = string.Format("[{0}]PlayAreaCursor", gameObject.name);
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
        private VRTK_PlayAreaCursor parent;
        private string ignoreTargetWithTagOrClass;
        private VRTK_TagOrScriptPolicyList targetTagOrScriptListPolicy;

        public void SetParent(GameObject setParent)
        {
            parent = setParent.GetComponent<VRTK_PlayAreaCursor>();
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
            if (parent.enabled && parent.gameObject.activeInHierarchy && ValidTarget(collider))
            {
                parent.SetPlayAreaCursorCollision(true);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (ValidTarget(collider))
            {
                parent.SetPlayAreaCursorCollision(false);
            }
        }
    }
}