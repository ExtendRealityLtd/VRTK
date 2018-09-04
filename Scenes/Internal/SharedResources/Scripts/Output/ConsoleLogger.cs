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

        public virtual void Log(float value)
        {
            Log("Float Value: " + value);
        }

        public virtual void Log(Vector2 value)
        {
            Log("Vector2 Value: " + value);
        }

        public virtual void Log(ObjectPointer.EventData data)
        {
            if (data != null && data.CollisionData.transform != null)
            {
                Debug.Log(string.Format("Colliding with {0} at {1} for {2} seconds", data.CollisionData.transform.name, data.CollisionData.point, data.hoverDuration));
            }
        }
    }
}
