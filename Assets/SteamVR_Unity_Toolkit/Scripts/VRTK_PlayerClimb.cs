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

        private Transform controllerTransform;
        private Vector3 startControllerPosition;
        private Vector3 startPosition;

        private Vector3 lastGoodHeadsetPosition;
        private bool headsetColliding = false;
        private bool isClimbing = false;

        private VRTK_PlayerPresence playerPresence;
        private bool lastGravitySetting;

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

        private void Start()
        {
            var controllerManager = FindObjectOfType<SteamVR_ControllerManager>();
            InitControllerListeners(controllerManager.left);
            InitControllerListeners(controllerManager.right);

            // Listen for teleport events 
            VRTK_BasicTeleport teleportComponent = GetComponent<VRTK_BasicTeleport>();
            if (teleportComponent)
            {
                teleportComponent.Teleporting += new TeleportEventHandler(OnTeleport);
            }

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

            // Required Component: VRTK_HeadsetCollisionFade
            var headCamera = VRTK_DeviceFinder.HeadsetTransform();
            var collisionFade = headCamera.GetComponent<VRTK_HeadsetCollisionFade>();
            if (collisionFade == null)
            {
                collisionFade = headCamera.gameObject.AddComponent<VRTK_HeadsetCollisionFade>();
            }
            collisionFade.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(OnHeadsetCollisionDetected);
            collisionFade.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(OnHeadsetCollisionEnded);
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
                if (useGravity)
                {
                    playerPresence.StopPhysicsFall();
                }

                OnPlayerClimbStarted(SetPlayerClimbEvent(e.controllerIndex, e.target));
                isClimbing = true;
                controllerTransform = ((VRTK_InteractGrab)sender).transform;
                startControllerPosition = GetPosition(controllerTransform);
                startPosition = transform.position;
            }
        }

        private void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            var controller = ((VRTK_InteractGrab)sender).gameObject;

            if (IsClimbableObject(e.target) && IsActiveClimbingController(controller))
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
                transform.position = lastGoodHeadsetPosition;
            }

            if (useGravity && carryMomentum)
            {
                var device = VRTK_DeviceFinder.ControllerByIndex(controllerIndex);
                playerPresence.StartPhysicsFall(-device.GetComponent<VRTK_ControllerEvents>().GetVelocity());
            }
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
                lastGoodHeadsetPosition = transform.position;
            }
        }


        private void InitControllerListeners(GameObject controller)
        {
            if (controller)
            {
                var grabbingController = controller.GetComponent<VRTK_InteractGrab>();
                if (grabbingController)
                {
                    grabbingController.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
                    grabbingController.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUngrabObject);
                }
            }
        }
    }
}