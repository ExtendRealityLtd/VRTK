// SDK Object Alias|Utilities|90063
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The GameObject that the SDK Object Alias script is applied to will become a child of the selected SDK Object.
    /// </summary>
    public class VRTK_SDKObjectAlias : MonoBehaviour
    {
        /// <summary>
        /// Valid SDK Objects
        /// </summary>
        /// <param name="Boundary">The main camera rig/play area object that defines the player boundary.</param>
        /// <param name="Headset">The main headset defines the player head.</param>
        /// <param name="HeadsetCamera">The main headset camera defines the player eyes.</param>
        public enum SDKObject
        {
            Boundary,
            Headset,
            HeadsetCamera
        }

        /// <summary>
        /// How to attach to the SDK Object.
        /// </summary>
        /// <param name="Child">Child the game object this script is attached to to the SDK Object.</param>
        /// <param name="MoveComponents">Copy the provided list of components to the SDK Object.</param>
        public enum AttachmentType
        {
            Child,
            MoveComponents
        }

        [Tooltip("The specific SDK Object to child this GameObject to or copy components to.")]
        public SDKObject sdkObject = SDKObject.Boundary;
        [Tooltip("How to attach to the SDK Object.")]
        public AttachmentType attachmentType = AttachmentType.Child;
        [Tooltip("The components to move over to the SDK Object.")]
        public List<Component> componentsToCopyToSDKObject = new List<Component>();

        protected VRTK_SDKManager sdkManager;

        protected virtual void OnEnable()
        {
            sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                sdkManager.LoadedSetupChanged += LoadedSetupChanged;
                if (sdkManager.loadedSetup != null)
                {
                    AttachToSDKObject();
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (sdkManager != null && !gameObject.activeSelf)
            {
                sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
            }
        }

        protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            if (sdkManager != null)
            {
                AttachToSDKObject();
            }
        }

        protected virtual void AttachToSDKObject()
        {
            Transform newParent = null;
            switch (sdkObject)
            {
                case SDKObject.Boundary:
                    newParent = VRTK_DeviceFinder.PlayAreaTransform();
                    break;
                case SDKObject.Headset:
                    newParent = VRTK_DeviceFinder.HeadsetTransform();
                    break;
                case SDKObject.HeadsetCamera:
                    newParent = VRTK_DeviceFinder.HeadsetCamera();
                    break;
            }

            if (newParent == null)
            {
                return;
            }

            switch (attachmentType)
            {
                case AttachmentType.Child:
                    Vector3 currentPosition = transform.localPosition;
                    Quaternion currentRotation = transform.localRotation;
                    Vector3 currentScale = transform.localScale;

                    transform.SetParent(newParent);
                    transform.localPosition = currentPosition;
                    transform.localRotation = currentRotation;
                    transform.localScale = currentScale;

                    break;
                case AttachmentType.MoveComponents:
                    const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    GameObject newParentGameObject = newParent.gameObject;
                    List<Component> componentCopies = new List<Component>(componentsToCopyToSDKObject.Count);

                    foreach (Component component in componentsToCopyToSDKObject)
                    {
                        Type type = component.GetType();
                        FieldInfo[] fieldInfos = type.GetFields(bindingFlags);
                        PropertyInfo[] propertyInfos = type.GetProperties(bindingFlags);

                        Component copy = newParentGameObject.AddComponent(type);
                        componentCopies.Add(copy);

                        foreach (FieldInfo fieldInfo in fieldInfos)
                        {
                            if (fieldInfo.Name == "name")
                            {
                                continue;
                            }

                            fieldInfo.SetValue(copy, fieldInfo.GetValue(component));
                        }

                        foreach (PropertyInfo propertyInfo in propertyInfos)
                        {
                            if (propertyInfo.Name == "name")
                            {
                                continue;
                            }

                            MethodInfo setterMethod = propertyInfo.GetSetMethod(true);
                            MethodInfo getterMethod = propertyInfo.GetGetMethod(true);

                            if (setterMethod != null && getterMethod != null)
                            {
                                try
                                {
                                    setterMethod.Invoke(copy, new[] { getterMethod.Invoke(component, null) });
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }
                    }

                    componentsToCopyToSDKObject.ForEach(Destroy);
                    componentsToCopyToSDKObject.Clear();
                    componentsToCopyToSDKObject.AddRange(componentCopies);

                    break;
            }
        }
    }
}