﻿// Content Handler|Controls3D|100100
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Manages objects defined as content. When taking out an object from a drawer and closing the drawer this object would otherwise disappear even if outside the drawer.
    /// </summary>
    /// <remarks>
    /// The script will use the boundaries of the control to determine if it is in or out and re-parent the object as necessary. It can be put onto individual objects or the parent of multiple objects. Using the latter way all interactable objects under that parent will become managed by the script.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` has a drawer with a collection of items that adhere to this concept.
    /// </example>
    [System.Obsolete("`VRTK_ContentHandler` has been deprecated. This script will be removed in a future version of VRTK.")]
    public class VRTK_ContentHandler : MonoBehaviour
    {
        [Tooltip("The 3D control responsible for the content.")]
        public VRTK_Control control;
        [Tooltip("The transform containing the meshes or colliders that define the inside of the control.")]
        public Transform inside;
        [Tooltip("Any transform that will act as the parent while the object is not inside the control.")]
        public Transform outside;

        protected virtual void Start()
        {
            VRTK_InteractableObject contentInteractableObject = GetComponent<VRTK_InteractableObject>();
            if (contentInteractableObject == null)
            {
                // treat as parent and assign to all children
                VRTK_InteractableObject[] foundObjects = GetComponentsInChildren<VRTK_InteractableObject>();
                for (int i = 0; i < foundObjects.Length; i++)
                {
                    VRTK_InteractableObject foundIO = foundObjects[i];
                    if (foundIO.GetComponent<VRTK_ContentHandler>() == null)
                    {
                        VRTK_ContentHandler childContentHandler = foundIO.gameObject.AddComponent<VRTK_ContentHandler>();
                        childContentHandler.control = control;
                        childContentHandler.inside = inside;
                        childContentHandler.outside = outside;
                    }
                }
            }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (control != null)
            {
                Bounds insideBounds = VRTK_SharedMethods.GetBounds(inside, null, control.GetContent().transform);
                Bounds objBounds = VRTK_SharedMethods.GetBounds(transform);

                if (objBounds.Intersects(insideBounds))
                {
                    transform.SetParent(control.GetContent().transform);
                }
                else
                {
                    transform.SetParent(outside);
                }
            }
        }
    }
}