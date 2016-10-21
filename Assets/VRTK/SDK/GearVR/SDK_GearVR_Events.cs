using System;
using System.Collections;
using UnityEngine;

namespace VRTK
{
    public class SDK_GearVR_Events : VRTK_ControllerEvents
    {
        /// <summary>
        /// Swipe directions (see 
        /// <see cref="GetSwipeDirection"/>
        /// )
        /// </summary>
        public enum SwipeDirection
        {
            Forward,
            Backward,
            Up,
            Down,
            None,
        };

        /// <summary>
        /// Actions that can be linked to controller input
        /// </summary>
        /// <param name="Pointer_Toggle">Turning a laser pointer on / off.</param>
        /// <param name="Pointer_Set">Setting a destination marker from the cursor position of the pointer.</param>
        /// <param name="Grab_Toggle">Grabbing game objects.</param>
        /// <param name="Use_Toggle">Using game objects.</param>
        /// <param name="Ui_Click">Clicking a UI element.</param>
        /// <param name="Menu_Toggle">Bringing up an in-game menu.</param>
        /// <param name="Undefined">No action</param>
        public enum ActionType
        {
            Pointer_Toggle,
            Pointer_Set,
            Grab_Toggle,
            Use_Toggle,
            Ui_Click,
            Menu_Toggle,
            Undefined
        };

        /// <summary>
        /// GearVR input alias
        /// </summary>
        /// <param name ="DPad_Tap">D-Pad is quickly touched and released</param>
        /// <param name ="DPad_Hold">D-Pad is is touched and the figer is held down</param>
        /// <param name ="DPad_SwipeForward">D-Pad is swiped forward</param>
        /// <param name ="DPad_SwipeBackward">D-Pad is swiped backward</param>
        /// <param name ="DPad_SwipeUp">D-Pad is swiped up</param>
        /// <param name ="DPad_SwipeDown">D-Pad is swiped down</param>
        /// <param name ="Back_Click">Back button is clicked</param>
        /// <param name ="Back_Hold">Back button is held down</param>
        /// <param name ="Undefined">No input specified</param>
        public enum InputAlias
        {
            DPad_Tap,
            DPad_Hold,
            DPad_SwipeForward,
            DPad_SwipeBackward,
            DPad_SwipeUp,
            DPad_SwipeDown,
            Back_Click,
            Back_Hold,
            Undefined
        };

        /// <summary>
        /// Input from the device.
        /// </summary>
        /// <remarks>This is an issue for device abstraction.
        /// It should be extensible, maybe definig it as a class:
        /// public class DeviceInputTypes
        /// {
        ///  // base class does not define any value
        /// }
        /// public class GearVrInputTypes : DeviceInputTypes
        /// {
        ///     public const int DPad = 0;
        ///     public const int BackButton = 1;
        /// }
        /// </remarks>
        public enum InputTypes
        {
            DPad,
            BackButton
        }

        [Header("GearVR")]

        [Tooltip("The button to use for the action of turning a laser pointer on / off.")]
        public InputAlias pointerToggleInput = InputAlias.Back_Click;
        [Tooltip("The button to use for the action of setting a destination marker from the cursor position of the pointer.")]
        public InputAlias pointerSetInput = InputAlias.DPad_SwipeForward;
        [Tooltip("The button to use for the action of grabbing game objects.")]
        public InputAlias grabToggleInput = InputAlias.DPad_Tap;
        [Tooltip("The button to use for the action of using game objects.")]
        public InputAlias useToggleInput = InputAlias.DPad_Hold;
        [Tooltip("The button to use for the action of clicking a UI element.")]
        public InputAlias uiClickInput = InputAlias.DPad_Tap;
        [Tooltip("The button to use for the action of bringing up an in-game menu.")]
        public InputAlias menuToggleInput = InputAlias.Back_Hold;

        [Tooltip("Error tolerance for swipe direction")]
        [Range(0.1f, 0.4f)]
        public float swipeDirTolerance = 0.3f;
        [Tooltip("Threshold for swipe movemet detection")]
        [Range(0.1f, 0.9f)]
        public float swipeDetectThreshold = 0.2f;

        [Tooltip("Delay for a D-Pad hold down event")]
        public float dPadHoldActivationTime = 0.75f;

        [Tooltip("Delay for a Back button hold down event")]
        public float backHoldActivationTime = 0.75f;

