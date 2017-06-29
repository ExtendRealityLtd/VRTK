using UnityEngine;

namespace VRTK
{
    public class VelocityTracker : MonoBehaviour
    {
        private SmoothedVector3 smoothedVelocity;
        private Vector3 lastPos;
        private int maxVelocitySamples = 5;

        public Vector3 Velocity
        {
            get
            {
                return smoothedVelocity.Position;
            }
        }

        void Awake()
        {
            smoothedVelocity = new SmoothedVector3(maxVelocitySamples);
            lastPos = transform.position;
        }

        void FixedUpdate()
        {
            Vector3 newPos = transform.position;
            Vector3 velocity = (newPos - lastPos) / Time.fixedDeltaTime;
            smoothedVelocity.AddSample(velocity);
            lastPos = newPos;
        }
    }
}
