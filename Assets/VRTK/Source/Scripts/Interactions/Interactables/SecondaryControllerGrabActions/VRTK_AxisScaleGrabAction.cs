// Axis Scale Grab Action|SecondaryControllerGrabActions|60030
namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;

    /// <summary>
    /// Scales the grabbed Interactable Object along the given axes based on the position of the secondary grabbing Interact Grab.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_AxisScaleGrabAction` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Secondary Grab Action Script` parameter to denote use of the secondary grab action.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and scale it by grabbing and pulling with the second controller.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Secondary Controller Grab Actions/VRTK_AxisScaleGrabAction")]
    public class VRTK_AxisScaleGrabAction : VRTK_BaseGrabAction
    {
        [Tooltip("The distance the secondary grabbing object must move away from the original grab position before the secondary grabbing object auto ungrabs the Interactable Object.")]
        public float ungrabDistance = 1f;
        [Tooltip("Locks the specified checked axes so they won't be scaled")]
        public Vector3State lockAxis = Vector3State.False;
        [Tooltip("If checked all the axes will be scaled together (unless locked)")]
        public bool uniformScaling = false;

        [Header("Obsolete Settings")]

        [System.Obsolete("`VRTK_AxisScaleGrabAction.lockXAxis` has been replaced with the `VRTK_AxisScaleGrabAction.lockAxis`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public bool lockXAxis = false;
        [System.Obsolete("`VRTK_AxisScaleGrabAction.lockYAxis` has been replaced with the `VRTK_AxisScaleGrabAction.lockAxis`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public bool lockYAxis = false;
        [System.Obsolete("`VRTK_AxisScaleGrabAction.lockZAxis` has been replaced with the `VRTK_AxisScaleGrabAction.lockAxis`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public bool lockZAxis = false;

        protected Vector3 initialScale;
        protected float initalLength;
        protected float initialScaleFactor;

        /// <summary>
        /// The Initalise method is used to set up the state of the secondary action when the Interactable Object is initially grabbed by a secondary Interact Grab.
        /// </summary>
        /// <param name="currentGrabbdObject">The Interactable Object script for the object currently being grabbed by the primary grabbing object.</param>
        /// <param name="currentPrimaryGrabbingObject">The Interact Grab script for the object that is associated with the primary grabbing object.</param>
        /// <param name="currentSecondaryGrabbingObject">The Interact Grab script for the object that is associated with the secondary grabbing object.</param>
        /// <param name="primaryGrabPoint">The point on the Interactable Object where the primary Interact Grab initially grabbed the Interactable Object.</param>
        /// <param name="secondaryGrabPoint">The point on the Interactable Object where the secondary Interact Grab initially grabbed the Interactable Object.</param>
        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
            initialScale = currentGrabbdObject.transform.localScale;
            initalLength = (grabbedObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
            initialScaleFactor = currentGrabbdObject.transform.localScale.x / initalLength;

#pragma warning disable 618
            if ((lockXAxis || lockYAxis || lockZAxis) && lockAxis == Vector3State.False)
            {
                lockAxis = new Vector3State(lockXAxis, lockYAxis, lockZAxis);
            }
#pragma warning restore 618
        }

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary Interact Grab.
        /// </summary>
        public override void ProcessUpdate()
        {
            base.ProcessUpdate();
            CheckForceStopDistance(ungrabDistance);
        }

        /// <summary>
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary Interact Grab and performs the scaling action.
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

            float finalScaleX = (lockAxis.xState ? existingScale.x : newScale.x);
            float finalScaleY = (lockAxis.yState ? existingScale.y : newScale.y);
            float finalScaleZ = (lockAxis.zState ? existingScale.z : newScale.z);

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

            Vector3 newScale = new Vector3(newScaleX, newScaleY, newScaleZ) + initialScale;
            ApplyScale(newScale);
        }

        protected virtual void UniformScale()
        {
            float adjustedLength = (grabbedObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
            float adjustedScale = initialScaleFactor * adjustedLength;

            Vector3 newScale = new Vector3(adjustedScale, adjustedScale, adjustedScale);
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