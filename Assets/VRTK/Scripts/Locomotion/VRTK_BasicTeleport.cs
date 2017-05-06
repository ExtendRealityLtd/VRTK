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
    /// The basic teleporter updates the user's x/z position in the game world to the position of a Base Pointer's tip location which is set via the `DestinationMarkerSet` event.
    /// </summary>
    /// <remarks>
    /// The y position is never altered so the basic teleporter cannot be used to move up and down game objects as it only allows for travel across a flat plane.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/004_CameraRig_BasicTeleport` uses the `VRTK_SimplePointer` script on the Controllers to initiate a laser pointer by pressing the `Touchpad` on the controller and when the laser pointer is deactivated (release the `Touchpad`) then the user is teleported to the location of the laser pointer tip as this is where the pointer destination marker position is set to.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_BasicTeleport")]
    public class VRTK_BasicTeleport : MonoBehaviour
    {
        [Header("Base Settings")]

        [Tooltip("The colour to fade to when blinking on teleport.")]
        public Color blinkToColor = Color.black;
        [Tooltip("The fade blink speed can be changed on the basic teleport script to provide a customised teleport experience. Setting the speed to 0 will mean no fade blink effect is present.")]
        public float blinkTransitionSpeed = 0.6f;
        [Tooltip("A range between 0 and 32 that determines how long the blink transition will stay blacked out depending on the distance being teleported. A value of 0 will not delay the teleport blink effect over any distance, a value of 32 will delay the teleport blink fade in even when the distance teleported is very close to the original position. This can be used to simulate time taking longer to pass the further a user teleports. A value of 16 provides a decent basis to simulate this to the user.")]
        [Range(0f, 32f)]
        public float distanceBlinkDelay = 0f;
        [Tooltip("If this is checked then the teleported location will be the position of the headset within the play area. If it is unchecked then the teleported location will always be the centre of the play area even if the headset position is not in the centre of the play area.")]
        public bool headsetPositionCompensation = true;
        [Tooltip("A specified VRTK_PolicyList to use to determine whether destination targets will be acted upon by the Teleporter.")]
        public VRTK_PolicyList targetListPolicy;
        [Tooltip("The max distance the teleport destination can be outside the nav mesh to be considered valid. If a value of `0` is given then the nav mesh restrictions will be ignored.")]
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

        /// <summary>
        /// The InitDestinationSetListener method is used to register the teleport script to listen to events from the given game object that is used to generate destination markers. Any destination set event emitted by a registered game object will initiate the teleport to the given destination location.
        /// </summary>
        /// <param name="markerMaker">The game object that is used to generate destination marker events, such as a controller.</param>
        /// <param name="register">Determines whether to register or unregister the listeners.</param>
        public virtual void InitDestinationSetListener(GameObject markerMaker, bool register)
        {
            if (markerMaker)
            {
                VRTK_DestinationMarker[] worldMarkers = markerMaker.GetComponentsInChildren<VRTK_DestinationMarker>();
                for (int i = 0; i < worldMarkers.Length; i++)
                {
                    VRTK_DestinationMarker worldMarker = worldMarkers[i];
                    if (register)
                    {
                        worldMarker.DestinationMarkerSet += new DestinationMarkerEventHandler(DoTeleport);
                        worldMarker.SetInvalidTarget(targetListPolicy);
                        worldMarker.SetNavMeshCheckDistance(navMeshLimitDistance);
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
        /// <returns>Returns true if the target is a valid location.</returns>
        public virtual bool ValidLocation(Transform target, Vector3 destinationPosition)
        {
            //If the target is one of the player objects or a UI Canvas then it's never a valid location
            if (VRTK_PlayerObject.IsPlayerObject(target.gameObject) || target.GetComponent<VRTK_UIGraphicRaycaster>())
            {
                return false;
            }

            bool validNavMeshLocation = false;
            if (target)
            {
                NavMeshHit hit;
                validNavMeshLocation = NavMesh.SamplePosition(destinationPosition, out hit, navMeshLimitDistance, NavMesh.AllAreas);
            }
            if (navMeshLimitDistance == 0f)
            {
                validNavMeshLocation = true;
            }

            return (validNavMeshLocation && target && !(VRTK_PolicyList.Check(target.gameObject, targetListPolicy)));
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

        protected virtual void Blink(float transitionSpeed)
        {
            fadeInTime = transitionSpeed;
            if (transitionSpeed > 0f)
            {
                VRTK_SDK_Bridge.HeadsetFade(blinkToColor, 0);
            }
            Invoke("ReleaseBlink", blinkPause);
        }

        protected virtual void DoTeleport(object sender, DestinationMarkerEventArgs e)
        {
            if (enableTeleport && ValidLocation(e.target, e.destinationPosition) && e.enableTeleport)
            {
                StartTeleport(sender, e);
                Vector3 newPosition = GetNewPosition(e.destinationPosition, e.target, e.forceDestinationPosition);
                CalculateBlinkDelay(blinkTransitionSpeed, newPosition);
                Blink(blinkTransitionSpeed);
                Vector3 updatedPosition = SetNewPosition(newPosition, e.target, e.forceDestinationPosition);
                Quaternion updatedRotation = SetNewRotation(e.destinationRotation);
                ProcessOrientation(sender, e, updatedPosition, updatedRotation);
                EndTeleport(sender, e);
            }
        }

        protected virtual void StartTeleport(object sender, DestinationMarkerEventArgs e)
        {
            OnTeleporting(sender, e);
        }

        protected virtual void ProcessOrientation(object sender, DestinationMarkerEventArgs e, Vector3 updatedPosition, Quaternion updatedRotation)
        {
        }

        protected virtual void EndTeleport(object sender, DestinationMarkerEventArgs e)
        {
            OnTeleported(sender, e);
        }

        protected virtual Vector3 SetNewPosition(Vector3 position, Transform target, bool forceDestinationPosition)
        {
            playArea.position = CheckTerrainCollision(position, target, forceDestinationPosition);
            return playArea.position;
        }

        protected virtual Quaternion SetNewRotation(Quaternion? rotation)
        {
            if (rotation != null)
            {
                playArea.rotation = (Quaternion)rotation;
            }
            return playArea.rotation;
        }

        protected virtual Vector3 GetNewPosition(Vector3 tipPosition, Transform target, bool returnOriginalPosition)
        {
            if (returnOriginalPosition)
            {
                return tipPosition;
            }

            float newX = (headsetPositionCompensation ? (tipPosition.x - (headset.position.x - playArea.position.x)) : tipPosition.x);
            float newY = playArea.position.y;
            float newZ = (headsetPositionCompensation ? (tipPosition.z - (headset.position.z - playArea.position.z)) : tipPosition.z);

            return new Vector3(newX, newY, newZ);
        }

        protected virtual Vector3 CheckTerrainCollision(Vector3 position, Transform target, bool useHeadsetForPosition)
        {
            Terrain targetTerrain = target.GetComponent<Terrain>();
            if (adjustYForTerrain && targetTerrain != null)
            {
                Vector3 checkPosition = (useHeadsetForPosition ? new Vector3(headset.position.x, position.y, headset.position.z) : position);
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
                float distance = Vector3.Distance(playArea.position, newPosition);
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
            foreach (VRTK_DestinationMarker destinationMarker in VRTK_ObjectCache.registeredDestinationMarkers)
            {
                if (destinationMarker.gameObject != leftHand && destinationMarker.gameObject != rightHand)
                {
                    InitDestinationSetListener(destinationMarker.gameObject, state);
                }
            }
        }
    }
}