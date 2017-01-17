namespace VRTK.Examples
{
    using UnityEngine;

    public class FireExtinguisher_Sprayer : VRTK_InteractableObject
    {
        public FireExtinguisher_Base baseCan;
        public float breakDistance = 0.12f;
        public float maxSprayPower = 5f;

        private GameObject waterSpray;
        private ParticleSystem particles;

        public void Spray(float power)
        {
            if (power <= 0)
            {
                particles.Stop();
            }

            if (power > 0)
            {
                if (particles.isPaused || particles.isStopped)
                {
                    particles.Play();
                }

#if UNITY_5_5_OR_NEWER
                var mainModule = particles.main;
                mainModule.startSpeedMultiplier = maxSprayPower * power;
#else
                particles.startSpeed = maxSprayPower * power;
#endif
            }
        }

        protected override void Awake()
        {
            base.Awake();
            waterSpray = transform.FindChild("WaterSpray").gameObject;
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
    }
}