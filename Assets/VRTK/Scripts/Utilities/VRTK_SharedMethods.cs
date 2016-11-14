// Shared Methods|Utilities|90020
namespace VRTK
{
    using UnityEngine;
    using System.Reflection;
    using Highlighters;

    /// <summary>
    /// The Shared Methods script is a collection of reusable static methods that are used across a range of different scripts.
    /// </summary>
    public class VRTK_SharedMethods : MonoBehaviour
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
        /// The SetPlayerObject method tags the given game object with a special player object class for easier identification.
        /// </summary>
        /// <param name="obj">The game object to add the player object class to.</param>
        /// <param name="objType">The type of player object that is to be assigned.</param>
        public static void SetPlayerObject(GameObject obj, VRTK_PlayerObject.ObjectTypes objType)
        {
            if (!obj.GetComponent<VRTK_PlayerObject>())
            {
                var playerObject = obj.AddComponent<VRTK_PlayerObject>();
                playerObject.objectType = objType;
            }
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
        /// The AngleSigned method returns the signed angle between the two vectors, v1 and v2, with normal 'n' as the rotation axis.
        /// </summary>
        /// <param name="v1">The first given vector.</param>
        /// <param name="v2">The second given vector.</param>
        /// <param name="n">The normal vector.</param>
        /// <returns>The signed angle between v1, v2 with the given normal as the rotation axis.</returns>
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// The TagOrScriptCheck method is used to check if a game object should be ignored based on a given string or policy list.
        /// </summary>
        /// <param name="obj">The game object to check.</param>
        /// <param name="tagOrScriptList">The policy list to use for checking.</param>
        /// <param name="ignoreString">The string to check against.</param>
        /// <param name="ignoreStringIsInclude">Determines whether it should actually look to see if the game object includes the policy list or string to make it not ignored.</param>
        /// <returns>Returns true of the given game object matches the policy list or given string logic.</returns>
        public static bool TagOrScriptCheck(GameObject obj, VRTK_TagOrScriptPolicyList tagOrScriptList, string ignoreString, bool ignoreStringIsInclude = false)
        {
            if (tagOrScriptList)
            {
                return tagOrScriptList.Find(obj);
            }
            else
            {
                if (!ignoreStringIsInclude)
                {
                    return (obj.tag == ignoreString || obj.GetComponent(ignoreString) != null);
                }
                else
                {
                    return (obj.tag != ignoreString && obj.GetComponent(ignoreString) == null);
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
            if(copyProperties)
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
        /// The GetActiveHighlighter method checks the given game object for a valid and active highlighter.
        /// </summary>
        /// <param name="obj">The game object to check for a highlighter on.</param>
        /// <returns>A valid and active highlighter.</returns>
        public static VRTK_BaseHighlighter GetActiveHighlighter(GameObject obj)
        {
            VRTK_BaseHighlighter objectHighlighter = null;
            foreach (var tmpHighlighter in obj.GetComponents<VRTK_BaseHighlighter>())
            {
                if (tmpHighlighter.active)
                {
                    objectHighlighter = tmpHighlighter;
                    break;
                }
            }

            return objectHighlighter;
        }

        /// <summary>
        /// The ColorDarken method takes a given colour and darkens it by the given percentage.
        /// </summary>
        /// <param name="color">The source colour to apply the darken to.</param>
        /// <param name="percent">The percent to darken the colour by.</param>
        /// <returns>The new colour with the darken applied.</returns>
        public static Color ColorDarken(Color color, float percent)
        {
            return new Color(ColorPercent(color.r, percent), ColorPercent(color.g, percent), ColorPercent(color.b, percent), color.a);
        }

        /// <summary>
        /// The IsEditTime method determines if the state of Unity is in the Unity Editor and the scene is not in play mode.
        /// </summary>
        /// <returns>Returns true if Unity is in the Unity Editor and not in play mode.</returns>
        public static bool IsEditTime()
        {
            return (Application.isEditor && !Application.isPlaying);
        }

        private static float ColorPercent(float value, float percent)
        {
            percent = Mathf.Clamp(percent, 0f, 100f);
            return (percent == 0f ? value : (value - (percent / 100f)));
        }
    }
}