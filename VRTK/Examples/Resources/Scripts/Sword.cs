namespace VRTK.Examples
{
    using UnityEngine;

    public class Sword : VRTK_InteractableObject
    {
        private VRTK_ControllerActions controllerActions;
        private float impactMagnifier = 120f;
        private float collisionForce = 0f;
        private float maxCollisionForce = 4000f;

        public float CollisionForce()
        {
            return collisionForce;
        }

        public override void Grabbed(GameObject grabbingObject)
        {
            base.Grabbed(grabbingObject);
            controllerActions = grabbingObject.GetComponent<VRTK_ControllerActions>();
        }

        protected override void Awake()
        {
            base.Awake();
            interactableRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (controllerActions && IsGrabbed())
            {
                collisionForce = VRTK_DeviceFinder.GetControllerVelocity(controllerActions.gameObject).magnitude * impactMagnifier;
                var hapticStrength = collisionForce / maxCollisionForce;
                controllerActions.TriggerHapticPulse(hapticStrength, 0.5f, 0.01f);
            }
            else
            {
                collisionForce = collision.relativeVelocity.magnitude * impactMagnifier;
            }
        }
    }
}