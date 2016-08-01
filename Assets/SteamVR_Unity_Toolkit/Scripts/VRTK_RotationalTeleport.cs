//====================================================================================
//
// Purpose: Provide teleportation which can also rotate of VR CameraRig to face the direction pointed by the non-teleporting controller
//
// This script must be attached to the [CameraRig] Prefab.
//
// Both Lef and Right Controllers must have the VRTK_SimplePointer attached to it to listen for the
// updated world position and direction to teleport to. The GameObject to point the direction must be pointing as direction Vector3.forward (x=0,y=0,z=1)
//
//====================================================================================
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_RotationalTeleport : VRTK_HeightAdjustTeleport
    {
        public enum RotationMethod
        {
            FaceBeamDirection,
            FacePointAimed
        }

        public enum HandType
        {
            Left = 0,
            Right,
            Custom
        }
        
        public RotationMethod rotationMethod = RotationMethod.FaceBeamDirection;

        [Tooltip("Sets which controller will effectively teleport. The other one will set the facing direction. Choose 'Custom' to manually override the teleport and rotation objects. This allows, for example, teleporting with the headset.")]        
        public HandType teleportHandController = HandType.Left;
        public GameObject directionPointerGameObject;

        public VRTK_WorldPointer teleportPointer;
        public VRTK_SimplePointer rotationPointer;


        private VRTK_WorldPointer leftControllerPointer;
        private VRTK_WorldPointer rightControllerPointer;
        private Transform headsetTransform;

        // Used to track users actions
        private bool destinationMarkerActive = false;
        private bool hasRotationBeenActivated = false;

        private Vector3 rotationPointerDestionationPosition = Vector3.zero;
        private Vector3 lastRotationDirection = Vector3.zero;

        // VRTK_SimplePointer doesn't give callbacks when is disable/enabled, so we need to track the session to avoid bugs
        private bool hasTeleportBeenInitialized = false;

        public void SetTeleportPointer(VRTK_WorldPointer pointer)
        {
            teleportPointer.DestinationMarkerEnter -= TeleportDestinationPointerActivated;
            teleportPointer.DestinationMarkerSet -= TeleportDestinationSet;
            teleportPointer.DestinationMarkerExit -= TeleportDestinationDeactivated;
            teleportPointer.OnPointerOn -= OnTeleportPointerOn;
            teleportPointer.OnPointerOff -= OnTeleportPointerOff;

            teleportPointer = pointer;

            teleportPointer.DestinationMarkerEnter += new DestinationMarkerEventHandler(TeleportDestinationPointerActivated);
            teleportPointer.DestinationMarkerSet += new DestinationMarkerEventHandler(TeleportDestinationSet);
            teleportPointer.DestinationMarkerExit += new DestinationMarkerEventHandler(TeleportDestinationDeactivated);
            teleportPointer.OnPointerOn += new PointerToogleEventHandler(OnTeleportPointerOn);
            teleportPointer.OnPointerOff += new PointerToogleEventHandler(OnTeleportPointerOff);
        }

        private void RotateCameraRig(DestinationMarkerEventArgs e)
        {
            Vector3 flatPointerDirection = Vector3.zero;

            if (rotationMethod == RotationMethod.FaceBeamDirection && hasRotationBeenActivated)
            {
                bool willRotate = false;

                if (rotationPointer.IsActive())
                {
                    flatPointerDirection = rotationPointer.GetPointedDirection();
                    willRotate = true;
                }
                else
                {
                    if (directionPointerGameObject != null)
                    {
                        flatPointerDirection = directionPointerGameObject.transform.forward;
                        willRotate = true;
                    }
                }

                if (willRotate)
                {
                    flatPointerDirection.y = 0f;

                    Vector3 headsetPlanarForward = headsetTransform.forward;
                    headsetPlanarForward.y = 0f;

                    Quaternion amountRotation = Quaternion.FromToRotation(headsetPlanarForward, flatPointerDirection);
                    transform.RotateAround(headsetTransform.position, Vector3.up, amountRotation.eulerAngles.y);
                }
            }
            else if (rotationMethod == RotationMethod.FacePointAimed && hasRotationBeenActivated)
            {
                flatPointerDirection = (rotationPointerDestionationPosition - e.destinationPosition);
                flatPointerDirection.y = 0f;


                Vector3 headsetPlanarForward = headsetTransform.forward;
                headsetPlanarForward.y = 0f;

                Quaternion amountRotation = Quaternion.FromToRotation(headsetPlanarForward, flatPointerDirection);
                transform.RotateAround(headsetTransform.position, Vector3.up, amountRotation.eulerAngles.y);
            }
        }

        /// <summary>
        /// Resets the gameobject direction to the user facing position
        /// </summary>
        /// <param name="sender"></param>
        private void OnTeleportPointerOn(object sender)
        {
            hasTeleportBeenInitialized = true;
            if (directionPointerGameObject != null)
            {
                Vector3 planarRotation = new Vector3(0f, headsetTransform.rotation.eulerAngles.y, 0f);
                directionPointerGameObject.transform.rotation = Quaternion.Euler(planarRotation);
            }
        }

        /// <summary>
        /// Resets all the variables that tracks pointing session
        /// </summary>
        /// <param name="sender"></param>
        private void OnTeleportPointerOff(object sender)
        {
            OnTeleportPointerOff();
        }

        private void OnTeleportPointerOff()
        {
            hasTeleportBeenInitialized = false;
            destinationMarkerActive = false;
            hasRotationBeenActivated = false;

            if (directionPointerGameObject != null)
            {
                directionPointerGameObject.SetActive(false);
            }
        }

        private void OnRotationPointerOn(object sender)
        {
            if (rotationMethod != RotationMethod.FaceBeamDirection)
                return;

            if (hasTeleportBeenInitialized)
            {
                hasRotationBeenActivated = true;

                if (directionPointerGameObject != null)
                {
                    Vector3 flatPointerDirection = rotationPointer.GetPointedDirection();
                    flatPointerDirection.y = 0f;

                    directionPointerGameObject.transform.LookAt(directionPointerGameObject.transform.position + flatPointerDirection);

                    if (destinationMarkerActive)
                        directionPointerGameObject.SetActive(true);
                }
            }
        }


        private void TeleportDestinationPointerActivated(object sender, DestinationMarkerEventArgs e)
        {
            if (!hasTeleportBeenInitialized)
                return;

            destinationMarkerActive = true;

            if (directionPointerGameObject != null)
            {
                if (directionPointerGameObject.activeSelf)
                    directionPointerGameObject.transform.position = e.destinationPosition;
                else
                {
                    if (hasRotationBeenActivated == true)
                        directionPointerGameObject.SetActive(true);
                }
            }
        }

        private void TeleportDestinationSet(object sender, DestinationMarkerEventArgs e)
        {
            if (!hasTeleportBeenInitialized)
                return;

            if (directionPointerGameObject != null)
                directionPointerGameObject.transform.position = e.destinationPosition;
        }

        private void TeleportDestinationDeactivated(object sender, DestinationMarkerEventArgs e)
        {
            destinationMarkerActive = false;

            if (directionPointerGameObject != null)
                directionPointerGameObject.SetActive(false);
        }

        private void RotateDirectionPointerGO()
        {
            if ((directionPointerGameObject != null) && teleportPointer.IsActive())
            {

                if (rotationMethod == RotationMethod.FaceBeamDirection && rotationPointer.IsActive())
                {
                    Vector3 flatPointerDirection = rotationPointer.GetPointedDirection();
                    flatPointerDirection.y = 0f;

                    directionPointerGameObject.transform.LookAt(directionPointerGameObject.transform.position + flatPointerDirection);
                }
                else if (rotationMethod == RotationMethod.FacePointAimed)
                {

                    if (hasRotationBeenActivated)
                    {
                        Vector3 flatPointerPosition = rotationPointerDestionationPosition;
                        flatPointerPosition.y = 0f;

                        directionPointerGameObject.transform.LookAt(flatPointerPosition);
                        directionPointerGameObject.transform.rotation = Quaternion.Euler(0f, directionPointerGameObject.transform.rotation.eulerAngles.y, 0f);
                    }
                }
            }

            // If another script disables the pointer script, the callback function OnTeleportPointerOff won't be called, so we do this manually
            if (!teleportPointer.IsActive() && directionPointerGameObject)
            {
                OnTeleportPointerOff();
            }
        }

        private void OnRotationDestinationEnter(object sender, DestinationMarkerEventArgs e)
        {

            if (destinationMarkerActive && hasTeleportBeenInitialized)
            {
                rotationPointerDestionationPosition = e.destinationPosition;
                hasRotationBeenActivated = true;
                if (directionPointerGameObject != null)
                {
                    directionPointerGameObject.SetActive(true);
                }
            }
        }

        private void OnRotationDestinationSet(object sender, DestinationMarkerEventArgs e)
        {
            if (destinationMarkerActive && hasTeleportBeenInitialized)
            {
                rotationPointerDestionationPosition = e.destinationPosition;
            }
        }

        // LateUpdate is used to be sure that the rotation will be set after the position.
        protected virtual void LateUpdate()
        {
            RotateDirectionPointerGO();
        }

        protected virtual void Awake()
        {
            if (directionPointerGameObject != null)
                directionPointerGameObject.SetActive(false);
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            leftControllerPointer = VRTK_DeviceFinder.GetControllerGameObject(VRTK_DeviceFinder.ControllerHand.Left).GetComponentInChildren<VRTK_WorldPointer>();
            rightControllerPointer = VRTK_DeviceFinder.GetControllerGameObject(VRTK_DeviceFinder.ControllerHand.Right).GetComponentInChildren<VRTK_WorldPointer>();

            if (teleportHandController == HandType.Left)
            {
                teleportPointer = leftControllerPointer;
                rotationPointer = (VRTK_SimplePointer)rightControllerPointer;

                if (teleportPointer == null || rotationPointer == null)
                    Debug.LogError("Could not find a Pointer on the feft controller or VRTK_SimplePointer on right controller");
            }
            else if (teleportHandController == HandType.Right)
            {
                teleportPointer = rightControllerPointer;
                rotationPointer = (VRTK_SimplePointer)leftControllerPointer;

                if (teleportPointer == null || rotationPointer == null)
                    Debug.LogError("Could not find a Pointer on the Right controller or VRTK_SimplePointer on left controller");
            }

            if (teleportPointer == null || rotationPointer == null)
                Debug.LogError("The teleportController and rotationController must be assigned on VRTK_RotationalTeleport");

            teleportPointer.DestinationMarkerEnter += new DestinationMarkerEventHandler(TeleportDestinationPointerActivated);
            teleportPointer.DestinationMarkerSet += new DestinationMarkerEventHandler(TeleportDestinationSet);
            teleportPointer.DestinationMarkerExit += new DestinationMarkerEventHandler(TeleportDestinationDeactivated);
            teleportPointer.OnPointerOn += new PointerToogleEventHandler(OnTeleportPointerOn);
            teleportPointer.OnPointerOff += new PointerToogleEventHandler(OnTeleportPointerOff);

            rotationPointer.DestinationMarkerEnter += new DestinationMarkerEventHandler(OnRotationDestinationEnter);
            rotationPointer.DestinationMarkerSet += new DestinationMarkerEventHandler(OnRotationDestinationSet);
            rotationPointer.OnPointerOn+= new PointerToogleEventHandler(OnRotationPointerOn);

            headsetTransform = VRTK_DeviceFinder.HeadsetTransform(); 
        }

        protected override void DoTeleport(object sender, DestinationMarkerEventArgs e)
        {
            if (sender != (object)teleportPointer || !hasTeleportBeenInitialized)
                return;

            if (e.enableTeleport)
            {
                if (sender == (object)teleportPointer)
                {
                    RotateCameraRig(e);
                }
            }

            base.DoTeleport(sender, e);

            if (directionPointerGameObject != null && rotationMethod==RotationMethod.FaceBeamDirection)
            {
                directionPointerGameObject.SetActive(false);
                hasRotationBeenActivated = false;
            }
        }
    }
}