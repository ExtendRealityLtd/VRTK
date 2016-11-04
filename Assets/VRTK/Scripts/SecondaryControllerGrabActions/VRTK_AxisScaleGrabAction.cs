// Axis Scale Grab Action|SecondaryControllerGrabActions|0020
namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;

    /// <summary>
    /// The Axis Scale Grab Action provides a mechanism to scale objects when they are grabbed with a secondary controller.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and scale it by grabbing and pulling with the second controller.
    /// </example>
    public class VRTK_AxisScaleGrabAction : VRTK_BaseGrabAction
    {
        [Tooltip("If checked the current X Axis of the object won't be scaled")]
        public bool lockXAxis = false;
        [Tooltip("If checked the current Y Axis of the object won't be scaled")]
        public bool lockYAxis = false;
        [Tooltip("If checked the current Z Axis of the object won't be scaled")]
        public bool lockZAxis = false;

        private Vector3 initialScale;

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
            initialScale = currentGrabbdObject.transform.localScale;
        }

        /// <summary>
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller and performs the scaling action.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            if (initialised)
            {
                NonUniformScale();
            }
        }

        private void NonUniformScale()
        {
            var existingScale = grabbedObject.transform.localScale;
            var initialRotatedPosition = grabbedObject.transform.rotation * grabbedObject.transform.position;
            var initialSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryInitialGrabPoint.position;
            var currentSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryGrabbingObject.transform.position;

            var newScaleX = CalculateAxisScale(initialRotatedPosition.x, initialSecondGrabRotatedPosition.x, currentSecondGrabRotatedPosition.x);
            var newScaleY = CalculateAxisScale(initialRotatedPosition.y, initialSecondGrabRotatedPosition.y, currentSecondGrabRotatedPosition.y);
            var newScaleZ = CalculateAxisScale(initialRotatedPosition.z, initialSecondGrabRotatedPosition.z, currentSecondGrabRotatedPosition.z);

            var newScale = new Vector3(newScaleX, newScaleY, newScaleZ);

            var rotatedScale = initialScale + newScale;

            var finalScaleX = (lockXAxis ? existingScale.x : rotatedScale.x);
            var finalScaleY = (lockYAxis ? existingScale.y : rotatedScale.y);
            var finalScaleZ = (lockZAxis ? existingScale.z : rotatedScale.z);
            var finalScale = new Vector3(finalScaleX, finalScaleY, finalScaleZ);

            if (finalScaleX > 0 && finalScaleY > 0 && finalScaleZ > 0)
            {
                grabbedObject.transform.localScale = finalScale;
            }
        }

        private float CalculateAxisScale(float centerPosition, float initialPosition, float currentPosition)
        {
            var distance = currentPosition - initialPosition;
            distance = (centerPosition < initialPosition ? distance : -distance);
            return distance;
        }
    }
}