        /// <summary>
        /// Emitted when the back button is pressed
        /// </summary>
        public event ControllerInteractionEventHandler BackButtonPress;
        /// <summary>
        /// Emitted when the back button is released
        /// </summary>
        public event ControllerInteractionEventHandler BackButtonRelease;
        /// <summary>
        /// Emitted when the back button is held down
        /// </summary>
        public event ControllerInteractionEventHandler BackButtonHold;
        /// <summary>
        /// Emitted when the back button is clicked
        /// </summary>
        public event ControllerInteractionEventHandler BackButtonClick;
        /// <summary>
        /// Emitted when swiping the D-Pad
        /// </summary>
        public event ControllerInteractionEventHandler DpadSwipe;
        /// <summary>
        /// Emitted when the D-Pad is touched
        /// </summary>
        public event ControllerInteractionEventHandler DpadTouchStart;
        /// <summary>
        /// Emitted when the D-Pad is released
        /// </summary>
        public event ControllerInteractionEventHandler DpadTouchEnd;
        /// <summary>
        /// Emitted when the D-Pad is tapped
        /// </summary>
        public event ControllerInteractionEventHandler DpadTap;
        /// <summary>
        /// Emitted when the finger is held down on the D-Pad
        /// </summary>
        public event ControllerInteractionEventHandler DpadHold;

        /// <summary>
        /// Convert a swipe direction to an angle.
        /// <see cref="AngleToSwipeDirection"/>
        /// </summary>
        /// <param name="swipe"></param>
        /// <returns>Return an angle of 0 (Forward),90 (Up),180 (Backward) or 270 (Down)</returns>
        public static float SwipeDirectionToAngle(SwipeDirection swipe)
        {
            switch (swipe)
            {
                case SwipeDirection.Forward:
                    return 0;
                case SwipeDirection.Up:
                    return 90;
                case SwipeDirection.Backward:
                    return 180;
                case SwipeDirection.Down:
                    return 270;
            }
            return -1;
        }

        /// <summary>
        /// Convert an angle to a swipe direction
        /// <see cref="SwipeDirectionToAngle"/>
        /// </summary>
        /// <param name="angle">It should be 0 (Forward),90 (Up),180 (Backward) or 270 (Down)</param>
        /// <returns>The swipe direction related to the angle (undefined if the ancgle is not 0,90,180 or 270)</returns>
        public static SwipeDirection AngleToSwipeDirection(float angle)
        {
            switch ((int)angle)
            {
                case 0:
                    return SwipeDirection.Forward;
                case 90:
                    return SwipeDirection.Up;
                case 180:
                    return SwipeDirection.Backward;
                case 270:
                    return SwipeDirection.Down;
            }
            return SwipeDirection.None;
        }

        /// <summary>
        ///  Detect the direction between the down and the given positions.
        /// </summary>
        /// <param name="initPos">Initial touch position</param>
        /// <param name="currPos">Current touch position</param>
        /// <param name="detectThreshold">Threshold below which the movements are ignored</param>
        /// <param name="dirTolerance">Tolerance on the shift from the main movement [0..1]</param>
        /// <returns>The swipe direction</returns>
        public static SwipeDirection GetSwipeDirection(Vector2 initPos, Vector2 currPos, float detectThreshold = 0f, float dirTolerance = 0.3f)
        {
            Vector2 dragOffset = currPos - initPos;
            if (dragOffset.magnitude < detectThreshold)
            {
                return SwipeDirection.None;
            }
            Vector2 swipeOffset = dragOffset.normalized;

            bool verticalSwipe = Mathf.Abs(swipeOffset.x) < dirTolerance;

            bool horizontalSwipe = Mathf.Abs(swipeOffset.y) < dirTolerance;

            if (swipeOffset.y > 0f && verticalSwipe)
            {
                return SwipeDirection.Up;
            }

            if (swipeOffset.y < 0f && verticalSwipe)
            {
                return SwipeDirection.Down;
            }

            if (swipeOffset.x > 0f && horizontalSwipe)
            {
                return SwipeDirection.Forward;
            }

            if (swipeOffset.x < 0f && horizontalSwipe)
            {
                return SwipeDirection.Backward;
            }

            return SwipeDirection.None;
        }

        /// <summary>
        /// Calculate the angle of the segment from the center of the dpad to the current coordinates.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns>Return the computed angle.</returns>
        /// <remarks>An identical method is already implemented as private method in VRTK_ControllerEvents,
        /// it could be a service method in a possible abstract ControllerEvent class.</remarks>
        public static float CalculateDpadAxisAngle(Vector2 axis)
        {
            float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
            angle = 90.0f - angle;
            if (angle < 0)
            {
                angle += 360.0f;
            }
            return angle;
        }

