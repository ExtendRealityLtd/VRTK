// SDK Transform Modify|Utilities|90150
namespace VRTK
{
    using UnityEngine;
    using System;
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
    [AddComponentMenu("VRTK/Scripts/Utilities/VRTK_SDKTransformModify")]
    public class VRTK_SDKTransformModify : VRTK_SDKControllerReady
    {
        [Tooltip("The target Transform to modify on enable. If this is left blank then the Transform the script is attached to will be used.")]
        public Transform target;
        [Tooltip("If this is checked then the target Transform will be reset to the original orientation when this script is disabled.")]
        public bool resetOnDisable = true;
        [Tooltip("A collection of SDK Transform overrides to change the given target Transform for each specified SDK.")]
        public List<VRTK_SDKTransformModifiers> sdkOverrides = new List<VRTK_SDKTransformModifiers>();

        protected Vector3 originalPosition;
        protected Quaternion originalRotation;
        protected Vector3 originalScale;

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

        /// <summary>
        /// The SetOrigins method sets the original position, rotation, scale of the target Transform.
        /// </summary>
        public virtual void SetOrigins()
        {
            if (target != null)
            {
                originalPosition = target.position;
                originalRotation = target.rotation;
                originalScale = target.localScale;
            }
        }

        protected override void OnEnable()
        {
            target = (target != null ? target : transform);
            SetOrigins();
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (resetOnDisable)
            {
                target.position = originalPosition;
                target.rotation = originalRotation;
                target.localScale = originalScale;
            }
        }

        protected override void ControllerReady(VRTK_ControllerReference controllerReference)
        {
            if (sdkManager != null && sdkManager.loadedSetup != null && gameObject.activeInHierarchy)
            {
                UpdateTransform(controllerReference);
            }
        }

        protected virtual VRTK_SDKTransformModifiers GetSelectedModifier(VRTK_ControllerReference controllerReference)
        {
            //attempt to find by the overall SDK set up to start with
            VRTK_SDKTransformModifiers selectedModifier = sdkOverrides.FirstOrDefault(item => item.loadedSDKSetup == sdkManager.loadedSetup);

            //If no sdk set up is found or it is null then try and find by the SDK controller
            if (selectedModifier == null)
            {
                SDK_BaseController.ControllerType currentControllerType = VRTK_DeviceFinder.GetCurrentControllerType(controllerReference);
                selectedModifier = sdkOverrides.FirstOrDefault(item => item.controllerType == currentControllerType);
            }
            return selectedModifier;
        }
    }
}