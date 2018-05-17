namespace VRTK.Examples.Output
{
    using UnityEngine;

    public class ConsoleLogger : MonoBehaviour
    {
        public virtual void Log(string value)
        {
            Debug.Log(value);
        }
    }
}
