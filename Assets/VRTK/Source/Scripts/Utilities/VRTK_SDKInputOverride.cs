// SDK Input Override|Utilities|90170
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class VRTK_SDKInputOverrideType
    {
        [Header("SDK settings")]

        [Tooltip("An optional SDK Setup to use to determine when to modify the transform.")]
        public VRTK_SDKSetup loadedSDKSetup;
        [Tooltip("An optional SDK controller type to use to determine when to modify the transform.")]
        public SDK_BaseController.ControllerType controllerType;
    }

    [Serializable]
    public class VRTK_SDKButtonInputOverrideType : VRTK_SDKInputOverrideType
    {
        [Header("Button Override")]

        [Tooltip("The button to override to.")]
        public VRTK_ControllerEvents.ButtonAlias overrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
    }

    [Serializable]
    public class VRTK_SDKVector2AxisInputOverrideType : VRTK_SDKInputOverrideType
    {
        [Header("Vector2 Axis Override")]

        [Tooltip("The Vector2 axis to override to.")]
        public VRTK_ControllerEvents.Vector2AxisAlias overrideAxis = VRTK_ControllerEvents.Vector2AxisAlias.Undefined;
    }

    /// <summary>
    /// Provides the ability to switch button mappings based on the current SDK or controller type
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_SDKInputOverride` script on any active scene GameObject.
    ///  * Customise the input button for each script type for each SDK controller type.
    /// </remarks>
    public class VRTK_SDKInputOverride : VRTK_SDKControllerReady
    {
        [Header("Interact Grab")]

        [Tooltip("The Interact Grab script to override the controls on.")]
        public VRTK_InteractGrab interactGrabScript;
        [Tooltip("The list of overrides.")]
        public List<VRTK_SDKButtonInputOverrideType> interactGrabOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("Interact Use")]

        [Tooltip("The Interact Use script to override the controls on.")]
        public VRTK_InteractUse interactUseScript;
        [Tooltip("The list of overrides.")]
        public List<VRTK_SDKButtonInputOverrideType> interactUseOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("Pointer")]

        [Tooltip("The Pointer script to override the controls on.")]
        public VRTK_Pointer pointerScript;
        [Tooltip("The list of overrides for the activation button.")]
        public List<VRTK_SDKButtonInputOverrideType> pointerActivationOverrides = new List<VRTK_SDKButtonInputOverrideType>();
        [Tooltip("The list of overrides for the selection button.")]
        public List<VRTK_SDKButtonInputOverrideType> pointerSelectionOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("UI Pointer")]

        [Tooltip("The UI Pointer script to override the controls on.")]
        public VRTK_UIPointer uiPointerScript;
        [Tooltip("The list of overrides for the activation button.")]
        public List<VRTK_SDKButtonInputOverrideType> uiPointerActivationOverrides = new List<VRTK_SDKButtonInputOverrideType>();
        [Tooltip("The list of overrides for the selection button.")]
        public List<VRTK_SDKButtonInputOverrideType> uiPointerSelectionOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("Pointer Direction Indicator")]

        [Tooltip("The Pointer Direction Indicator script to override the controls on.")]
        public VRTK_PointerDirectionIndicator pointerDirectionIndicatorScript;
        [Tooltip("The list of overrides for the coordinate axis.")]
        public List<VRTK_SDKVector2AxisInputOverrideType> directionIndicatorCoordinateOverrides = new List<VRTK_SDKVector2AxisInputOverrideType>();

        [Header("Touchpad Control")]

        [Tooltip("The Touchpad Control script to override the controls on.")]
        public VRTK_TouchpadControl touchpadControlScript;
        [Tooltip("The list of overrides for the Touchpad Control coordinate axis.")]
        public List<VRTK_SDKVector2AxisInputOverrideType> touchpadControlCoordinateOverrides = new List<VRTK_SDKVector2AxisInputOverrideType>();
        [Tooltip("The list of overrides for the activation button.")]
        public List<VRTK_SDKButtonInputOverrideType> touchpadControlActivationOverrides = new List<VRTK_SDKButtonInputOverrideType>();
        [Tooltip("The list of overrides for the modifier button.")]
        public List<VRTK_SDKButtonInputOverrideType> touchpadControlModifierOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("Button Control")]

        [Tooltip("The ButtonControl script to override the controls on.")]
        public VRTK_ButtonControl buttonControlScript;
        [Tooltip("The list of overrides for the forward button.")]
        public List<VRTK_SDKButtonInputOverrideType> buttonControlForwardOverrides = new List<VRTK_SDKButtonInputOverrideType>();
        [Tooltip("The list of overrides for the backward button.")]
        public List<VRTK_SDKButtonInputOverrideType> buttonControlBackwardOverrides = new List<VRTK_SDKButtonInputOverrideType>();
        [Tooltip("The list of overrides for the left button.")]
        public List<VRTK_SDKButtonInputOverrideType> buttonControlLeftOverrides = new List<VRTK_SDKButtonInputOverrideType>();
        [Tooltip("The list of overrides for the right button.")]
        public List<VRTK_SDKButtonInputOverrideType> buttonControlRightOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("Slingshot Jump")]

        [Tooltip("The SlingshotJump script to override the controls on.")]
        public VRTK_SlingshotJump slingshotJumpScript;
        [Tooltip("The list of overrides for the activation button.")]
        public List<VRTK_SDKButtonInputOverrideType> slingshotJumpActivationOverrides = new List<VRTK_SDKButtonInputOverrideType>();
        [Tooltip("The list of overrides for the cancel button.")]
        public List<VRTK_SDKButtonInputOverrideType> slingshotJumpCancelOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("Move In Place")]

        [Tooltip("The MoveInPlace script to override the controls on.")]
        public VRTK_MoveInPlace moveInPlaceScript;
        [Tooltip("The list of overrides for the engage button.")]
        public List<VRTK_SDKButtonInputOverrideType> moveInPlaceEngageOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        [Header("Step Multiplier")]

        [Tooltip("The Step Multiplier script to override the controls on.")]
        public VRTK_StepMultiplier stepMultiplierScript;
        [Tooltip("The list of overrides for the activation button.")]
        public List<VRTK_SDKButtonInputOverrideType> stepMultiplierActivationOverrides = new List<VRTK_SDKButtonInputOverrideType>();

        /// <summary>
        /// The ForceManage method forces the inputs to be updated even without an SDK change event occuring.
        /// </summary>
        public virtual void ForceManage()
        {
            ManageInputs();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ManageInputs();
        }

        protected override void OnDisable()
        {
            if (!gameObject.activeSelf)
            {
                base.OnDisable();
            }
        }

        protected override void ControllerReady(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_SDKManager.GetLoadedSDKSetup() != null && gameObject.activeInHierarchy)
            {
                ManageInputs();
            }
        }

        protected virtual VRTK_SDKButtonInputOverrideType GetSelectedModifier(List<VRTK_SDKButtonInputOverrideType> overrideTypes, VRTK_ControllerReference controllerReference)
        {
            VRTK_SDKButtonInputOverrideType selectedModifier = null;
            //attempt to find by the overall SDK set up to start with
            if (VRTK_SDKManager.GetLoadedSDKSetup() != null)
            {
                selectedModifier = overrideTypes.FirstOrDefault(item => item.loadedSDKSetup == VRTK_SDKManager.GetLoadedSDKSetup());
            }

            //If no sdk set up is found or it is null then try and find by the SDK controller
            if (selectedModifier == null)
            {
                SDK_BaseController.ControllerType currentControllerType = VRTK_DeviceFinder.GetCurrentControllerType(controllerReference);
                selectedModifier = overrideTypes.FirstOrDefault(item => item.controllerType == currentControllerType);
            }
            return selectedModifier;
        }

        protected virtual VRTK_SDKVector2AxisInputOverrideType GetSelectedModifier(List<VRTK_SDKVector2AxisInputOverrideType> overrideTypes, VRTK_ControllerReference controllerReference)
        {
            //attempt to find by the overall SDK set up to start with
            VRTK_SDKVector2AxisInputOverrideType selectedModifier = overrideTypes.FirstOrDefault(item => item.loadedSDKSetup == VRTK_SDKManager.GetLoadedSDKSetup());

            //If no sdk set up is found or it is null then try and find by the SDK controller
            if (selectedModifier == null)
            {
                SDK_BaseController.ControllerType currentControllerType = VRTK_DeviceFinder.GetCurrentControllerType(controllerReference);
                selectedModifier = overrideTypes.FirstOrDefault(item => item.controllerType == currentControllerType);
            }
            return selectedModifier;
        }

        protected virtual void ManageInputs()
        {
            ManageInteractGrab();
            ManageInteractUse();
            ManagePointer();
            ManageUIPointer();
            ManagePointerDirectionIndicator();
            ManageTouchpadControl();
            ManageButtonControl();
            ManageSlingshotJump();
            ManageMoveInPlace();
            ManageStepMultiplier();
        }

        protected virtual VRTK_ControllerReference GetReferenceFromEvents(VRTK_ControllerEvents controllerEvents)
        {
            return VRTK_ControllerReference.GetControllerReference((controllerEvents != null ? controllerEvents.gameObject : null));
        }

        protected virtual VRTK_ControllerReference GetRightThenLeftReference()
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Right);
            return (VRTK_ControllerReference.IsValid(controllerReference) ? controllerReference : VRTK_ControllerReference.GetControllerReference(SDK_BaseController.ControllerHand.Left));
        }

        protected virtual void ManageInteractGrab()
        {
            if (interactGrabScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(interactGrabScript.controllerEvents);
                VRTK_SDKButtonInputOverrideType selectedModifier = GetSelectedModifier(interactGrabOverrides, controllerReference);
                if (selectedModifier != null && selectedModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    interactGrabScript.enabled = false;
                    interactGrabScript.grabButton = selectedModifier.overrideButton;
                    interactGrabScript.enabled = true;
                }
            }
        }

        protected virtual void ManageInteractUse()
        {
            if (interactUseScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(interactUseScript.controllerEvents);
                VRTK_SDKButtonInputOverrideType selectedModifier = GetSelectedModifier(interactUseOverrides, controllerReference);
                if (selectedModifier != null && selectedModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    interactUseScript.enabled = false;
                    interactUseScript.useButton = selectedModifier.overrideButton;
                    interactUseScript.enabled = true;
                }
            }
        }

        protected virtual void ManagePointer()
        {
            if (pointerScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(pointerScript.controllerEvents);
                VRTK_SDKButtonInputOverrideType selectedActivationModifier = GetSelectedModifier(pointerActivationOverrides, controllerReference);
                if (selectedActivationModifier != null && selectedActivationModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    pointerScript.enabled = false;
                    pointerScript.activationButton = selectedActivationModifier.overrideButton;
                    pointerScript.enabled = true;
                }

                VRTK_SDKButtonInputOverrideType selectedSelectionModifier = GetSelectedModifier(pointerSelectionOverrides, controllerReference);
                if (selectedSelectionModifier != null && selectedSelectionModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    pointerScript.enabled = false;
                    pointerScript.selectionButton = selectedSelectionModifier.overrideButton;
                    pointerScript.enabled = true;
                }
            }
        }

        protected virtual void ManageUIPointer()
        {
            if (uiPointerScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(uiPointerScript.controllerEvents);
                VRTK_SDKButtonInputOverrideType selectedActivationModifier = GetSelectedModifier(uiPointerActivationOverrides, controllerReference);
                if (selectedActivationModifier != null && selectedActivationModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    uiPointerScript.enabled = false;
                    uiPointerScript.activationButton = selectedActivationModifier.overrideButton;
                    uiPointerScript.enabled = true;
                }

                VRTK_SDKButtonInputOverrideType selectedSelectionModifier = GetSelectedModifier(uiPointerSelectionOverrides, controllerReference);
                if (selectedSelectionModifier != null && selectedSelectionModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    uiPointerScript.enabled = false;
                    uiPointerScript.selectionButton = selectedSelectionModifier.overrideButton;
                    uiPointerScript.enabled = true;
                }
            }
        }

        protected virtual void ManagePointerDirectionIndicator()
        {
            if (pointerDirectionIndicatorScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(pointerDirectionIndicatorScript.GetControllerEvents());
                VRTK_SDKVector2AxisInputOverrideType selectedCoorinateModifier = GetSelectedModifier(directionIndicatorCoordinateOverrides, controllerReference);
                if (selectedCoorinateModifier != null && selectedCoorinateModifier.overrideAxis != VRTK_ControllerEvents.Vector2AxisAlias.Undefined)
                {
                    pointerDirectionIndicatorScript.enabled = false;
                    pointerDirectionIndicatorScript.coordinateAxis = selectedCoorinateModifier.overrideAxis;
                    pointerDirectionIndicatorScript.enabled = true;
                }
            }
        }

        protected virtual void ManageTouchpadControl()
        {
            if (touchpadControlScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(touchpadControlScript.controller);
                VRTK_SDKVector2AxisInputOverrideType selectedCoorinateModifier = GetSelectedModifier(touchpadControlCoordinateOverrides, controllerReference);
                if (selectedCoorinateModifier != null && selectedCoorinateModifier.overrideAxis != VRTK_ControllerEvents.Vector2AxisAlias.Undefined)
                {
                    touchpadControlScript.enabled = false;
                    touchpadControlScript.coordinateAxis = selectedCoorinateModifier.overrideAxis;
                    touchpadControlScript.enabled = true;
                }

                VRTK_SDKButtonInputOverrideType selectedActivationModifier = GetSelectedModifier(touchpadControlActivationOverrides, controllerReference);
                if (selectedActivationModifier != null && selectedActivationModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    touchpadControlScript.enabled = false;
                    touchpadControlScript.primaryActivationButton = selectedActivationModifier.overrideButton;
                    touchpadControlScript.enabled = true;
                }

                VRTK_SDKButtonInputOverrideType selectedActionModifier = GetSelectedModifier(touchpadControlModifierOverrides, controllerReference);
                if (selectedActionModifier != null && selectedActionModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    touchpadControlScript.enabled = false;
                    touchpadControlScript.actionModifierButton = selectedActionModifier.overrideButton;
                    touchpadControlScript.enabled = true;
                }
            }
        }

        protected virtual void ManageButtonControl()
        {
            if (buttonControlScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(buttonControlScript.controller);
                VRTK_SDKButtonInputOverrideType selectedForwardModifier = GetSelectedModifier(buttonControlForwardOverrides, controllerReference);
                if (selectedForwardModifier != null && selectedForwardModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    buttonControlScript.enabled = false;
                    buttonControlScript.forwardButton = selectedForwardModifier.overrideButton;
                    buttonControlScript.enabled = true;
                }

                VRTK_SDKButtonInputOverrideType selectedBackwardModifier = GetSelectedModifier(buttonControlBackwardOverrides, controllerReference);
                if (selectedBackwardModifier != null && selectedBackwardModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    buttonControlScript.enabled = false;
                    buttonControlScript.backwardButton = selectedBackwardModifier.overrideButton;
                    buttonControlScript.enabled = true;
                }

                VRTK_SDKButtonInputOverrideType selectedLeftModifier = GetSelectedModifier(buttonControlLeftOverrides, controllerReference);
                if (selectedLeftModifier != null && selectedLeftModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    buttonControlScript.enabled = false;
                    buttonControlScript.leftButton = selectedLeftModifier.overrideButton;
                    buttonControlScript.enabled = true;
                }

                VRTK_SDKButtonInputOverrideType selectedRightModifier = GetSelectedModifier(buttonControlRightOverrides, controllerReference);
                if (selectedRightModifier != null && selectedRightModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    buttonControlScript.enabled = false;
                    buttonControlScript.rightButton = selectedRightModifier.overrideButton;
                    buttonControlScript.enabled = true;
                }
            }
        }

        protected virtual void ManageSlingshotJump()
        {
            if (slingshotJumpScript != null)
            {
                VRTK_ControllerReference controllerReference = GetRightThenLeftReference();
                VRTK_SDKButtonInputOverrideType selectedActivationModifier = GetSelectedModifier(slingshotJumpActivationOverrides, controllerReference);
                if (selectedActivationModifier != null && selectedActivationModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    slingshotJumpScript.enabled = false;
                    slingshotJumpScript.SetActivationButton(selectedActivationModifier.overrideButton);
                    slingshotJumpScript.enabled = true;
                }

                VRTK_SDKButtonInputOverrideType selectedCancelModifier = GetSelectedModifier(slingshotJumpCancelOverrides, controllerReference);
                if (selectedCancelModifier != null && selectedCancelModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    slingshotJumpScript.enabled = false;
                    slingshotJumpScript.SetCancelButton(selectedCancelModifier.overrideButton);
                    slingshotJumpScript.enabled = true;
                }
            }
        }

        protected virtual void ManageMoveInPlace()
        {
            if (moveInPlaceScript != null)
            {
                VRTK_ControllerReference controllerReference = GetRightThenLeftReference();
                VRTK_SDKButtonInputOverrideType selectedEngageModifier = GetSelectedModifier(moveInPlaceEngageOverrides, controllerReference);
                if (selectedEngageModifier != null && selectedEngageModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    moveInPlaceScript.enabled = false;
                    moveInPlaceScript.engageButton = selectedEngageModifier.overrideButton;
                    moveInPlaceScript.enabled = true;
                }
            }
        }

        protected virtual void ManageStepMultiplier()
        {
            if (stepMultiplierScript != null)
            {
                VRTK_ControllerReference controllerReference = GetReferenceFromEvents(stepMultiplierScript.controllerEvents);
                VRTK_SDKButtonInputOverrideType selectedModifier = GetSelectedModifier(stepMultiplierActivationOverrides, controllerReference);
                if (selectedModifier != null && selectedModifier.overrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    stepMultiplierScript.enabled = false;
                    stepMultiplierScript.activationButton = selectedModifier.overrideButton;
                    stepMultiplierScript.enabled = true;
                }
            }
        }
    }
}