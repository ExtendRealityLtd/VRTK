namespace VRTK.Examples
{
    using UnityEngine;

    public class FireExtinguisher_Sprayer : VRTK_InteractableObject
    {
        [Header("Fire Extinguisher Sprayer Settings")]

        public FireExtinguisher_Base baseCan;
        public float breakDistance = 0.12f;
        public float maxSprayPower = 5f;

        protected GameObject waterSpray;
        protected ParticleSystem particles;

        public void Spray(float power)
        {
            if (power <= 0)
            {
                particles.Stop();
            }
            else
            {
                PlayParticles(power);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            waterSpray = transform.Find("WaterSpray").gameObject;
            particles = waterSpray.GetComponent<ParticleSystem>();
            particles.Stop();
        }

        protected override void Update()
        {
            base.Update();
            if (Vector3.Distance(transform.position, baseCan.transform.position) > breakDistance)
            {
                ForceStopInteracting();
            }
        }

        protected virtual void PlayParticles(float power)
        {
            if (particles.isPaused || particles.isStopped)
            {
                particles.Play();
            }

#if UNITY_5_5_OR_NEWER
            ParticleSystem.MainModule mainModule = particles.main;
            mainModule.startSpeedMultiplier = maxSprayPower * power;
#else
                particles.startSpeed = maxSprayPower * power;
#endif
        }
    }
}