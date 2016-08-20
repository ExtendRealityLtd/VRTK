namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public struct PlayerClimbEventArgs
    {
        public uint controllerIndex;
        public GameObject target;
    }

    public delegate void PlayerClimbEventHandler(object sender, PlayerClimbEventArgs e);

    public class VRTK_PlayerClimb : MonoBehaviour
    {
        public event PlayerClimbEventHandler PlayerClimbStarted;
        public event PlayerClimbEventHandler PlayerClimbEnded;

        public bool usePlayerScale = true;
        public bool useGravity = true;
        public float safeZoneTeleportOffset = 0.4f;

        private Transform headCamera;
        private Transform controllerTransform;
        private Vector3 startControllerPosition;
        private Vector3 startPosition;

        private Vector3 lastGoodHeadsetPosition;
        private bool headsetColliding = false;
        private bool isClimbing = false;

        private VRTK_PlayerPresence playerPresence;
        private bool lastGravitySetting;
        private VRTK_HeadsetCollisionFade collisionFade;
        private SteamVR_ControllerManager controllerManager;

        private GameObject climbingObject;

        private void OnPlayerClimbStarted(PlayerClimbEventArgs e)
        {
            if (PlayerClimbStarted != null)
            {
                PlayerClimbStarted(this, e);
            }
        }

        private void OnPlayerClimbEnded(PlayerClimbEventArgs e)
        {
            if (PlayerClimbEnded != null)
            {
                PlayerClimbEnded(this, e);
            }
        }

        private PlayerClimbEventArgs SetPlayerClimbEvent(uint controllerIndex, GameObject target)
        {
            PlayerClimbEventArgs e;
            e.controllerIndex = controllerIndex;
            e.target = target;
            return e;
        }

        private void Awake()
        {
            // Required Component: VRTK_PlayerPresence
            playerPresence = GetComponent<VRTK_PlayerPresence>();
            if (useGravity)
            {
                if (!playerPresence)
                {
                    playerPresence = gameObject.AddComponent<VRTK_PlayerPresence>();
                }

                playerPresence.SetFallingPhysicsOnlyParams(true);
            }

            controllerManager = FindObjectOfType<SteamVR_ControllerManager>();
            headCamera = VRTK_DeviceFinder.HeadsetTransform();
            collisionFade = headCamera.GetComponent<VRTK_HeadsetCollisionFade>();
            if (collisionFade == null)
            {
                collisionFade = headCamera.gameObject.AddComponent<VRTK_HeadsetCollisionFade>();
            }
        }

        private void OnEnable()
        {
            InitListeners(true);
        }

        private void OnDisable()
        {
            Ungrab(false, 0, climbingObject);
            InitListeners(false);
        }

        private void InitListeners(bool state)
        {
            InitControllerListeners(controllerManager.left, state);
            InitControllerListeners(controllerManager.right, state);

            InitTeleportListener(state);
            InitCollisionFadeListener(state);
        }

        private void InitTeleportListener(bool state)
        {
            // Listen for teleport events 
            VRTK_BasicTeleport teleportComponent = GetComponent<VRTK_BasicTeleport>();
            if (teleportComponent)
            {
                if (state)
                {
                    teleportComponent.Teleporting += new TeleportEventHandler(OnTeleport);
                }
                else
                {
                    teleportComponent.Teleporting -= new TeleportEventHandler(OnTeleport);
                }
            }
        }

        private void InitCollisionFadeListener(bool state)
        {
            if (state)
            {
                collisionFade.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(OnHeadsetCollisionDetected);
                collisionFade.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(OnHeadsetCollisionEnded);
            }
            else
            {
                collisionFade.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(OnHeadsetCollisionDetected);
                collisionFade.HeadsetCollisionEnded -= new HeadsetCollisionEventHandler(OnHeadsetCollisionEnded);
            }
        }

        private Vector3 GetPosition(Transform objTransform)
        {
            if (usePlayerScale)
            {
                return transform.localRotation * Vector3.Scale(objTransform.localPosition, transform.localScale);
            }

            return transform.localRotation * objTransform.localPosition;
        }

        private void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (IsClimbableObject(e.target))
            {
                climbingObject = e.target;
                if (useGravity)
                {
                    playerPresence.StopPhysicsFall();
                }

                OnPlayerClimbStarted(SetPlayerClimbEvent(e.controllerIndex, climbingObject));
                isClimbing = true;
                controllerTransform = ((VRTK_InteractGrab)sender).transform;
                startControllerPosition = GetPosition(controllerTransform);
                startPosition = transform.position;
            }
        }

        private void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            var controller = ((VRTK_InteractGrab)sender).gameObject;

            if (e.target && IsClimbableObject(e.target) && IsActiveClimbingController(controller))
            {
                Ungrab(true, e.controllerIndex, e.target);
            }
        }

        private void OnTeleport(object sender, DestinationMarkerEventArgs e)
        {
            Ungrab(false, e.controllerIndex, e.target.gameObject);
        }

        private void OnHeadsetCollisionDetected(object sender, HeadsetCollisionEventArgs e)
        {
            headsetColliding = true;
        }

        private void OnHeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
        {
            headsetColliding = false;
        }

        private void Ungrab(bool carryMomentum, uint controllerIndex, GameObject target)
        {
            OnPlayerClimbEnded(SetPlayerClimbEvent(controllerIndex, target));
            isClimbing = false;

            // Move to the last safe spot
            if (headsetColliding)
            {
                Vector3 headsetPosition = headCamera.transform.position;

                Vector3 moveVector = lastGoodHeadsetPosition-headsetPosition;
                Vector3 moveDirection = moveVector.normalized;
                Vector3 moveOffset = moveDirection * safeZoneTeleportOffset;

                transform.position += moveVector + moveOffset;
            }

            if (useGravity && carryMomentum)
            {
                Vector3 velocity = Vector3.zero;
                var device = VRTK_DeviceFinder.ControllerByIndex(controllerIndex);

                if (device)
                {
                    velocity = -device.GetComponent<VRTK_ControllerEvents>().GetVelocity();
                    if (usePlayerScale)
                    {
                        velocity = Vector3.Scale(velocity, transform.localScale);
                    }
                }

                playerPresence.StartPhysicsFall(velocity);
            }
            climbingObject = null;
        }

        private bool IsActiveClimbingController(GameObject controller)
        {
            return controller.transform == controllerTransform;
        }

        private bool IsClimbableObject(GameObject obj)
        {
            var interactObject = obj.GetComponent<VRTK_InteractableObject>();
            return interactObject != null && interactObject.AttachIsClimbObject();
        }

        private void Update()
        {
            if (isClimbing)
            {
                transform.position = startPosition - (GetPosition(controllerTransform) - startControllerPosition);
            }

            if (!headsetColliding)
            {
                lastGoodHeadsetPosition = headCamera.transform.position;
            }
        }


        private void InitControllerListeners(GameObject controller, bool state)
        {
            if (controller)
            {
                var grabbingController = controller.GetComponent<VRTK_InteractGrab>();
                if (grabbingController)
                {
                    if (state)
                    {
                        grabbingController.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
                        grabbingController.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUngrabObject);
                    }
                    else
                    {
                        grabbingController.ControllerGrabInteractableObject -= new ObjectInteractEventHandler(OnGrabObject);
                        grabbingController.ControllerUngrabInteractableObject -= new ObjectInteractEventHandler(OnUngrabObject);
                    }
                }
            }
        }
    }
}