// Destination Point|Prefabs|0055
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// The Destination Point allows for a specific scene marker that can be teleported to.
    /// </summary>
    /// <remarks>
    /// The destination points can provide a useful way of having specific teleport locations in a scene.
    ///
    /// The destination points can also have a locked state if the `Enable Teleport` flag is disabled.
    /// </remarks>
    /// <example>
    /// `044_CameraRig_RestrictedTeleportZones` uses the `VRTK_DestinationPoint` prefab to set up a collection of pre-defined teleport locations.
    /// </example>
    public class VRTK_DestinationPoint : VRTK_DestinationMarker
    {
        /// <summary>
        /// Allowed snap to rotation types.
        /// </summary>
        /// <param name="NoRotation">No rotation information will be emitted in the destination set payload.</param>
        /// <param name="RotateWithNoHeadsetOffset">The destination point's rotation will be emitted without taking into consideration the current headset rotation.</param>
        /// <param name="RotateWithHeadsetOffset">The destination point's rotation will be emitted and will take into consideration the current headset rotation.</param>
        public enum RotationTypes
        {
            NoRotation,
            RotateWithNoHeadsetOffset,
            RotateWithHeadsetOffset
        }

        [Header("Destination Point Settings")]

        [Tooltip("The GameObject to use to represent the default cursor state.")]
        public GameObject defaultCursorObject;
        [Tooltip("The GameObject to use to represent the hover cursor state.")]
        public GameObject hoverCursorObject;
        [Tooltip("The GameObject to use to represent the locked cursor state.")]
        public GameObject lockedCursorObject;
        [Tooltip("An optional transform to determine the destination location for the destination marker. This can be useful to offset the destination location from the destination point. If this is left empty then the destiantion point transform will be used.")]
        public Transform destinationLocation;
        [Tooltip("If this is checked then after teleporting, the play area will be snapped to the origin of the destination point. If this is false then it's possible to teleport to anywhere within the destination point collider.")]
        public bool snapToPoint = true;
        [Tooltip("If this is checked, then the pointer cursor will be hidden when a valid destination point is hovered over.")]
        public bool hidePointerCursorOnHover = true;
        [Tooltip("Determines if the play area will be rotated to the rotation of the destination point upon the destination marker being set.")]
        public RotationTypes snapToRotation = RotationTypes.NoRotation;

        public static VRTK_DestinationPoint currentDestinationPoint;

        protected Collider pointCollider;
        protected bool createdCollider;
        protected Rigidbody pointRigidbody;
        protected bool createdRigidbody;
        protected Coroutine initaliseListeners;
        protected bool isActive;
        protected VRTK_BasePointerRenderer.VisibilityStates storedCursorState;
        protected Coroutine setDestination;
        protected bool currentTeleportState;
        protected Transform playArea;
        protected Transform headset;

        /// <summary>
        /// The ResetDestinationPoint resets the destination point back to the default state.
        /// </summary>
        public virtual void ResetDestinationPoint()
        {
            ResetPoint();
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CreateColliderIfRequired();
            SetupRigidbody();
            initaliseListeners = StartCoroutine(ManageDestinationMarkersAtEndOfFrame());
            ResetPoint();
            currentTeleportState = enableTeleport;
            currentDestinationPoint = null;
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            headset = VRTK_DeviceFinder.HeadsetTransform();
            destinationLocation = (destinationLocation != null ? destinationLocation : transform);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (initaliseListeners != null)
            {
                StopCoroutine(initaliseListeners);
            }

            if (setDestination != null)
            {
                StopCoroutine(setDestination);
            }
            ManageDestinationMarkers(false);
            if (createdCollider)
            {
                Destroy(pointCollider);
            }

            if (createdRigidbody)
            {
                Destroy(pointRigidbody);
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            if (enableTeleport != currentTeleportState)
            {
                ResetPoint();
            }
            currentTeleportState = enableTeleport;
        }

        protected virtual void CreateColliderIfRequired()
        {
            pointCollider = GetComponentInChildren<Collider>();
            createdCollider = false;
            if (pointCollider == null)
            {
                pointCollider = gameObject.AddComponent<SphereCollider>();
                createdCollider = true;
            }

            pointCollider.isTrigger = true;
        }

        protected virtual void SetupRigidbody()
        {
            pointRigidbody = GetComponent<Rigidbody>();
            createdRigidbody = false;
            if (pointRigidbody == null)
            {
                pointRigidbody = gameObject.AddComponent<Rigidbody>();
                createdRigidbody = true;
            }
            pointRigidbody.isKinematic = true;
            pointRigidbody.useGravity = false;
        }

        protected virtual IEnumerator ManageDestinationMarkersAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            if (enabled)
            {
                ManageDestinationMarkers(true);
            }
        }

        protected virtual void ManageDestinationMarkers(bool state)
        {
            ManageDestinationMarkerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), state);
            ManageDestinationMarkerListeners(VRTK_DeviceFinder.GetControllerRightHand(), state);

            foreach (var destinationMarker in VRTK_ObjectCache.registeredDestinationMarkers)
            {
                ManageDestinationMarkerListeners(destinationMarker.gameObject, state);
            }
        }

        protected virtual void ManageDestinationMarkerListeners(GameObject markerMaker, bool register)
        {
            if (markerMaker != null)
            {
                VRTK_DestinationMarker[] worldMarkers = markerMaker.GetComponentsInChildren<VRTK_DestinationMarker>();
                for (int i = 0; i < worldMarkers.Length; i++)
                {
                    VRTK_DestinationMarker worldMarker = worldMarkers[i];
                    if (worldMarker == this)
                    {
                        continue;
                    }
                    if (register)
                    {
                        worldMarker.DestinationMarkerEnter += DoDestinationMarkerEnter;
                        worldMarker.DestinationMarkerExit += DoDestinationMarkerExit;
                        worldMarker.DestinationMarkerSet += DoDestinationMarkerSet;
                    }
                    else
                    {
                        worldMarker.DestinationMarkerEnter -= DoDestinationMarkerEnter;
                        worldMarker.DestinationMarkerExit -= DoDestinationMarkerExit;
                        worldMarker.DestinationMarkerSet -= DoDestinationMarkerSet;
                    }
                }
            }
        }

        protected virtual void DoDestinationMarkerEnter(object sender, DestinationMarkerEventArgs e)
        {
            if (!isActive && e.raycastHit.transform == transform)
            {
                isActive = true;
                ToggleCursor(sender, false);
                EnablePoint();
                OnDestinationMarkerEnter(SetDestinationMarkerEvent(0f, e.raycastHit.transform, e.raycastHit, e.raycastHit.transform.position, e.controllerReference, false, GetRotation()));
            }
        }

        protected virtual void DoDestinationMarkerExit(object sender, DestinationMarkerEventArgs e)
        {
            if (isActive && e.raycastHit.transform == transform)
            {
                isActive = false;
                ToggleCursor(sender, true);
                ResetPoint();
                OnDestinationMarkerExit(SetDestinationMarkerEvent(0f, e.raycastHit.transform, e.raycastHit, e.raycastHit.transform.position, e.controllerReference, false, GetRotation()));
            }
        }

        protected virtual void DoDestinationMarkerSet(object sender, DestinationMarkerEventArgs e)
        {
            if (e.raycastHit.transform == transform)
            {
                currentDestinationPoint = this;
                if (snapToPoint)
                {
                    e.raycastHit.point = destinationLocation.position;
                    setDestination = StartCoroutine(DoDestinationMarkerSetAtEndOfFrame(e));
                }
            }
            else if (currentDestinationPoint != this)
            {
                ResetPoint();
            }
        }

        protected virtual IEnumerator DoDestinationMarkerSetAtEndOfFrame(DestinationMarkerEventArgs e)
        {
            yield return new WaitForEndOfFrame();
            if (enabled)
            {
                e.raycastHit.point = destinationLocation.position;
                DisablePoint();
                OnDestinationMarkerSet(SetDestinationMarkerEvent(e.distance, transform, e.raycastHit, destinationLocation.position, e.controllerReference, false, GetRotation()));
            }
        }

        protected virtual void ToggleCursor(object sender, bool state)
        {
            if (hidePointerCursorOnHover && sender.GetType().Equals(typeof(VRTK_Pointer)))
            {
                VRTK_Pointer pointer = (VRTK_Pointer)sender;

                if (!state)
                {
                    storedCursorState = pointer.pointerRenderer.cursorVisibility;
                    pointer.pointerRenderer.cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
                }
                else
                {
                    pointer.pointerRenderer.cursorVisibility = storedCursorState;
                }
            }
        }

        protected virtual void EnablePoint()
        {
            ToggleObject(lockedCursorObject, false);
            ToggleObject(defaultCursorObject, false);
            ToggleObject(hoverCursorObject, true);
        }

        protected virtual void DisablePoint()
        {
            pointCollider.enabled = false;
            ToggleObject(lockedCursorObject, false);
            ToggleObject(defaultCursorObject, false);
            ToggleObject(hoverCursorObject, false);
        }

        protected virtual void ResetPoint()
        {
            currentDestinationPoint = null;
            ToggleObject(hoverCursorObject, false);
            if (enableTeleport)
            {
                pointCollider.enabled = true;
                ToggleObject(defaultCursorObject, true);
                ToggleObject(lockedCursorObject, false);
            }
            else
            {
                pointCollider.enabled = false;
                ToggleObject(lockedCursorObject, true);
                ToggleObject(defaultCursorObject, false);
            }
        }

        protected virtual void ToggleObject(GameObject givenObject, bool state)
        {
            if (givenObject != null)
            {
                givenObject.SetActive(state);
            }
        }

        protected virtual Quaternion? GetRotation()
        {
            if (snapToRotation == RotationTypes.NoRotation)
            {
                return null;
            }

            float offset = (snapToRotation == RotationTypes.RotateWithHeadsetOffset && playArea != null && headset != null ? playArea.eulerAngles.y - headset.eulerAngles.y : 0f);
            return Quaternion.Euler(0f, destinationLocation.localEulerAngles.y + offset, 0f);
        }
    }
}