        /// <summary>
        /// Check if the back button id pressed or the D-Pad is touched.
        /// </summary>
        /// <param name="inputType">Type of input (back button or D-Pad)</param>
        /// <param name="pressed">If true check if it is pressed, else check if it was released</param>
        /// <returns>Return true if the checked state matches the actual state.</returns>
        public static bool IsGearVrButtonPressed(InputTypes inputType, bool pressed)
        {
            switch (inputType)
            {
                case InputTypes.DPad:
                    if (pressed)
                    {
                        return Input.GetButtonDown("Fire1");
                    }
                    else
                    {
                        return Input.GetButtonUp("Fire1");
                    }
                case InputTypes.BackButton:
                    if (pressed)
                    {
                        return Input.GetButtonDown("Cancel");
                    }
                    else
                    {
                        return Input.GetButtonUp("Cancel");
                    }
            }
            return false;
        }


        public virtual void OnBackClick(ControllerInteractionEventArgs e)
        {
            if (BackButtonClick != null)
            {
                BackButtonClick(this, e);
            }
        }

        public virtual void OnBackHold(ControllerInteractionEventArgs e)
        {
            if (BackButtonHold != null)
            {
                BackButtonHold(this, e);
            }
        }

        public virtual void OnDpadSwipe(ControllerInteractionEventArgs e)
        {
            if (DpadSwipe != null)
            {
                DpadSwipe(this, e);
            }
        }

        public virtual void OnDpadHold(ControllerInteractionEventArgs e)
        {
            if (DpadHold != null)
            {
                DpadHold(this, e);
            }
        }

        public virtual void OnDpadTap(ControllerInteractionEventArgs e)
        {
            if (DpadTap != null)
            {
                DpadTap(this, e);
            }
        }

        /// <summary>
        /// Convert an input alias to the related action, according to the configuration.
        /// </summary>
        public ActionType GetInputAction(InputAlias inputAlias)
        {
            if (pointerToggleInput == inputAlias)
            {
                return ActionType.Pointer_Toggle;
            }
            if (pointerSetInput == inputAlias)
            {
                return ActionType.Pointer_Set;
            }
            if (grabToggleInput == inputAlias)
            {
                return ActionType.Grab_Toggle;
            }
            if (useToggleInput == inputAlias)
            {
                return ActionType.Use_Toggle;
            }
            if (uiClickInput == inputAlias)
            {
                return ActionType.Ui_Click;
            }
            if (menuToggleInput == inputAlias)
            {
                return ActionType.Menu_Toggle;
            }
            return ActionType.Undefined;
        }

        protected Vector2 downPosition;
        protected Vector2 upPosition;

        /// <summary>
        /// Detect the direction between the down and the up positions.
        /// </summary>
        /// <returns>The swipe direction</returns>
        protected SwipeDirection DetectSwipe()
        {
            return GetSwipeDirection(downPosition, upPosition, swipeDetectThreshold, swipeDirTolerance);
        }

        /// <summary>
        /// Do the given action emitting the relates alias events
        /// </summary>
        /// <param name="action">The action type</param>
        /// <param name="e">Event payload</param>
        /// <param name="on">Toggle on/off</param>
        protected void DoAction(ActionType action, ControllerInteractionEventArgs e, bool on)
        {
            switch (action)
            {
                case ActionType.Pointer_Toggle:
                    if (on)
                    {
                        pointerPressed = true;
                        OnAliasPointerOn(e);
                    }
                    else
                    {
                        pointerPressed = false;
                        OnAliasPointerOff(e);
                    }
                    break;
                case ActionType.Pointer_Set:
                    if (on)
                    {
                        OnAliasPointerSet(e);
                    }
                    break;
                case ActionType.Grab_Toggle:
                    if (on)
                    {
                        grabPressed = true;
                        OnAliasGrabOn(e);
                    }
                    else
                    {
                        grabPressed = false;
                        OnAliasGrabOff(e);
                    }
                    break;
                case ActionType.Use_Toggle:
                    if (on)
                    {
                        usePressed = true;
                        OnAliasUseOn(e);
                    }
                    else
                    {
                        usePressed = false;
                        OnAliasUseOff(e);
                    }
                    break;
                case ActionType.Ui_Click:
                    if (on)
                    {
                        uiClickPressed = true;
                        OnAliasUIClickOn(e);
                    }
                    else
                    {
                        uiClickPressed = false;
                        OnAliasUIClickOff(e);
                    }
                    break;
                case ActionType.Menu_Toggle:
                    if (on)
                    {
                        menuPressed = true;
                        OnAliasMenuOn(e);
                    }
                    else
                    {
                        menuPressed = false;
                        OnAliasMenuOff(e);
                    }
                    break;
                case ActionType.Undefined:
                    break;
            }
        }

