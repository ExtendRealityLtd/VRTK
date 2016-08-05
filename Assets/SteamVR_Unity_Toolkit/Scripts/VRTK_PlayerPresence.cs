﻿namespace VRTK
{
    using UnityEngine;

    public class VRTK_PlayerPresence : MonoBehaviour
    {
        public float headsetYOffset = 0.2f;
        public bool ignoreGrabbedCollisions = true;
        public bool resetPositionOnCollision = true;

        private Transform headset;
        private Rigidbody rb;
        private BoxCollider bc;
        private Vector3 lastGoodPosition;
        private bool lastGoodPositionSet = false;
        private float highestHeadsetY = 0f;
        private float crouchMargin = 0.5f;
        private float lastPlayAreaY = 0f;

        public Transform GetHeadset()
        {
            return headset;
        }

        private void Start()
        {
            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);

            lastGoodPositionSet = false;
            headset = VRTK_DeviceFinder.HeadsetTransform();
            CreateCollider();
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
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 100;
            rb.freezeRotation = true;

            bc = gameObject.AddComponent<BoxCollider>();
            bc.center = new Vector3(0f, 1f, 0f);
            bc.size = new Vector3(0.25f, 1f, 0.25f);

            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
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

        private void Update()
        {
            SetHeadsetY();
            UpdateCollider();
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