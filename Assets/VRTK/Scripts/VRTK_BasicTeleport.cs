// Basic Teleport|Scripts|0070
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="DestinationMarkerEventArgs"/></param>
    public delegate void TeleportEventHandler(object sender, DestinationMarkerEventArgs e);

    /// <summary>
    /// The basic teleporter updates the `[CameraRig]` x/z position in the game world to the position of a World Pointer's tip location which is set via the `WorldPointerDestinationSet` event. The y position is never altered so the basic teleporter cannot be used to move up and down game objects as it only allows for travel across a flat plane.
    /// </summary>
    /// <remarks>
    /// The Basic Teleport script is attached to the `[CameraRig]` prefab.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/004_CameraRig_BasicTeleport` uses the `VRTK_SimplePointer` script on the Controllers to initiate a laser pointer by pressing the `Touchpad` on the controller and when the laser pointer is deactivated (release the `Touchpad`) then the user is teleported to the location of the laser pointer tip as this is where the pointer destination marker position is set to.
    /// </example>
    public class VRTK_BasicTeleport : MonoBehaviour
    {
        [Tooltip("The fade blink speed can be changed on the basic teleport script to provide a customised teleport experience. Setting the speed to 0 will mean no fade blink effect is present.")]
        public float blinkTransitionSpeed = 0.6f;
        [Tooltip("A range between 0 and 32 that determines how long the blink transition will stay blacked out depending on the distance being teleported. A value of 0 will not delay the teleport blink effect over any distance, a value of 32 will delay the teleport blink fade in even when the distance teleported is very close to the original position. This can be used to simulate time taking longer to pass the further a user teleports. A value of 16 provides a decent basis to simulate this to the user.")]
        [Range(0f, 32f)]
        public float distanceBlinkDelay = 0f;
        [Tooltip("If this is checked then the teleported location will be the position of the headset within the play area. If it is unchecked then the teleported location will always be the centre of the play area even if the headset position is not in the centre of the play area.")]
        public bool headsetPositionCompensation = true;
        [Tooltip("A string that specifies an object Tag or the name of a Script attached to an object and notifies the teleporter that the destination is to be ignored so the user cannot teleport to that location. It also ensure the pointer colour is set to the miss colour.")]
        public string ignoreTargetWithTagOrClass;
        [Tooltip("A specified VRTK_TagOrScriptPolicyList to use to determine whether destination targets will be acted upon by the Teleporter. If a list is provided then the 'Ignore Target With Tag Or Class' parameter will be ignored.")]
        public VRTK_TagOrScriptPolicyList targetTagOrScriptListPolicy;
        [Tooltip("The max distance the nav mesh edge can be from the teleport destination to be considered valid. If a value of `0` is given then the nav mesh restriction will be ignored.")]
        public float navMeshLimitDistance = 0f;

        /// <summary>
        /// Emitted when the teleport process has begun.
        /// </summary>
        public event TeleportEventHandler Teleporting;
        /// <summary>
        /// Emitted when the teleport process has successfully completed.
        /// </summary>
        public event TeleportEventHandler Teleported;

        protected Transform eyeCamera;
        protected bool adjustYForTerrain = false;
        protected bool enableTeleport = true;

        private float blinkPause = 0f;
        private float fadeInTime = 0f;
        private float maxBlinkTransitionSpeed = 1.5f;
        private float maxBlinkDistance = 33f;

        /// <summary>
        /// The InitDestinationSetListener method is used to register the teleport script to listen to events from the given game object that is used to generate destination markers. Any destination set event emitted by a registered game object will initiate the teleport to the given destination location.
        /// </summary>
        /// <param name="markerMaker">The game object that is used to generate destination marker events, such as a controller.</param>
        /// <param name="register">Determines whether to register or unregister the listeners.</param>
        public void InitDestinationSetListener(GameObject markerMaker, bool register)
        {
            if (markerMaker)
            {
                foreach (var worldMarker in markerMaker.GetComponents<VRTK_DestinationMarker>())
                {
                    if (register)
                    {
                        worldMarker.DestinationMarkerSet += new DestinationMarkerEventHandler(DoTeleport);
                        worldMarker.SetInvalidTarget(ignoreTargetWithTagOrClass, targetTagOrScriptListPolicy);
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
        public void ToggleTeleportEnabled(bool state)
        {
            enableTeleport = state;
        }

        protected virtual void Awake()
        {
            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
            eyeCamera = Utilities.AddCameraFade();
        }

        protected virtual void OnEnable()
        {
            adjustYForTerrain = false;
            enableTeleport = true;
            StartCoroutine(InitListenersAtEndOfFrame());
            VRTK_ObjectCache.registeredTeleporters.Add(this);
        }

        protected virtual void OnDisable()
        {
            InitDestinationMarkerListeners(false);
            VRTK_ObjectCache.registeredTeleporters.Remove(this);
        }

        protected void OnTeleporting(object sender, DestinationMarkerEventArgs e)
        {
            if (Teleporting != null)
            {
                Teleporting(this, e);
            }
        }

        protected void OnTeleported(object sender, DestinationMarkerEventArgs e)
        {
            if (Teleported != null)
            {
                Teleported(this, e);
            }
        }

        protected virtual void Blink(float transitionSpeed)
        {
            fadeInTime = transitionSpeed;
            VRTK_SDK_Bridge.HeadsetFade(Color.black, 0);
            Invoke("ReleaseBlink", blinkPause);
        }

        protected virtual bool ValidLocation(Transform target, Vector3 destinationPosition)
        {
            //If the target is one of the player objects or a UI Canvas then it's never a valid location
            if (target.GetComponent<VRTK_PlayerObject>() || target.GetComponent<VRTK_UIGraphicRaycaster>())
            {
                return false;
            }

            bool validNavMeshLocation = false;
            if (target)
            {
                NavMeshHit hit;
                validNavMeshLocation = NavMesh.SamplePosition(destinationPosition, out hit, 0.1f, NavMesh.AllAreas);
            }
            if (navMeshLimitDistance == 0f)
            {
                validNavMeshLocation = true;
            }

            return (validNavMeshLocation && target && !(Utilities.TagOrScriptCheck(target.gameObject, targetTagOrScriptListPolicy, ignoreTargetWithTagOrClass)));
        }

        protected virtual void DoTeleport(object sender, DestinationMarkerEventArgs e)
        {
            if (enableTeleport && ValidLocation(e.target, e.destinationPosition) && e.enableTeleport)
            {
                OnTeleporting(sender, e);
                Vector3 newPosition = GetNewPosition(e.destinationPosition, e.target);
                CalculateBlinkDelay(blinkTransitionSpeed, newPosition);
                Blink(blinkTransitionSpeed);
                SetNewPosition(newPosition, e.target);
                OnTeleported(sender, e);
            }
        }

        protected virtual void SetNewPosition(Vector3 position, Transform target)
        {
            transform.position = CheckTerrainCollision(position, target);
        }

        protected virtual Vector3 GetNewPosition(Vector3 tipPosition, Transform target)
        {
            float newX = (headsetPositionCompensation ? (tipPosition.x - (eyeCamera.position.x - transform.position.x)) : tipPosition.x);
            float newY = transform.position.y;
            float newZ = (headsetPositionCompensation ? (tipPosition.z - (eyeCamera.position.z - transform.position.z)) : tipPosition.z);

            return new Vector3(newX, newY, newZ);
        }

        protected Vector3 CheckTerrainCollision(Vector3 position, Transform target)
        {
            if (adjustYForTerrain && target.GetComponent<Terrain>())
            {
                var terrainHeight = Terrain.activeTerrain.SampleHeight(position);
                position.y = (terrainHeight > position.y ? position.y : terrainHeight);
            }
            return position;
        }

        private void CalculateBlinkDelay(float blinkSpeed, Vector3 newPosition)
        {
            blinkPause = 0f;
            if (distanceBlinkDelay > 0f)
            {
                float distance = Vector3.Distance(transform.position, newPosition);
                blinkPause = Mathf.Clamp((distance * blinkTransitionSpeed) / (maxBlinkDistance - distanceBlinkDelay), 0, maxBlinkTransitionSpeed);
                blinkPause = (blinkSpeed <= 0.25 ? 0f : blinkPause);
            }
        }

        private void ReleaseBlink()
        {
            VRTK_SDK_Bridge.HeadsetFade(Color.clear, fadeInTime);
            fadeInTime = 0f;
        }

        private IEnumerator InitListenersAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            InitDestinationMarkerListeners(true);
        }

        private void InitDestinationMarkerListeners(bool state)
        {
            var leftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            var rightHand = VRTK_DeviceFinder.GetControllerRightHand();
            InitDestinationSetListener(leftHand, state);
            InitDestinationSetListener(rightHand, state);
            foreach (var destinationMarker in VRTK_ObjectCache.registeredDestinationMarkers)
            {
                if (destinationMarker.gameObject != leftHand && destinationMarker.gameObject != rightHand)
                {
                    InitDestinationSetListener(destinationMarker.gameObject, state);
                }
            }
        }
    }
}