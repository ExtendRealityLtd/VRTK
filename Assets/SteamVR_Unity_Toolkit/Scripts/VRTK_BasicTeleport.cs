//====================================================================================
//
// Purpose: Provide basic teleportation of VR CameraRig
//
// This script must be attached to the [CameraRig] Prefab
//
// A GameObject must have the VRTK_WorldPointer attached to it to listen for the
// updated world position to teleport to.
//
//====================================================================================

namespace VRTK
{
    using UnityEngine;

    public delegate void TeleportEventHandler(object sender, DestinationMarkerEventArgs e);

    public class VRTK_BasicTeleport : MonoBehaviour
    {
        public float blinkTransitionSpeed = 0.6f;
        [Range(0f, 32f)]
        public float distanceBlinkDelay = 0f;
        public bool headsetPositionCompensation = true;
        public string ignoreTargetWithTagOrClass;
        [Tooltip("The max distance the nav mesh edge can be from the teleport destination to be considered valid.\n[0 = ignore nav mesh limits]")]
        public float navMeshLimitDistance = 0f;

        public event TeleportEventHandler Teleporting;
        public event TeleportEventHandler Teleported;

        protected Transform eyeCamera;
        protected bool adjustYForTerrain = false;
        protected bool enableTeleport = true;

        private float blinkPause = 0f;
        private float fadeInTime = 0f;
        private float maxBlinkTransitionSpeed = 1.5f;
        private float maxBlinkDistance = 33f;
        private SteamVR_ControllerManager controllerManager;

        public void InitDestinationSetListener(GameObject markerMaker, bool register)
        {
            if (markerMaker)
            {
                foreach (var worldMarker in markerMaker.GetComponents<VRTK_DestinationMarker>())
                {
                    if (register)
                    {
                        worldMarker.DestinationMarkerSet += new DestinationMarkerEventHandler(DoTeleport);
                        worldMarker.SetInvalidTarget(ignoreTargetWithTagOrClass);
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

        protected virtual void Awake()
        {
            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
            eyeCamera = Utilities.AddCameraFade();
            controllerManager = FindObjectOfType<SteamVR_ControllerManager>();
        }

        protected virtual void OnEnable()
        {
            adjustYForTerrain = false;
            enableTeleport = true;
            InitDestinationMarkerListeners(true);
            InitHeadsetCollisionListener(true);
        }

        protected virtual void OnDisable()
        {
            InitDestinationMarkerListeners(false);
            InitHeadsetCollisionListener(false);
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
            SteamVR_Fade.Start(Color.black, 0);
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

            return (validNavMeshLocation && target && target.tag != ignoreTargetWithTagOrClass && target.GetComponent(ignoreTargetWithTagOrClass) == null);
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
            SteamVR_Fade.Start(Color.clear, fadeInTime);
            fadeInTime = 0f;
        }

        private void InitDestinationMarkerListeners(bool state)
        {
            if (controllerManager)
            {
                InitDestinationSetListener(controllerManager.left, state);
                InitDestinationSetListener(controllerManager.right, state);
            }

            foreach (var destinationMarker in FindObjectsOfType<VRTK_DestinationMarker>())
            {
                if (destinationMarker.gameObject != controllerManager.left && destinationMarker.gameObject != controllerManager.right)
                {
                    InitDestinationSetListener(destinationMarker.gameObject, state);
                }
            }
        }

        private void InitHeadsetCollisionListener(bool state)
        {
            var headset = FindObjectOfType<VRTK_HeadsetCollisionFade>();
            if (headset)
            {
                if (state)
                {
                    headset.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(DisableTeleport);
                    headset.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(EnableTeleport);
                }
                else
                {
                    headset.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(DisableTeleport);
                    headset.HeadsetCollisionEnded -= new HeadsetCollisionEventHandler(EnableTeleport);
                }
            }
        }

        private void DisableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            enableTeleport = false;
        }

        private void EnableTeleport(object sender, HeadsetCollisionEventArgs e)
        {
            enableTeleport = true;
        }
    }
}