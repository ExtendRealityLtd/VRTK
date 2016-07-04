namespace VRTK
{
    using UnityEngine;

    public class Utilities : MonoBehaviour
    {
        /// <summary>
        /// Returns the bounds of the transform including all children.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="excludeRotation">Resets the rotation of the transform temporarily to 0 to eliminate skewed bounds.</param>
        /// <returns></returns>
        public static Bounds GetBounds(Transform transform, Transform excludeRotation = null)
        {
            Quaternion oldRotation = Quaternion.identity;
            if (excludeRotation)
            {
                oldRotation = excludeRotation.rotation;
                excludeRotation.rotation = Quaternion.identity;
            }

            Bounds bounds = new Bounds(transform.position, Vector3.zero);
            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
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
                if (o <= value) return false;
            }
            return true;
        }

        public static void SetPlayerObject(GameObject obj, VRTK.VRTK_PlayerObject.ObjectTypes objType)
        {
            if(!obj.GetComponent<VRTK_PlayerObject>())
            {
                var playerObject = obj.AddComponent<VRTK_PlayerObject>();
                playerObject.objectType = objType;
            }
        }

        public static Transform AddCameraFade()
        {
            var camera = DeviceFinder.HeadsetCamera();
            if (camera && !camera.gameObject.GetComponent<SteamVR_Fade>())
            {
                camera.gameObject.AddComponent<SteamVR_Fade>();
            }
            return camera;
        }
    }
}