namespace VRTK.Examples
{
    using UnityEngine;

    public class Sword : VRTK_InteractableObject
    {
        private float impactMagnifier = 120f;
        private float collisionForce = 0f;
        private float maxCollisionForce = 4000f;
        private GameObject grabbingController;

        public float CollisionForce()
        {
            return collisionForce;
        }

        public override void Grabbed(GameObject grabbingObject)
        {
            base.Grabbed(grabbingObject);
            grabbingController = grabbingObject;
        }

        public override void Ungrabbed(GameObject previousGrabbingObject)
        {
            base.Ungrabbed(previousGrabbingObject);
            grabbingController = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            grabbingController = null;
            interactableRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (grabbingController != null && IsGrabbed())
            {
                collisionForce = VRTK_DeviceFinder.GetControllerVelocity(grabbingController).magnitude * impactMagnifier;
                var hapticStrength = collisionForce / maxCollisionForce;
                VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(grabbingController), hapticStrength, 0.5f, 0.01f);
            }
            else
            {
                collisionForce = collision.relativeVelocity.magnitude * impactMagnifier;
            }
        }
    }
}