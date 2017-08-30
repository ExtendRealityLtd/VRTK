// Control Direction Grab Action|SecondaryControllerGrabActions|60040
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
    [AddComponentMenu("VRTK/Scripts/Interactions/Secondary Controller Grab Actions/VRTK_ControlDirectionGrabAction")]
    public class VRTK_ControlDirectionGrabAction : VRTK_BaseGrabAction
    {
        [Tooltip("The distance the secondary controller must move away from the original grab position before the secondary controller auto ungrabs the object.")]
        public float ungrabDistance = 1f;
        [Tooltip("The speed in which the object will snap back to it's original rotation when the secondary controller stops grabbing it. `0` for instant snap, `infinity` for no snap back.")]
        public float releaseSnapSpeed = 0.1f;
        [Tooltip("Prevent the secondary controller rotating the grabbed object through it's z-axis.")]
        public bool lockZRotation = true;

        protected Vector3 initialPosition;
        protected Quaternion initialRotation;
        protected Quaternion releaseRotation;
        protected Coroutine snappingOnRelease;

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
            if (!grabbedObject.grabAttachMechanicScript.precisionGrab)
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
            base.OnDropAction();
            StopRealignOnRelease();
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
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller and influences the rotation of the object.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            base.ProcessFixedUpdate();
            if (initialised)
            {
                AimObject();
            }
        }

        protected virtual void StopRealignOnRelease()
        {
            if (snappingOnRelease != null)
            {
                StopCoroutine(snappingOnRelease);
            }
            snappingOnRelease = null;
        }

        protected virtual IEnumerator RealignOnRelease()
        {
            float elapsedTime = 0f;

            while (elapsedTime < releaseSnapSpeed)
            {
                transform.localRotation = Quaternion.Lerp(releaseRotation, initialRotation, (elapsedTime / releaseSnapSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.localRotation = initialRotation;
            transform.localPosition = initialPosition;
        }

        protected virtual void AimObject()
        {
            if (lockZRotation)
            {
                ZLockedAim();
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position, secondaryGrabbingObject.transform.TransformDirection(Vector3.forward));
            }

            if (grabbedObject.grabAttachMechanicScript.precisionGrab)
            {
                transform.Translate(primaryGrabbingObject.controllerAttachPoint.transform.position - primaryInitialGrabPoint.position, Space.World);
            }
        }

        protected virtual void ZLockedAim()
        {
            Vector3 forward = (secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position).normalized;

            // calculate rightLocked rotation
            Quaternion rightLocked = Quaternion.LookRotation(forward, Vector3.Cross(-primaryGrabbingObject.transform.right, forward).normalized);

            // delta from current rotation to the rightLocked rotation
            Quaternion rightLockedDelta = Quaternion.Inverse(grabbedObject.transform.rotation) * rightLocked;

            float rightLockedAngle;
            Vector3 rightLockedAxis;

            // forward direction and roll
            rightLockedDelta.ToAngleAxis(out rightLockedAngle, out rightLockedAxis);

            if (rightLockedAngle > 180f)
            {
                // remap ranges from 0-360 to -180 to 180
                rightLockedAngle -= 360f;
            }

            // make any negative values into positive values;
            rightLockedAngle = Mathf.Abs(rightLockedAngle);

            // calculate upLocked rotation
            Quaternion upLocked = Quaternion.LookRotation(forward, primaryGrabbingObject.transform.forward);

            // delta from current rotation to the upLocked rotation
            Quaternion upLockedDelta = Quaternion.Inverse(grabbedObject.transform.rotation) * upLocked;

            float upLockedAngle;
            Vector3 upLockedAxis;

            // forward direction and roll
            upLockedDelta.ToAngleAxis(out upLockedAngle, out upLockedAxis);

            // remap ranges from 0-360 to -180 to 180
            if (upLockedAngle > 180f)
            {
                upLockedAngle -= 360f;
            }

            // make any negative values into positive values;
            upLockedAngle = Mathf.Abs(upLockedAngle);

            // assign the one that involves less change to roll
            grabbedObject.transform.rotation = (upLockedAngle < rightLockedAngle ? upLocked : rightLocked);
        }
    }
}