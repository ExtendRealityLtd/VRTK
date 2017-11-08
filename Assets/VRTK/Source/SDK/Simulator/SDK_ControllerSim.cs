namespace VRTK
{
    using UnityEngine;

    public class SDK_ControllerSim : MonoBehaviour
    {
        [HideInInspector]
        public bool selected;

        protected VRTK_VelocityEstimator cachedVelocityEstimator;
        protected float magnitude;
        protected Vector3 axis;

        public Vector3 GetVelocity()
        {
            SetCaches();
            return cachedVelocityEstimator.GetVelocityEstimate();
        }

        public Vector3 GetAngularVelocity()
        {
            SetCaches();
            return cachedVelocityEstimator.GetAngularVelocityEstimate();
        }

        protected virtual void OnEnable()
        {
            SetCaches();
        }

        protected virtual void SetCaches()
        {
            if (cachedVelocityEstimator == null)
            {
                cachedVelocityEstimator = (GetComponent<VRTK_VelocityEstimator>() != null ? GetComponent<VRTK_VelocityEstimator>() : gameObject.AddComponent<VRTK_VelocityEstimator>());
            }
        }
    }
}
