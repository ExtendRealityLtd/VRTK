using UnityEngine;
using System.Collections;

public class Sword : SteamVR_InteractableObject {
    SteamVR_ControllerActions controllerActions;
    float impactMagnifier = 100f;
    float collisionForce = 0f;

    public float CollisionForce()
    {
        return collisionForce;
    }

    public override void Grabbed(GameObject grabbingObject)
    {
        base.Grabbed(grabbingObject);
        controllerActions = grabbingObject.GetComponent<SteamVR_ControllerActions>();
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
