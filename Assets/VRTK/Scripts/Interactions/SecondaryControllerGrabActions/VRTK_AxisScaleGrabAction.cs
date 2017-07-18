// Axis Scale Grab Action|SecondaryControllerGrabActions|60030
namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;

    /// <summary>
    /// The Axis Scale Grab Action provides a mechanism to scale objects when they are grabbed with a secondary controller.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and scale it by grabbing and pulling with the second controller.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Secondary Controller Grab Actions/VRTK_AxisScaleGrabAction")]
    public class VRTK_AxisScaleGrabAction : VRTK_BaseGrabAction
    {
        [Tooltip("The distance the secondary controller must move away from the original grab position before the secondary controller auto ungrabs the object.")]
        public float ungrabDistance = 1f;
        [Tooltip("If checked the current X Axis of the object won't be scaled")]
        public bool lockXAxis = false;
        [Tooltip("If checked the current Y Axis of the object won't be scaled")]
        public bool lockYAxis = false;
        [Tooltip("If checked the current Z Axis of the object won't be scaled")]
        public bool lockZAxis = false;
        [Tooltip("If checked all the axes will be scaled together (unless locked)")]
        public bool uniformScaling = false;

        protected Vector3 initialScale;
        protected float initalLength;
        protected float initialScaleFactor;

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
            initalLength = (grabbedObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
            initialScaleFactor = currentGrabbdObject.transform.localScale.x / initalLength;
        }

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary controller.
        /// </summary>
        public override void ProcessUpdate()
        {
            base.ProcessUpdate();
            CheckForceStopDistance(ungrabDistance);
        }

        /// <summary>
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller and performs the scaling action.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            base.ProcessFixedUpdate();
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

        protected virtual void ApplyScale(Vector3 newScale)
        {
            Vector3 existingScale = grabbedObject.transform.localScale;

            float finalScaleX = (lockXAxis ? existingScale.x : newScale.x);
            float finalScaleY = (lockYAxis ? existingScale.y : newScale.y);
            float finalScaleZ = (lockZAxis ? existingScale.z : newScale.z);

            if (finalScaleX > 0 && finalScaleY > 0 && finalScaleZ > 0)
            {
                grabbedObject.transform.localScale = new Vector3(finalScaleX, finalScaleY, finalScaleZ); ;
            }
        }

        protected virtual void NonUniformScale()
        {
            Vector3 initialRotatedPosition = grabbedObject.transform.rotation * grabbedObject.transform.position;
            Vector3 initialSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryInitialGrabPoint.position;
            Vector3 currentSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryGrabbingObject.transform.position;

            float newScaleX = CalculateAxisScale(initialRotatedPosition.x, initialSecondGrabRotatedPosition.x, currentSecondGrabRotatedPosition.x);
            float newScaleY = CalculateAxisScale(initialRotatedPosition.y, initialSecondGrabRotatedPosition.y, currentSecondGrabRotatedPosition.y);
            float newScaleZ = CalculateAxisScale(initialRotatedPosition.z, initialSecondGrabRotatedPosition.z, currentSecondGrabRotatedPosition.z);

            var newScale = new Vector3(newScaleX, newScaleY, newScaleZ) + initialScale;
            ApplyScale(newScale);
        }

        protected virtual void UniformScale()
        {
            float adjustedLength = (grabbedObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
            float adjustedScale = initialScaleFactor * adjustedLength;

            var newScale = new Vector3(adjustedScale, adjustedScale, adjustedScale);
            ApplyScale(newScale);
        }

        protected virtual float CalculateAxisScale(float centerPosition, float initialPosition, float currentPosition)
        {
            float distance = currentPosition - initialPosition;
            distance = (centerPosition < initialPosition ? distance : -distance);
            return distance;
        }
    }
}