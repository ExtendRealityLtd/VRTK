// Shared Methods|Utilities|90030
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine.VR;

    /// <summary>
    /// The Shared Methods script is a collection of reusable static methods that are used across a range of different scripts.
    /// </summary>
    public static class VRTK_SharedMethods
    {
        /// <summary>
        /// The GetBounds methods returns the bounds of the transform including all children in world space.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="excludeRotation">Resets the rotation of the transform temporarily to 0 to eliminate skewed bounds.</param>
        /// <param name="excludeTransform">Does not consider the stated object when calculating the bounds.</param>
        /// <returns>The bounds of the transform.</returns>
        public static Bounds GetBounds(Transform transform, Transform excludeRotation = null, Transform excludeTransform = null)
        {
            Quaternion oldRotation = Quaternion.identity;
            if (excludeRotation)
            {
                oldRotation = excludeRotation.rotation;
                excludeRotation.rotation = Quaternion.identity;
            }

            bool boundsInitialized = false;
            Bounds bounds = new Bounds(transform.position, Vector3.zero);

            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (excludeTransform != null && renderer.transform.IsChildOf(excludeTransform))
                {
                    continue;
                }

                // do late initialization in case initial transform does not contain any renderers
                if (!boundsInitialized)
                {
                    bounds = new Bounds(renderer.transform.position, Vector3.zero);
                    boundsInitialized = true;
                }
                bounds.Encapsulate(renderer.bounds);
            }

            if (bounds.size.magnitude == 0)
            {
                // do second pass as there were no renderers, this time with colliders
                BoxCollider[] colliders = transform.GetComponentsInChildren<BoxCollider>();
                foreach (BoxCollider collider in colliders)
                {
                    if (excludeTransform != null && collider.transform.IsChildOf(excludeTransform))
                    {
                        continue;
                    }

                    // do late initialization in case initial transform does not contain any colliders
                    if (!boundsInitialized)
                    {
                        bounds = new Bounds(collider.transform.position, Vector3.zero);
                        boundsInitialized = true;
                    }
                    bounds.Encapsulate(collider.bounds);
                }
            }

            if (excludeRotation)
            {
                excludeRotation.rotation = oldRotation;
            }

            return bounds;
        }

        /// <summary>
        /// The IsLowest method checks to see if the given value is the lowest number in the given array of values.
        /// </summary>
        /// <param name="value">The value to check to see if it is lowest.</param>
        /// <param name="others">The array of values to check against.</param>
        /// <returns>Returns true if the value is lower than all numbers in the given array, returns false if it is not the lowest.</returns>
        public static bool IsLowest(float value, float[] others)
        {
            foreach (float o in others)
            {
                if (o <= value)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The AddCameraFade method finds the headset camera and adds a headset fade script to it.
        /// </summary>
        /// <returns>The transform of the headset camera.</returns>
        public static Transform AddCameraFade()
        {
            var camera = VRTK_DeviceFinder.HeadsetCamera();
            VRTK_SDK_Bridge.AddHeadsetFade(camera);
            return camera;
        }

        /// <summary>
        /// The CreateColliders method attempts to add box colliders to all child objects in the given object that have a renderer but no collider.
        /// </summary>
        /// <param name="obj">The game object to attempt to add the colliders to.</param>
        public static void CreateColliders(GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (!renderer.gameObject.GetComponent<Collider>())
                {
                    renderer.gameObject.AddComponent<BoxCollider>();
                }
            }
        }

        /// <summary>
        /// The CloneComponent method takes a source component and copies it to the given destination game object.
        /// </summary>
        /// <param name="source">The component to copy.</param>
        /// <param name="destination">The game object to copy the component to.</param>
        /// <param name="copyProperties">Determines whether the properties of the component as well as the fields should be copied.</param>
        /// <returns>The component that has been cloned onto the given game object.</returns>
        public static Component CloneComponent(Component source, GameObject destination, bool copyProperties = false)
        {
            Component tmpComponent = destination.gameObject.AddComponent(source.GetType());
            if (copyProperties)
            {
                foreach (PropertyInfo p in source.GetType().GetProperties())
                {
                    if (p.CanWrite)
                    {
                        p.SetValue(tmpComponent, p.GetValue(source, null), null);
                    }
                }
            }

            foreach (FieldInfo f in source.GetType().GetFields())
            {
                f.SetValue(tmpComponent, f.GetValue(source));
            }
            return tmpComponent;
        }

        /// <summary>
        /// The ColorDarken method takes a given colour and darkens it by the given percentage.
        /// </summary>
        /// <param name="color">The source colour to apply the darken to.</param>
        /// <param name="percent">The percent to darken the colour by.</param>
        /// <returns>The new colour with the darken applied.</returns>
        public static Color ColorDarken(Color color, float percent)
        {
            return new Color(NumberPercent(color.r, percent), NumberPercent(color.g, percent), NumberPercent(color.b, percent), color.a);
        }

        /// <summary>
        /// The RoundFloat method is used to round a given float to the given decimal places.
        /// </summary>
        /// <param name="givenFloat">The float to round.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <param name="rawFidelity">If this is true then the decimal places must be given in the decimal multiplier, e.g. 10 for 1dp, 100 for 2dp, etc.</param>
        /// <returns>The rounded float.</returns>
        public static float RoundFloat(float givenFloat, int decimalPlaces, bool rawFidelity = false)
        {
            float roundBy = (rawFidelity ? decimalPlaces : Mathf.Pow(10.0f, decimalPlaces));
            return Mathf.Round(givenFloat * roundBy) / roundBy;
        }

        /// <summary>
        /// The IsEditTime method determines if the state of Unity is in the Unity Editor and the scene is not in play mode.
        /// </summary>
        /// <returns>Returns true if Unity is in the Unity Editor and not in play mode.</returns>
        public static bool IsEditTime()
        {
#if UNITY_EDITOR
            return !EditorApplication.isPlayingOrWillChangePlaymode;
#else
            return false;
#endif
        }

        /// <summary>
        /// The TriggerHapticPulse/1 method calls a single haptic pulse call on the controller for a single tick.
        /// </summary>
        /// <param name="controllerIndex">The controller index to activate the haptic feedback on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        [Obsolete("`VRTK_SharedMethods.TriggerHapticPulse(controllerIndex, strength)` has been replaced with `VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strength)`. This method will be removed in a future version of VRTK.")]
        public static void TriggerHapticPulse(uint controllerIndex, float strength)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerIndex), strength);
        }

        /// <summary>
        /// The TriggerHapticPulse/3 method calls a haptic pulse for a specified amount of time rather than just a single tick. Each pulse can be separated by providing a `pulseInterval` to pause between each haptic pulse.
        /// </summary>
        /// <param name="controllerIndex">The controller index to activate the haptic feedback on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        /// <param name="duration">The length of time the rumble should continue for.</param>
        /// <param name="pulseInterval">The interval to wait between each haptic pulse.</param>
        [Obsolete("`VRTK_SharedMethods.TriggerHapticPulse(controllerIndex, strength, duration, pulseInterval)` has been replaced with `VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strength, duration, pulseInterval)`. This method will be removed in a future version of VRTK.")]
        public static void TriggerHapticPulse(uint controllerIndex, float strength, float duration, float pulseInterval)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerIndex), strength, duration, pulseInterval);
        }

        /// <summary>
        /// The CancelHapticPulse method cancels the existing running haptic pulse on the given controller index.
        /// </summary>
        /// <param name="controllerIndex">The controller index to cancel the haptic feedback on.</param>
        [Obsolete("`VRTK_SharedMethods.CancelHapticPulse(controllerIndex)` has been replaced with `VRTK_SharedMethods.CancelHapticPulse(controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static void CancelHapticPulse(uint controllerIndex)
        {
            VRTK_ControllerHaptics.CancelHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerIndex));
        }

        /// <summary>
        /// The SetOpacity method allows the opacity of the given GameObject to be changed. A lower alpha value will make the object more transparent, such as `0.5f` will make the controller partially transparent where as `0f` will make the controller completely transparent.
        /// </summary>
        /// <param name="model">The GameObject to change the renderer opacity on.</param>
        /// <param name="alpha">The alpha level to apply to opacity of the controller object. `0f` to `1f`.</param>
        /// <param name="transitionDuration">The time to transition from the current opacity to the new opacity.</param>
        [Obsolete("`VRTK_SharedMethods.SetOpacity(model, alpha, transitionDuration)` has been replaced with `VRTK_ObjectAppearance.SetOpacity(model, alpha, transitionDuration)`. This method will be removed in a future version of VRTK.")]
        public static void SetOpacity(GameObject model, float alpha, float transitionDuration = 0f)
        {
            VRTK_ObjectAppearance.SetOpacity(model, alpha, transitionDuration);
        }

        /// <summary>
        /// The SetRendererVisible method turns on renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle on.
        /// </summary>
        /// <param name="model">The GameObject to show the renderers for.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        [Obsolete("`VRTK_SharedMethods.SetRendererVisible(model, ignoredModel)` has been replaced with `VRTK_ObjectAppearance.SetRendererVisible(model, ignoredModel)`. This method will be removed in a future version of VRTK.")]
        public static void SetRendererVisible(GameObject model, GameObject ignoredModel = null)
        {
            VRTK_ObjectAppearance.SetRendererVisible(model, ignoredModel);
        }

        /// <summary>
        /// The SetRendererHidden method turns off renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle on.
        /// </summary>
        /// <param name="model">The GameObject to hide the renderers for.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        [Obsolete("`VRTK_SharedMethods.SetRendererHidden(model, ignoredModel)` has been replaced with `VRTK_ObjectAppearance.SetRendererHidden(model, ignoredModel)`. This method will be removed in a future version of VRTK.")]
        public static void SetRendererHidden(GameObject model, GameObject ignoredModel = null)
        {
            VRTK_ObjectAppearance.SetRendererHidden(model, ignoredModel);
        }

        /// <summary>
        /// The ToggleRenderer method turns on or off the renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle of.
        /// </summary>
        /// <param name="state">If true then the renderers will be enabled, if false the renderers will be disabled.</param>
        /// <param name="model">The GameObject to toggle the renderer states of.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        [Obsolete("`VRTK_SharedMethods.ToggleRenderer(state, model, ignoredModel)` has been replaced with `VRTK_ObjectAppearance.ToggleRenderer(state, model, ignoredModel)`. This method will be removed in a future version of VRTK.")]
        public static void ToggleRenderer(bool state, GameObject model, GameObject ignoredModel = null)
        {
            VRTK_ObjectAppearance.ToggleRenderer(state, model, ignoredModel);
        }

        /// <summary>
        /// The HighlightObject method calls the Highlight method on the highlighter attached to the given GameObject with the provided colour.
        /// </summary>
        /// <param name="model">The GameObject to attempt to call the Highlight on.</param>
        /// <param name="highlightColor">The colour to highlight to.</param>
        /// <param name="fadeDuration">The duration in time to fade from the initial colour to the target colour.</param>
        [Obsolete("`VRTK_SharedMethods.HighlightObject(model, highlightColor, fadeDuration)` has been replaced with `VRTK_ObjectAppearance.HighlightObject(model, highlightColor, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public static void HighlightObject(GameObject model, Color? highlightColor, float fadeDuration = 0f)
        {
            VRTK_ObjectAppearance.HighlightObject(model, highlightColor, fadeDuration);
        }

        /// <summary>
        /// The UnhighlightObject method calls the Unhighlight method on the highlighter attached to the given GameObject.
        /// </summary>
        /// <param name="model">The GameObject to attempt to call the Unhighlight on.</param>
        [Obsolete("`VRTK_SharedMethods.UnhighlightObject(model)` has been replaced with `VRTK_ObjectAppearance.UnhighlightObject(model)`. This method will be removed in a future version of VRTK.")]
        public static void UnhighlightObject(GameObject model)
        {
            VRTK_ObjectAppearance.UnhighlightObject(model);
        }

        /// <summary>
        /// The Mod method is used to find the remainder of the sum a/b.
        /// </summary>
        /// <param name="a">The dividend value.</param>
        /// <param name="b">The divisor value.</param>
        /// <returns>The remainder value.</returns>
        public static float Mod(float a, float b)
        {
            return a - b * Mathf.Floor(a / b);
        }

        /// <summary>
        /// Finds the first <see cref="GameObject"/> with a given name and an ancestor that has a specific component.
        /// </summary>
        /// <remarks>
        /// This method returns active as well as inactive <see cref="GameObject"/>s in the scene. It doesn't return assets.
        /// For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.
        /// </remarks>
        /// <typeparam name="T">The component type that needs to be on an ancestor of the wanted <see cref="GameObject"/>. Must be a subclass of <see cref="Component"/>.</typeparam>
        /// <param name="gameObjectName">The name of the wanted <see cref="GameObject"/>. If it contains a '/' character, this method traverses the hierarchy like a path name, beginning on the game object that has a component of type <typeparamref name="T"/>.</param>
        /// <returns>The <see cref="GameObject"/> with name <paramref name="gameObjectName"/> and an ancestor that has a <typeparamref name="T"/>. If no such <see cref="GameObject"/> is found <see langword="null"/> is returned.</returns>
        public static GameObject FindEvenInactiveGameObject<T>(string gameObjectName = null) where T : Component
        {
            if (string.IsNullOrEmpty(gameObjectName))
            {
                T foundComponent = FindEvenInactiveComponent<T>();
                return foundComponent == null ? null : foundComponent.gameObject;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            IEnumerable<GameObject> gameObjects = Resources.FindObjectsOfTypeAll<T>()
                                                           .Select(component => component.gameObject)
                                                           .Where(gameObject => gameObject.scene == activeScene);

#if UNITY_EDITOR
            gameObjects = gameObjects.Where(gameObject => !AssetDatabase.Contains(gameObject));
#endif

            return gameObjects.Select(gameObject =>
                              {
                                  Transform transform = gameObject.transform.Find(gameObjectName);
                                  return transform == null ? null : transform.gameObject;
                              })
                              .FirstOrDefault(gameObject => gameObject != null);
        }

        /// <summary>
        /// Finds all components of a given type.
        /// </summary>
        /// <remarks>
        /// This method returns components from active as well as inactive <see cref="GameObject"/>s in the scene. It doesn't return assets.
        /// For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.
        /// </remarks>
        /// <typeparam name="T">The component type to search for. Must be a subclass of <see cref="Component"/>.</typeparam>
        /// <returns>All the found components. If no component is found an empty array is returned.</returns>
        public static T[] FindEvenInactiveComponents<T>() where T : Component
        {
            Scene activeScene = SceneManager.GetActiveScene();
            return Resources.FindObjectsOfTypeAll<T>()
                            .Where(@object => @object.gameObject.scene == activeScene)
#if UNITY_EDITOR
                            .Where(@object => !AssetDatabase.Contains(@object))
#endif
                            .ToArray();
        }

        /// <summary>
        /// Finds the first component of a given type.
        /// </summary>
        /// <remarks>
        /// This method returns components from active as well as inactive <see cref="GameObject"/>s in the scene. It doesn't return assets.
        /// For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.
        /// </remarks>
        /// <typeparam name="T">The component type to search for. Must be a subclass of <see cref="Component"/>.</typeparam>
        /// <returns>The found component. If no component is found <see langword="null"/> is returned.</returns>
        public static T FindEvenInactiveComponent<T>() where T : Component
        {
            Scene activeScene = SceneManager.GetActiveScene();
            return Resources.FindObjectsOfTypeAll<T>()
                            .Where(@object => @object.gameObject.scene == activeScene)
#if UNITY_EDITOR
                            .FirstOrDefault(@object => !AssetDatabase.Contains(@object));
#else
                            .FirstOrDefault();
#endif
        }

        /// <summary>
        /// The GenerateVRTKObjectName method is used to create a standard name string for any VRTK generated object.
        /// </summary>
        /// <param name="autoGen">An additiona [AUTOGEN] prefix will be added if this is true.</param>
        /// <param name="replacements">A collection of parameters to add to the generated name.</param>
        /// <returns>The generated name string.</returns>
        public static string GenerateVRTKObjectName(bool autoGen, params object[] replacements)
        {
            string toFormat = "[VRTK]";
            if (autoGen)
            {
                toFormat += "[AUTOGEN]";
            }
            for (int i = 0; i < replacements.Length; i++)
            {
                toFormat += "[{" + i + "}]";
            }
            return string.Format(toFormat, replacements);
        }

        /// <summary>
        /// The GetGPUTimeLastFrame retrieves the time spent by the GPU last frame, in seconds, as reported by the VR SDK.
        /// </summary>
        /// <returns>The total GPU time utilized last frame as measured by the VR subsystem.</returns>
        public static float GetGPUTimeLastFrame()
        {
#if UNITY_5_6_OR_NEWER
            float gpuTimeLastFrame;
            if (VRStats.TryGetGPUTimeLastFrame(out gpuTimeLastFrame))
            {
                return gpuTimeLastFrame;
            }
            return 0f;
#else
            return VRStats.gpuTimeLastFrame;
#endif
        }

        /// <summary>
        /// The Vector2ShallowCompare method compares two given Vector2 objects based on the given fidelity, which is the equivalent of comparing rounded Vector2 elements to determine if the Vector2 elements are equal.
        /// </summary>
        /// <param name="vectorA">The Vector2 to compare against.</param>
        /// <param name="vectorB">The Vector2 to compare with</param>
        /// <param name="compareFidelity">The number of decimal places to use when doing the comparison on the float elements within the Vector2.</param>
        /// <returns>Returns true if the given Vector2 objects match based on the given fidelity.</returns>
        public static bool Vector2ShallowCompare(Vector2 vectorA, Vector2 vectorB, int compareFidelity)
        {
            var distanceVector = vectorA - vectorB;
            return (Math.Round(Mathf.Abs(distanceVector.x), compareFidelity, MidpointRounding.AwayFromZero) < float.Epsilon &&
                    Math.Round(Mathf.Abs(distanceVector.y), compareFidelity, MidpointRounding.AwayFromZero) < float.Epsilon);
        }

        /// <summary>
        /// The NumberPercent method is used to determine the percentage of a given value.
        /// </summary>
        /// <param name="value">The value to determine the percentage from</param>
        /// <param name="percent">The percentage to find within the given value.</param>
        /// <returns>A float containing the percentage value based on the given input.</returns>
        public static float NumberPercent(float value, float percent)
        {
            percent = Mathf.Clamp(percent, 0f, 100f);
            return (percent == 0f ? value : (value - (percent / 100f)));
        }

        /// <summary>
        /// The SetGlobalScale method is used to set a transform scale based on a global scale instead of a local scale.
        /// </summary>
        /// <param name="transform">The reference to the transform to scale.</param>
        /// <param name="globalScale">A Vector3 of a global scale to apply to the given transform.</param>
        public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
        }

        /// <summary>
        /// The GetTypeUnknownAssembly method is used to find a Type without knowing the exact assembly it is in.
        /// </summary>
        /// <param name="typeName">The name of the type to get.</param>
        /// <returns>The Type, or null if none is found.</returns>
        public static Type GetTypeUnknownAssembly(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

#if UNITY_EDITOR
        public static BuildTargetGroup[] GetValidBuildTargetGroups()
        {
            return Enum.GetValues(typeof(BuildTargetGroup)).Cast<BuildTargetGroup>().Where(group =>
            {
                if (group == BuildTargetGroup.Unknown)
                {
                    return false;
                }

                string targetGroupName = Enum.GetName(typeof(BuildTargetGroup), group);
                FieldInfo targetGroupFieldInfo = typeof(BuildTargetGroup).GetField(targetGroupName, BindingFlags.Public | BindingFlags.Static);

                return targetGroupFieldInfo != null && targetGroupFieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length == 0;
            }).ToArray();
        }
#endif
    }
}