namespace VRTK.Examples
{
    using UnityEngine;

    public class InteractableLamp : MonoBehaviour
    {
        public VRTK_InteractableObject linkedObject;

        protected Rigidbody[] lampRigidbodies = new Rigidbody[0];

        protected virtual void OnEnable()
        {
            linkedObject = (linkedObject == null ? GetComponent<VRTK_InteractableObject>() : linkedObject);

            if (linkedObject != null)
            {
                linkedObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
                linkedObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
            }

            lampRigidbodies = transform.parent.GetComponentsInChildren<Rigidbody>();
        }

        protected virtual void OnDisable()
        {
            if (linkedObject != null)
            {
                linkedObject.InteractableObjectGrabbed -= InteractableObjectGrabbed;
                linkedObject.InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
            }
        }

        protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            ToggleKinematics(true);
        }

        protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            ToggleKinematics(false);
        }

        protected virtual void ToggleKinematics(bool state)
        {
            foreach (Rigidbody elementRigidbody in lampRigidbodies)
            {
                elementRigidbody.isKinematic = state;
            }
        }
    }
}