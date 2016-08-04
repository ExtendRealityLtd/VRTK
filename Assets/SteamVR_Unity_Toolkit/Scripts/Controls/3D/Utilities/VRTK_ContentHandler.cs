namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// This script will make objects inside of control contents properly interactable by reparenting them. 
    /// Otherwise they would disappear if e.g. a drawer is closed even if the object was outside the drawer.
    /// We need to go through collisions instead of hooking to the Ungrabbed event since ungrabbing will most likely happen above the inside causing the object to drop first. 
    /// This approach is much more stable and allows for many more scenarios.
    /// Not supported yet: detect if an object is taken from one content manager and dropped into another one.
    /// </summary>
    public class VRTK_ContentHandler : MonoBehaviour
    {
        public VRTK_Control control;
        public Transform inside;
        public Transform outside;

        private void Start()
        {
            VRTK_InteractableObject io = GetComponent<VRTK_InteractableObject>();
            if (io == null)
            {
                // treat as parent and assign to all children
                foreach (VRTK_InteractableObject childIo in GetComponentsInChildren<VRTK_InteractableObject>())
                {
                    if (childIo.GetComponent<VRTK_ContentHandler>() == null)
                    {
                        VRTK_ContentHandler ch = childIo.gameObject.AddComponent<VRTK_ContentHandler>();
                        ch.control = control;
                        ch.inside = inside;
                        ch.outside = outside;
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Bounds insideBounds = Utilities.GetBounds(inside, null, control.GetContent().transform);
            Bounds objBounds = Utilities.GetBounds(transform);

            if (objBounds.Intersects(insideBounds))
            {
                transform.parent = control.GetContent().transform;
            }
            else
            {
                transform.parent = outside;
            }
        }

    }
}