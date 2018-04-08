// Avatar Hands|Prefabs|0150
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections;

    [Serializable]
    public sealed class AxisOverrides
    {
        /// <summary>
        /// Determine when to apply the override.
        /// </summary>
        public enum ApplyOverrideType
        {
            /// <summary>
            /// Never apply the override.
            /// </summary>
            Never,
            /// <summary>
            /// Always apply the override.
            /// </summary>
            Always,
            /// <summary>
            /// Only apply the override when the state is set to digital.
            /// </summary>
            DigitalState,
            /// <summary>
            /// Only apply the override when the state is set to axis.
            /// </summary>
            AxisState,
            /// <summary>
            /// Only apply the override when the state is set to sense axis.
            /// </summary>
            SenseAxisState,
            /// <summary>
            /// Only apply the override when the state is set to axis or sense axis.
            /// </summary>
            AxisAndSenseAxisState
        }

        [Header("Global Override Settings")]

        [Tooltip("Determines whether to ignore all of the given overrides on an Interaction event.")]
        public bool ignoreAllOverrides = true;
        [Tooltip("Sets the Animation parameter for the interaction type and can be used to change the Idle pose based on interaction type.")]
        public float stateValue = -1f;

        [Header("Thumb Override Settings")]

        [Tooltip("Determines when to apply the given thumb override.")]
        public ApplyOverrideType applyThumbOverride = ApplyOverrideType.Always;
        [Tooltip("The axis override for the thumb on an Interact Touch event. Will only be applicable if the thumb button state is not touched.")]
        [Range(0f, 1f)]
        public float thumbOverride;

        [Header("Index Finger Override Settings")]

        [Tooltip("Determines when to apply the given index finger override.")]
        public ApplyOverrideType applyIndexOverride = ApplyOverrideType.Always;
        [Tooltip("The axis override for the index finger on an Interact Touch event. Will only be applicable if the index finger button state is not touched.")]
        [Range(0f, 1f)]
        public float indexOverride;

        [Header("Middle Finger Override Settings")]

        [Tooltip("Determines when to apply the given middle finger override.")]
        public ApplyOverrideType applyMiddleOverride = ApplyOverrideType.Always;
        [Tooltip("The axis override for the middle finger on an Interact Touch event. Will only be applicable if the middle finger button state is not touched.")]
        [Range(0f, 1f)]
        public float middleOverride;

        [Header("Ring Finger Override Settings")]

        [Tooltip("Determines when to apply the given ring finger override.")]
        public ApplyOverrideType applyRingOverride = ApplyOverrideType.Always;
        [Tooltip("The axis override for the ring finger on an Interact Touch event. Will only be applicable if the ring finger button state is not touched.")]
        [Range(0f, 1f)]
        public float ringOverride;

        [Header("Pinky Finger Override Settings")]

        [Tooltip("Determines when to apply the given pinky finger override.")]
        public ApplyOverrideType applyPinkyOverride = ApplyOverrideType.Always;
        [Tooltip("The axis override for the pinky finger on an Interact Touch event.  Will only be applicable if the pinky finger button state is not touched.")]
        [Range(0f, 1f)]
        public float pinkyOverride;
    }

    /// <summary>
    /// Provides a custom controller hand model with psuedo finger functionality.
    /// </summary>
    /// <remarks>
    /// **Prefab Usage:**
    ///  * Place the `VRTK/Prefabs/AvatarHands/BasicHands/VRTK_BasicHand` prefab as a child of either the left or right script alias.
    ///  * If the prefab is being used in the left hand then check the `Mirror Model` parameter.
    ///  * By default, the avatar hand controller will detect which controller is connected and represent it accordingly.
    ///  * Optionally, use SDKTransformModify scripts to adjust the hand orientation based on different controller types.
    /// </remarks>
    /// <example>
    /// `032_Controller_CustomControllerModel` uses the `VRTK_BasicHand` prefab to display custom avatar hands for the left and right controller.
    /// </example>
    public class VRTK_AvatarHandController : MonoBehaviour
    {
        [Header("Hand Settings")]

        [Tooltip("The controller type to use for default finger settings.")]
        public SDK_BaseController.ControllerType controllerType;
        [Tooltip("Determines whether the Finger and State settings are auto set based on the connected controller type.")]
        public bool setFingersForControllerType = true;
        [Tooltip("If this is checked then the model will be mirrored, tick this if the avatar hand is for the left hand controller.")]
        public bool mirrorModel = false;
        [Tooltip("The speed in which a finger will transition to it's destination position if the finger state is `Digital`.")]
        public float animationSnapSpeed = 0.1f;

        [Header("Digital Finger Settings")]

        [Tooltip("The button alias to control the thumb if the thumb state is `Digital`.")]
        public VRTK_ControllerEvents.ButtonAlias thumbButton = VRTK_ControllerEvents.ButtonAlias.TouchpadTouch;
        [Tooltip("The button alias to control the index finger if the index finger state is `Digital`.")]
        public VRTK_ControllerEvents.ButtonAlias indexButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
        [Tooltip("The button alias to control the middle finger if the middle finger state is `Digital`.")]
        public VRTK_ControllerEvents.ButtonAlias middleButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The button alias to control the ring finger if the ring finger state is `Digital`.")]
        public VRTK_ControllerEvents.ButtonAlias ringButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The button alias to control the pinky finger if the pinky finger state is `Digital`.")]
        public VRTK_ControllerEvents.ButtonAlias pinkyButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The button alias to control the middle, ring and pinky finger if the three finger state is `Digital`.")]
        public VRTK_ControllerEvents.ButtonAlias threeFingerButton = VRTK_ControllerEvents.ButtonAlias.GripPress;

        [Header("Axis Finger Settings")]

        [Tooltip("The button type to listen for axis changes to control the thumb.")]
        public SDK_BaseController.ButtonTypes thumbAxisButton = SDK_BaseController.ButtonTypes.Touchpad;
        [Tooltip("The button type to listen for axis changes to control the index finger.")]
        public SDK_BaseController.ButtonTypes indexAxisButton = SDK_BaseController.ButtonTypes.Trigger;
        [Tooltip("The button type to listen for axis changes to control the middle finger.")]
        public SDK_BaseController.ButtonTypes middleAxisButton = SDK_BaseController.ButtonTypes.MiddleFinger;
        [Tooltip("The button type to listen for axis changes to control the ring finger.")]
        public SDK_BaseController.ButtonTypes ringAxisButton = SDK_BaseController.ButtonTypes.RingFinger;
        [Tooltip("The button type to listen for axis changes to control the pinky finger.")]
        public SDK_BaseController.ButtonTypes pinkyAxisButton = SDK_BaseController.ButtonTypes.PinkyFinger;
        [Tooltip("The button type to listen for axis changes to control the middle, ring and pinky finger.")]
        public SDK_BaseController.ButtonTypes threeFingerAxisButton = SDK_BaseController.ButtonTypes.Grip;

        [Header("Finger State Settings")]

        [Tooltip("The Axis Type to utilise when dealing with the thumb state. Not all controllers support all axis types on all of the available buttons.")]
        public VRTK_ControllerEvents.AxisType thumbState = VRTK_ControllerEvents.AxisType.Digital;
        public VRTK_ControllerEvents.AxisType indexState = VRTK_ControllerEvents.AxisType.Digital;
        public VRTK_ControllerEvents.AxisType middleState = VRTK_ControllerEvents.AxisType.Digital;
        public VRTK_ControllerEvents.AxisType ringState = VRTK_ControllerEvents.AxisType.Digital;
        public VRTK_ControllerEvents.AxisType pinkyState = VRTK_ControllerEvents.AxisType.Digital;
        public VRTK_ControllerEvents.AxisType threeFingerState = VRTK_ControllerEvents.AxisType.Digital;

        [Header("Finger Axis Overrides")]

        [Tooltip("Finger axis overrides on an Interact NearTouch event.")]
        public AxisOverrides nearTouchOverrides;
        [Tooltip("Finger axis overrides on an Interact Touch event.")]
        public AxisOverrides touchOverrides;
        [Tooltip("Finger axis overrides on an Interact Grab event.")]
        public AxisOverrides grabOverrides;
        [Tooltip("Finger axis overrides on an Interact Use event.")]
        public AxisOverrides useOverrides;

        [Header("Custom Settings")]

        [Tooltip("The Transform that contains the avatar hand model. If this is left blank then a child GameObject named `Model` will be searched for to use as the Transform.")]
        public Transform handModel;
        [Tooltip("The controller to listen for the events on. If this is left blank as it will be auto populated by finding the Controller Events script on the parent GameObject.")]
        public VRTK_ControllerEvents controllerEvents;
        [Tooltip("An optional Interact NearTouch to listen for near touch events on. If this is left blank as it will attempt to be auto populated by finding the Interact NearTouch script on the parent GameObject.")]
        public VRTK_InteractNearTouch interactNearTouch;
        [Tooltip("An optional Interact Touch to listen for touch events on. If this is left blank as it will attempt to be auto populated by finding the Interact Touch script on the parent GameObject.")]
        public VRTK_InteractTouch interactTouch;
        [Tooltip("An optional Interact Grab to listen for grab events on. If this is left blank as it will attempt to be auto populated by finding the Interact Grab script on the parent GameObject.")]
        public VRTK_InteractGrab interactGrab;
        [Tooltip("An optional Interact Use to listen for use events on. If this is left blank as it will attempt to be auto populated by finding the Interact Use script on the parent GameObject.")]
        public VRTK_InteractUse interactUse;

        #region Protected class variables
        protected Animator animator;

        protected enum OverrideState
        {
            NoOverride,
            IsOverriding,
            WasOverring,
            KeepOverring
        }

        // Index Finger Mapping: 0 = thumb, 1 = index, 2 = middle, 3 = ring, 4 = pinky
        protected bool[] fingerStates = new bool[5];
        protected bool[] fingerChangeStates = new bool[5];
        protected float[] fingerAxis = new float[5];
        protected float[] fingerRawAxis = new float[5];
        protected float[] fingerUntouchedAxis = new float[5];
        protected float[] fingerSaveAxis = new float[5];
        protected float[] fingerForceAxis = new float[5];

        protected OverrideState[] overrideAxisValues = new OverrideState[5];
        protected VRTK_ControllerEvents.AxisType[] axisTypes = new VRTK_ControllerEvents.AxisType[5];
        protected Coroutine[] fingerAnimationRoutine = new Coroutine[5];

        protected VRTK_ControllerEvents.ButtonAlias savedThumbButtonState = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias savedIndexButtonState = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias savedMiddleButtonState = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias savedRingButtonState = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias savedPinkyButtonState = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias savedThreeFingerButtonState = VRTK_ControllerEvents.ButtonAlias.Undefined;

        protected SDK_BaseController.ButtonTypes savedThumbAxisButtonState;
        protected SDK_BaseController.ButtonTypes savedIndexAxisButtonState;
        protected SDK_BaseController.ButtonTypes savedMiddleAxisButtonState;
        protected SDK_BaseController.ButtonTypes savedRingAxisButtonState;
        protected SDK_BaseController.ButtonTypes savedPinkyAxisButtonState;
        protected SDK_BaseController.ButtonTypes savedThreeFingerAxisButtonState;

        protected VRTK_ControllerReference controllerReference;

        #endregion Protected class variables

        #region MonoBehaviour methods
        protected virtual void OnEnable()
        {
            animator = GetComponent<Animator>();
            controllerEvents = (controllerEvents != null ? controllerEvents : GetComponentInParent<VRTK_ControllerEvents>());
            interactNearTouch = (interactNearTouch != null ? interactNearTouch : GetComponentInParent<VRTK_InteractNearTouch>());
            interactTouch = (interactTouch != null ? interactTouch : GetComponentInParent<VRTK_InteractTouch>());
            interactGrab = (interactGrab != null ? interactGrab : GetComponentInParent<VRTK_InteractGrab>());
            interactUse = (interactUse != null ? interactUse : GetComponentInParent<VRTK_InteractUse>());

            controllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvents.gameObject);
        }

        protected virtual void OnDisable()
        {
            UnsubscribeEvents();
            controllerType = SDK_BaseController.ControllerType.Undefined;
            for (int i = 0; i < fingerAnimationRoutine.Length; i++)
            {
                if (fingerAnimationRoutine[i] != null)
                {
                    fingerAnimationRoutine[i] = null;
                }
            }
        }

        protected virtual void Update()
        {
            if (controllerType == SDK_BaseController.ControllerType.Undefined)
            {
                DetectController();
            }

            if (animator != null)
            {
                ProcessFinger(thumbState, 0);
                ProcessFinger(indexState, 1);
                ProcessFinger(middleState, 2);
                ProcessFinger(ringState, 3);
                ProcessFinger(pinkyState, 4);
            }
        }
        #endregion MonoBehaviour methods

        protected virtual void SubscribeButtonEvent(VRTK_ControllerEvents.ButtonAlias buttonType, ref VRTK_ControllerEvents.ButtonAlias saveType, ControllerInteractionEventHandler eventHandler)
        {
            if (buttonType != VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                saveType = buttonType;
                controllerEvents.SubscribeToButtonAliasEvent(buttonType, true, eventHandler);
                controllerEvents.SubscribeToButtonAliasEvent(buttonType, false, eventHandler);
            }
        }

        protected virtual void UnsubscribeButtonEvent(VRTK_ControllerEvents.ButtonAlias buttonType, ControllerInteractionEventHandler eventHandler)
        {
            if (buttonType != VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                controllerEvents.UnsubscribeToButtonAliasEvent(buttonType, true, eventHandler);
                controllerEvents.UnsubscribeToButtonAliasEvent(buttonType, false, eventHandler);
            }
        }

        protected virtual void SubscribeButtonAxisEvent(SDK_BaseController.ButtonTypes buttonType, ref SDK_BaseController.ButtonTypes saveType, VRTK_ControllerEvents.AxisType axisType, ControllerInteractionEventHandler eventHandler)
        {
            saveType = buttonType;
            controllerEvents.SubscribeToAxisAliasEvent(buttonType, axisType, eventHandler);
        }

        protected virtual void UnsubscribeButtonAxisEvent(SDK_BaseController.ButtonTypes buttonType, VRTK_ControllerEvents.AxisType axisType, ControllerInteractionEventHandler eventHandler)
        {
            controllerEvents.UnsubscribeToAxisAliasEvent(buttonType, axisType, eventHandler);
        }

        #region Subscription Managers
        protected virtual void SubscribeEvents()
        {
            if (controllerEvents != null)
            {
                SubscribeButtonEvent(thumbButton, ref savedThumbButtonState, DoThumbEvent);
                SubscribeButtonEvent(indexButton, ref savedIndexButtonState, DoIndexEvent);
                SubscribeButtonEvent(middleButton, ref savedMiddleButtonState, DoMiddleEvent);
                SubscribeButtonEvent(ringButton, ref savedRingButtonState, DoRingEvent);
                SubscribeButtonEvent(pinkyButton, ref savedPinkyButtonState, DoPinkyEvent);
                SubscribeButtonEvent(threeFingerButton, ref savedThreeFingerButtonState, DoThreeFingerEvent);

                SubscribeButtonAxisEvent(thumbAxisButton, ref savedThumbAxisButtonState, thumbState, DoThumbAxisEvent);
                SubscribeButtonAxisEvent(indexAxisButton, ref savedIndexAxisButtonState, indexState, DoIndexAxisEvent);
                SubscribeButtonAxisEvent(middleAxisButton, ref savedMiddleAxisButtonState, middleState, DoMiddleAxisEvent);
                SubscribeButtonAxisEvent(ringAxisButton, ref savedRingAxisButtonState, ringState, DoRingAxisEvent);
                SubscribeButtonAxisEvent(pinkyAxisButton, ref savedPinkyAxisButtonState, pinkyState, DoPinkyAxisEvent);
                SubscribeButtonAxisEvent(threeFingerAxisButton, ref savedThreeFingerAxisButtonState, threeFingerState, DoThreeFingerAxisEvent);
            }

            if (interactNearTouch != null)
            {
                interactNearTouch.ControllerNearTouchInteractableObject += DoControllerNearTouch;
                interactNearTouch.ControllerNearUntouchInteractableObject += DoControllerNearUntouch;
            }

            if (interactTouch != null)
            {
                interactTouch.ControllerTouchInteractableObject += DoControllerTouch;
                interactTouch.ControllerUntouchInteractableObject += DoControllerUntouch;
            }

            if (interactGrab != null)
            {
                interactGrab.ControllerGrabInteractableObject += DoControllerGrab;
                interactGrab.ControllerUngrabInteractableObject += DoControllerUngrab;
            }

            if (interactUse != null)
            {
                interactUse.ControllerUseInteractableObject += DoControllerUse;
                interactUse.ControllerUnuseInteractableObject += DoControllerUnuse;
            }
        }

        protected virtual void UnsubscribeEvents()
        {
            if (controllerEvents != null)
            {
                UnsubscribeButtonEvent(savedThumbButtonState, DoThumbEvent);
                UnsubscribeButtonEvent(savedIndexButtonState, DoIndexEvent);
                UnsubscribeButtonEvent(savedMiddleButtonState, DoMiddleEvent);
                UnsubscribeButtonEvent(savedRingButtonState, DoRingEvent);
                UnsubscribeButtonEvent(savedPinkyButtonState, DoPinkyEvent);
                UnsubscribeButtonEvent(savedThreeFingerButtonState, DoThreeFingerEvent);

                UnsubscribeButtonAxisEvent(savedThumbAxisButtonState, thumbState, DoThumbAxisEvent);
                UnsubscribeButtonAxisEvent(savedIndexAxisButtonState, indexState, DoIndexAxisEvent);
                UnsubscribeButtonAxisEvent(savedMiddleAxisButtonState, middleState, DoMiddleAxisEvent);
                UnsubscribeButtonAxisEvent(savedRingAxisButtonState, ringState, DoRingAxisEvent);
                UnsubscribeButtonAxisEvent(savedPinkyAxisButtonState, pinkyState, DoPinkyAxisEvent);
                UnsubscribeButtonAxisEvent(savedThreeFingerAxisButtonState, threeFingerState, DoThreeFingerAxisEvent);
            }

            if (interactNearTouch != null)
            {
                interactNearTouch.ControllerNearTouchInteractableObject -= DoControllerNearTouch;
                interactNearTouch.ControllerNearUntouchInteractableObject -= DoControllerNearUntouch;
            }

            if (interactTouch != null)
            {
                interactTouch.ControllerTouchInteractableObject -= DoControllerTouch;
                interactTouch.ControllerUntouchInteractableObject -= DoControllerUntouch;
            }

            if (interactGrab != null)
            {
                interactGrab.ControllerGrabInteractableObject -= DoControllerGrab;
                interactGrab.ControllerUngrabInteractableObject -= DoControllerUngrab;
            }

            if (interactUse != null)
            {
                interactUse.ControllerUseInteractableObject -= DoControllerUse;
                interactUse.ControllerUnuseInteractableObject -= DoControllerUnuse;
            }
        }
        #endregion Subscription Managers

        #region Event methods
        protected virtual void SetFingerEvent(int fingerIndex, ControllerInteractionEventArgs e)
        {
            if (overrideAxisValues[fingerIndex] == OverrideState.NoOverride)
            {
                fingerChangeStates[fingerIndex] = true;
                fingerStates[fingerIndex] = (e.buttonPressure == 0f ? false : true);
            }
        }

        protected virtual void SetFingerAxisEvent(int fingerIndex, ControllerInteractionEventArgs e)
        {
            fingerRawAxis[fingerIndex] = e.buttonPressure;
            if (overrideAxisValues[fingerIndex] == OverrideState.NoOverride)
            {
                fingerAxis[fingerIndex] = e.buttonPressure;
            }
        }

        protected virtual void DoThumbEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerEvent(0, e);
        }

        protected virtual void DoIndexEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerEvent(1, e);
        }

        protected virtual void DoMiddleEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerEvent(2, e);
        }

        protected virtual void DoRingEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerEvent(3, e);
        }

        protected virtual void DoPinkyEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerEvent(4, e);
        }

        protected virtual void DoThreeFingerEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerEvent(2, e);
            SetFingerEvent(3, e);
            SetFingerEvent(4, e);
        }

        protected virtual void DoThumbAxisEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerAxisEvent(0, e);
        }

        protected virtual void DoIndexAxisEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerAxisEvent(1, e);
        }

        protected virtual void DoMiddleAxisEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerAxisEvent(2, e);
        }

        protected virtual void DoRingAxisEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerAxisEvent(3, e);
        }

        protected virtual void DoPinkyAxisEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerAxisEvent(4, e);
        }

        protected virtual void DoThreeFingerAxisEvent(object sender, ControllerInteractionEventArgs e)
        {
            SetFingerAxisEvent(2, e);
            SetFingerAxisEvent(3, e);
            SetFingerAxisEvent(4, e);
        }

        protected virtual bool IsButtonPressed(int arrayIndex)
        {
            float minValue = (axisTypes[arrayIndex] == VRTK_ControllerEvents.AxisType.SenseAxis && controllerEvents != null ? controllerEvents.senseAxisPressThreshold : 0f);
            return (fingerStates[arrayIndex] || fingerRawAxis[arrayIndex] > minValue);
        }

        protected virtual void SaveFingerAxis(int arrayIndex, float updateAxis)
        {
            fingerSaveAxis[arrayIndex] = (fingerSaveAxis[arrayIndex] != fingerForceAxis[arrayIndex] ? updateAxis : fingerSaveAxis[arrayIndex]);
        }

        protected virtual void HandleOverrideOn(bool ignoreAllOverrides, float[] givenFingerAxis, bool[] overridePermissions, float[] overrideValues)
        {
            if (!ignoreAllOverrides)
            {
                for (int i = 0; i < overrideAxisValues.Length; i++)
                {
                    if (overridePermissions[i] && !IsButtonPressed(i) && overrideAxisValues[i] != OverrideState.WasOverring)
                    {
                        SetOverrideValue(i, ref overrideAxisValues, OverrideState.IsOverriding);
                        if (overrideAxisValues[i] == OverrideState.NoOverride)
                        {
                            fingerUntouchedAxis[i] = givenFingerAxis[i];
                        }
                        SaveFingerAxis(i, givenFingerAxis[i]);
                        fingerForceAxis[i] = overrideValues[i];
                    }
                }
            }
        }

        protected virtual void HandleOverrideOff(bool ignoreAllOverrides, bool[] overridePermissions, bool keepOverriding)
        {
            if (!ignoreAllOverrides)
            {
                for (int i = 0; i < fingerAxis.Length; i++)
                {
                    if (overridePermissions[i] && !IsButtonPressed(i) && overrideAxisValues[i] == OverrideState.IsOverriding)
                    {
                        SetOverrideValue(i, ref overrideAxisValues, (keepOverriding ? OverrideState.KeepOverring : OverrideState.WasOverring));
                        fingerAxis[i] = fingerForceAxis[i];
                        fingerForceAxis[i] = fingerSaveAxis[i];
                    }
                }
            }
        }

        protected virtual float CorrectOverrideValue(float givenOverride)
        {
            return (givenOverride == 0f ? 0.0001f : givenOverride);
        }

        protected virtual bool ApplyFingerOverrides(AxisOverrides.ApplyOverrideType overrideType, int arrayIndex)
        {
            if (overrideType == AxisOverrides.ApplyOverrideType.Always ||
                (overrideType == AxisOverrides.ApplyOverrideType.DigitalState && axisTypes[arrayIndex] == VRTK_ControllerEvents.AxisType.Digital) ||
                (overrideType == AxisOverrides.ApplyOverrideType.AxisState && axisTypes[arrayIndex] == VRTK_ControllerEvents.AxisType.Axis) ||
                (overrideType == AxisOverrides.ApplyOverrideType.SenseAxisState && axisTypes[arrayIndex] == VRTK_ControllerEvents.AxisType.SenseAxis) ||
                (overrideType == AxisOverrides.ApplyOverrideType.AxisAndSenseAxisState && (axisTypes[arrayIndex] == VRTK_ControllerEvents.AxisType.Axis || axisTypes[arrayIndex] == VRTK_ControllerEvents.AxisType.SenseAxis)))
            {
                return true;
            }
            return false;
        }

        protected virtual bool[] GetOverridePermissions(AxisOverrides overrideType)
        {
            bool[] overrides = new bool[]
            {
                ApplyFingerOverrides(overrideType.applyThumbOverride, 0),
                ApplyFingerOverrides(overrideType.applyIndexOverride, 1),
                ApplyFingerOverrides(overrideType.applyMiddleOverride, 2),
                ApplyFingerOverrides(overrideType.applyRingOverride, 3),
                ApplyFingerOverrides(overrideType.applyPinkyOverride, 4)
            };
            return overrides;
        }

        protected virtual float[] GetOverrideValues(AxisOverrides overrideType)
        {
            float[] overrides = new float[]
            {
                CorrectOverrideValue(overrideType.thumbOverride),
                CorrectOverrideValue(overrideType.indexOverride),
                CorrectOverrideValue(overrideType.middleOverride),
                CorrectOverrideValue(overrideType.ringOverride),
                CorrectOverrideValue(overrideType.pinkyOverride)
            };
            return overrides;
        }

        protected virtual void SetAnimatorStateOn(string state, AxisOverrides overrides)
        {
            animator.SetFloat(state, (overrides.ignoreAllOverrides ? -1f : overrides.stateValue));
        }

        protected virtual void SetAnimatorStateOff(string state)
        {
            animator.SetFloat(state, -1f);
        }

        protected virtual void DoControllerNearTouch(object sender, ObjectInteractEventArgs e)
        {
            if (interactTouch != null && interactTouch.GetTouchedObject() == null)
            {
                SetAnimatorStateOn("NearTouchState", nearTouchOverrides);
                HandleOverrideOn(nearTouchOverrides.ignoreAllOverrides, fingerAxis, GetOverridePermissions(nearTouchOverrides), GetOverrideValues(nearTouchOverrides));
            }
        }

        protected virtual void DoControllerNearUntouch(object sender, ObjectInteractEventArgs e)
        {
            if (interactNearTouch.GetNearTouchedObjects().Count == 0 && (interactTouch == null || interactTouch.GetTouchedObject() == null))
            {
                for (int i = 0; i < fingerUntouchedAxis.Length; i++)
                {
                    if (!IsButtonPressed(i))
                    {
                        SetOverrideValue(i, ref overrideAxisValues, OverrideState.WasOverring);
                        fingerForceAxis[i] = fingerUntouchedAxis[i];
                    }
                }
                SetAnimatorStateOff("NearTouchState");
                HandleOverrideOff(nearTouchOverrides.ignoreAllOverrides, GetOverridePermissions(nearTouchOverrides), false);
            }
        }

        protected virtual void DoControllerTouch(object sender, ObjectInteractEventArgs e)
        {
            SetAnimatorStateOn("TouchState", touchOverrides);
            HandleOverrideOn(touchOverrides.ignoreAllOverrides, fingerAxis, GetOverridePermissions(touchOverrides), GetOverrideValues(touchOverrides));
        }

        protected virtual void DoControllerUntouch(object sender, ObjectInteractEventArgs e)
        {
            if (interactNearTouch == null || nearTouchOverrides.ignoreAllOverrides)
            {
                for (int i = 0; i < fingerUntouchedAxis.Length; i++)
                {
                    if (!IsButtonPressed(i))
                    {
                        SetOverrideValue(i, ref overrideAxisValues, OverrideState.WasOverring);
                        fingerForceAxis[i] = fingerUntouchedAxis[i];
                    }
                }
            }
            SetAnimatorStateOff("TouchState");
            HandleOverrideOff(touchOverrides.ignoreAllOverrides, GetOverridePermissions(touchOverrides), false);
        }

        protected virtual void DoControllerGrab(object sender, ObjectInteractEventArgs e)
        {
            bool isUsing = (interactUse != null && interactUse.GetUsingObject() != null);
            float[] overrideValues = (GetOverrideValues((isUsing ? useOverrides : grabOverrides)));
            float[] overrideFingerAxis = (isUsing ? GetOverrideValues(grabOverrides) : fingerAxis);
            SetAnimatorStateOn("GrabState", grabOverrides);
            HandleOverrideOn(grabOverrides.ignoreAllOverrides, overrideFingerAxis, GetOverridePermissions(grabOverrides), overrideValues);
        }

        protected virtual void DoControllerUngrab(object sender, ObjectInteractEventArgs e)
        {
            SetAnimatorStateOff("GrabState");
            HandleOverrideOff(grabOverrides.ignoreAllOverrides, GetOverridePermissions(touchOverrides), false);
        }

        protected virtual void DoControllerUse(object sender, ObjectInteractEventArgs e)
        {
            bool isGrabbing = (interactGrab != null && interactGrab.GetGrabbedObject() != null);
            float[] overrideFingerAxis = (isGrabbing ? GetOverrideValues(grabOverrides) : fingerAxis);
            SetAnimatorStateOn("UseState", useOverrides);
            HandleOverrideOn(useOverrides.ignoreAllOverrides, overrideFingerAxis, GetOverridePermissions(useOverrides), GetOverrideValues(useOverrides));
        }

        protected virtual void DoControllerUnuse(object sender, ObjectInteractEventArgs e)
        {
            SetAnimatorStateOff("UseState");
            HandleOverrideOff(useOverrides.ignoreAllOverrides, GetOverridePermissions(useOverrides), true);
        }
        #endregion Event methods

        protected virtual void DetectController()
        {
            controllerType = VRTK_DeviceFinder.GetCurrentControllerType(controllerReference);
            if (controllerType != SDK_BaseController.ControllerType.Undefined)
            {
                if (setFingersForControllerType)
                {
                    switch (controllerType)
                    {
                        case SDK_BaseController.ControllerType.SteamVR_ViveWand:
                        case SDK_BaseController.ControllerType.SteamVR_WindowsMRController:
                        case SDK_BaseController.ControllerType.WindowsMR_MotionController:
                            thumbState = VRTK_ControllerEvents.AxisType.Digital;
                            indexState = VRTK_ControllerEvents.AxisType.Axis;
                            middleState = VRTK_ControllerEvents.AxisType.Digital;
                            ringState = VRTK_ControllerEvents.AxisType.Digital;
                            pinkyState = VRTK_ControllerEvents.AxisType.Digital;
                            threeFingerState = VRTK_ControllerEvents.AxisType.Digital;
                            break;
                        case SDK_BaseController.ControllerType.Oculus_OculusTouch:
                        case SDK_BaseController.ControllerType.SteamVR_OculusTouch:
                            thumbState = VRTK_ControllerEvents.AxisType.Digital;
                            indexState = VRTK_ControllerEvents.AxisType.Axis;
                            middleState = VRTK_ControllerEvents.AxisType.Digital;
                            ringState = VRTK_ControllerEvents.AxisType.Digital;
                            pinkyState = VRTK_ControllerEvents.AxisType.Digital;
                            threeFingerState = VRTK_ControllerEvents.AxisType.Axis;
                            break;
                        case SDK_BaseController.ControllerType.SteamVR_ValveKnuckles:
                            thumbState = VRTK_ControllerEvents.AxisType.Digital;
                            indexState = VRTK_ControllerEvents.AxisType.SenseAxis;
                            middleState = VRTK_ControllerEvents.AxisType.SenseAxis;
                            ringState = VRTK_ControllerEvents.AxisType.SenseAxis;
                            pinkyState = VRTK_ControllerEvents.AxisType.SenseAxis;
                            threeFingerState = VRTK_ControllerEvents.AxisType.SenseAxis;
                            threeFingerAxisButton = SDK_BaseController.ButtonTypes.StartMenu;
                            break;
                        default:
                            thumbState = VRTK_ControllerEvents.AxisType.Digital;
                            indexState = VRTK_ControllerEvents.AxisType.Digital;
                            middleState = VRTK_ControllerEvents.AxisType.Digital;
                            ringState = VRTK_ControllerEvents.AxisType.Digital;
                            pinkyState = VRTK_ControllerEvents.AxisType.Digital;
                            threeFingerState = VRTK_ControllerEvents.AxisType.Digital;
                            break;
                    }
                }
                UnsubscribeEvents();
                SubscribeEvents();
                if (mirrorModel)
                {
                    mirrorModel = false;
                    MirrorHand();
                }
            }
        }

        protected virtual void MirrorHand()
        {
            Transform modelTransform = (handModel != null ? handModel : transform.Find("Model"));
            if (modelTransform != null)
            {
                modelTransform.localScale = new Vector3(modelTransform.localScale.x * -1f, modelTransform.localScale.y, modelTransform.localScale.z);
            }
        }

        protected virtual void SetOverrideValue(int stateIndex, ref OverrideState[] overrideState, OverrideState stateValue)
        {
            overrideState[stateIndex] = stateValue;
        }

        protected virtual void ProcessFinger(VRTK_ControllerEvents.AxisType state, int arrayIndex)
        {
            axisTypes[arrayIndex] = state;
            if (overrideAxisValues[arrayIndex] != OverrideState.NoOverride)
            {
                if (fingerAxis[arrayIndex] != fingerForceAxis[arrayIndex])
                {
                    LerpChangePosition(arrayIndex, fingerAxis[arrayIndex], fingerForceAxis[arrayIndex], animationSnapSpeed);
                }
                else if (overrideAxisValues[arrayIndex] == OverrideState.WasOverring)
                {
                    SetOverrideValue(arrayIndex, ref overrideAxisValues, OverrideState.NoOverride);
                }
            }
            else
            {
                if (state == VRTK_ControllerEvents.AxisType.Digital)
                {
                    if (fingerChangeStates[arrayIndex])
                    {
                        fingerChangeStates[arrayIndex] = false;
                        float startAxis = (fingerStates[arrayIndex] ? 0f : 1f);
                        float targetAxis = (fingerStates[arrayIndex] ? 1f : 0f);
                        LerpChangePosition(arrayIndex, startAxis, targetAxis, animationSnapSpeed);
                    }
                }
                else
                {
                    SetFingerPosition(arrayIndex, fingerAxis[arrayIndex]);
                }
            }

            //Final sanity check, if you're not touching anything but the override is still set, then clear the override.
            if (((interactTouch == null && interactNearTouch == null) || (interactNearTouch == null && interactTouch.GetTouchedObject() == null) || (interactNearTouch != null && interactNearTouch.GetNearTouchedObjects().Count == 0)) && overrideAxisValues[arrayIndex] != OverrideState.NoOverride)
            {
                SetOverrideValue(arrayIndex, ref overrideAxisValues, OverrideState.NoOverride);
            }
        }

        protected virtual void LerpChangePosition(int arrayIndex, float startPosition, float targetPosition, float speed)
        {
            fingerAnimationRoutine[arrayIndex] = StartCoroutine(ChangePosition(arrayIndex, startPosition, targetPosition, speed));

        }

        protected virtual IEnumerator ChangePosition(int arrayIndex, float startAxis, float targetAxis, float time)
        {
            float elapsedTime = 0f;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                float currentAxis = Mathf.Lerp(startAxis, targetAxis, (elapsedTime / time));
                SetFingerPosition(arrayIndex, currentAxis);
                yield return null;
            }
            SetFingerPosition(arrayIndex, targetAxis);
            fingerAnimationRoutine[arrayIndex] = null;
        }

        protected virtual void SetFingerPosition(int arrayIndex, float axis)
        {
            int animationLayer = arrayIndex + 1;
            animator.SetLayerWeight(animationLayer, axis);
            fingerAxis[arrayIndex] = axis;
            if (overrideAxisValues[arrayIndex] == OverrideState.WasOverring)
            {
                SetOverrideValue(arrayIndex, ref overrideAxisValues, OverrideState.NoOverride);
            }
        }
    }
}