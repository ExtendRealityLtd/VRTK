namespace VRTK.Examples
{
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using UnityEngine;

    public class ParticleChanger : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="ParticleSystem"/> to control.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public ParticleSystem Particles { get; set; }

        public virtual void UpdateEmissionRate(float rate)
        {
            ParticleSystem.EmissionModule emissions = Particles.emission;
            emissions.rateOverTime = rate;
        }
    }
}