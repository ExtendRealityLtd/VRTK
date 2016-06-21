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
            collisionForce = collision.impulse.magnitude * impactMagnifier;
            controllerActions.TriggerHapticPulse(40, (ushort)collisionForce);
        }
    }
}
