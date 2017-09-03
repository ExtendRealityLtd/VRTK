// SDK Object State|Utilities|90160
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Reflection;

    /// <summary>
    /// The SDK Object State script can be used to set the enable/active state of a GameObject or Component based on SDK information.
    /// </summary>
    /// <remarks>
    /// The state can be determined by:
    ///  * The current loaded SDK setup.
    ///  * The current attached Headset type.
    ///  * The current attached Controller type.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Utilities/VRTK_SDKObjectState")]
    public class VRTK_SDKObjectState : MonoBehaviour
    {
        [Header("Target Settings")]

        [Tooltip("The GameObject or Component that is the target of the enable/disable action. If this is left blank then the GameObject that the script is attached to will be used as the `Target`.")]
        public Object target = null;
        [Tooltip("The state to set the `Target` to when this script is enabled. Checking this box will enable/activate the `Target`, unchecking will disable/deactivate the `Target`.")]
        public bool objectState = false;
        [Tooltip("If the currently loaded SDK Setup matches the one provided here then the `Target` state will be set to the desired `Object State`.")]
        public VRTK_SDKSetup loadedSDKSetup = null;
        [Tooltip("If the attached headset type matches the selected headset then the `Target` state will be set to the desired `Object State`.")]
        public VRTK_DeviceFinder.Headsets headsetType = VRTK_DeviceFinder.Headsets.Unknown;
        [Tooltip("If the current controller type matches the selected controller type then the `Target` state will be set to the desired `Object State`.")]
        public SDK_BaseController.ControllerType controllerType = SDK_BaseController.ControllerType.Undefined;

        protected VRTK_SDKManager sdkManager;
        protected Coroutine checkToggleRoutine;
        //TODO: REPLACE WITH GENERIC CONTROLLER TYPE AVAILABLE EVENT
        protected int findControllerAttempts = 100;
        protected float findControllerAttemptsDelay = 0.1f;
        protected Coroutine attemptFindControllerModel;

        /// <summary>
        /// The SetStateByControllerReference method sets the object state based on the controller type of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">A controller reference to check for the controller type of.</param>
        public virtual void SetStateByControllerReference(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                SDK_BaseController.ControllerType foundControllerType = VRTK_DeviceFinder.GetCurrentControllerType(controllerReference);
                if (foundControllerType != SDK_BaseController.ControllerType.Undefined && controllerType == foundControllerType)
                {
                    ToggleObject();
                }
            }
        }

        protected virtual void OnEnable()
        {
            sdkManager = VRTK_SDKManager.instance;
            target = (target != null ? target : gameObject);
            sdkManager.LoadedSetupChanged += LoadedSetupChanged;
            checkToggleRoutine = StartCoroutine(CheckToggleAtEndOfFrame());
        }

        protected virtual void OnDisable()
        {
            sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
            if (checkToggleRoutine != null)
            {
                StopCoroutine(checkToggleRoutine);
            }
            if (attemptFindControllerModel != null)
            {
                StopCoroutine(attemptFindControllerModel);
            }
        }

        protected virtual IEnumerator CheckToggleAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            CheckToggle();
        }

        protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            CheckToggle();
        }

        protected virtual void CheckToggle()
        {
            ToggleOnSDK();
            ToggleOnHeadset();
            ToggleOnController();
        }

        protected virtual void ToggleOnSDK()
        {
            if (loadedSDKSetup != null && loadedSDKSetup == sdkManager.loadedSetup)
            {
                ToggleObject();
            }
        }

        protected virtual void ToggleOnHeadset()
        {
            if (headsetType != VRTK_DeviceFinder.Headsets.Unknown && headsetType == VRTK_DeviceFinder.GetHeadsetType(true))
            {
                ToggleObject();
            }
        }

        protected virtual void ToggleOnController()
        {
            if (controllerType != SDK_BaseController.ControllerType.Undefined)
            {
                attemptFindControllerModel = StartCoroutine(AttemptFindController(findControllerAttempts, findControllerAttemptsDelay));
            }
        }

        protected virtual IEnumerator AttemptFindController(int attempts, float delay)
        {
            WaitForSeconds delayInstruction = new WaitForSeconds(delay);
            SDK_BaseController.ControllerType foundControllerType = VRTK_DeviceFinder.GetCurrentControllerType();

            while (foundControllerType == SDK_BaseController.ControllerType.Undefined && attempts > 0)
            {
                foundControllerType = VRTK_DeviceFinder.GetCurrentControllerType();
                attempts--;
                yield return delayInstruction;
            }

            if (foundControllerType != SDK_BaseController.ControllerType.Undefined && controllerType == foundControllerType)
            {
                ToggleObject();
            }
        }

        protected virtual void ToggleObject()
        {
            if (target is GameObject)
            {
                ToggleGameObject();
            }
            else if (target.GetType().IsSubclassOf(typeof(Component)))
            {
                ToggleComponent();
            }
        }

        protected virtual void ToggleGameObject()
        {
            if (target != null)
            {
                GameObject toggleTarget = (GameObject)target;
                toggleTarget.SetActive(objectState);
            }
        }

        protected virtual void ToggleComponent()
        {
            if (target != null)
            {
                Component toggleTarget = (Component)target;
                PropertyInfo property = toggleTarget.GetType().GetProperty("enabled");
                if (property != null)
                {
                    property.SetValue(toggleTarget, objectState, null);
                }
            }
        }
    }
}