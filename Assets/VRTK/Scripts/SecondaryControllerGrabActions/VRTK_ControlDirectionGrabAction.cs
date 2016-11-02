// Control Direction Grab Action|SecondaryControllerGrabActions|0030
namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// The Control Direction Grab Action provides a mechanism to control the facing direction of the object when they are grabbed with a secondary controller.
    /// </summary>
    /// <remarks>
    /// For an object to correctly be rotated it must be created with the front of the object pointing down the z-axis (forward) and the upwards of the object pointing up the y-axis (up). 
    ///
    /// It's not possible to control the direction of an interactable object with a `Fixed_Joint` as the joint fixes the rotation of the object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and control their direction with the second controller.
    /// </example>
    public class VRTK_ControlDirectionGrabAction : VRTK_BaseGrabAction
    {
        [Tooltip("The speed in which the object will snap back to it's original rotation when the secondary controller stops grabbing it. `0` for instant snap, `infinity` for no snap back.")]
        public float releaseSnapSpeed = 0.1f;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Quaternion releaseRotation;
        private Coroutine snappingOnRelease;

        /// <summary>
        /// The Initalise method is used to set up the state of the secondary action when the object is initially grabbed by a secondary controller.
        /// </summary>
        /// <param name="currentGrabbdObject">The Interactable Object script for the object currently being grabbed by the primary controller.</param>
        /// <param name="currentPrimaryGrabbingObject">The Interact Grab script for the object that is associated with the primary controller.</param>
        /// <param name="currentSecondaryGrabbingObject">The Interact Grab script for the object that is associated with the secondary controller.</param>
        /// <param name="primaryGrabPoint">The point on the object where the primary controller initially grabbed the object.</param>
        /// <param name="secondaryGrabPoint">The point on the object where the secondary controller initially grabbed the object.</param>
        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
            initialPosition = currentGrabbdObject.transform.localPosition;
            initialRotation = currentGrabbdObject.transform.localRotation;
            StopRealignOnRelease();
        }

        /// <summary>
        /// The ResetAction method is used to reset the secondary action when the object is no longer grabbed by a secondary controller.
        /// </summary>
        public override void ResetAction()
        {
            releaseRotation = transform.localRotation;
            if (!grabbedObject.precisionSnap)
            {
                if (releaseSnapSpeed < float.MaxValue && releaseSnapSpeed > 0)
                {
                    snappingOnRelease = StartCoroutine(RealignOnRelease());
                }
                else if (releaseSnapSpeed == 0)
                {
                    transform.localRotation = initialRotation;
                }
            }
            base.ResetAction();
        }

        /// <summary>
        /// The OnDropAction method is executed when the current grabbed object is dropped and can be used up to clean up any secondary grab actions.
        /// </summary>
        public override void OnDropAction()
        {
            StopRealignOnRelease();
        }

        /// <summary>
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller and influences the rotation of the object.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            if (initialised)
            {
                AimObject();
            }
        }

        private void StopRealignOnRelease()
        {
            if (snappingOnRelease != null)
            {
                StopCoroutine(snappingOnRelease);
            }
            snappingOnRelease = null;
        }

        private IEnumerator RealignOnRelease()
        {
            var elapsedTime = 0f;

            while (elapsedTime < releaseSnapSpeed)
            {
                transform.localRotation = Quaternion.Lerp(releaseRotation, initialRotation, (elapsedTime / releaseSnapSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.localRotation = initialRotation;
            transform.localPosition = initialPosition;
        }

        private void AimObject()
        {
            transform.rotation = Quaternion.LookRotation(secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position, secondaryGrabbingObject.transform.TransformDirection(Vector3.forward));
            if (grabbedObject.precisionSnap)
            {
                transform.Translate(primaryGrabbingObject.controllerAttachPoint.transform.position - primaryInitialGrabPoint.position, Space.World);
            }
        }
    }
}