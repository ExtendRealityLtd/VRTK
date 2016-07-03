using UnityEngine;
using System.Collections;
using VRTK;

public class ArrowNotch : MonoBehaviour
{
    private GameObject arrow;
    private VRTK_InteractableObject obj;

    private void Start()
    {
        arrow = this.transform.FindChild("Arrow").gameObject;
        obj = this.GetComponent<VRTK_InteractableObject>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        var handle = collider.GetComponentInParent<BowHandle>();

        if (handle != null && obj != null && handle.aim.IsHeld() && obj.IsGrabbed())
        {
            handle.nockSide = collider.transform;
            arrow.transform.parent = handle.arrowNockingPoint;

            CopyNotchToArrow();

            collider.GetComponentInParent<BowAim>().SetArrow(arrow);
            Destroy(this.gameObject);
        }
    }

    private void CopyNotchToArrow()
    {
        GameObject notchCopy = Instantiate(this.gameObject, this.transform.position, this.transform.rotation) as GameObject;
        notchCopy.name = this.name;
        arrow.GetComponent<Arrow>().SetArrowHolder(notchCopy);
        arrow.GetComponent<Arrow>().OnNock();
    }
}