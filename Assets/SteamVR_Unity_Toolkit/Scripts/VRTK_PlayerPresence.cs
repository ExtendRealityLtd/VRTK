﻿namespace VRTK
{
    using UnityEngine;

    public struct PlayerPresenceEventArgs
    {
        public float fallDistance;
    }

    public delegate void PlayerPresenceEventHandler(object sender, PlayerPresenceEventArgs e);

    public class VRTK_PlayerPresence : MonoBehaviour
    {
        public event PlayerPresenceEventHandler PresenceFallStarted;
        public event PlayerPresenceEventHandler PresenceFallEnded;

        public float headsetYOffset = 0.2f;
        public bool ignoreGrabbedCollisions = true;
        public bool resetPositionOnCollision = true;
        public bool fallingPhysicsOnly = false;

        private Transform headset;
        private Rigidbody rb;
        private BoxCollider bc;
        private Vector3 lastGoodPosition;
        private bool lastGoodPositionSet = false;
        private float highestHeadsetY = 0f;
        private float crouchMargin = 0.5f;
        private float lastPlayAreaY = 0f;

        private float fallStartHeight = 0.0f;

        private bool isFalling = false;

        public void SetFallingPhysicsOnlyParams(bool falling)
        {
            fallingPhysicsOnly = falling;

            if (fallingPhysicsOnly)
            {
                DisablePhysics();
            }
            else
            {
                EnablePhysics();
            }
        }

        public bool IsFalling()
        {
            return fallingPhysicsOnly && isFalling;
        }

        public Transform GetHeadset()
        {
            return headset;
        }

        public void StartPhysicsFall(Vector3 velocity)
        {
            if (!isFalling && fallingPhysicsOnly)
            {
                OnPresenceFallStarted(SetPlayerPhysicsEvent(0));

                isFalling = true;
                EnablePhysics();
                rb.velocity = velocity + new Vector3(0.0f, -0.001f, 0.0f);
                fallStartHeight = transform.position.y;
            }
        }

        public void StopPhysicsFall()
        {
            if (!fallingPhysicsOnly)
            {
                return;
            }

            float fallHeight = fallStartHeight - transform.position.y;
            OnPresenceFallEnded(SetPlayerPhysicsEvent(fallHeight));

            isFalling = false;
            DisablePhysics();
        }

        private void OnPresenceFallStarted(PlayerPresenceEventArgs e)
        {
            if (PresenceFallStarted != null)
            {
                PresenceFallStarted(this, e);
            }
        }

        private void OnPresenceFallEnded(PlayerPresenceEventArgs e)
        {
            if (PresenceFallEnded != null)
            {
                PresenceFallEnded(this, e);
            }
        }

        private PlayerPresenceEventArgs SetPlayerPhysicsEvent(float fallDistance)
        {
            PlayerPresenceEventArgs e;
            e.fallDistance = fallDistance;
            return e;
        }

        private void Awake()
        {
            CreateCollider();
        }

        private void Start()
        {
            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);

            lastGoodPositionSet = false;
            headset = VRTK_DeviceFinder.HeadsetTransform();
            InitHeadsetListeners();

            var controllerManager = FindObjectOfType<SteamVR_ControllerManager>();
            InitControllerListeners(controllerManager.left);
            InitControllerListeners(controllerManager.right);
        }

        private void InitHeadsetListeners()
        {
            if (headset.GetComponent<VRTK_HeadsetCollisionFade>())
            {
                headset.GetComponent<VRTK_HeadsetCollisionFade>().HeadsetCollisionDetect += new HeadsetCollisionEventHandler(OnHeadsetCollision);
            }
        }

        private void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.target.GetComponent<Collider>())
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), e.target.GetComponent<Collider>(), true);
            }

            foreach (var childCollider in e.target.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), childCollider, true);
            }
        }

        private void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.target.GetComponent<VRTK_InteractableObject>() && !e.target.GetComponent<VRTK_InteractableObject>().IsGrabbed())
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), e.target.GetComponent<Collider>(), false);
            }
        }

        private void OnHeadsetCollision(object sender, HeadsetCollisionEventArgs e)
        {
            if (resetPositionOnCollision && lastGoodPositionSet)
            {
                SteamVR_Fade.Start(Color.black, 0f);
                transform.position = lastGoodPosition;
            }
        }

        private void CreateCollider()
        {
            rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.mass = 100;
            rb.freezeRotation = true;

            bc = gameObject.GetComponent<BoxCollider>();
            if (bc == null)
            {
                bc = gameObject.AddComponent<BoxCollider>();
            }
            bc.center = new Vector3(0f, 1f, 0f);
            bc.size = new Vector3(0.25f, 1f, 0.25f);
            
            if (fallingPhysicsOnly)
            {
                DisablePhysics();
            }

            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        private void EnablePhysics()
        {
            rb.isKinematic = false;
            bc.isTrigger = false;
        }

        private void DisablePhysics()
        {
            rb.isKinematic = true;
            bc.isTrigger = true;
        }

        private void UpdateCollider()
        {
            var playAreaHeightAdjustment = 0.009f;
            var newBCYSize = (headset.transform.position.y - headsetYOffset) - transform.position.y;
            var newBCYCenter = (newBCYSize != 0 ? (newBCYSize / 2) + playAreaHeightAdjustment : 0);

            bc.size = new Vector3(bc.size.x, newBCYSize, bc.size.z);
            bc.center = new Vector3(headset.localPosition.x, newBCYCenter, headset.localPosition.z);
        }

        private void SetHeadsetY()
        {
            //if the play area height has changed then always recalc headset height
            var floorVariant = 0.005f;
            if (transform.position.y > lastPlayAreaY + floorVariant || transform.position.y < lastPlayAreaY - floorVariant)
            {
                highestHeadsetY = 0f;
            }

            if (headset.transform.position.y > highestHeadsetY)
            {
                highestHeadsetY = headset.transform.position.y;
            }

            if (headset.transform.position.y > highestHeadsetY - crouchMargin)
            {
                lastGoodPositionSet = true;
                lastGoodPosition = transform.position;
            }

            lastPlayAreaY = transform.position.y;
        }

        private void FixedUpdate()
        {
            SetHeadsetY();
            UpdateCollider();
        }

        private void Update()
        {
            if (isFalling && fallingPhysicsOnly)
            {
                if (rb.velocity == Vector3.zero)
                {
                    StopPhysicsFall();
                }
            }
        }

        private void InitControllerListeners(GameObject controller)
        {
            if (controller)
            {
                var grabbingController = controller.GetComponent<VRTK_InteractGrab>();
                if (grabbingController && ignoreGrabbedCollisions)
                {
                    grabbingController.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
                    grabbingController.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUngrabObject);
                }
            }
        }
    }
}