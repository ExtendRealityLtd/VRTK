using UnityEngine;
using System.Collections;
using VRTK;

public class Sword : VRTK_InteractableObject
{
    VRTK_ControllerActions controllerActions;
    float impactMagnifier = 200f;
    float collisionForce = 0f;

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
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (controllerActions && IsGrabbed())
        {
#if UNITY_5_3_OR_NEWER
            collisionForce = collision.impulse.magnitude * impactMagnifier;
#else
            collisionForce = collision.relativeVelocity.magnitude*impactMagnifier;
#endif
            controllerActions.TriggerHapticPulse(40, (ushort)collisionForce);
        }
    }
}
