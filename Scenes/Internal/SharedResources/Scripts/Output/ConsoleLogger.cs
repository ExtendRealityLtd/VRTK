namespace VRTK.Examples.Output
{
    using UnityEngine;
    using VRTK.Core.Pointer;

    public class ConsoleLogger : MonoBehaviour
    {
        public virtual void Log(string value)
        {
            Debug.Log(value);
        }

        public virtual void Log(PointerData data, object initiatior = null)
        {
            if (data != null && data.CollisionData.transform != null)
            {
                Debug.Log(string.Format("Colliding with {0} at {1} for {2} seconds", data.CollisionData.transform.name, data.CollisionData.point, data.hoverDuration));
            }
        }
    }
}
