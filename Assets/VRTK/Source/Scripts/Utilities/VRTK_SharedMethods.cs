// Shared Methods|Utilities|90060
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
#if UNITY_2017_2_OR_NEWER
    using UnityEngine.XR;
#else
    using XRSettings = UnityEngine.VR.VRSettings;
    using XRStats = UnityEngine.VR.VRStats;
#endif

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
            if (excludeRotation != null)
            {
                oldRotation = excludeRotation.rotation;
                excludeRotation.rotation = Quaternion.identity;
            }

            bool boundsInitialized = false;
            Bounds bounds = new Bounds(transform.position, Vector3.zero);

            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
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
                for (int i = 0; i < colliders.Length; i++)
                {
                    BoxCollider collider = colliders[i];
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

            if (excludeRotation != null)
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
            for (int i = 0; i < others.Length; i++)
            {
                if (others[i] <= value)
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
            Transform camera = VRTK_DeviceFinder.HeadsetCamera();
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
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer.gameObject.GetComponent<Collider>() == null)
                {
                    renderer.gameObject.AddComponent<BoxCollider>();
                }
            }
        }

        /// <summary>
        /// The ColliderExclude method reduces the colliders in the setA array by those matched in the setB array.
        /// </summary>
        /// <param name="setA">The array that contains all of the relevant colliders.</param>
        /// <param name="setB">The array that contains the colliders to remove from setA.</param>
        /// <returns>A Collider array that is a subset of setA that doesn't contain the colliders from setB.</returns>
        public static Collider[] ColliderExclude(Collider[] setA, Collider[] setB)
        {
            return setA.Except(setB).ToArray<Collider>();
        }

        /// <summary>
        /// The GetCollidersInGameObjects method iterates through a GameObject array and returns all of the unique found colliders for all GameObejcts.
        /// </summary>
        /// <param name="gameObjects">An array of GameObjects to get the colliders for.</param>
        /// <param name="searchChildren">If this is `true` then the given GameObjects will also have their child GameObjects searched for colliders.</param>
        /// <param name="includeInactive">If this is `true` then the inactive GameObjects in the array will also be checked for Colliders. Only relevant if `searchChildren` is `true`.</param>
        /// <returns>An array of Colliders that are found in the given GameObject array.</returns>
        public static Collider[] GetCollidersInGameObjects(GameObject[] gameObjects, bool searchChildren, bool includeInactive)
        {
            HashSet<Collider> foundColliders = new HashSet<Collider>();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                Collider[] gameObjectColliders = (searchChildren ? gameObjects[i].GetComponentsInChildren<Collider>(includeInactive) : gameObjects[i].GetComponents<Collider>());
                for (int j = 0; j < gameObjectColliders.Length; j++)
                {
                    foundColliders.Add(gameObjectColliders[j]);
                }
            }
            return foundColliders.ToArray();
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
                PropertyInfo[] foundProperties = source.GetType().GetProperties();
                for (int i = 0; i < foundProperties.Length; i++)
                {
                    PropertyInfo foundProperty = foundProperties[i];
                    if (foundProperty.CanWrite)
                    {
                        foundProperty.SetValue(tmpComponent, foundProperty.GetValue(source, null), null);
                    }
                }
            }

            FieldInfo[] foundFields = source.GetType().GetFields();
            for (int i = 0; i < foundFields.Length; i++)
            {
                FieldInfo foundField = foundFields[i];
                foundField.SetValue(tmpComponent, foundField.GetValue(source));
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
        /// Finds the first GameObject with a given name and an ancestor that has a specific component.
        /// </summary>
        /// <remarks>
        /// This method returns active as well as inactive GameObjects in the scene. It doesn't return assets.
        /// For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.
        /// </remarks>
        /// <typeparam name="T">The component type that needs to be on an ancestor of the wanted GameObject. Must be a subclass of `Component`.</typeparam>
        /// <param name="gameObjectName">The name of the wanted GameObject. If it contains a '/' character, this method traverses the hierarchy like a path name, beginning on the game object that has a component of type `T`.</param>
        /// <returns>The GameObject with name `gameObjectName` and an ancestor that has a `T`. If no such GameObject is found then `null` is returned.</returns>
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
        /// This method returns components from active as well as inactive GameObjects in the scene. It doesn't return assets.
        /// For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.
        /// </remarks>
        /// <typeparam name="T">The component type to search for. Must be a subclass of `Component`.</typeparam>
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
        /// This method returns components from active as well as inactive GameObjects in the scene. It doesn't return assets.
        /// For performance reasons it is recommended to not use this function every frame. Cache the result in a member variable at startup instead.
        /// </remarks>
        /// <typeparam name="T">The component type to search for. Must be a subclass of `Component`.</typeparam>
        /// <returns>The found component. If no component is found `null` is returned.</returns>
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
            return (XRStats.TryGetGPUTimeLastFrame(out gpuTimeLastFrame) ? gpuTimeLastFrame : 0f);
#else
            return XRStats.gpuTimeLastFrame;
#endif
        }

        /// <summary>
        /// The Vector2ShallowCompare method compares two given Vector2 objects based on the given fidelity, which is the equivalent of comparing rounded Vector2 elements to determine if the Vector2 elements are equal.
        /// </summary>
        /// <param name="vectorA">The Vector2 to compare against.</param>
        /// <param name="vectorB">The Vector2 to compare with</param>
        /// <param name="compareFidelity">The number of decimal places to use when doing the comparison on the float elements within the Vector2.</param>
        /// <returns>Returns `true` if the given Vector2 objects match based on the given fidelity.</returns>
        public static bool Vector2ShallowCompare(Vector2 vectorA, Vector2 vectorB, int compareFidelity)
        {
            Vector2 distanceVector = vectorA - vectorB;
            return (Math.Round(Mathf.Abs(distanceVector.x), compareFidelity, MidpointRounding.AwayFromZero) < float.Epsilon &&
                    Math.Round(Mathf.Abs(distanceVector.y), compareFidelity, MidpointRounding.AwayFromZero) < float.Epsilon);
        }

        /// <summary>
        /// The Vector3ShallowCompare method compares two given Vector3 objects based on the given threshold, which is the equavelent of checking the distance between two Vector3 objects are above the threshold distance.
        /// </summary>
        /// <param name="vectorA">The Vector3 to compare against.</param>
        /// <param name="vectorB">The Vector3 to compare with</param>
        /// <param name="threshold">The distance in which the two Vector3 objects can be within to be considered true</param>
        /// <returns>Returns `true` if the given Vector3 objects are within the given threshold distance.</returns>
        public static bool Vector3ShallowCompare(Vector3 vectorA, Vector3 vectorB, float threshold)
        {
            return (Vector3.Distance(vectorA, vectorB) < threshold);
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
        /// The VectorHeading method calculates the current heading of the target position in relation to the origin position.
        /// </summary>
        /// <param name="originPosition">The point to use as the originating position for the heading calculation.</param>
        /// <param name="targetPosition">The point to use as the target position for the heading calculation.</param>
        /// <returns>A Vector3 containing the heading changes of the target position in relation to the origin position.</returns>
        public static Vector3 VectorHeading(Vector3 originPosition, Vector3 targetPosition)
        {
            return targetPosition - originPosition;
        }

        /// <summary>
        /// The VectorDirection method calculates the direction the target position is in relation to the origin position.
        /// </summary>
        /// <param name="originPosition">The point to use as the originating position for the direction calculation.</param>
        /// <param name="targetPosition">The point to use as the target position for the direction calculation.</param>
        /// <returns>A Vector3 containing the direction of the target position in relation to the origin position.</returns>
        public static Vector3 VectorDirection(Vector3 originPosition, Vector3 targetPosition)
        {
            Vector3 heading = VectorHeading(originPosition, targetPosition);
            return heading * DividerToMultiplier(heading.magnitude);
        }

        /// <summary>
        /// The DividerToMultiplier method takes a number to be used in a division and converts it to be used for multiplication. (e.g. 2 / 2 becomes 2 * 0.5)
        /// </summary>
        /// <param name="value">The number to convert into the multplier value.</param>
        /// <returns>The calculated number that can replace the divider number in a multiplication sum.</returns>
        public static float DividerToMultiplier(float value)
        {
            return (value != 0f ? 1f / value : 1f);
        }

        /// <summary>
        /// The NormalizeValue method takes a given value between a specified range and returns the normalized value between 0f and 1f.
        /// </summary>
        /// <param name="value">The actual value to normalize.</param>
        /// <param name="minValue">The minimum value the actual value can be.</param>
        /// <param name="maxValue">The maximum value the actual value can be.</param>
        /// <param name="threshold">The threshold to force to the minimum or maximum value if the normalized value is within the threhold limits.</param>
        /// <returns></returns>
        public static float NormalizeValue(float value, float minValue, float maxValue, float threshold = 0f)
        {
            float normalizedMax = maxValue - minValue;
            float normalizedValue = normalizedMax - (maxValue - value);
            float result = normalizedValue * DividerToMultiplier(normalizedMax); ;
            result = (result < threshold ? 0f : result);
            result = (result > 1f - threshold ? 1f : result);
            return Mathf.Clamp(result, 0f, 1f);
        }

        /// <summary>
        /// The AxisDirection method returns the relevant direction Vector3 based on the axis index in relation to x,y,z.
        /// </summary>
        /// <param name="axisIndex">The axis index of the axis. `0 = x` `1 = y` `2 = z`</param>
        /// <param name="givenTransform">An optional Transform to get the Axis Direction for. If this is `null` then the World directions will be used.</param>
        /// <returns>The direction Vector3 based on the given axis index.</returns>
        public static Vector3 AxisDirection(int axisIndex, Transform givenTransform = null)
        {
            Vector3[] worldDirections = (givenTransform != null ? new Vector3[] { givenTransform.right, givenTransform.up, givenTransform.forward } : new Vector3[] { Vector3.right, Vector3.up, Vector3.forward });
            return worldDirections[(int)Mathf.Clamp(axisIndex, 0f, worldDirections.Length)];
        }

        /// <summary>
        /// The AddListValue method adds the given value to the given list. If `preventDuplicates` is `true` then the given value will only be added if it doesn't already exist in the given list.
        /// </summary>
        /// <typeparam name="TValue">The datatype for the list value.</typeparam>
        /// <param name="list">The list to retrieve the value from.</param>
        /// <param name="value">The value to attempt to add to the list.</param>
        /// <param name="preventDuplicates">If this is `false` then the value provided will always be appended to the list. If this is `true` the value provided will only be added to the list if it doesn't already exist.</param>
        /// <returns>Returns `true` if the given value was successfully added to the list. Returns `false` if the given value already existed in the list and `preventDuplicates` is `true`.</returns>
        public static bool AddListValue<TValue>(List<TValue> list, TValue value, bool preventDuplicates = false)
        {
            if (list != null && (!preventDuplicates || !list.Contains(value)))
            {
                list.Add(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// The GetDictionaryValue method attempts to retrieve a value from a given dictionary for the given key. It removes the need for a double dictionary lookup to ensure the key is valid and has the option of also setting the missing key value to ensure the dictionary entry is valid.
        /// </summary>
        /// <typeparam name="TKey">The datatype for the dictionary key.</typeparam>
        /// <typeparam name="TValue">The datatype for the dictionary value.</typeparam>
        /// <param name="dictionary">The dictionary to retrieve the value from.</param>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <param name="defaultValue">The value to utilise when either setting the missing key (if `setMissingKey` is `true`) or the default value to return when no key is found (if `setMissingKey` is `false`).</param>
        /// <param name="setMissingKey">If this is `true` and the given key is not present, then the dictionary value for the given key will be set to the `defaultValue` parameter. If this is `false` and the given key is not present then the `defaultValue` parameter will be returned as the value.</param>
        /// <returns>The found value for the given key in the given dictionary, or the default value if no key is found.</returns>
        public static TValue GetDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue), bool setMissingKey = false)
        {
            bool keyExists;
            return GetDictionaryValue(dictionary, key, out keyExists, defaultValue, setMissingKey);
        }

        /// <summary>
        /// The GetDictionaryValue method attempts to retrieve a value from a given dictionary for the given key. It removes the need for a double dictionary lookup to ensure the key is valid and has the option of also setting the missing key value to ensure the dictionary entry is valid.
        /// </summary>
        /// <typeparam name="TKey">The datatype for the dictionary key.</typeparam>
        /// <typeparam name="TValue">The datatype for the dictionary value.</typeparam>
        /// <param name="dictionary">The dictionary to retrieve the value from.</param>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <param name="keyExists">Sets the given parameter to `true` if the key exists in the given dictionary or sets to `false` if the key didn't existing in the given dictionary.</param>
        /// <param name="defaultValue">The value to utilise when either setting the missing key (if `setMissingKey` is `true`) or the default value to return when no key is found (if `setMissingKey` is `false`).</param>
        /// <param name="setMissingKey">If this is `true` and the given key is not present, then the dictionary value for the given key will be set to the `defaultValue` parameter. If this is `false` and the given key is not present then the `defaultValue` parameter will be returned as the value.</param>
        /// <returns>The found value for the given key in the given dictionary, or the default value if no key is found.</returns>
        public static TValue GetDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, out bool keyExists, TValue defaultValue = default(TValue), bool setMissingKey = false)
        {
            keyExists = false;
            if (dictionary == null)
            {
                return defaultValue;
            }

            TValue outputValue;
            if (dictionary.TryGetValue(key, out outputValue))
            {
                keyExists = true;
            }
            else
            {
                if (setMissingKey)
                {
                    dictionary.Add(key, defaultValue);
                }
                outputValue = defaultValue;
            }
            return outputValue;
        }

        /// <summary>
        /// The AddDictionaryValue method attempts to add a value for the given key in the given dictionary if the key does not already exist. If `overwriteExisting` is `true` then it always set the value even if they key exists.
        /// </summary>
        /// <typeparam name="TKey">The datatype for the dictionary key.</typeparam>
        /// <typeparam name="TValue">The datatype for the dictionary value.</typeparam>
        /// <param name="dictionary">The dictionary to set the value for.</param>
        /// <param name="key">The key to set the value for.</param>
        /// <param name="value">The value to set at the given key in the given dictionary.</param>
        /// <param name="overwriteExisting">If this is `true` then the value for the given key will always be set to the provided value. If this is `false` then the value for the given key will only be set if the given key is not found in the given dictionary.</param>
        /// <returns>Returns `true` if the given value was successfully added to the dictionary at the given key. Returns `false` if the given key already existed in the dictionary and `overwriteExisting` is `false`.</returns>
        public static bool AddDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue value, bool overwriteExisting = false)
        {
            if (dictionary != null)
            {
                if (overwriteExisting)
                {
                    dictionary[key] = value;
                    return true;
                }
                else
                {
                    bool keyExists;
                    GetDictionaryValue(dictionary, key, out keyExists, value, true);
                    return !keyExists;
                }
            }
            return false;
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
            Assembly[] foundAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < foundAssemblies.Length; i++)
            {
                type = foundAssemblies[i].GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        /// <summary>
        /// The GetEyeTextureResolutionScale method returns the render scale for the resolution.
        /// </summary>
        /// <returns>Returns a float with the render scale for the resolution.</returns>
        public static float GetEyeTextureResolutionScale()
        {
#if UNITY_2017_2_OR_NEWER
            return XRSettings.eyeTextureResolutionScale;
#else
            return XRSettings.renderScale;
#endif
        }

        /// <summary>
        /// The SetEyeTextureResolutionScale method sets the render scale for the resolution.
        /// </summary>
        /// <param name="value">The value to set the render scale to.</param>
        public static void SetEyeTextureResolutionScale(float value)
        {
#if UNITY_2017_2_OR_NEWER
            XRSettings.eyeTextureResolutionScale = value;
#else
            XRSettings.renderScale = value;
#endif
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