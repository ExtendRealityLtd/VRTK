// Basic Teleport|Locomotion|20010
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
#if UNITY_5_5_OR_NEWER
    using UnityEngine.AI;
#endif

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="DestinationMarkerEventArgs"/></param>
    public delegate void TeleportEventHandler(object sender, DestinationMarkerEventArgs e);

    /// <summary>
    /// Updates the `x/z` position of the SDK Camera Rig with an optional screen fade.
    /// </summary>
    /// <remarks>
    ///   > The `y` position is not altered by the Basic Teleport so it only allows for movement across a 2D plane.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_BasicTeleport` script on any active scene GameObject.
    ///
    /// **Script Dependencies:**
    ///  * An optional Destination Marker (such as a Pointer) to set the destination of the teleport location.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/004_CameraRig_BasicTeleport` uses the `VRTK_Pointer` script on the Controllers to initiate a laser pointer by pressing the `Touchpad` on the controller and when the laser pointer is deactivated (release the `Touchpad`) then the user is teleported to the location of the laser pointer tip as this is where the pointer destination marker position is set to.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_BasicTeleport")]
    public class VRTK_BasicTeleport : MonoBehaviour
    {
        [Header("Base Settings")]

        [Tooltip("The colour to fade to when fading on teleport.")]
        public Color blinkToColor = Color.black;
        [Tooltip("The time taken to fade to the `Blink To Color`. Setting the speed to `0` will mean no fade effect is present.")]
        public float blinkTransitionSpeed = 0.6f;
        [Tooltip("Determines how long the fade will stay present out depending on the distance being teleported. A value of `0` will not delay the teleport fade effect over any distance, a max value will delay the teleport fade in even when the distance teleported is very close to the original position.")]
        [Range(0f, 32f)]
        public float distanceBlinkDelay = 0f;
        [Tooltip("If this is checked then the teleported location will be the position of the headset within the play area. If it is unchecked then the teleported location will always be the centre of the play area even if the headset position is not in the centre of the play area.")]
        public bool headsetPositionCompensation = true;
        [Tooltip("A specified VRTK_PolicyList to use to determine whether destination targets will be acted upon by the teleporter.")]
        public VRTK_PolicyList targetListPolicy;
        [Tooltip("An optional NavMeshData object that will be utilised for limiting the teleport to within any scene NavMesh.")]
        public VRTK_NavMeshData navMeshData;

        [Header("Obsolete Settings")]

        [System.Obsolete("`VRTK_BasicTeleport.navMeshLimitDistance` is no longer used, use `VRTK_BasicTeleport.processNavMesh` instead. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public float navMeshLimitDistance = 0f;

        /// <summary>
        /// Emitted when the teleport process has begun.
        /// </summary>
        public event TeleportEventHandler Teleporting;
        /// <summary>
        /// Emitted when the teleport process has successfully completed.
        /// </summary>
        public event TeleportEventHandler Teleported;

        protected Transform headset;
        protected Transform playArea;
        protected bool adjustYForTerrain = false;
        protected bool enableTeleport = true;

        protected float blinkPause = 0f;
        protected float fadeInTime = 0f;
        protected float maxBlinkTransitionSpeed = 1.5f;
        protected float maxBlinkDistance = 33f;
        protected Coroutine initaliseListeners;
        protected bool useGivenForcedPosition = false;
        protected Vector3 givenForcedPosition = Vector3.zero;
        protected Quaternion? givenForcedRotation = null;

        /// <summary>
        /// The InitDestinationSetListener method is used to register the teleport script to listen to events from the given game object that is used to generate destination markers. Any destination set event emitted by a registered game object will initiate the teleport to the given destination location.
        /// </summary>
        /// <param name="markerMaker">The game object that is used to generate destination marker events, such as a controller.</param>
        /// <param name="register">Determines whether to register or unregister the listeners.</param>
        public virtual void InitDestinationSetListener(GameObject markerMaker, bool register)
        {
            if (markerMaker != null)
            {
                VRTK_DestinationMarker[] worldMarkers = markerMaker.GetComponentsInChildren<VRTK_DestinationMarker>();
                for (int i = 0; i < worldMarkers.Length; i++)
                {
                    VRTK_DestinationMarker worldMarker = worldMarkers[i];
                    if (register)
                    {
                        worldMarker.DestinationMarkerSet += new DestinationMarkerEventHandler(DoTeleport);
                        if (worldMarker.targetListPolicy == null)
                        {
                            worldMarker.targetListPolicy = targetListPolicy;
                        }
                        worldMarker.SetNavMeshData(navMeshData);
                        worldMarker.SetHeadsetPositionCompensation(headsetPositionCompensation);
                    }
                    else
                    {
                        worldMarker.DestinationMarkerSet -= new DestinationMarkerEventHandler(DoTeleport);
                    }
                }
            }
        }

        /// <summary>
        /// The ToggleTeleportEnabled method is used to determine whether the teleporter will initiate a teleport on a destination set event, if the state is true then the teleporter will work as normal, if the state is false then the teleporter will not be operational.
        /// </summary>
        /// <param name="state">Toggles whether the teleporter is enabled or disabled.</param>
        public virtual void ToggleTeleportEnabled(bool state)
        {
            enableTeleport = state;
        }

        /// <summary>
        /// The ValidLocation method determines if the given target is a location that can be teleported to
        /// </summary>
        /// <param name="target">The Transform that the destination marker is touching.</param>
        /// <param name="destinationPosition">The position in world space that is the destination.</param>
        /// <returns>Returns `true` if the target is a valid location.</returns>
        public virtual bool ValidLocation(Transform target, Vector3 destinationPosition)
        {
            //If the target is one of the player objects or a UI Canvas then it's never a valid location
            if (VRTK_PlayerObject.IsPlayerObject(target.gameObject) || target.GetComponent<VRTK_UIGraphicRaycaster>())
            {
                return false;
            }

            bool validNavMeshLocation = false;
            if (navMeshData != null)
            {
                if (target != null)
                {
                    NavMeshHit hit;
                    validNavMeshLocation = NavMesh.SamplePosition(destinationPosition, out hit, navMeshData.distanceLimit, navMeshData.validAreas);
                }
            }
            else
            {
                validNavMeshLocation = true;
            }

            return (validNavMeshLocation && target != null && !(VRTK_PolicyList.Check(target.gameObject, targetListPolicy)));
        }

        /// <summary>
        /// The Teleport/1 method calls the teleport to update position without needing to listen for a Destination Marker event.
        /// </summary>
        /// <param name="teleportArgs">The pseudo Destination Marker event for the teleport action.</param>
        public virtual void Teleport(DestinationMarkerEventArgs teleportArgs)
        {
            DoTeleport(this, teleportArgs);
        }

        /// <summary>
        /// The Teleport/4 method calls the teleport to update position without needing to listen for a Destination Marker event.
        ///  It will build a destination marker out of the provided parameters.
        /// </summary>
        /// <param name="target">The Transform of the destination object.</param>
        /// <param name="destinationPosition">The world position to teleport to.</param>
        /// <param name="destinationRotation">The world rotation to teleport to.</param>
        /// <param name="forceDestinationPosition">If `true` then the given destination position should not be altered by anything consuming the payload.</param>
        public virtual void Teleport(Transform target, Vector3 destinationPosition, Quaternion? destinationRotation = null, bool forceDestinationPosition = false)
        {
            DestinationMarkerEventArgs teleportArgs = BuildTeleportArgs(target, destinationPosition, destinationRotation, forceDestinationPosition);
            Teleport(teleportArgs);
        }

        /// <summary>
        /// The ForceTeleport method forces the position to update to a given destination and ignores any target checking or floor adjustment.
        /// </summary>
        /// <param name="destinationPosition">The world position to teleport to.</param>
        /// <param name="destinationRotation">The world rotation to teleport to.</param>
        public virtual void ForceTeleport(Vector3 destinationPosition, Quaternion? destinationRotation = null)
        {
            DestinationMarkerEventArgs teleportArgs = BuildTeleportArgs(null, destinationPosition, destinationRotation);
            StartTeleport(this, teleportArgs);
            Quaternion updatedRotation = SetNewRotation(destinationRotation);
            Vector3 finalDestination = GetCompensatedPosition(destinationPosition, destinationPosition);
            CalculateBlinkDelay(blinkTransitionSpeed, finalDestination);
            Blink(blinkTransitionSpeed);
            if (ValidRigObjects())
            {
                playArea.position = finalDestination;
            }
            ProcessOrientation(this, teleportArgs, finalDestination, updatedRotation);
            EndTeleport(this, teleportArgs);
        }

        /// <summary>
        /// The SetActualTeleportDestination method forces the destination of a teleport event to the given Vector3.
        /// </summary>
        /// <param name="actualPosition">The actual position that the teleport event should use as the final location.</param>
        /// <param name="actualRotation">The actual rotation that the teleport event should use as the final location.</param>
        public virtual void SetActualTeleportDestination(Vector3 actualPosition, Quaternion? actualRotation)
        {
            useGivenForcedPosition = true;
            givenForcedPosition = actualPosition;
            givenForcedRotation = actualRotation;
        }

        /// <summary>
        /// The ResetActualTeleportDestination method removes any previous forced destination position that was set by the SetActualTeleportDestination method.
        /// </summary>
        public virtual void ResetActualTeleportDestination()
        {
            useGivenForcedPosition = false;
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
            headset = VRTK_SharedMethods.AddCameraFade();
            playArea = VRTK_DeviceFinder.PlayAreaTransform();

            adjustYForTerrain = false;
            enableTeleport = true;
            initaliseListeners = StartCoroutine(InitListenersAtEndOfFrame());
            VRTK_ObjectCache.registeredTeleporters.Add(this);
        }

        protected virtual void OnDisable()
        {
            if (initaliseListeners != null)
            {
                StopCoroutine(initaliseListeners);
            }
            InitDestinationMarkerListeners(false);
            VRTK_ObjectCache.registeredTeleporters.Remove(this);
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Blink(float transitionSpeed)
        {
            fadeInTime = transitionSpeed;
            if (transitionSpeed > 0f)
            {
                VRTK_SDK_Bridge.HeadsetFade(blinkToColor, 0);
            }
            Invoke("ReleaseBlink", blinkPause);
        }

        protected virtual DestinationMarkerEventArgs BuildTeleportArgs(Transform target, Vector3 destinationPosition, Quaternion? destinationRotation = null, bool forceDestinationPosition = false)
        {
            DestinationMarkerEventArgs teleportArgs = new DestinationMarkerEventArgs();
            teleportArgs.distance = (ValidRigObjects() ? Vector3.Distance(new Vector3(headset.position.x, playArea.position.y, headset.position.z), destinationPosition) : 0f);
            teleportArgs.target = target;
            teleportArgs.raycastHit = new RaycastHit();
            teleportArgs.destinationPosition = destinationPosition;
            teleportArgs.destinationRotation = destinationRotation;
            teleportArgs.forceDestinationPosition = forceDestinationPosition;
            teleportArgs.enableTeleport = true;
            return teleportArgs;
        }

        protected virtual bool ValidRigObjects()
        {
            if (headset == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_BasicTeleport", "rig headset", ". Are you trying to access the headset before the SDK Manager has initialised it?"));
                return false;
            }
            if (playArea == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_BasicTeleport", "rig boundaries", ". Are you trying to access the boundaries before the SDK Manager has initialised it?"));
                return false;
            }
            return true;
        }

        protected virtual void DoTeleport(object sender, DestinationMarkerEventArgs e)
        {
            if (enableTeleport && ValidLocation(e.target, e.destinationPosition) && e.enableTeleport)
            {
                if (useGivenForcedPosition)
                {
                    e.destinationPosition = givenForcedPosition;
                    e.destinationRotation = (givenForcedRotation != null ? givenForcedRotation : e.destinationRotation);
                    ResetActualTeleportDestination();
                }
                StartTeleport(sender, e);
                Quaternion updatedRotation = SetNewRotation(e.destinationRotation);
                Vector3 newPosition = GetNewPosition(e.destinationPosition, e.target, e.forceDestinationPosition);
                CalculateBlinkDelay(blinkTransitionSpeed, newPosition);
                Blink(blinkTransitionSpeed);
                Vector3 updatedPosition = SetNewPosition(newPosition, e.target, e.forceDestinationPosition);
                ProcessOrientation(sender, e, updatedPosition, updatedRotation);
                EndTeleport(sender, e);
            }
        }

        protected virtual void StartTeleport(object sender, DestinationMarkerEventArgs e)
        {
            OnTeleporting(sender, e);
        }

        protected virtual void ProcessOrientation(object sender, DestinationMarkerEventArgs e, Vector3 targetPosition, Quaternion targetRotation)
        {
        }

        protected virtual void EndTeleport(object sender, DestinationMarkerEventArgs e)
        {
            OnTeleported(sender, e);
        }

        protected virtual Vector3 SetNewPosition(Vector3 position, Transform target, bool forceDestinationPosition)
        {
            if (ValidRigObjects())
            {
                playArea.position = CheckTerrainCollision(position, target, forceDestinationPosition);
                return playArea.position;
            }
            return Vector3.zero;
        }

        protected virtual Quaternion SetNewRotation(Quaternion? rotation)
        {
            if (ValidRigObjects())
            {
                if (rotation != null)
                {
                    playArea.rotation = (Quaternion)rotation;
                }
                return playArea.rotation;
            }
            return Quaternion.identity;
        }

        protected virtual Vector3 GetNewPosition(Vector3 tipPosition, Transform target, bool returnOriginalPosition)
        {
            if (returnOriginalPosition)
            {
                return tipPosition;
            }

            return GetCompensatedPosition(tipPosition, playArea.position);
        }

        protected virtual Vector3 GetCompensatedPosition(Vector3 givenPosition, Vector3 defaultPosition)
        {
            float newX = 0f;
            float newY = 0f;
            float newZ = 0f;

            if (ValidRigObjects())
            {
                newX = (headsetPositionCompensation ? (givenPosition.x - (headset.position.x - playArea.position.x)) : givenPosition.x);
                newY = defaultPosition.y;
                newZ = (headsetPositionCompensation ? (givenPosition.z - (headset.position.z - playArea.position.z)) : givenPosition.z);
            }

            return new Vector3(newX, newY, newZ);
        }

        protected virtual Vector3 CheckTerrainCollision(Vector3 position, Transform target, bool useHeadsetForPosition)
        {
            Terrain targetTerrain = target.GetComponent<Terrain>();
            if (adjustYForTerrain && targetTerrain != null)
            {
                Vector3 checkPosition = (useHeadsetForPosition && ValidRigObjects() ? new Vector3(headset.position.x, position.y, headset.position.z) : position);
                float terrainHeight = targetTerrain.SampleHeight(checkPosition);
                position.y = (terrainHeight > position.y ? position.y : targetTerrain.GetPosition().y + terrainHeight);
            }
            return position;
        }

        protected virtual void OnTeleporting(object sender, DestinationMarkerEventArgs e)
        {
            if (Teleporting != null)
            {
                Teleporting(this, e);
            }
        }

        protected virtual void OnTeleported(object sender, DestinationMarkerEventArgs e)
        {
            if (Teleported != null)
            {
                Teleported(this, e);
            }
        }

        protected virtual void CalculateBlinkDelay(float blinkSpeed, Vector3 newPosition)
        {
            blinkPause = 0f;
            if (distanceBlinkDelay > 0f)
            {
                float minBlink = 0.5f;
                float distance = (ValidRigObjects() ? Vector3.Distance(playArea.position, newPosition) : 0f);
                blinkPause = Mathf.Clamp((distance * blinkTransitionSpeed) / (maxBlinkDistance - distanceBlinkDelay), minBlink, maxBlinkTransitionSpeed);
                blinkPause = (blinkSpeed <= 0.25 ? minBlink : blinkPause);
            }
        }

        protected virtual void ReleaseBlink()
        {
            VRTK_SDK_Bridge.HeadsetFade(Color.clear, fadeInTime);
            fadeInTime = 0f;
        }

        protected virtual IEnumerator InitListenersAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            if (enabled)
            {
                InitDestinationMarkerListeners(true);
            }
        }

        protected virtual void InitDestinationMarkerListeners(bool state)
        {
            GameObject leftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            GameObject rightHand = VRTK_DeviceFinder.GetControllerRightHand();

            InitDestinationSetListener(leftHand, state);
            InitDestinationSetListener(rightHand, state);
            for (int i = 0; i < VRTK_ObjectCache.registeredDestinationMarkers.Count; i++)
            {
                VRTK_DestinationMarker destinationMarker = VRTK_ObjectCache.registeredDestinationMarkers[i];
                if (destinationMarker.gameObject != leftHand && destinationMarker.gameObject != rightHand)
                {
                    InitDestinationSetListener(destinationMarker.gameObject, state);
                }
            }
        }
    }
}