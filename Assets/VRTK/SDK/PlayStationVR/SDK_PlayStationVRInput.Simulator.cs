using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRTK
{
    /// <remarks>
    ///     The Simulator extension allows you to use the mouse and keyboard to move around play area and hands mimic hand
    ///     movement without the need of exporting.
    ///     most of the code is grabbed from the simulator SDK
    /// </remarks>
    public partial class SDK_PlayStationVRInput
    {
        #region Public fields

        [Header("Simulator Input")]
        [Tooltip("Show control information in the upper left corner of the screen.")]
        public bool showControlHints =true;

        [Tooltip("Hide hands when disabling them.")]
        public bool hideHandsAtSwitch;

        [Tooltip("Reset hand position and rotation when enabling them.")]
        public bool resetHandsAtSwitch = true;

        [Tooltip("Lock the mouse cursor to the game window when the mouse movement key is pressed.")]
        public bool lockMouseToView = true;

        [Header("Adjustments")] [Tooltip("Adjust hand movement speed.")]
        public float handMoveMultiplier = 0.2f;

        [Tooltip("Adjust hand rotation speed.")]
        public float handRotationMultiplier = 0.5f;

        [Tooltip("Adjust player movement speed.")]
        public float playerMoveMultiplier = 2;

        [Tooltip("Adjust player rotation speed.")]
        public float playerRotationMultiplier = 0.5f;

        [Header("Operation Key Bindings")] [Tooltip("Key used to enable mouse input if a button press is required.")]
        public KeyCode mouseMovementKey = KeyCode.Mouse2;

        [Tooltip("Key used to toggle control hints on/off.")]
        public KeyCode toggleControlHints = KeyCode.F1;

        [Tooltip("Key used to switch between left and righ hand.")]
        public KeyCode changeHands = KeyCode.Tab;

        [Tooltip("Key used to switch hands On/Off.")]
        public KeyCode handsOnOff = KeyCode.LeftAlt;

        [Tooltip("Key used to switch between positional and rotational movement.")]
        public KeyCode rotationPosition = KeyCode.LeftShift;

        [Tooltip("Key used to switch between X/Y and X/Z axis.")]
        public KeyCode changeAxis = KeyCode.LeftControl;

        [Header("Movement Key Bindings")] [Tooltip("Key used to move forward.")]
        public KeyCode moveForward = KeyCode.W;

        [Tooltip("Key used to move to the left.")]
        public KeyCode moveLeft = KeyCode.A;

        [Tooltip("Key used to move backwards.")]
        public KeyCode moveBackward = KeyCode.S;

        [Tooltip("Key used to move to the right.")]
        public KeyCode moveRight = KeyCode.D;

        [Header("Controller Key Bindings")] [Tooltip("Key used to simulate trigger button.")]
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
        public KeyCode touchModifier =  KeyCode.T;

        [Tooltip("Key used to switch between hair touch mode.")]
        public KeyCode hairTouchModifier = KeyCode.H;

        [Tooltip("Key used to toggle radius limit")]
        public KeyCode RadiusLimitKey = KeyCode.Alpha0;

        [Tooltip("Key used increase speed multipliers")]
        public KeyCode IncreaseMultipliersKey = KeyCode.Greater;

        [Tooltip("Key used decrease speed multipliers")]
        public KeyCode DecreaseMultipliersKey = KeyCode.Less;

        #endregion

        #region Private fields

        private bool isHand;
        private Text hintText;
        private SDK_PlayStationMoveController currentHand;
        private Vector3 oldPos;

        [SerializeField] private float controllerRadiusLimit = 1;

        #endregion

        private void SetUpEditorSimulator()
        {
        
            hintCanvas = Instantiate(Resources.Load("PlayStation Editor Control Hints", typeof(GameObject))) as GameObject;
            if (hintCanvas != null) {
                hintText = hintCanvas.GetComponentInChildren<Text>();
                hintCanvas.SetActive(showControlHints);
            }
            oldPos = Input.mousePosition;

            SDK_PlayStationVRController controllerSDK =
                VRTK_SDK_Bridge.GetControllerSDK() as SDK_PlayStationVRController;
            if (controllerSDK == null)
            {
                return;
            }
            Dictionary<SDK_PlayStationMoveController.PlayStationKeys, KeyCode> keyMappings =
                new Dictionary<SDK_PlayStationMoveController.PlayStationKeys, KeyCode>
                {
                    {SDK_PlayStationMoveController.PlayStationKeys.Trigger, triggerAlias},
                    {SDK_PlayStationMoveController.PlayStationKeys.Middle, gripAlias},
                    {SDK_PlayStationMoveController.PlayStationKeys.Cross, buttonOneAlias},
                    {SDK_PlayStationMoveController.PlayStationKeys.Circle, buttonTwoAlias},
                    {SDK_PlayStationMoveController.PlayStationKeys.Start, startMenuAlias},
                    {SDK_PlayStationMoveController.PlayStationKeys.Triangle, touchModifier},
                    {SDK_PlayStationMoveController.PlayStationKeys.Square, hairTouchModifier}
                };
            controllerSDK.SetKeyMappings(keyMappings);
            rightController.ActiveController = true;
            leftController.ActiveController = false;
            currentHand = rightController;
        }

        private void UpdateSimulator()
        {
            if (Input.GetKeyDown(toggleControlHints))
            {
                showControlHints = !showControlHints;
                hintCanvas.SetActive(showControlHints);
            }
            RadiusLimiter();
            MultiplierInputMonitor();


            if (lockMouseToView)
            {
                Cursor.lockState = Input.GetKey(mouseMovementKey) ? CursorLockMode.Locked : CursorLockMode.None;
            }
            else if (Input.GetKeyDown(mouseMovementKey))
            {
                oldPos = Input.mousePosition;
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
                if (currentHand.ControllerType == SDK_PlayStationMoveController.Controller.Secondary)
                {
                    currentHand = rightController;
                    rightController.ActiveController = true;
                    leftController.ActiveController = false;
                }
                else
                {
                    currentHand = leftController;
                    rightController.ActiveController = false;
                    leftController.ActiveController = true;
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

            if (showControlHints)
            {
                UpdateHints();
            }
        }

        private void UpdateHands()
        {
            Vector3 mouseDiff = GetMouseDelta();

            if (!IsAcceptingMouseInput())
            {
                return;
            }
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

        private void RadiusLimiter()
        {
            Vector3 playerChest = currentHand.transform.parent.position + Vector3.up * -.17f;

            float distance = Vector3.Distance(playerChest, currentHand.transform.position);
            if (!(distance > controllerRadiusLimit))
            {
                return;
            }
            Vector3 deltaVector3 = playerChest - currentHand.transform.position;
            deltaVector3 = deltaVector3.normalized;
            deltaVector3 *= distance - controllerRadiusLimit;
            currentHand.transform.position += deltaVector3;
        }

        private void MultiplierInputMonitor()
        {
            if (Input.GetKeyDown(IncreaseMultipliersKey))
            {
                AdjustMultipliers(.1f);
            }
            if (Input.GetKeyDown(DecreaseMultipliersKey))
            {
                AdjustMultipliers(-.1f);
            }
        }

        private void AdjustMultipliers(float value)
        {
            handMoveMultiplier += value;

            handRotationMultiplier += value;

            playerMoveMultiplier += value;

            playerRotationMultiplier += value;
        }

        private void UpdateRotation()
        {
            Vector3 mouseDiff = GetMouseDelta();

            if (!IsAcceptingMouseInput())
            {
                return;
            }
            Vector3 rot = transform.rotation.eulerAngles;
            rot.y += (mouseDiff * playerRotationMultiplier).x;
            transform.localRotation = Quaternion.Euler(rot);

            rot = myCamera.rotation.eulerAngles;

            if (rot.x > 180)
            {
                rot.x -= 360;
            }

            if (!(rot.x < 80) || !(rot.x > -80))
            {
                return;
            }
            rot.x += (mouseDiff * playerRotationMultiplier).y * -1;
            rot.x = Mathf.Clamp(rot.x, -79, 79);
            myCamera.rotation = Quaternion.Euler(rot);
        }

        private void UpdatePosition()
        {
            if (Input.GetKey(moveForward))
            {
                transform.Translate(transform.forward * Time.deltaTime * playerMoveMultiplier, Space.World);
            }
            else if (Input.GetKey(moveBackward))
            {
                transform.Translate(-transform.forward * Time.deltaTime * playerMoveMultiplier, Space.World);
            }
            if (Input.GetKey(moveLeft))
            {
                transform.Translate(-transform.right * Time.deltaTime * playerMoveMultiplier, Space.World);
            }
            else if (Input.GetKey(moveRight))
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
                rightHand.transform.localPosition = new Vector3(0.15f, -.17f, 0.34f);
                rightHand.transform.localRotation = Quaternion.identity;
                leftHand.transform.localPosition = new Vector3(-0.15f, -.17f, 0.34f);
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
            if(hintText == null)
            {
                return;
            }
            string hints = "";
            Func<KeyCode, string> key = k => "<b>" + k.ToString() + "</b>";

            string mouseInputRequires = "";

            mouseInputRequires = " (" + key(mouseMovementKey) + ")";


            // WASD Movement
            string WASD = moveForward + moveLeft.ToString() + moveBackward + moveRight;
            hints += "<b>" + WASD + "</b>: " + "Move Player/Playspace\n";

            if (isHand)
            {
                // Controllers
                if (Input.GetKey(rotationPosition))
                {
                    hints += "Mouse: Controller Rotation" + mouseInputRequires + "\n";
                }
                else
                {
                    hints += "Mouse: Controller Position" + mouseInputRequires + "\n";
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

                hints += "Button Press Mode Modifiers: Touch ("
                         + key(touchModifier)
                         + "), Hair Touch ("
                         + key(hairTouchModifier)
                         + ")\n";

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
                hints += "Mouse: HMD Rotation" + mouseInputRequires + "\n";
                hints += "Modes: Controller (" + key(handsOnOff) + ")\n";
            }

            hintText.text = hints.TrimEnd();
        }

        private bool IsAcceptingMouseInput()
        {
            return Input.GetKey(mouseMovementKey);
        }

        private Vector3 GetMouseDelta()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                return new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
            Vector3 mouseDiff = Input.mousePosition - oldPos;
            oldPos = Input.mousePosition;
            return mouseDiff;
        }


    }
}