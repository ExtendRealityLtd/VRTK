namespace VRTK.Examples.Prefabs.Items.Shotgun
{
    using UnityEngine;
    using UnityEngine.Events;

    public class PumpAction : MonoBehaviour
    {
        public GameObject source;
        public ConfigurableJoint joint;
        public float resetDistance = 1f;
        public float activationDistance = 1f;

        public UnityEvent Reset = new UnityEvent();
        public UnityEvent Activated = new UnityEvent();
        public UnityEvent ResetAfterActivated = new UnityEvent();

        protected bool reset;
        protected bool activated;
        protected bool hasBeenActive;
        protected Vector3 originalPosition;

        protected virtual void Awake()
        {
            if (source != null)
            {
                originalPosition = source.transform.localPosition + (Vector3.forward * joint.linearLimit.limit);
            }
        }

        protected virtual void Update()
        {
            if (source == null)
            {
                return;
            }

            float distance = Vector3.Distance(originalPosition, source.transform.localPosition);

            bool previousReset = reset;
            reset = (distance <= resetDistance);
            if (reset && !previousReset)
            {
                Reset?.Invoke();
            }

            if (hasBeenActive && reset)
            {
                ResetAfterActivated?.Invoke();
                hasBeenActive = false;
            }

            bool previousActivated = activated;
            activated = (distance >= activationDistance);
            if (activated && !previousActivated)
            {
                hasBeenActive = true;
                Activated?.Invoke();
            }
        }
    }
}