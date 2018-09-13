namespace VRTK.Examples.Prefabs.Items.Handgun
{
    using UnityEngine;

    public class TriggerSqueezer : MonoBehaviour
    {
        public GameObject trigger;
        public float multiplier = 15f;

        protected Vector3 originalRotation;

        public virtual void SqueezeBy(float value)
        {
            if (trigger != null)
            {
                trigger.transform.localEulerAngles = originalRotation + (Vector3.right * (value * multiplier));
            }
        }

        protected virtual void Awake()
        {
            if (trigger != null)
            {
                originalRotation = trigger.transform.localEulerAngles;
            }
        }
    }
}