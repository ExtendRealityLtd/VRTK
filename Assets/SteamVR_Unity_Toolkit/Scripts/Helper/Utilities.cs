namespace VRTK
{
    using UnityEngine;

    public class Utilities : MonoBehaviour
    {
        public static Bounds getBounds(Transform transform)
        {
            Bounds bounds = new Bounds(transform.position, Vector3.zero);
            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
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
    }
}