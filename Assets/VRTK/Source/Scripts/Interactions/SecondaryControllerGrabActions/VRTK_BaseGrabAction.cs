// Base Grab Action|SecondaryControllerGrabActions|60010
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
        protected bool isActionable = true;
        protected bool isSwappable = false;

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
        /// The IsActionable method is used to determine if the secondary grab action performs an action on grab.
        /// </summary>
        /// <returns>Is true if the secondary grab action does perform an action on secondary grab.</returns>
        public virtual bool IsActionable()
        {
            return isActionable;
        }

        /// <summary>
        /// The IsSwappable method is used to determine if the secondary grab action allows to swab the grab state to another grabbing object.
        /// </summary>
        /// <returns>Is true if the grab action allows swapping to another grabbing object.</returns>
        public virtual bool IsSwappable()
        {
            return isSwappable;
        }

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary controller.
        /// </summary>
        public virtual void ProcessUpdate()
        {
        }

        /// <summary>
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller.
        /// </summary>
        public virtual void ProcessFixedUpdate()
        {
        }

        /// <summary>
        /// The OnDropAction method is executed when the current grabbed object is dropped and can be used up to clean up any secondary grab actions.
        /// </summary>
        public virtual void OnDropAction()
        {
        }

        protected virtual void CheckForceStopDistance(float ungrabDistance)
        {
            if (initialised && Vector3.Distance(secondaryGrabbingObject.transform.position, secondaryInitialGrabPoint.position) > ungrabDistance)
            {
                grabbedObject.ForceStopSecondaryGrabInteraction();
            }
        }
    }
}