        /// <summary>
        /// Build an event payload.
        /// </summary>
        /// <param name="value">button pressed or released</param>
        /// <returns></returns>
        /// <remarks>A similar method is already implemented as private method in VRTK_ControllerEvents,
        /// it could be a service method in a possible abstract ControllerEvent class.</remarks>
        protected ControllerInteractionEventArgs SetGearVrButtonEvent(bool value)
        {
            ControllerInteractionEventArgs e;
            e.controllerIndex = 0;
            e.buttonPressure = value ? 1f : 0f;
            Vector3 touchPos = Vector3.zero;
            touchPos.x = (GetDpadCoords().x - Screen.width / 2) / Screen.width;
            touchPos.y = (GetDpadCoords().y - Screen.height / 2) / Screen.height;
            e.touchpadAxis = touchPos;
            e.touchpadAngle = CalculateDpadAxisAngle(e.touchpadAxis);
            return e;
        }

        /// <summary>
        /// Update method called by Unity Engine.
        /// </summary>
        /// <remarks>It makes sense to declare this method virtual and let derived classes access and/or override the base class method.</remarks>
        protected virtual void Update()
        {
            // Back button
            if (IsGearVrButtonPressed(InputTypes.BackButton, true))
            {
                OnBackButtonPressed(SetGearVrButtonEvent(true));
            }
            else if (IsGearVrButtonPressed(InputTypes.BackButton, false))
            {
                OnBackButtonReleased(SetGearVrButtonEvent(false));
            }

            // D-Pad
            if (IsGearVrButtonPressed(InputTypes.DPad, true))
            {
                OnDpadTouchStart(SetGearVrButtonEvent(true));
            }
            else if (IsGearVrButtonPressed(InputTypes.DPad, false))
            {
                OnDpadTouchEnd(SetGearVrButtonEvent(false));
            }
        }

        private bool dPadTouched = false;
        private bool dPadHeld = false;
        private bool backPressed = false;
        private bool backHeld = false;
        private float dPadHoldTime = 0f;
        private float dBackHoldTime = 0f;

        private void OnDpadTouchStart(ControllerInteractionEventArgs e)
        {
            dPadHoldTime = Time.time;
            downPosition = GetDpadCoords();
            dPadTouched = true;
            uiClickPressed = true;
            OnAliasUIClickOn(e);
            if (DpadTouchStart != null)
            {
                DpadTouchStart(this, e);
            }
            StartCoroutine(HandleDpadTouch(e));
        }

        private void OnDpadTouchEnd(ControllerInteractionEventArgs e)
        {
            if (DpadTouchEnd != null)
            {
                DpadTouchEnd(this, e);
            }
            dPadTouched = false;
            uiClickPressed = false;
            OnAliasUIClickOff(e);
            // default swipe direction = none
            SwipeDirection swipe = SwipeDirection.None;
            upPosition = GetDpadCoords();
            swipe = DetectSwipe();
            if (swipe != SwipeDirection.None)
            {
                Debug.Log("Swipe detected: " + swipe);
                e.touchpadAngle = SwipeDirectionToAngle(swipe);
                StartCoroutine(HandleDpadSwipe(e));
            }
            else if (dPadHeld)
            {
                DoAction(GetInputAction(InputAlias.DPad_Hold), e, false);
            }
            else
            {
                Debug.Log("D-Pad tap detected");
                StartCoroutine(HandleDpadTap(e));
            }
            dPadHeld = false;
        }


        private void OnBackButtonPressed(ControllerInteractionEventArgs e)
        {
            dBackHoldTime = Time.time;
            if (BackButtonPress != null)
            {
                BackButtonPress(this, e);
            }
            backPressed = true;
            StartCoroutine(HandleBackPress(e));
        }

        private void OnBackButtonReleased(ControllerInteractionEventArgs e)
        {
            backPressed = false;
            if (BackButtonRelease != null)
            {
                BackButtonRelease(this, e);
            }
            if (backHeld)
            {
                Debug.Log("Back hold released");
                DoAction(GetInputAction(InputAlias.Back_Hold), e, false);
            }
            else
            {
                Debug.Log("Back click detected");
                StartCoroutine(HandleBackClick(e));
            }
            backHeld = false;
        }

