﻿// Button|Controls3D|100020
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="Control3DEventArgs"/></param>
    public delegate void Button3DEventHandler(object sender, Control3DEventArgs e);

    /// <summary>
    /// Attaching the script to a game object will allow the user to interact with it as if it were a push button. The direction into which the button should be pushable can be freely set and auto-detection is supported. Since this is physics-based there needs to be empty space in the push direction so that the button can move.
    /// </summary>
    /// <remarks>
    /// The script will instantiate the required Rigidbody and ConstantForce components automatically in case they do not exist yet.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/025_Controls_Overview` shows a collection of pressable buttons that are interacted with by activating the rigidbody on the controller by pressing the grab button without grabbing an object.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Controls/3D/VRTK_Button")]
    public class VRTK_Button : VRTK_Control
    {

        [Serializable]
        [Obsolete("`VRTK_Control.ButtonEvents` has been replaced with delegate events. `VRTK_Button_UnityEvents` is now required to access Unity events. This method will be removed in a future version of VRTK.")]
        public class ButtonEvents
        {
            public UnityEvent OnPush;
        }

        /// <summary>
        /// 3D Control Button Directions
        /// </summary>
        /// <param name="autodetect">Attempt to auto detect the axis</param>
        /// <param name="x">X axis</param>
        /// <param name="y">Y axis</param>
        /// <param name="z">Z axis</param>
        /// <param name="negX">Negative X axis</param>
        /// <param name="negY">Negative Y axis</param>
        /// <param name="negZ">Negative Z axis</param>
        public enum ButtonDirection
        {
            autodetect,
            x,
            y,
            z,
            negX,
            negY,
            negZ
        }

        [Tooltip("An optional game object to which the button will be connected. If the game object moves the button will follow along.")]
        public GameObject connectedTo;
        [Tooltip("The axis on which the button should move. All other axis will be frozen.")]
        public ButtonDirection direction = ButtonDirection.autodetect;
        [Tooltip("The local distance the button needs to be pushed until a push event is triggered.")]
        public float activationDistance = 1.0f;
        [Tooltip("The amount of force needed to push the button down as well as the speed with which it will go back into its original position.")]
        public float buttonStrength = 5.0f;

        [Tooltip("The events specific to the button control. This parameter is deprecated and will be removed in a future version of VRTK.")]
        [Obsolete("`VRTK_Control.events` has been replaced with delegate events. `VRTK_Button_UnityEvents` is now required to access Unity events. This method will be removed in a future version of VRTK.")]
        public ButtonEvents events;

        /// <summary>
        /// Emitted when the 3D Button has reached it's activation distance.
        /// </summary>
        public event Button3DEventHandler Pushed;

        protected const float MAX_AUTODETECT_ACTIVATION_LENGTH = 4f; // full hight of button
        protected ButtonDirection finalDirection;
        protected Vector3 restingPosition;
        protected Vector3 activationDir;
        protected Rigidbody buttonRigidbody;
        protected ConfigurableJoint buttonJoint;
        protected ConstantForce buttonForce;
        protected int forceCount = 0;

        public virtual void OnPushed(Control3DEventArgs e)
        {
            if (Pushed != null)
            {
                Pushed(this, e);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!enabled || !setupSuccessful)
            {
                return;
            }

            // visualize activation distance
            Gizmos.DrawLine(bounds.center, bounds.center + activationDir);
        }

        protected override void InitRequiredComponents()
        {
            restingPosition = transform.position;

            if (!GetComponent<Collider>())
            {
                gameObject.AddComponent<BoxCollider>();
            }

            buttonRigidbody = GetComponent<Rigidbody>();
            if (buttonRigidbody == null)
            {
                buttonRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            buttonRigidbody.isKinematic = false;
            buttonRigidbody.useGravity = false;

            buttonForce = GetComponent<ConstantForce>();
            if (buttonForce == null)
            {
                buttonForce = gameObject.AddComponent<ConstantForce>();
            }

            if (connectedTo)
            {
                Rigidbody connectedToRigidbody = connectedTo.GetComponent<Rigidbody>();
                if (connectedToRigidbody == null)
                {
                    connectedToRigidbody = connectedTo.AddComponent<Rigidbody>();
                }
                connectedToRigidbody.useGravity = false;
            }
        }

        protected override bool DetectSetup()
        {
            finalDirection = (direction == ButtonDirection.autodetect ? DetectDirection() : direction);
            if (finalDirection == ButtonDirection.autodetect)
            {
                activationDir = Vector3.zero;
                return false;
            }
            if (direction != ButtonDirection.autodetect)
            {
                activationDir = CalculateActivationDir();
            }

            if (buttonForce)
            {
                buttonForce.force = GetForceVector();
            }

            if (Application.isPlaying)
            {
                buttonJoint = GetComponent<ConfigurableJoint>();

                bool recreate = false;
                Rigidbody oldBody = null;
                Vector3 oldAnchor = Vector3.zero;
                Vector3 oldAxis = Vector3.zero;

                if (buttonJoint)
                {
                    // save old values, needs to be recreated
                    oldBody = buttonJoint.connectedBody;
                    oldAnchor = buttonJoint.anchor;
                    oldAxis = buttonJoint.axis;
                    DestroyImmediate(buttonJoint);
                    recreate = true;
                }

                // since limit applies to both directions object needs to be moved halfway to activation before adding joint
                transform.position = transform.position + ((activationDir.normalized * activationDistance) * 0.5f);
                buttonJoint = gameObject.AddComponent<ConfigurableJoint>();

                if (recreate)
                {
                    buttonJoint.connectedBody = oldBody;
                    buttonJoint.anchor = oldAnchor;
                    buttonJoint.axis = oldAxis;
                }
                if (connectedTo)
                {
                    buttonJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
                }

                SoftJointLimit buttonJointLimits = new SoftJointLimit();
                buttonJointLimits.limit = activationDistance * 0.501f; // set limit to half (since it applies to both directions) and a tiny bit larger since otherwise activation distance might be missed
                buttonJoint.linearLimit = buttonJointLimits;

                buttonJoint.angularXMotion = ConfigurableJointMotion.Locked;
                buttonJoint.angularYMotion = ConfigurableJointMotion.Locked;
                buttonJoint.angularZMotion = ConfigurableJointMotion.Locked;
                buttonJoint.xMotion = ConfigurableJointMotion.Locked;
                buttonJoint.yMotion = ConfigurableJointMotion.Locked;
                buttonJoint.zMotion = ConfigurableJointMotion.Locked;

                switch (finalDirection)
                {
                    case ButtonDirection.x:
                    case ButtonDirection.negX:
                        if (Mathf.RoundToInt(Mathf.Abs(transform.right.x)) == 1)
                        {
                            buttonJoint.xMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.up.x)) == 1)
                        {
                            buttonJoint.yMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.x)) == 1)
                        {
                            buttonJoint.zMotion = ConfigurableJointMotion.Limited;
                        }
                        break;
                    case ButtonDirection.y:
                    case ButtonDirection.negY:
                        if (Mathf.RoundToInt(Mathf.Abs(transform.right.y)) == 1)
                        {
                            buttonJoint.xMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.up.y)) == 1)
                        {
                            buttonJoint.yMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.y)) == 1)
                        {
                            buttonJoint.zMotion = ConfigurableJointMotion.Limited;
                        }
                        break;
                    case ButtonDirection.z:
                    case ButtonDirection.negZ:
                        if (Mathf.RoundToInt(Mathf.Abs(transform.right.z)) == 1)
                        {
                            buttonJoint.xMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.up.z)) == 1)
                        {
                            buttonJoint.yMotion = ConfigurableJointMotion.Limited;
                        }
                        else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.z)) == 1)
                        {
                            buttonJoint.zMotion = ConfigurableJointMotion.Limited;
                        }
                        break;
                }
            }

            return true;
        }

        protected override ControlValueRange RegisterValueRange()
        {
            return new ControlValueRange()
            {
                controlMin = 0,
                controlMax = 1
            };
        }

        protected override void HandleUpdate()
        {
            // trigger events
            float oldState = value;
            if (ReachedActivationDistance())
            {
                if (oldState == 0)
                {
                    value = 1;

#pragma warning disable 0618
                    /// <obsolete>
                    /// This is an obsolete call that will be removed in a future version
                    /// </obsolete>
                    events.OnPush.Invoke();
#pragma warning restore 0618

                    OnPushed(SetControlEvent());
                }
            }
            else
            {
                value = 0;
            }
        }

        protected virtual void FixedUpdate()
        {
            // update reference position if no force is acting on the button to support scenarios where the button is moved at runtime with a connected body
            if (forceCount == 0 && buttonJoint.connectedBody)
            {
                restingPosition = transform.position;
            }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            // TODO: this will not always be triggered for some reason, we probably need some "healing"
            forceCount -= 1;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            forceCount += 1;
        }

        protected virtual ButtonDirection DetectDirection()
        {
            ButtonDirection returnDirection = ButtonDirection.autodetect;
            Bounds bounds = VRTK_SharedMethods.GetBounds(transform);

            // shoot rays from the center of the button to learn about surroundings
            RaycastHit hitForward;
            RaycastHit hitBack;
            RaycastHit hitLeft;
            RaycastHit hitRight;
            RaycastHit hitUp;
            RaycastHit hitDown;
            Physics.Raycast(bounds.center, Vector3.forward, out hitForward, bounds.extents.z * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.back, out hitBack, bounds.extents.z * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.left, out hitLeft, bounds.extents.x * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.right, out hitRight, bounds.extents.x * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.up, out hitUp, bounds.extents.y * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);
            Physics.Raycast(bounds.center, Vector3.down, out hitDown, bounds.extents.y * MAX_AUTODETECT_ACTIVATION_LENGTH, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);

            // shortest valid ray wins
            float lengthX = (hitRight.collider != null) ? hitRight.distance : float.MaxValue;
            float lengthY = (hitDown.collider != null) ? hitDown.distance : float.MaxValue;
            float lengthZ = (hitBack.collider != null) ? hitBack.distance : float.MaxValue;
            float lengthNegX = (hitLeft.collider != null) ? hitLeft.distance : float.MaxValue;
            float lengthNegY = (hitUp.collider != null) ? hitUp.distance : float.MaxValue;
            float lengthNegZ = (hitForward.collider != null) ? hitForward.distance : float.MaxValue;

            float extents = 0;
            Vector3 hitPoint = Vector3.zero;
            if (VRTK_SharedMethods.IsLowest(lengthX, new float[] { lengthY, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                returnDirection = ButtonDirection.negX;
                hitPoint = hitRight.point;
                extents = bounds.extents.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthY, new float[] { lengthX, lengthZ, lengthNegX, lengthNegY, lengthNegZ }))
            {
                returnDirection = ButtonDirection.y;
                hitPoint = hitDown.point;
                extents = bounds.extents.y;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthZ, new float[] { lengthX, lengthY, lengthNegX, lengthNegY, lengthNegZ }))
            {
                returnDirection = ButtonDirection.z;
                hitPoint = hitBack.point;
                extents = bounds.extents.z;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegX, new float[] { lengthX, lengthY, lengthZ, lengthNegY, lengthNegZ }))
            {
                returnDirection = ButtonDirection.x;
                hitPoint = hitLeft.point;
                extents = bounds.extents.x;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegY, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegZ }))
            {
                returnDirection = ButtonDirection.negY;
                hitPoint = hitUp.point;
                extents = bounds.extents.y;
            }
            else if (VRTK_SharedMethods.IsLowest(lengthNegZ, new float[] { lengthX, lengthY, lengthZ, lengthNegX, lengthNegY }))
            {
                returnDirection = ButtonDirection.negZ;
                hitPoint = hitForward.point;
                extents = bounds.extents.z;
            }

            // determin activation distance
            activationDistance = (Vector3.Distance(hitPoint, bounds.center) - extents) * 0.95f;

            if (returnDirection == ButtonDirection.autodetect || activationDistance < 0.001f)
            {
                // auto-detection was not possible or colliding with object already
                returnDirection = ButtonDirection.autodetect;
                activationDistance = 0;
            }
            else
            {
                activationDir = hitPoint - bounds.center;
            }

            return returnDirection;
        }

        protected virtual Vector3 CalculateActivationDir()
        {
            Bounds bounds = VRTK_SharedMethods.GetBounds(transform, transform);

            Vector3 buttonDirection = Vector3.zero;
            float extents = 0;
            switch (direction)
            {
                case ButtonDirection.x:
                case ButtonDirection.negX:
                    if (Mathf.RoundToInt(Mathf.Abs(transform.right.x)) == 1)
                    {
                        buttonDirection = transform.right;
                        extents = bounds.extents.x;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.up.x)) == 1)
                    {
                        buttonDirection = transform.up;
                        extents = bounds.extents.y;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.x)) == 1)
                    {
                        buttonDirection = transform.forward;
                        extents = bounds.extents.z;
                    }
                    buttonDirection *= (direction == ButtonDirection.x) ? -1 : 1;
                    break;
                case ButtonDirection.y:
                case ButtonDirection.negY:
                    if (Mathf.RoundToInt(Mathf.Abs(transform.right.y)) == 1)
                    {
                        buttonDirection = transform.right;
                        extents = bounds.extents.x;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.up.y)) == 1)
                    {
                        buttonDirection = transform.up;
                        extents = bounds.extents.y;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.y)) == 1)
                    {
                        buttonDirection = transform.forward;
                        extents = bounds.extents.z;
                    }
                    buttonDirection *= (direction == ButtonDirection.y) ? -1 : 1;
                    break;
                case ButtonDirection.z:
                case ButtonDirection.negZ:
                    if (Mathf.RoundToInt(Mathf.Abs(transform.right.z)) == 1)
                    {
                        buttonDirection = transform.right;
                        extents = bounds.extents.x;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.up.z)) == 1)
                    {
                        buttonDirection = transform.up;
                        extents = bounds.extents.y;
                    }
                    else if (Mathf.RoundToInt(Mathf.Abs(transform.forward.z)) == 1)
                    {
                        buttonDirection = transform.forward;
                        extents = bounds.extents.z;
                    }
                    buttonDirection *= (direction == ButtonDirection.z) ? -1 : 1;
                    break;
            }

            // subtract width of button
            return (buttonDirection * (extents + activationDistance));
        }

        protected virtual bool ReachedActivationDistance()
        {
            return (Vector3.Distance(transform.position, restingPosition) >= activationDistance);
        }

        protected virtual Vector3 GetForceVector()
        {
            return (-activationDir.normalized * buttonStrength);
        }
    }
}