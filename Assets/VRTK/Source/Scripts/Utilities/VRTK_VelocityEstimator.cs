// Velocity Estimator|Utilities|90180
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// The Velocity Estimator is used to calculate an estimated velocity on a moving object that is moving outside of Unity physics.
    /// </summary>
    /// <remarks>
    /// Objects that have rigidbodies and use Unity physics to move around will automatically provide accurate velocity and angular velocity information.
    ///
    /// Any object that is moved around using absolute positions or moving the transform position will not be able to provide accurate velocity or angular velocity information.
    /// When the Velocity Estimator script is applied to any GameObject it will provide a best case estimation of what the object's velocity and angular velocity is based on a given number of position and rotation samples.
    /// The more samples used, the higher the precision but the script will be more demanding on processing time.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Utilities/VRTK_VelocityEstimator")]
    public class VRTK_VelocityEstimator : MonoBehaviour
    {
        [Tooltip("Begin the sampling routine when the script is enabled.")]
        public bool autoStartSampling = true;
        [Tooltip("The number of frames to average when calculating velocity.")]
        public int velocityAverageFrames = 5;
        [Tooltip("The number of frames to average when calculating angular velocity.")]
        public int angularVelocityAverageFrames = 10;

        protected Vector3[] velocitySamples;
        protected Vector3[] angularVelocitySamples;
        protected int currentSampleCount;
        protected Coroutine calculateSamplesRoutine;

        /// <summary>
        /// The StartEstimation method begins logging samples of position and rotation for the GameObject.
        /// </summary>
        public virtual void StartEstimation()
        {
            EndEstimation();
            calculateSamplesRoutine = StartCoroutine(EstimateVelocity());
        }

        /// <summary>
        /// The EndEstimation method stops logging samples of position and rotation for the GameObject.
        /// </summary>
        public virtual void EndEstimation()
        {
            if (calculateSamplesRoutine != null)
            {
                StopCoroutine(calculateSamplesRoutine);
                calculateSamplesRoutine = null;
            }
        }

        /// <summary>
        /// The GetVelocityEstimate method returns the current velocity estimate.
        /// </summary>
        /// <returns>The velocity estimate vector of the GameObject</returns>
        public virtual Vector3 GetVelocityEstimate()
        {
            Vector3 velocity = Vector3.zero;
            int velocitySampleCount = Mathf.Min(currentSampleCount, velocitySamples.Length);
            if (velocitySampleCount != 0)
            {
                for (int i = 0; i < velocitySampleCount; i++)
                {
                    velocity += velocitySamples[i];
                }
                velocity *= (1.0f / velocitySampleCount);
            }
            return velocity;
        }

        /// <summary>
        /// The GetAngularVelocityEstimate method returns the current angular velocity estimate.
        /// </summary>
        /// <returns>The angular velocity estimate vector of the GameObject</returns>
        public virtual Vector3 GetAngularVelocityEstimate()
        {
            Vector3 angularVelocity = Vector3.zero;
            int angularVelocitySampleCount = Mathf.Min(currentSampleCount, angularVelocitySamples.Length);
            if (angularVelocitySampleCount != 0)
            {
                for (int i = 0; i < angularVelocitySampleCount; i++)
                {
                    angularVelocity += angularVelocitySamples[i];
                }
                angularVelocity *= (1.0f / angularVelocitySampleCount);
            }

            return angularVelocity;
        }

        /// <summary>
        /// The GetAccelerationEstimate method returns the current acceleration estimate.
        /// </summary>
        /// <returns>The acceleration estimate vector of the GameObject</returns>
        public virtual Vector3 GetAccelerationEstimate()
        {
            Vector3 average = Vector3.zero;
            for (int i = 2 + currentSampleCount - velocitySamples.Length; i < currentSampleCount; i++)
            {
                if (i < 2)
                {
                    continue;
                }

                int first = i - 2;
                int second = i - 1;

                Vector3 v1 = velocitySamples[first % velocitySamples.Length];
                Vector3 v2 = velocitySamples[second % velocitySamples.Length];
                average += v2 - v1;
            }
            average *= (1.0f / Time.deltaTime);
            return average;
        }

        protected virtual void OnEnable()
        {
            InitArrays();
            if (autoStartSampling)
            {
                StartEstimation();
            }
        }

        protected virtual void OnDisable()
        {
            EndEstimation();
        }

        protected virtual void InitArrays()
        {
            velocitySamples = new Vector3[velocityAverageFrames];
            angularVelocitySamples = new Vector3[angularVelocityAverageFrames];
        }

        protected virtual IEnumerator EstimateVelocity()
        {
            currentSampleCount = 0;

            Vector3 previousPosition = transform.localPosition;
            Quaternion previousRotation = transform.localRotation;
            while (true)
            {
                yield return new WaitForEndOfFrame();

                float velocityFactor = 1.0f / Time.deltaTime;

                int v = currentSampleCount % velocitySamples.Length;
                int w = currentSampleCount % angularVelocitySamples.Length;
                currentSampleCount++;

                velocitySamples[v] = velocityFactor * (transform.localPosition - previousPosition);
                Quaternion deltaRotation = transform.localRotation * Quaternion.Inverse(previousRotation);

                float theta = 2.0f * Mathf.Acos(Mathf.Clamp(deltaRotation.w, -1.0f, 1.0f));
                if (theta > Mathf.PI)
                {
                    theta -= 2.0f * Mathf.PI;
                }

                Vector3 angularVelocity = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);
                if (angularVelocity.sqrMagnitude > 0.0f)
                {
                    angularVelocity = theta * velocityFactor * angularVelocity.normalized;
                }

                angularVelocitySamples[w] = angularVelocity;

                previousPosition = transform.localPosition;
                previousRotation = transform.localRotation;
            }
        }
    }
}