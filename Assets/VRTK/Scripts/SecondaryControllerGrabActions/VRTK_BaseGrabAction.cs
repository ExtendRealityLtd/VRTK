// Base Grab Action|SecondaryControllerGrabActions|0010
namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;

    /// <summary>
    /// The Base Grab Action is an abstract class that all other secondary controller actions inherit and are required to implement the public abstract methods.
    /// </summary>
    /// <remarks>
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseGrabAction : MonoBehaviour
    {
        protected VRTK_InteractableObject grabbedObject;
        protected VRTK_InteractGrab primaryGrabbingObject;
        protected VRTK_InteractGrab secondaryGrabbingObject;
        protected Transform primaryInitialGrabPoint;
        protected Transform secondaryInitialGrabPoint;
        protected bool initialised = false;

        /// <summary>
        /// The Initalise method is used to set up the state of the secondary action when the object is initially grabbed by a secondary controller.
        /// </summary>
        /// <param name="currentGrabbdObject">The Interactable Object script for the object currently being grabbed by the primary controller.</param>
        /// <param name="currentPrimaryGrabbingObject">The Interact Grab script for the object that is associated with the primary controller.</param>
        /// <param name="currentSecondaryGrabbingObject">The Interact Grab script for the object that is associated with the secondary controller.</param>
        /// <param name="primaryGrabPoint">The point on the object where the primary controller initially grabbed the object.</param>
        /// <param name="secondaryGrabPoint">The point on the object where the secondary controller initially grabbed the object.</param>
        public virtual void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            grabbedObject = currentGrabbdObject;
            primaryGrabbingObject = currentPrimaryGrabbingObject;
            secondaryGrabbingObject = currentSecondaryGrabbingObject;
            primaryInitialGrabPoint = primaryGrabPoint;
            secondaryInitialGrabPoint = secondaryGrabPoint;
            initialised = true;
        }

        /// <summary>
        /// The ResetAction method is used to reset the secondary action when the object is no longer grabbed by a secondary controller.
        /// </summary>
        public virtual void ResetAction()
        {
            grabbedObject = null;
            primaryGrabbingObject = null;
            secondaryGrabbingObject = null;
            primaryInitialGrabPoint = null;
            secondaryInitialGrabPoint = null;
            initialised = false;
        }

        /// <summary>
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller.
        /// </summary>
        public abstract void ProcessFixedUpdate();

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary controller.
        /// </summary>
        public abstract void ProcessUpdate();
    }
}