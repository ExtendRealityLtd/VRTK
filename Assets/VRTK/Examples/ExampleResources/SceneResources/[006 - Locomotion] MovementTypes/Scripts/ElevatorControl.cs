namespace VRTK.Examples
{
    using UnityEngine;
    using VRTK.Controllables;

    public class ElevatorControl : MonoBehaviour
    {
        public VRTK_BaseControllable controllable;
        public GameObject platform;
        public float maxHeight;

        protected virtual void OnEnable()
        {
            if (controllable != null)
            {
                controllable.ValueChanged += ValueChanged;
            }
        }

        protected virtual void OnDisable()
        {
            if (controllable != null)
            {
                controllable.ValueChanged -= ValueChanged;
            }
        }

        protected virtual void ValueChanged(object sender, ControllableEventArgs e)
        {
            if (platform != null)
            {
                platform.transform.localPosition = Vector3.up * (maxHeight * e.normalizedValue);
            }
        }
    }
}