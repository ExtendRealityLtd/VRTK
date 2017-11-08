﻿// Play Area Cursor|Pointers|10050
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="collidedWith">The collider that is/was being collided with.</param>
    public struct PlayAreaCursorEventArgs
    {
        public Collider collider;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="PlayAreaCursorEventArgs"/></param>
    public delegate void PlayAreaCursorEventHandler(object sender, PlayAreaCursorEventArgs e);

    /// <summary>
    /// Provides a visual representation of the play area boundaries that tracks to the cursor position of a pointer.
    /// </summary>
    /// <remarks>
    /// **Optional Components:**
    ///  * `VRTK_PointerDirectionIndicator` - A Pointer Direction Indicator to set the cursor rotation to.
    /// 
    /// **Script Usage:**
    ///  * Place the `VRTK_PlayAreaCursor` script on the same GameObject as the Pointer Renderer script it is linked to.
    ///  * Link the required Play Area Cursor script to the `Playarea Cursor` parameter on the required Pointer Renderer script.
    ///
    /// **Script Dependencies:**
    ///  * A Base Pointer Renderer script attached to a valid Pointer script is required so the PlayArea Cursor script can be linked to follow the valid Base Pointer Renderer cursor GameObject.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/012_Controller_PointerWithAreaCollision` shows how a Bezier Pointer with the Play Area Cursor and Collision Detection enabled can be used to traverse a game area but not allow teleporting into areas where the walls or other objects would fall into the play area space enabling the user to enter walls.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Pointers/VRTK_PlayAreaCursor")]
    public class VRTK_PlayAreaCursor : MonoBehaviour
    {
        [Header("Appearance Settings")]

        [Tooltip("If this is checked then the pointer valid/invalid colours will also be used to change the colour of the play area cursor when colliding/not colliding.")]
        public bool usePointerColor = true;
        [Tooltip("Determines the size of the play area cursor and collider. If the values are left as zero then the Play Area Cursor will be sized to the calibrated Play Area space.")]
        public Vector2 playAreaCursorDimensions = Vector2.zero;
        [Tooltip("If this is checked then if the play area cursor is colliding with any other object then the pointer colour will change to the `Pointer Miss Color` and the `DestinationMarkerSet` event will not be triggered, which will prevent teleporting into areas where the play area will collide.")]
        public bool handlePlayAreaCursorCollisions = false;
        [Tooltip("If this is checked then if the user's headset is outside of the play area cursor bounds then it is considered a collision even if the play area isn't colliding with anything.")]
        public bool headsetOutOfBoundsIsCollision = false;
        [Tooltip("If this is checked then the play area cursor will be displayed when the location is invalid.")]
        public bool displayOnInvalidLocation = true;
        [Tooltip("A specified VRTK_PolicyList to use to determine whether the play area cursor collisions will be acted upon.")]
        public VRTK_PolicyList targetListPolicy;

        [Header("Custom Settings")]
        [Tooltip("A custom Pointer Direction Indicator to use to determine the rotation of the Play Area Cursor.")]
        public VRTK_PointerDirectionIndicator directionIndicator;
        [Tooltip("A custom GameObject to use for the play area cursor representation for when the location is valid.")]
        public GameObject validLocationObject;
        [Tooltip("A custom GameObject to use for the play area cursor representation for when the location is invalid.")]
        public GameObject invalidLocationObject;

        /// <summary>
        /// Emitted when the play area collides with another object.
        /// </summary>
        public event PlayAreaCursorEventHandler PlayAreaCursorStartCollision;
        /// <summary>
        /// Emitted when the play area stops colliding with another object.
        /// </summary>
        public event PlayAreaCursorEventHandler PlayAreaCursorEndCollision;

        protected bool headsetPositionCompensation;
        protected bool playAreaCursorCollided = false;
        protected bool headsetOutOfBounds = false;
        protected Transform playArea;
        protected GameObject playAreaCursor;
        protected GameObject[] playAreaCursorBoundaries;
        protected BoxCollider playAreaCursorCollider;
        protected Transform headset;
        protected Renderer[] boundaryRenderers = new Renderer[0];
        protected GameObject playAreaCursorValidChild;
        protected GameObject playAreaCursorInvalidChild;

        protected int btmRightInner = 0;
        protected int btmLeftInner = 1;
        protected int topLeftInner = 2;
        protected int topRightInner = 3;
        protected int btmRightOuter = 4;
        protected int btmLeftOuter = 5;
        protected int topLeftOuter = 6;
        protected int topRightOuter = 7;

        public virtual void OnPlayAreaCursorStartCollision(PlayAreaCursorEventArgs e)
        {
            if (PlayAreaCursorStartCollision != null)
            {
                PlayAreaCursorStartCollision(this, e);
            }
        }

        public virtual void OnPlayAreaCursorEndCollision(PlayAreaCursorEventArgs e)
        {
            if (PlayAreaCursorEndCollision != null)
            {
                PlayAreaCursorEndCollision(this, e);
            }
        }

        /// <summary>
        /// The HasCollided method returns the state of whether the play area cursor has currently collided with another valid object.
        /// </summary>
        /// <returns>Returns `true` if the play area is colliding with a valid object and `false` if not.</returns>
        public virtual bool HasCollided()
        {
            return (playAreaCursorCollided || headsetOutOfBounds);
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
        /// <param name="collider">The state of whether to check for play area collisions.</param>
        public virtual void SetPlayAreaCursorCollision(bool state, Collider collider = null)
        {
            playAreaCursorCollided = false;
            if (handlePlayAreaCursorCollisions)
            {
                playAreaCursorCollided = (!enabled ? false : state);
                EmitEvent(collider);
            }
        }

        /// <summary>
        /// The SetMaterialColor method sets the current material colour on the play area cursor.
        /// </summary>
        /// <param name="color">The colour to update the play area cursor material to.</param>
        /// <param name="validity">Determines if the colour being set is based from a valid location or invalid location.</param>
        public virtual void SetMaterialColor(Color color, bool validity)
        {
            if (validLocationObject == null)
            {
                ToggleVisibility(!(!validity && !displayOnInvalidLocation));

                if (usePointerColor)
                {
                    for (int i = 0; i < playAreaCursorBoundaries.Length; i++)
                    {
                        SetCursorColor(playAreaCursorBoundaries[i], color);
                    }
                }
            }
            else
            {
                ToggleValidPlayAreaState(!playAreaCursorCollided);
                if (usePointerColor)
                {
                    SetCursorColor(playAreaCursor, color);
                }
            }
        }

        /// <summary>
        /// The SetPlayAreaCursorTransform method is used to update the position of the play area cursor in world space to the given location.
        /// </summary>
        /// <param name="location">The location where to draw the play area cursor.</param>
        public virtual void SetPlayAreaCursorTransform(Vector3 location)
        {
            Vector3 offset = Vector3.zero;
            if (headsetPositionCompensation)
            {
                Vector3 playAreaPos = new Vector3(playArea.transform.position.x, 0f, playArea.transform.position.z);
                Vector3 headsetPos = new Vector3(headset.position.x, 0f, headset.position.z);
                offset = playAreaPos - headsetPos;
            }

            if (playAreaCursor != null)
            {
                if (playAreaCursor.activeInHierarchy && handlePlayAreaCursorCollisions && headsetOutOfBoundsIsCollision)
                {
                    Vector3 checkPoint = new Vector3(location.x, playAreaCursor.transform.position.y + (playAreaCursor.transform.localScale.y * 2), location.z);
                    if (!playAreaCursorCollider.bounds.Contains(checkPoint))
                    {
                        headsetOutOfBounds = true;
                    }
                    else
                    {
                        headsetOutOfBounds = false;
                    }
                }
                playAreaCursor.transform.rotation = (directionIndicator != null ? directionIndicator.transform.rotation : playArea.rotation);
                playAreaCursor.transform.position = location + offset;
            }
        }

        /// <summary>
        /// The ToggleState method enables or disables the visibility of the play area cursor.
        /// </summary>
        /// <param name="state">The state of whether to show or hide the play area cursor.</param>
        public virtual void ToggleState(bool state)
        {
            state = (!enabled ? false : state);
            if (playAreaCursor != null)
            {
                playAreaCursor.SetActive(state);
            }
        }

        /// <summary>
        /// The IsActive method returns whether the play area cursor GameObject is active or not.
        /// </summary>
        /// <returns>Returns `true` if the play area cursor GameObject is active.</returns>
        public virtual bool IsActive()
        {
            return (playAreaCursor != null ? playAreaCursor.activeInHierarchy : false);
        }

        /// <summary>
        /// The GetPlayAreaContainer method returns the created GameObject that holds the play area cursor representation.
        /// </summary>
        /// <returns>The GameObject that is the container of the play area cursor.</returns>
        public virtual GameObject GetPlayAreaContainer()
        {
            return playAreaCursor;
        }

        /// <summary>
        /// The ToggleVisibility method enables or disables the play area cursor renderers to allow the cursor to be seen or hidden.
        /// </summary>
        /// <param name="state">The state of the cursor visibility. True will show the renderers and false will hide the renderers.</param>
        public virtual void ToggleVisibility(bool state)
        {
            if (playAreaCursor != null && boundaryRenderers.Length == 0)
            {
                boundaryRenderers = playAreaCursor.GetComponentsInChildren<Renderer>();
            }

            for (int i = 0; i < boundaryRenderers.Length; i++)
            {
                if (boundaryRenderers[i] != null)
                {
                    boundaryRenderers[i].enabled = state;
                }
            }
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);

            headset = VRTK_DeviceFinder.HeadsetTransform();
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            playAreaCursorBoundaries = new GameObject[4];
            InitPlayAreaCursor();
        }

        protected virtual void OnDisable()
        {
            if (playAreaCursor != null)
            {
                Destroy(playAreaCursor);
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            if (enabled && IsActive())
            {
                UpdateCollider();
            }
        }

        protected virtual PlayAreaCursorEventArgs SetEventPayload(Collider collider)
        {
            PlayAreaCursorEventArgs e;
            e.collider = collider;
            return e;
        }

        protected virtual void EmitEvent(Collider collider)
        {
            if (collider != null)
            {
                if (playAreaCursorCollided)
                {
                    OnPlayAreaCursorStartCollision(SetEventPayload(collider));
                }
                else
                {
                    OnPlayAreaCursorEndCollision(SetEventPayload(collider));
                }
            }
        }

        protected virtual void InitPlayAreaCursor()
        {
            if (playArea == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
                return;
            }

            Vector3[] cursorDrawVertices = VRTK_SDK_Bridge.GetPlayAreaVertices();
            if (validLocationObject != null)
            {
                GeneratePlayAreaCursorFromPrefab(cursorDrawVertices);
            }
            else
            {
                if (cursorDrawVertices == null || cursorDrawVertices.Length < 8)
                {
                    cursorDrawVertices = new Vector3[] {
                        new Vector3(0.8f, 0f, -0.8f),
                        new Vector3(-0.8f, 0f, -0.8f),
                        new Vector3(-0.8f, 0f, 0.8f),
                        new Vector3(0.8f, 0f, 0.8f),
                        new Vector3(1f, 0f, -1f),
                        new Vector3(-1f, 0f, -1f),
                        new Vector3(-1f, 0f, 1f),
                        new Vector3(1f, 0f, 1f) };
                }
                GeneratePlayAreaCursor(cursorDrawVertices);
            }

            if (playAreaCursor != null)
            {
                playAreaCursor.SetActive(false);
                VRTK_PlayerObject.SetPlayerObject(playAreaCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
                CreateCursorCollider(playAreaCursor);
                playAreaCursor.AddComponent<Rigidbody>().isKinematic = true;

                VRTK_PlayAreaCollider playAreaCursorScript = playAreaCursor.AddComponent<VRTK_PlayAreaCollider>();
                playAreaCursorScript.SetParent(this);
                playAreaCursorScript.SetIgnoreTarget(targetListPolicy);
                playAreaCursor.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }

        protected virtual void SetCursorColor(GameObject cursorObject, Color color)
        {
            Renderer playareaRenderer = cursorObject.GetComponentInChildren<Renderer>();

            if (playareaRenderer != null && playareaRenderer.material && playareaRenderer.material.HasProperty("_Color"))
            {
                playareaRenderer.material.color = color;
                playareaRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                playareaRenderer.receiveShadows = false;
            }
        }


        protected virtual void ToggleValidPlayAreaState(bool state)
        {
            if (playAreaCursorValidChild != null)
            {
                playAreaCursorValidChild.SetActive(state);
            }
            if (playAreaCursorInvalidChild != null)
            {
                playAreaCursorInvalidChild.SetActive((displayOnInvalidLocation ? !state : false));
            }
        }

        protected virtual string GeneratePlayAreaCursorName()
        {
            return VRTK_SharedMethods.GenerateVRTKObjectName(true, gameObject.name, "PlayAreaCursor");
        }

        protected virtual void GeneratePlayAreaCursorFromPrefab(Vector3[] cursorDrawVertices)
        {
            playAreaCursor = new GameObject(GeneratePlayAreaCursorName());

            float width = cursorDrawVertices[btmRightOuter].x - cursorDrawVertices[topLeftOuter].x;
            float length = cursorDrawVertices[topLeftOuter].z - cursorDrawVertices[btmRightOuter].z;
            if (playAreaCursorDimensions != Vector2.zero)
            {
                width = (playAreaCursorDimensions.x == 0 ? playAreaCursor.transform.localScale.x : playAreaCursorDimensions.x);
                length = (playAreaCursorDimensions.y == 0 ? playAreaCursor.transform.localScale.z : playAreaCursorDimensions.y);
            }
            float height = 0.01f;

            playAreaCursorValidChild = Instantiate(validLocationObject);
            playAreaCursorValidChild.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "ValidArea");
            playAreaCursorValidChild.transform.SetParent(playAreaCursor.transform);

            if (invalidLocationObject != null)
            {
                playAreaCursorInvalidChild = Instantiate(invalidLocationObject);
                playAreaCursorInvalidChild.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, "InvalidArea");
                playAreaCursorInvalidChild.transform.SetParent(playAreaCursor.transform);
            }

            playAreaCursor.transform.localScale = new Vector3(width, height, length);
            playAreaCursorValidChild.transform.localScale = Vector3.one;
            if (invalidLocationObject != null)
            {
                playAreaCursorInvalidChild.transform.localScale = Vector3.one;
            }
            playAreaCursor.SetActive(false);
        }

        protected virtual void GeneratePlayAreaCursor(Vector3[] cursorDrawVertices)
        {
            if (playAreaCursorDimensions != Vector2.zero)
            {
                float customAreaPadding = VRTK_SDK_Bridge.GetPlayAreaBorderThickness();

                cursorDrawVertices[btmRightOuter] = new Vector3(playAreaCursorDimensions.x / 2, 0f, (playAreaCursorDimensions.y / 2) * -1);
                cursorDrawVertices[btmLeftOuter] = new Vector3((playAreaCursorDimensions.x / 2) * -1, 0f, (playAreaCursorDimensions.y / 2) * -1);
                cursorDrawVertices[topLeftOuter] = new Vector3((playAreaCursorDimensions.x / 2) * -1, 0f, playAreaCursorDimensions.y / 2);
                cursorDrawVertices[topRightOuter] = new Vector3(playAreaCursorDimensions.x / 2, 0f, playAreaCursorDimensions.y / 2);

                cursorDrawVertices[btmRightInner] = cursorDrawVertices[btmRightOuter] + new Vector3(-customAreaPadding, 0f, customAreaPadding);
                cursorDrawVertices[btmLeftInner] = cursorDrawVertices[btmLeftOuter] + new Vector3(customAreaPadding, 0f, customAreaPadding);
                cursorDrawVertices[topLeftInner] = cursorDrawVertices[topLeftOuter] + new Vector3(customAreaPadding, 0f, -customAreaPadding);
                cursorDrawVertices[topRightInner] = cursorDrawVertices[topRightOuter] + new Vector3(-customAreaPadding, 0f, -customAreaPadding);
            }

            float width = cursorDrawVertices[btmRightOuter].x - cursorDrawVertices[topLeftOuter].x;
            float length = cursorDrawVertices[topLeftOuter].z - cursorDrawVertices[btmRightOuter].z;
            float height = 0.01f;

            playAreaCursor = new GameObject(GeneratePlayAreaCursorName());
            playAreaCursor.transform.SetParent(null);
            playAreaCursor.transform.localScale = new Vector3(width, height, length);

            float playAreaBoundaryX = playArea.transform.localScale.x / 2;
            float playAreaBoundaryZ = playArea.transform.localScale.z / 2;
            float heightOffset = 0f;

            DrawPlayAreaCursorBoundary(0, cursorDrawVertices[btmLeftOuter].x, cursorDrawVertices[btmRightOuter].x, cursorDrawVertices[btmRightInner].z, cursorDrawVertices[btmRightOuter].z, height, new Vector3(0f, heightOffset, playAreaBoundaryZ));
            DrawPlayAreaCursorBoundary(1, cursorDrawVertices[btmLeftOuter].x, cursorDrawVertices[btmLeftInner].x, cursorDrawVertices[topLeftOuter].z, cursorDrawVertices[btmLeftOuter].z, height, new Vector3(playAreaBoundaryX, heightOffset, 0f));
            DrawPlayAreaCursorBoundary(2, cursorDrawVertices[btmLeftOuter].x, cursorDrawVertices[btmRightOuter].x, cursorDrawVertices[btmRightInner].z, cursorDrawVertices[btmRightOuter].z, height, new Vector3(0f, heightOffset, -playAreaBoundaryZ));
            DrawPlayAreaCursorBoundary(3, cursorDrawVertices[btmLeftOuter].x, cursorDrawVertices[btmLeftInner].x, cursorDrawVertices[topLeftOuter].z, cursorDrawVertices[btmLeftOuter].z, height, new Vector3(-playAreaBoundaryX, heightOffset, 0f));
        }

        protected virtual void DrawPlayAreaCursorBoundary(int index, float left, float right, float top, float bottom, float thickness, Vector3 localPosition)
        {
            GameObject playAreaCursorBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playAreaCursorBoundary.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, gameObject.name, "PlayAreaCursorBoundary", index);
            VRTK_PlayerObject.SetPlayerObject(playAreaCursorBoundary, VRTK_PlayerObject.ObjectTypes.Pointer);

            float width = (right - left) / 1.065f;
            float length = (top - bottom) / 1.08f;
            float height = thickness;

            playAreaCursorBoundary.transform.localScale = new Vector3(width, height, length);
            Destroy(playAreaCursorBoundary.GetComponent<BoxCollider>());
            playAreaCursorBoundary.layer = LayerMask.NameToLayer("Ignore Raycast");

            playAreaCursorBoundary.transform.SetParent(playAreaCursor.transform);
            playAreaCursorBoundary.transform.localPosition = localPosition;

            playAreaCursorBoundaries[index] = playAreaCursorBoundary;
        }

        protected virtual void CreateCursorCollider(GameObject cursor)
        {
            playAreaCursorCollider = cursor.AddComponent<BoxCollider>();
            playAreaCursorCollider.isTrigger = true;
            playAreaCursorCollider.center = new Vector3(0f, 65f, 0f);
            playAreaCursorCollider.size = new Vector3(1f, 1f, 1f);
        }

        protected virtual void UpdateCollider()
        {
            float playAreaHeightAdjustment = 1f;
            float newBCYSize = (headset.transform.position.y - playArea.transform.position.y) * 100f;
            float newBCYCenter = (newBCYSize != 0 ? (newBCYSize / 2) + playAreaHeightAdjustment : 0);

            playAreaCursorCollider.size = new Vector3(playAreaCursorCollider.size.x, newBCYSize, playAreaCursorCollider.size.z);
            playAreaCursorCollider.center = new Vector3(playAreaCursorCollider.center.x, newBCYCenter, playAreaCursorCollider.center.z);
        }
    }

    public class VRTK_PlayAreaCollider : MonoBehaviour
    {
        protected VRTK_PlayAreaCursor parent;
        protected VRTK_PolicyList targetListPolicy;

        public virtual void SetParent(VRTK_PlayAreaCursor setParent)
        {
            parent = setParent;
        }

        public virtual void SetIgnoreTarget(VRTK_PolicyList list = null)
        {
            targetListPolicy = list;
        }

        protected virtual void OnDisable()
        {
            if (parent != null)
            {
                parent.SetPlayAreaCursorCollision(false);
            }
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            if (parent != null && parent.enabled && parent.gameObject.activeInHierarchy && ValidTarget(collider))
            {
                parent.SetPlayAreaCursorCollision(true, collider);
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            if (parent != null && ValidTarget(collider))
            {
                parent.SetPlayAreaCursorCollision(false, collider);
            }
        }

        protected virtual bool ValidTarget(Collider collider)
        {
            return (!collider.isTrigger && !VRTK_PlayerObject.IsPlayerObject(collider.gameObject) && !(VRTK_PolicyList.Check(collider.gameObject, targetListPolicy)));
        }
    }
}