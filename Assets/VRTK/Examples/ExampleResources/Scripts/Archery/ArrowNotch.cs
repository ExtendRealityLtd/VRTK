namespace VRTK.Examples.Archery
{
    using UnityEngine;

    public class ArrowNotch : MonoBehaviour
    {
        private GameObject arrow;
        private VRTK_InteractableObject obj;

        private void Start()
        {
            arrow = transform.FindChild("Arrow").gameObject;
            obj = GetComponent<VRTK_InteractableObject>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            var handle = collider.GetComponentInParent<BowHandle>();

            if (handle != null && obj != null && handle.aim.IsHeld() && obj.IsGrabbed())
            {
                handle.nockSide = collider.transform;
                arrow.transform.SetParent(handle.arrowNockingPoint);

                CopyNotchToArrow();

                collider.GetComponentInParent<BowAim>().SetArrow(arrow);
                Destroy(gameObject);
            }
        }

        private void CopyNotchToArrow()
        {
            GameObject notchCopy = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;
            notchCopy.name = name;
            arrow.GetComponent<Arrow>().SetArrowHolder(notchCopy);
            arrow.GetComponent<Arrow>().OnNock();
        }
    }
}