        private IEnumerator HandleDpadTouch(ControllerInteractionEventArgs e)
        {
            SwipeDirection swipe = SwipeDirection.None;
            while (Time.time < dPadHoldTime + dPadHoldActivationTime)
            {
                if (!dPadTouched)
                {
                    dPadHoldTime = 0;
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }
            dPadHoldTime = 0;
            if (dPadTouched)
            {
                swipe = SwipeDirection.None;
                swipe = DetectSwipe();
                if (swipe == SwipeDirection.None)
                {
                    Debug.Log("D-Pad hold detected");
                    DoAction(GetInputAction(InputAlias.DPad_Hold), e, true);
                    dPadHeld = true;
                    OnDpadHold(e);
                }
            }
        }

        private IEnumerator HandleBackPress(ControllerInteractionEventArgs e)
        {
            while (Time.time < dBackHoldTime + backHoldActivationTime)
            {
                if (!backPressed)
                {
                    dBackHoldTime = 0;
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }
            dBackHoldTime = 0;
            if (backPressed)
            {
                Debug.Log("Back hold detected");
                backHeld = true;
                DoAction(GetInputAction(InputAlias.Back_Hold), e, true);
                OnBackHold(e);
            }
        }

        private IEnumerator HandleBackClick(ControllerInteractionEventArgs e)
        {
            ActionType backClickAction = GetInputAction(InputAlias.Back_Click);
            DoAction(backClickAction, e, true);
            yield return new WaitForFixedUpdate();
            DoAction(backClickAction, e, false);
            OnBackClick(e);
        }

        private IEnumerator HandleDpadTap(ControllerInteractionEventArgs e)
        {
            ActionType dPadTapAction = GetInputAction(InputAlias.DPad_Tap);
            DoAction(dPadTapAction, e, true);
            yield return new WaitForFixedUpdate();
            DoAction(dPadTapAction, e, false);
            OnDpadTap(e);
        }

        private IEnumerator HandleDpadSwipe(ControllerInteractionEventArgs e)
        {
            switch (AngleToSwipeDirection(e.touchpadAngle))
            {
                case SwipeDirection.Forward:
                    {
                        ActionType swipeForwardAction = GetInputAction(InputAlias.DPad_SwipeForward);
                        DoAction(swipeForwardAction, e, true);
                        yield return new WaitForFixedUpdate();
                        DoAction(swipeForwardAction, e, false);
                    }
                    break;
                case SwipeDirection.Backward:
                    {
                        ActionType swipeBackwardAction = GetInputAction(InputAlias.DPad_SwipeBackward);
                        DoAction(swipeBackwardAction, e, true);
                        yield return new WaitForFixedUpdate();
                        DoAction(swipeBackwardAction, e, false);
                    }
                    break;
                case SwipeDirection.Up:
                    {
                        ActionType swipeUpAction = GetInputAction(InputAlias.DPad_SwipeUp);
                        DoAction(swipeUpAction, e, true);
                        yield return new WaitForFixedUpdate();
                        DoAction(swipeUpAction, e, false);
                    }
                    break;
                case SwipeDirection.Down:
                    {
                        ActionType swipeDownAction = GetInputAction(InputAlias.DPad_SwipeDown);
                        DoAction(swipeDownAction, e, true);
                        yield return new WaitForFixedUpdate();
                        DoAction(swipeDownAction, e, false);
                    }
                    break;
            }
            OnDpadSwipe(e);
        }



        protected Vector2 GetDpadCoords()
        {
            Vector2 coords = Vector2.zero;
            // On GearVR forward seems to be -x and backward +x
#if UNITY_ANDROID && !UNITY_EDITOR
            coords.x = -Input.mousePosition.x;
            coords.y = Input.mousePosition.y;
#else
            // else (e.g. in Unity Editor) D-Pad can be simulated with the mouse
            coords = Input.mousePosition;
#endif
            return coords;
        }

        private void Start()
        {
            // disable any unused button
            pointerToggleButton = ButtonAlias.Undefined;
            grabToggleButton = ButtonAlias.Undefined;
            useToggleButton = ButtonAlias.Undefined;
            uiClickButton = ButtonAlias.Undefined;
            menuToggleButton = ButtonAlias.Undefined;
            // fix a problem with VRTK_WorldPointer with a workaround
            // let the pointerSetButton be different from pointerToggleButton
            pointerSetButton = ButtonAlias.Trigger_Press;
            // change needed in VRTK_WorldPointer.InvalidConstantBeam(): do not consider
            // pointerSetButton equal to pointerToggleButton if they are both Undefined
        }

    }
}
