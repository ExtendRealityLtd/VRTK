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

        VRTK_PlayerPhysics playerPhysics;
        private bool lastGravitySetting;

        public virtual void OnPlayerClimbStarted(PlayerClimbEventArgs e)
        {
            if (PlayerClimbStarted != null)
                PlayerClimbStarted(this, e);
        }

        public virtual void OnPlayerClimbEnded(PlayerClimbEventArgs e)
        {
            if (PlayerClimbEnded != null)
                PlayerClimbEnded(this, e);
        }

        protected PlayerClimbEventArgs SetPlayerClimbEvent(uint controllerIndex, GameObject target)
        {
            PlayerClimbEventArgs e;
            e.controllerIndex = controllerIndex;
            e.target = target;
            return e;
        }

        private void Start()
        {
            var controllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();
            InitControllerListeners(controllerManager.left);
            InitControllerListeners(controllerManager.right);

            // Listen for teleport events 
            VRTK_BasicTeleport teleportComponent = GetComponent<VRTK_BasicTeleport>();
            if (teleportComponent)
            {
                teleportComponent.Teleporting += new TeleportEventHandler(OnTeleport);
            } 

            // Required Components

            // VRTK_PlayerPhysics
            playerPhysics = GetComponent<VRTK_PlayerPhysics>();
            if (!playerPhysics && useGravity)
            {
                playerPhysics = gameObject.AddComponent<VRTK_PlayerPhysics>();
            }

            // VRTK_HeadsetCollisionFade
            Transform headCamera = GameObject.FindObjectOfType<SteamVR_GameView>().GetComponent<Transform>();
            VRTK_HeadsetCollisionFade collisionFade = headCamera.GetComponent<VRTK_HeadsetCollisionFade>();
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
                return Vector3.Scale(objTransform.localPosition, gameObject.transform.localScale);
            }

            return objTransform.localPosition;
        }

    
        private void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {
            VRTK_InteractableObject interactObject = e.target.GetComponent<VRTK_InteractableObject>();
            if( interactObject && interactObject.AttachIsClimbObject() )
            {
                if (useGravity)
                {
                    playerPhysics.StopPhysicsFall();
                }

                OnPlayerClimbStarted(SetPlayerClimbEvent(e.controllerIndex, e.target));
                isClimbing = true;
                controllerTransform = ((VRTK_InteractGrab)sender).gameObject.transform;
                startControllerPosition = GetPosition(controllerTransform);
                startPosition = gameObject.transform.position;
            }
        }

        private void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            VRTK_InteractableObject interactObject = e.target.GetComponent<VRTK_InteractableObject>();
            if (interactObject && interactObject.AttachIsClimbObject() && 
                IsActiveClimbingController(((VRTK_InteractGrab)sender).gameObject))
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
                gameObject.transform.position = lastGoodHeadsetPosition;
            }

            if (useGravity && carryMomentum)
            {
                var device = SteamVR_Controller.Input((int)controllerIndex);
                playerPhysics.StartPhysicsFall(-device.velocity);
            }
        }

        private bool IsActiveClimbingController(GameObject controller)
        {
            return controller.transform == controllerTransform;
        }

        private void Update()
        {
            if (isClimbing)
            {
                gameObject.transform.position = startPosition - (GetPosition(controllerTransform)-startControllerPosition);
            }

            if (!headsetColliding)
                lastGoodHeadsetPosition = gameObject.transform.position;
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