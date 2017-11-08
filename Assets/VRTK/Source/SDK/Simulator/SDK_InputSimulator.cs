﻿namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The `[VRSimulator_CameraRig]` prefab is a mock Camera Rig set up that can be used to develop with VRTK without the need for VR Hardware.
    /// </summary>
    /// <remarks>
    /// Use the mouse and keyboard to move around both play area and hands and interacting with objects without the need of a hmd or VR controls.
    /// </remarks>
    public class SDK_InputSimulator : MonoBehaviour
    {
        /// <summary>
        /// Mouse input mode types
        /// </summary>
        public enum MouseInputMode
        {
            /// <summary>
            /// Mouse movement is always treated as mouse input.
            /// </summary>
            Always,
            /// <summary>
            /// Mouse movement is only treated as movement when a button is pressed.
            /// </summary>
            RequiresButtonPress
        }

        #region Public fields

        [Tooltip("Show control information in the upper left corner of the screen.")]
        public bool showControlHints = true;
        [Tooltip("Hide hands when disabling them.")]
        public bool hideHandsAtSwitch = false;
        [Tooltip("Reset hand position and rotation when enabling them.")]
        public bool resetHandsAtSwitch = true;
		
        [Header("Mouse cursor lock")]

        [Tooltip("Lock the mouse cursor to the game window.")]
        public bool lockMouseToView = true;
        [Tooltip("Whether the mouse movement always acts as input or requires a button press.")]
        public MouseInputMode mouseMovementInput = MouseInputMode.Always;

        [Header("Adjustments")]

        [Tooltip("Adjust hand movement speed.")]
        public float handMoveMultiplier = 0.002f;
        [Tooltip("Adjust hand rotation speed.")]
        public float handRotationMultiplier = 0.5f;
        [Tooltip("Adjust player movement speed.")]
        public float playerMoveMultiplier = 5;
        [Tooltip("Adjust player rotation speed.")]
        public float playerRotationMultiplier = 0.5f;
        [Tooltip("Adjust player sprint speed.")]
        public float playerSprintMultiplier = 2;

        [Header("Operation Key Bindings")]

        [Tooltip("Key used to enable mouse input if a button press is required.")]
        public KeyCode mouseMovementKey = KeyCode.Mouse1;
        [Tooltip("Key used to toggle control hints on/off.")]
        public KeyCode toggleControlHints = KeyCode.F1;
        [Tooltip("Key used to toggle control hints on/off.")]
        public KeyCode toggleMouseLock = KeyCode.F4;
        [Tooltip("Key used to switch between left and righ hand.")]
        public KeyCode changeHands = KeyCode.Tab;
        [Tooltip("Key used to switch hands On/Off.")]
        public KeyCode handsOnOff = KeyCode.LeftAlt;
        [Tooltip("Key used to switch between positional and rotational movement.")]
        public KeyCode rotationPosition = KeyCode.LeftShift;
        [Tooltip("Key used to switch between X/Y and X/Z axis.")]
        public KeyCode changeAxis = KeyCode.LeftControl;
        [Tooltip("Key used to distance pickup with left hand.")]
        public KeyCode distancePickupLeft = KeyCode.Mouse0;
        [Tooltip("Key used to distance pickup with right hand.")]
        public KeyCode distancePickupRight = KeyCode.Mouse1;
        [Tooltip("Key used to enable distance pickup.")]
        public KeyCode distancePickupModifier = KeyCode.LeftControl;

        [Header("Movement Key Bindings")]

        [Tooltip("Key used to move forward.")]
        public KeyCode moveForward = KeyCode.W;
        [Tooltip("Key used to move to the left.")]
        public KeyCode moveLeft = KeyCode.A;
        [Tooltip("Key used to move backwards.")]
        public KeyCode moveBackward = KeyCode.S;
        [Tooltip("Key used to move to the right.")]
        public KeyCode moveRight = KeyCode.D;
        [Tooltip("Key used to sprint.")]
        public KeyCode sprint = KeyCode.LeftShift;

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
        private GameObject hintCanvas;
        private Text hintText;
        private Transform rightHand;
        private Transform leftHand;
        private Transform currentHand;
        private Vector3 oldPos;
        private Transform neck;
        private SDK_ControllerSim rightController;
        private SDK_ControllerSim leftController;
        private static GameObject cachedCameraRig;
        private static bool destroyed = false;
        private float sprintMultiplier = 1;
        private GameObject crossHairPanel;

        #endregion

        /// <summary>
        /// The FindInScene method is used to find the `[VRSimulator_CameraRig]` GameObject within the current scene.
        /// </summary>
        /// <returns>Returns the found `[VRSimulator_CameraRig]` GameObject if it is found. If it is not found then it prints a debug log error.</returns>
        public static GameObject FindInScene()
        {
            if (cachedCameraRig == null && !destroyed)
            {
                cachedCameraRig = VRTK_SharedMethods.FindEvenInactiveGameObject<SDK_InputSimulator>();
                if (!cachedCameraRig)
                {
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "[VRSimulator_CameraRig]", "SDK_InputSimulator", ". check that the `VRTK/Prefabs/CameraRigs/[VRSimulator_CameraRig]` prefab been added to the scene."));
                }
            }
            return cachedCameraRig;
        }

        private void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        private void OnEnable()
        {
            hintCanvas = transform.Find("Canvas/Control Hints").gameObject;
            crossHairPanel = transform.Find("Canvas/CrosshairPanel").gameObject;
            hintText = hintCanvas.GetComponentInChildren<Text>();
            hintCanvas.SetActive(showControlHints);
            rightHand = transform.Find("RightHand");
            rightHand.gameObject.SetActive(false);
            leftHand = transform.Find("LeftHand");
            leftHand.gameObject.SetActive(false);
            currentHand = rightHand;
            oldPos = Input.mousePosition;
            neck = transform.Find("Neck");
            leftHand.Find("Hand").GetComponent<Renderer>().material.color = Color.red;
            rightHand.Find("Hand").GetComponent<Renderer>().material.color = Color.green;
            rightController = rightHand.GetComponent<SDK_ControllerSim>();
            leftController = leftHand.GetComponent<SDK_ControllerSim>();
            rightController.selected = true;
            leftController.selected = false;
            destroyed = false;

            SDK_SimController controllerSDK = VRTK_SDK_Bridge.GetControllerSDK() as SDK_SimController;
            if (controllerSDK != null)
            {
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
                controllerSDK.SetKeyMappings(keyMappings);
            }
            rightHand.gameObject.SetActive(true);
            leftHand.gameObject.SetActive(true);
            crossHairPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
            destroyed = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleControlHints))
            {
                showControlHints = !showControlHints;
                hintCanvas.SetActive(showControlHints);
            }

            if (Input.GetKeyDown(toggleMouseLock))
            {
                lockMouseToView = !lockMouseToView;
            }

            if (mouseMovementInput == MouseInputMode.RequiresButtonPress)
            {
                if (lockMouseToView)
                {
                    Cursor.lockState = Input.GetKey(mouseMovementKey) ? CursorLockMode.Locked : CursorLockMode.None;
                }
                else if (Input.GetKeyDown(mouseMovementKey))
                {
                    oldPos = Input.mousePosition;
                }
            }
            else
            {
                Cursor.lockState = lockMouseToView ? CursorLockMode.Locked : CursorLockMode.None;
            }

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
                    rightController.selected = true;
                    leftController.selected = false;
                }
                else
                {
                    currentHand = leftHand;
                    rightController.selected = false;
                    leftController.selected = true;
                }
            }

            if (isHand)
            {
                UpdateHands();
            }
            else
            {
                UpdateRotation();
                if (Input.GetKeyDown(distancePickupRight) && Input.GetKey(distancePickupModifier))
                {
                    TryPickup(true);
                }
                else if (Input.GetKeyDown(distancePickupLeft) && Input.GetKey(distancePickupModifier))
                {
                    TryPickup(false);
                }
                if (Input.GetKey(sprint))
                {
                    sprintMultiplier = playerSprintMultiplier;
                }
                else
                {
                    sprintMultiplier = 1;
                }
                if (Input.GetKeyDown(distancePickupModifier))
                {
                    crossHairPanel.SetActive(true);
                }
                else if (Input.GetKeyUp(distancePickupModifier))
                {
                    crossHairPanel.SetActive(false);
                }
            }

            UpdatePosition();

            if (showControlHints)
            {
                UpdateHints();
            }
        }

        private void TryPickup(bool rightHand)
        {
            Ray screenRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (Physics.Raycast(screenRay, out hit))
            {
                VRTK_InteractableObject io = hit.collider.gameObject.GetComponent<VRTK_InteractableObject>();
                if (io != null)
                {
                    GameObject hand;
                    if (rightHand)
                    {
                        hand = VRTK_DeviceFinder.GetControllerRightHand();
                    }
                    else
                    {
                        hand = VRTK_DeviceFinder.GetControllerLeftHand();
                    }
                    VRTK_InteractGrab grab = hand.GetComponent<VRTK_InteractGrab>();
                    if (grab.GetGrabbedObject() == null)
                    {
                        hand.GetComponent<VRTK_InteractTouch>().ForceTouch(hit.collider.gameObject);
                        grab.AttemptGrab();
                    }
                }
            }
        }

        private void UpdateHands()
        {
            Vector3 mouseDiff = GetMouseDelta();

            if (IsAcceptingMouseInput())
            {
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
            }
        }

        private void UpdateRotation()
        {
            Vector3 mouseDiff = GetMouseDelta();

            if (IsAcceptingMouseInput())
            {
                Vector3 rot = transform.localRotation.eulerAngles;
                rot.y += (mouseDiff * playerRotationMultiplier).x;
                transform.localRotation = Quaternion.Euler(rot);

                rot = neck.rotation.eulerAngles;

                if (rot.x > 180)
                {
                    rot.x -= 360;
                }

                if (rot.x < 80 && rot.x > -80)
                {
                    rot.x += (mouseDiff * playerRotationMultiplier).y * -1;
                    rot.x = Mathf.Clamp(rot.x, -79, 79);
                    neck.rotation = Quaternion.Euler(rot);
                }
            }
        }

        private void UpdatePosition()
        {
            float moveMod = Time.deltaTime * playerMoveMultiplier * sprintMultiplier;
            if (Input.GetKey(moveForward))
            {
                transform.Translate(transform.forward * moveMod, Space.World);
            }
            else if (Input.GetKey(moveBackward))
            {
                transform.Translate(-transform.forward * moveMod, Space.World);
            }
            if (Input.GetKey(moveLeft))
            {
                transform.Translate(-transform.right * moveMod, Space.World);
            }
            else if (Input.GetKey(moveRight))
            {
                transform.Translate(transform.right * moveMod, Space.World);
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

        private void UpdateHints()
        {
            string hints = "";
            Func<KeyCode, string> key = (k) => "<b>" + k.ToString() + "</b>";

            string mouseInputRequires = "";
            if (mouseMovementInput == MouseInputMode.RequiresButtonPress)
            {
                mouseInputRequires = " (" + key(mouseMovementKey) + ")";
            }

            // WASD Movement
            string movementKeys = moveForward.ToString() + moveLeft.ToString() + moveBackward.ToString() + moveRight.ToString();
            hints += "Toggle Control Hints: " + key(toggleControlHints) + "\n\n";
            hints += "Move Player/Playspace: <b>" + movementKeys + "</b>\n";
            hints += "Sprint Modifier: (" + key(sprint) + ")\n\n";

            if (isHand)
            {
                // Controllers
                if (Input.GetKey(rotationPosition))
                {
                    hints += "Mouse: <b>Controller Rotation" + mouseInputRequires + "</b>\n";
                }
                else
                {
                    hints += "Mouse: <b>Controller Position" + mouseInputRequires + "</b>\n";
                }
                hints += "Modes: HMD (" + key(handsOnOff) + "), Rotation (" + key(rotationPosition) + ")\n";

                hints += "Controller Hand: " + currentHand.name.Replace("Hand", "") + " (" + key(changeHands) + ")\n";

                string axis = Input.GetKey(changeAxis) ? "X/Y" : "X/Z";
                hints += "Axis: " + axis + " (" + key(changeAxis) + ")\n";

                // Controller Buttons
                string pressMode = "Press";
                if (Input.GetKey(hairTouchModifier))
                {
                    pressMode = "Hair Touch";
                }
                else if (Input.GetKey(touchModifier))
                {
                    pressMode = "Touch";
                }

                hints += "\nButton Press Mode Modifiers: Touch (" + key(touchModifier) + "), Hair Touch (" + key(hairTouchModifier) + ")\n";

                hints += "Trigger " + pressMode + ": " + key(triggerAlias) + "\n";
                hints += "Grip " + pressMode + ": " + key(gripAlias) + "\n";
                if (!Input.GetKey(hairTouchModifier))
                {
                    hints += "Touchpad " + pressMode + ": " + key(touchpadAlias) + "\n";
                    hints += "Button One " + pressMode + ": " + key(buttonOneAlias) + "\n";
                    hints += "Button Two " + pressMode + ": " + key(buttonTwoAlias) + "\n";
                    hints += "Start Menu " + pressMode + ": " + key(startMenuAlias) + "\n";
                }
            }
            else
            {
                // HMD Input
                hints += "Mouse: <b>HMD Rotation" + mouseInputRequires + "</b>\n";
                hints += "Modes: Controller (" + key(handsOnOff) + ")\n";
                hints += "Distance Pickup Modifier: (" + key(distancePickupModifier) + ")\n";
                hints += "Distance Pickup Left Hand: (" + key(distancePickupLeft) + ")\n";
                hints += "Distance Pickup Right Hand: (" + key(distancePickupRight) + ")\n";
            }

            hintText.text = hints.TrimEnd();
        }

        private bool IsAcceptingMouseInput()
        {
            return mouseMovementInput == MouseInputMode.Always || Input.GetKey(mouseMovementKey);
        }

        private Vector3 GetMouseDelta()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                return new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
            else
            {
                Vector3 mouseDiff = Input.mousePosition - oldPos;
                oldPos = Input.mousePosition;
                return mouseDiff;
            }
        }
    }
}
