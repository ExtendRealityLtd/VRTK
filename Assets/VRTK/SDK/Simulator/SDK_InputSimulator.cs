// VR Simulator|Prefabs|0005
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The `VRSimulatorCameraRig` prefab is a mock Camera Rig set up that can be used to develop with VRTK without the need for VR Hardware.
    /// </summary>
    /// <remarks>
    /// Use the mouse and keyboard to move around both play area and hands and interacting with objects without the need of a hmd or VR controls.
    /// </remarks>
    public class SDK_InputSimulator : MonoBehaviour
    {
        #region Public fields

        [Tooltip("Hide hands when disabling them.")]
        public bool hideHandsAtSwitch = false;
        [Tooltip("Reset hand position and rotation when enabling them.")]
        public bool resetHandsAtSwitch = true;

        [Header("Adjustments")]

        [Tooltip("Adjust hand movement speed.")]
        public float handMoveMultiplier = 0.002f;
        [Tooltip("Adjust hand rotation speed.")]
        public float handRotationMultiplier = 0.5f;
        [Tooltip("Adjust player movement speed.")]
        public float playerMoveMultiplier = 5;
        [Tooltip("Adjust player rotation speed.")]
        public float playerRotationMultiplier = 0.5f;

        [Header("Operation Key Bindings")]

        [Tooltip("Key used to switch between left and righ hand.")]
        public KeyCode changeHands = KeyCode.Tab;
        [Tooltip("Key used to switch hands On/Off.")]
        public KeyCode handsOnOff = KeyCode.LeftAlt;
        [Tooltip("Key used to switch between positional and rotational movement.")]
        public KeyCode rotationPosition = KeyCode.LeftShift;
        [Tooltip("Key used to switch between X/Y and X/Z axis.")]
        public KeyCode changeAxis = KeyCode.LeftControl;

        [Header("Controller Key Bindings")]
        [Tooltip("Key used to simulate trigger button.")]
        public KeyCode triggerAlias = KeyCode.Mouse1;
        [Tooltip("Key used to simulate grip button.")]
        public KeyCode gripAlias = KeyCode.Mouse0;
        [Tooltip("Key used to simulate touchpad button.")]
        public KeyCode touchpadAlias = KeyCode.Q;
        [Tooltip("Key used to simulate button one.")]
        public KeyCode buttonOneAlias = KeyCode.E;
        [Tooltip("Key used to simulate button two.")]
        public KeyCode buttonTwoAlias = KeyCode.R;
        [Tooltip("Key used to simulate start menu button.")]
        public KeyCode startMenuAlias = KeyCode.F;
        [Tooltip("Key used to switch between button touch and button press mode.")]
        public KeyCode touchModifier = KeyCode.T;
        [Tooltip("Key used to switch between hair touch mode.")]
        public KeyCode hairTouchModifier = KeyCode.H;

        #endregion
        #region Private fields

        private bool isHand = false;
        private Transform rightHand;
        private Transform leftHand;
        private Transform currentHand;
        private Vector3 oldPos;
        private Transform myCamera;
        private SDK_ControllerSim rightController;
        private SDK_ControllerSim leftController;
        private static GameObject cachedCameraRig;
        private static bool destroyed = false;

        #endregion

        /// <summary>
        /// The FindInScene method is used to find the `VRSimulatorCameraRig` GameObject within the current scene.
        /// </summary>
        /// <returns>Returns the found `VRSimulatorCameraRig` GameObject if it is found. If it is not found then it prints a debug log error.</returns>
        public static GameObject FindInScene()
        {
            if (cachedCameraRig == null && !destroyed)
            {
                cachedCameraRig = GameObject.Find("VRSimulatorCameraRig");
                if (!cachedCameraRig)
                {
                    Debug.LogError("No `VRSimulatorCameraRig` GameObject is found in the scene, have you added the `VRTK/Prefabs/VRSimulatorCameraRig` prefab to the scene?");
                }
            }
            return cachedCameraRig;
        }

        private void Awake()
        {
            rightHand = transform.FindChild("RightHand");
            rightHand.gameObject.SetActive(false);
            leftHand = transform.FindChild("LeftHand");
            leftHand.gameObject.SetActive(false);
            currentHand = rightHand;
            oldPos = Input.mousePosition;
            myCamera = transform.FindChild("Camera");
            leftHand.FindChild("Hand").GetComponent<Renderer>().material.color = Color.red;
            rightHand.FindChild("Hand").GetComponent<Renderer>().material.color = Color.green;
            rightController = rightHand.GetComponent<SDK_ControllerSim>();
            leftController = leftHand.GetComponent<SDK_ControllerSim>();
            rightController.Selected = true;
            leftController.Selected = false;
            destroyed = false;

#if VRTK_SDK_SIM
            Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode>()
            {
                {"Trigger", triggerAlias },
                {"Grip", gripAlias },
                {"TouchpadPress", touchpadAlias },
                {"ButtonOne", buttonOneAlias },
                {"ButtonTwo", buttonTwoAlias },
                {"StartMenu", startMenuAlias },
                {"TouchModifier", touchModifier },
                {"HairTouchModifier", hairTouchModifier }
            };
            SDK_SimController controllerSDK = (SDK_SimController)VRTK_SDK_Bridge.GetControllerSDK();
            controllerSDK.SetKeyMappings(keyMappings);
#endif
        }

        private void OnDestroy()
        {
            destroyed = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(handsOnOff))
            {
                if (isHand)
                {
                    SetMove();
                }
                else
                {
                    SetHand();
                }
            }

            if (Input.GetKeyDown(changeHands))
            {
                if (currentHand.name == "LeftHand")
                {
                    currentHand = rightHand;
                    rightController.Selected = true;
                    leftController.Selected = false;
                }
                else
                {
                    currentHand = leftHand;
                    rightController.Selected = false;
                    leftController.Selected = true;
                }
            }

            if (isHand)
            {
                UpdateHands();
            }
            else
            {
                UpdateRotation();
            }

            UpdatePosition();
        }

        private void UpdateHands()
        {
            Vector3 mouseDiff = Input.mousePosition - oldPos;

            if (Input.GetKey(rotationPosition)) //Rotation
            {
                if (Input.GetKey(changeAxis))
                {
                    Vector3 rot = Vector3.zero;
                    rot.x += (mouseDiff * handRotationMultiplier).y;
                    rot.y += (mouseDiff * handRotationMultiplier).x;
                    currentHand.transform.Rotate(rot * Time.deltaTime);
                }
                else
                {
                    Vector3 rot = Vector3.zero;
                    rot.z += (mouseDiff * handRotationMultiplier).x;
                    rot.x += (mouseDiff * handRotationMultiplier).y;
                    currentHand.transform.Rotate(rot * Time.deltaTime);
                }
            }
            else //Position
            {
                if (Input.GetKey(changeAxis))
                {
                    Vector3 pos = Vector3.zero;
                    pos += mouseDiff * handMoveMultiplier;
                    currentHand.transform.Translate(pos * Time.deltaTime);
                }
                else
                {
                    Vector3 pos = Vector3.zero;
                    pos.x += (mouseDiff * handMoveMultiplier).x;
                    pos.z += (mouseDiff * handMoveMultiplier).y;
                    currentHand.transform.Translate(pos * Time.deltaTime);
                }
            }
            oldPos = Input.mousePosition;
        }

        private void UpdateRotation()
        {
            Vector3 mouseDiff = Input.mousePosition - oldPos;

            Vector3 rot = transform.rotation.eulerAngles;
            rot.y += (mouseDiff * playerRotationMultiplier).x;
            transform.localRotation = Quaternion.Euler(rot);

            rot = myCamera.rotation.eulerAngles;

            if (rot.x > 180)
            {
                rot.x -= 360;
            }

            if (rot.x < 80 && rot.x > -80)
            {
                rot.x += (mouseDiff * playerRotationMultiplier).y * -1;
                rot.x = Mathf.Clamp(rot.x, -79, 79);
                myCamera.rotation = Quaternion.Euler(rot);
            }

            oldPos = Input.mousePosition;
        }

        private void UpdatePosition()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(transform.forward * Time.deltaTime * playerMoveMultiplier, Space.World);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(-transform.forward * Time.deltaTime * playerMoveMultiplier, Space.World);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-transform.right * Time.deltaTime * playerMoveMultiplier, Space.World);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(transform.right * Time.deltaTime * playerMoveMultiplier, Space.World);
            }
        }

        private void SetHand()
        {
            Cursor.visible = false;
            isHand = true;
            rightHand.gameObject.SetActive(true);
            leftHand.gameObject.SetActive(true);
            oldPos = Input.mousePosition;
            if (resetHandsAtSwitch)
            {
                rightHand.transform.localPosition = new Vector3(0.2f, 1.2f, 0.5f);
                rightHand.transform.localRotation = Quaternion.identity;
                leftHand.transform.localPosition = new Vector3(-0.2f, 1.2f, 0.5f);
                leftHand.transform.localRotation = Quaternion.identity;
            }
        }

        private void SetMove()
        {
            Cursor.visible = true;
            isHand = false;
            if (hideHandsAtSwitch)
            {
                rightHand.gameObject.SetActive(false);
                leftHand.gameObject.SetActive(false);
            }
        }
    }
}
