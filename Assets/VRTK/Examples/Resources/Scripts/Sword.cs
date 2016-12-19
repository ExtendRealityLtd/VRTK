namespace VRTK.Examples
{
    using UnityEngine;

    public class Sword : VRTK_InteractableObject
    {
        private VRTK_ControllerActions controllerActions;
        private VRTK_ControllerEvents controllerEvents;
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
            controllerEvents = grabbingObject.GetComponent<VRTK_ControllerEvents>();
        }

        protected override void Awake()
        {
            base.Awake();
            interactableRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (controllerActions && controllerEvents && IsGrabbed())
            {
                collisionForce = controllerEvents.GetVelocity().magnitude * impactMagnifier;
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