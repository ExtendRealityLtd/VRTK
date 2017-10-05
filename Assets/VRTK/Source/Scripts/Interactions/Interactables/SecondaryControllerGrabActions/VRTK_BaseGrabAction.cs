// Base Grab Action|SecondaryControllerGrabActions|60010
namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;

    /// <summary>
    /// Provides a base that all secondary controller grab attach can inherit from.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///   > This is an abstract class that is to be inherited to a concrete class that provides secondary controller grab action functionality, therefore this script should not be directly used.
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
        /// The Initalise method is used to set up the state of the secondary action when the Interactable Object is initially grabbed by a secondary Interact Grab.
        /// </summary>
        /// <param name="currentGrabbdObject">The Interactable Object script for the object currently being grabbed by the primary grabbing object.</param>
        /// <param name="currentPrimaryGrabbingObject">The Interact Grab script for the object that is associated with the primary grabbing object.</param>
        /// <param name="currentSecondaryGrabbingObject">The Interact Grab script for the object that is associated with the secondary grabbing object.</param>
        /// <param name="primaryGrabPoint">The point on the Interactable Object where the primary Interact Grab initially grabbed the Interactable Object.</param>
        /// <param name="secondaryGrabPoint">The point on the Interactable Object where the secondary Interact Grab initially grabbed the Interactable Object.</param>
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
        /// The ResetAction method is used to reset the secondary action when the Interactable Object is no longer grabbed by a secondary Interact Grab.
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
        /// <returns>Returns `true` if the secondary grab action does perform an action on secondary grab.</returns>
        public virtual bool IsActionable()
        {
            return isActionable;
        }

        /// <summary>
        /// The IsSwappable method is used to determine if the secondary grab action allows to swab the grab state to another grabbing Interactable Object.
        /// </summary>
        /// <returns>Returns `true` if the grab action allows swapping to another grabbing object.</returns>
        public virtual bool IsSwappable()
        {
            return isSwappable;
        }

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary Interact Grab.
        /// </summary>
        public virtual void ProcessUpdate()
        {
        }

        /// <summary>
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary Interact Grab.
        /// </summary>
        public virtual void ProcessFixedUpdate()
        {
        }

        /// <summary>
        /// The OnDropAction method is executed when the current grabbed Interactable Object is dropped and can be used up to clean up any secondary grab actions.
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