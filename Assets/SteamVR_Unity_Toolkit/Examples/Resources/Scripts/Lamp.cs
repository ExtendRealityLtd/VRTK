using UnityEngine;
using System.Collections;

public class Lamp : VRTK_InteractableObject
{
    public override void Grabbed(GameObject grabbingObject)
    {
        base.Grabbed(grabbingObject);
        ToggleKinematics(false);
    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);
        ToggleKinematics(true);
    }

    private void ToggleKinematics(bool state)
    {
        foreach(Rigidbody rigid in this.transform.parent.GetComponentsInChildren<Rigidbody>())
        {
            rigid.isKinematic = state;
        }
    }
}
