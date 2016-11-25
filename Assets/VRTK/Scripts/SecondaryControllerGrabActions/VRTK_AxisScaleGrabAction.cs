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
        [Tooltip("If checked all the axes will be scaled together (unless locked)")]
        public bool uniformScaling = false;

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
                if (uniformScaling)
                {
                    UniformScale();
                }
                else
                {
                    NonUniformScale();
                }
            }
        }

        private void ApplyScale(Vector3 newScale)
        {
            var existingScale = grabbedObject.transform.localScale;
            Vector3 rotatedScale = initialScale + newScale;

            float finalScaleX = (lockXAxis ? existingScale.x : rotatedScale.x);
            float finalScaleY = (lockYAxis ? existingScale.y : rotatedScale.y);
            float finalScaleZ = (lockZAxis ? existingScale.z : rotatedScale.z);
            var finalScale = new Vector3(finalScaleX, finalScaleY, finalScaleZ);

            if (finalScaleX > 0 && finalScaleY > 0 && finalScaleZ > 0)
            {
                grabbedObject.transform.localScale = finalScale;
            }
        }

        private void NonUniformScale()
        {
            Vector3 initialRotatedPosition = grabbedObject.transform.rotation * grabbedObject.transform.position;
            Vector3 initialSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryInitialGrabPoint.position;
            Vector3 currentSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryGrabbingObject.transform.position;

            float newScaleX = CalculateAxisScale(initialRotatedPosition.x, initialSecondGrabRotatedPosition.x, currentSecondGrabRotatedPosition.x);
            float newScaleY = CalculateAxisScale(initialRotatedPosition.y, initialSecondGrabRotatedPosition.y, currentSecondGrabRotatedPosition.y);
            float newScaleZ = CalculateAxisScale(initialRotatedPosition.z, initialSecondGrabRotatedPosition.z, currentSecondGrabRotatedPosition.z);

            var newScale = new Vector3(newScaleX, newScaleY, newScaleZ);
            ApplyScale(newScale);
        }

        private void UniformScale()
        {
            Vector3 initialRotatedPosition = grabbedObject.transform.rotation * grabbedObject.transform.position;
            Vector3 initialSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryInitialGrabPoint.position;
            Vector3 currentSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryGrabbingObject.transform.position;

            float initialDistance = Vector3.Distance(initialSecondGrabRotatedPosition, initialRotatedPosition);
            float distance = Vector3.Distance(currentSecondGrabRotatedPosition, initialRotatedPosition);
            float deltaDistance = distance - initialDistance;

            Vector3 newScale = new Vector3(deltaDistance, deltaDistance, deltaDistance);
            ApplyScale(newScale);
        }

        private float CalculateAxisScale(float centerPosition, float initialPosition, float currentPosition)
        {
            float distance = currentPosition - initialPosition;
            distance = (centerPosition < initialPosition ? distance : -distance);
            return distance;
        }
    }
}