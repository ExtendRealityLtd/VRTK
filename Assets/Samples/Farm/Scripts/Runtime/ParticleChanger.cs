namespace VRTK.Examples
{
    using UnityEngine;

    public class ParticleChanger : MonoBehaviour
    {
        [Tooltip("The ParticleSystem to control.")]
        [SerializeField]
        private ParticleSystem particles;
        /// <summary>
        /// The <see cref="ParticleSystem"/> to control.
        /// </summary>
        public ParticleSystem Particles
        {
            get
            {
                return particles;
            }
            set
            {
                particles = value;
            }
        }

        public virtual void UpdateEmissionRate(float rate)
        {
            ParticleSystem.EmissionModule emissions = Particles.emission;
            emissions.rateOverTime = rate;
        }
    }
}