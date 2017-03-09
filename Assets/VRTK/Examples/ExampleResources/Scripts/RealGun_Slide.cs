namespace VRTK.Examples
{
    using UnityEngine;

    public class RealGun_Slide : VRTK_InteractableObject
    {
        private float restPosition;
        private float fireTimer = 0f;
        private float fireDistance = 0.05f;
        private float boltSpeed = 0.01f;

        public void Fire()
        {
            fireTimer = fireDistance;
        }

        protected override void Awake()
        {
            base.Awake();
            restPosition = transform.localPosition.z;
        }

        protected override void Update()
        {
            base.Update();
            if (transform.localPosition.z >= restPosition)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, restPosition);
            }

            if (fireTimer == 0 && transform.localPosition.z < restPosition && !IsGrabbed())
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + boltSpeed);
            }

            if (fireTimer > 0)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - boltSpeed);
                fireTimer -= boltSpeed;
            }

            if (fireTimer < 0)
            {
                fireTimer = 0;
            }
        }
    }
}
