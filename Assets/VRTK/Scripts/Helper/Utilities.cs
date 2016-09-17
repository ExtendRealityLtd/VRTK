namespace VRTK
{
    using UnityEngine;
    using System.Reflection;
    using Highlighters;

    public class Utilities : MonoBehaviour
    {
        /// <summary>
        /// Returns the bounds of the transform including all children in world space.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="excludeRotation">Resets the rotation of the transform temporarily to 0 to eliminate skewed bounds.</param>
        /// <param name="excludeTransform">Does not consider the stated object when calculating the bounds.</param>
        /// <returns></returns>
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

        public static void SetPlayerObject(GameObject obj, VRTK_PlayerObject.ObjectTypes objType)
        {
            if (!obj.GetComponent<VRTK_PlayerObject>())
            {
                var playerObject = obj.AddComponent<VRTK_PlayerObject>();
                playerObject.objectType = objType;
            }
        }

        public static Transform AddCameraFade()
        {
            var camera = VRTK_DeviceFinder.HeadsetCamera();
            VRTK_SDK_Bridge.AddHeadsetFade(camera);
            return camera;
        }

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
        /// Returns the signed angle between the two vectors, v1 and v2, 
        /// with normal 'n' as the rotation axis.
        /// </summary>
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        public static bool TagOrScriptCheck(GameObject obj, VRTK_TagOrScriptPolicyList tagOrScriptList, string ignoreString)
        {
            if (tagOrScriptList)
            {
                return tagOrScriptList.Find(obj);
            }
            else
            {
                return (obj.tag == ignoreString || obj.GetComponent(ignoreString) != null);
            }
        }

        public static Component CloneComponent(Component source, GameObject destination)
        {
            Component tmpComponent = destination.gameObject.AddComponent(source.GetType());
            foreach (FieldInfo f in source.GetType().GetFields())
            {
                f.SetValue(tmpComponent, f.GetValue(source));
            }
            return tmpComponent;
        }

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
    }
}