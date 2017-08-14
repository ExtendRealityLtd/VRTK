// SDK Transform Modify|Utilities|90064
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class VRTK_SDKTransformModifiers
    {
        [Header("SDK settings")]

        [Tooltip("An optional SDK Setup to use to determine when to modify the transform.")]
        public VRTK_SDKSetup loadedSDKSetup;
        [Tooltip("An optional SDK controller type to use to determine when to modify the transform.")]
        public SDK_BaseController.ControllerType controllerType;

        [Header("Transform Override Settings")]

        [Tooltip("The new local position to change the transform to.")]
        public Vector3 position = Vector3.zero;
        [Tooltip("The new local rotation in eular angles to change the transform to.")]
        public Vector3 rotation = Vector3.zero;
        [Tooltip("The new local scale to change the transform to.")]
        public Vector3 scale = Vector3.one;
    }

    /// <summary>
    /// The SDK Transform Modify can be used to change a transform orientation at runtime based on the currently used SDK or SDK controller.
    /// </summary>
    public class VRTK_SDKTransformModify : MonoBehaviour
    {
        [Tooltip("The target transform to modify on enable. If this is left blank then the transform the script is attached to will be used.")]
        public Transform target;
        [Tooltip("A collection of SDK Transform overrides to change the given target transform for each specified SDK.")]
        public List<VRTK_SDKTransformModifiers> sdkOverrides = new List<VRTK_SDKTransformModifiers>();

        protected VRTK_SDKManager sdkManager;
        protected int findControllerAttempts = 25;
        protected float findControllerAttemptsDelay = 0.1f;
        protected Coroutine attemptFindControllerModel;

        /// <summary>
        /// The UpdateTransform method updates the Transform data on the current GameObject for the specified settings.
        /// </summary>
        /// <param name="controllerReference">An optional reference to the controller to update the transform with.</param>
        public virtual void UpdateTransform(VRTK_ControllerReference controllerReference = null)
        {
            if (target == null)
            {
                return;
            }

            VRTK_SDKTransformModifiers selectedModifier = GetSelectedModifier(controllerReference);

            //If a modifier is found then change the transform
            if (selectedModifier != null)
            {
                target.localPosition = selectedModifier.position;
                target.localEulerAngles = selectedModifier.rotation;
                target.localScale = selectedModifier.scale;
            }
        }

        protected virtual void OnEnable()
        {
            target = (target != null ? target : transform);
            sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                sdkManager.LoadedSetupChanged += LoadedSetupChanged;
                if (sdkManager.loadedSetup != null)
                {
                    FindController();
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (sdkManager != null && !gameObject.activeSelf)
            {
                sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
            }

            if (attemptFindControllerModel != null)
            {
                StopCoroutine(attemptFindControllerModel);
                attemptFindControllerModel = null;
            }
        }

        protected virtual void OnDestroy()
        {
            if (sdkManager != null)
            {
                sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
            }
        }

        protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            FindController();
        }

        protected virtual void FindController()
        {
            if (gameObject.activeInHierarchy)
            {
                attemptFindControllerModel = StartCoroutine(AttemptFindController(findControllerAttempts, findControllerAttemptsDelay));
            }
        }

        protected virtual IEnumerator AttemptFindController(int attempts, float delay)
        {
            WaitForSeconds delayInstruction = new WaitForSeconds(delay);
            SDK_BaseController.ControllerType controllerType = VRTK_DeviceFinder.GetCurrentControllerType();

            while (controllerType == SDK_BaseController.ControllerType.Undefined && attempts > 0)
            {
                controllerType = VRTK_DeviceFinder.GetCurrentControllerType();
                attempts--;
                yield return delayInstruction;
            }
            UpdateTransform();
        }

        protected virtual VRTK_SDKTransformModifiers GetSelectedModifier(VRTK_ControllerReference controllerReference)
        {
            //attempt to find by the overall SDK set up to start with
            VRTK_SDKTransformModifiers selectedModifier = sdkOverrides.FirstOrDefault(item => item.loadedSDKSetup == sdkManager.loadedSetup);

            //If no sdk set up is found or it is null then try and find by the SDK controller
            if (selectedModifier == null)
            {
                SDK_BaseController.ControllerType currentController = VRTK_DeviceFinder.GetCurrentControllerType(controllerReference);
                selectedModifier = sdkOverrides.FirstOrDefault(item => item.controllerType == currentController);
            }
            return selectedModifier;
        }
    }
}