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
    using System.Collections;

    public delegate void TeleportEventHandler(object sender, DestinationMarkerEventArgs e);

    public class VRTK_BasicTeleport : MonoBehaviour
    {
        public float blinkTransitionSpeed = 0.6f;
        [Range(0f, 32f)]
        public float distanceBlinkDelay = 0f;
        public bool headsetPositionCompensation = true;
        public string ignoreTargetWithTagOrClass;

        public event TeleportEventHandler Teleporting;
        public event TeleportEventHandler Teleported;

        protected Transform eyeCamera;
        protected bool adjustYForTerrain = false;
        protected bool enableTeleport = true;

        private float blinkPause = 0f;
        private float fadeInTime = 0f;
        private float maxBlinkTransitionSpeed = 1.5f;
        private float maxBlinkDistance = 33f;

        protected void OnTeleporting(object sender, DestinationMarkerEventArgs e) {
            if (Teleporting != null)
                Teleporting(this, e);
        }

        protected void OnTeleported(object sender, DestinationMarkerEventArgs e) {
            if (Teleported != null)
                Teleported(this, e);
        }

        public void InitDestinationSetListener(GameObject markerMaker)
        {
            if (markerMaker)
            {
                var worldMarker = markerMaker.GetComponent<VRTK_DestinationMarker>();
                if (worldMarker)
                {
                    worldMarker.DestinationMarkerSet += new DestinationMarkerEventHandler(DoTeleport);
                    worldMarker.SetInvalidTarget(ignoreTargetWithTagOrClass);
                }
            }
        }

        protected virtual void Start()
        {
            this.name = "PlayerObject_" + this.name;
            adjustYForTerrain = false;
            eyeCamera = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();

            var controllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();
            InitDestinationSetListener(controllerManager.left);
            InitDestinationSetListener(controllerManager.right);
            InitHeadsetCollisionListener();

            enableTeleport = true;
        }

        protected virtual void Blink(float transitionSpeed)
        {
            fadeInTime = transitionSpeed;
            SteamVR_Fade.Start(Color.black, 0);
            Invoke("ReleaseBlink", blinkPause);
        }

        protected virtual bool ValidLocation(Transform target)
        {
            return (target && target.tag != ignoreTargetWithTagOrClass && target.GetComponent(ignoreTargetWithTagOrClass) == null);
        }

        protected virtual void DoTeleport(object sender, DestinationMarkerEventArgs e)
        {
            if (enableTeleport && ValidLocation(e.target) && e.enableTeleport)
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
            this.transform.position = CheckTerrainCollision(position, target);
        }

        protected virtual Vector3 GetNewPosition(Vector3 tipPosition, Transform target)
        {
            float newX = (headsetPositionCompensation ? (tipPosition.x - (eyeCamera.position.x - this.transform.position.x)) : tipPosition.x);
            float newY = this.transform.position.y;
            float newZ = (headsetPositionCompensation ? (tipPosition.z - (eyeCamera.position.z - this.transform.position.z)) : tipPosition.z);

            return new Vector3(newX, newY, newZ);
        }

        protected Vector3 CheckTerrainCollision(Vector3 position, Transform target)
        {
            if (adjustYForTerrain && target.GetComponent<Terrain>())
            {
                position.y = Terrain.activeTerrain.SampleHeight(position);
            }
            return position;
        }

        private void CalculateBlinkDelay(float blinkSpeed, Vector3 newPosition)
        {
            blinkPause = 0f;
            if (distanceBlinkDelay > 0f)
            {
                float distance = Vector3.Distance(this.transform.position, newPosition);
                blinkPause = Mathf.Clamp((distance * blinkTransitionSpeed) / (maxBlinkDistance - distanceBlinkDelay), 0, maxBlinkTransitionSpeed);
                blinkPause = (blinkSpeed <= 0.25 ? 0f : blinkPause);
            }
        }

        private void ReleaseBlink()
        {
            SteamVR_Fade.Start(Color.clear, fadeInTime);
            fadeInTime = 0f;
        }

        private void InitHeadsetCollisionListener()
        {
            var headset = GameObject.FindObjectOfType<VRTK_HeadsetCollisionFade>();
            if (headset)
            {
                headset.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(DisableTeleport);
                headset.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(EnableTeleport);
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