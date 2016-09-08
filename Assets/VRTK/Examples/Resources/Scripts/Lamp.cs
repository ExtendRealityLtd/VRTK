namespace VRTK.Examples
{
    using UnityEngine;

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
            foreach (Rigidbody rigid in transform.parent.GetComponentsInChildren<Rigidbody>())
            {
                rigid.isKinematic = state;
            }
        }
    }
}