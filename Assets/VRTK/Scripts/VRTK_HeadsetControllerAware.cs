// Headset Controller Aware|Scripts|0112
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="raycastHit">The Raycast Hit struct of item that is obscuring the path to the controller.</param>
    /// <param name="controllerIndex">The index of the controller that is being or has been obscured or being or has been glanced.</param>
    public struct HeadsetControllerAwareEventArgs
    {
        public RaycastHit raycastHit;
        public uint controllerIndex;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="HeadsetControllerAwareEventArgs"/></param>
    public delegate void HeadsetControllerAwareEventHandler(object sender, HeadsetControllerAwareEventArgs e);

    /// <summary>
    /// The purpose of Headset Controller Aware is to allow the headset to know if something is blocking the path between the headset and controllers and to know if the headset is looking at a controller.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/029_Controller_Tooltips` displays tooltips that have been added to the controllers and are only visible when the controller is being looked at.
    /// </example>
    public class VRTK_HeadsetControllerAware : MonoBehaviour
    {
        [Tooltip("If this is checked then the left controller will be checked if items obscure it's path from the headset.")]
        public bool trackLeftController = true;
        [Tooltip("If this is checked then the right controller will be checked if items obscure it's path from the headset.")]
        public bool trackRightController = true;
        [Tooltip("The radius of the accepted distance from the controller origin point to determine if the controller is being looked at.")]
        public float controllerGlanceRadius = 0.15f;
        [Tooltip("A custom transform to provide the world space position of the right controller.")]
        public Transform customRightControllerOrigin;
        [Tooltip("A custom transform to provide the world space position of the left controller.")]
        public Transform customLeftControllerOrigin;

        /// <summary>
        /// Emitted when the controller is obscured by another object.
        /// </summary>
        public event HeadsetControllerAwareEventHandler ControllerObscured;
        /// <summary>
        /// Emitted when the controller is no longer obscured by an object.
        /// </summary>
        public event HeadsetControllerAwareEventHandler ControllerUnobscured;

        /// <summary>
        /// Emitted when the controller is seen by the headset view.
        /// </summary>
        public event HeadsetControllerAwareEventHandler ControllerGlanceEnter;
        /// <summary>
        /// Emitted when the controller is no longer seen by the headset view.
        /// </summary>
        public event HeadsetControllerAwareEventHandler ControllerGlanceExit;

        private GameObject leftController;
        private GameObject rightController;
        private Transform headset;

        private bool leftControllerObscured = false;
        private bool rightControllerObscured = false;
        private bool leftControllerLastState = false;
        private bool rightControllerLastState = false;

        private bool leftControllerGlance = false;
        private bool rightControllerGlance = false;
        private bool leftControllerGlanceLastState = false;
        private bool rightControllerGlanceLastState = false;

        public virtual void OnControllerObscured(HeadsetControllerAwareEventArgs e)
        {
            if (ControllerObscured != null)
            {
                ControllerObscured(this, e);
            }
        }

        public virtual void OnControllerUnobscured(HeadsetControllerAwareEventArgs e)
        {
            if (ControllerUnobscured != null)
            {
                ControllerUnobscured(this, e);
            }
        }

        public virtual void OnControllerGlanceEnter(HeadsetControllerAwareEventArgs e)
        {
            if (ControllerGlanceEnter != null)
            {
                ControllerGlanceEnter(this, e);
            }
        }

        public virtual void OnControllerGlanceExit(HeadsetControllerAwareEventArgs e)
        {
            if (ControllerGlanceExit != null)
            {
                ControllerGlanceExit(this, e);
            }
        }

        /// <summary>
        /// The LeftControllerObscured method returns the state of if the left controller is being obscured from the path of the headset.
        /// </summary>
        /// <returns>Returns true if the path between the headset and the controller is obscured.</returns>
        public bool LeftControllerObscured()
        {
            return leftControllerObscured;
        }

        /// <summary>
        /// The RightControllerObscured method returns the state of if the right controller is being obscured from the path of the headset.
        /// </summary>
        /// <returns>Returns true if the path between the headset and the controller is obscured.</returns>
        public bool RightControllerObscured()
        {
            return rightControllerObscured;
        }

        /// <summary>
        /// the LeftControllerGlanced method returns the state of if the headset is currently looking at the left controller or not.
        /// </summary>
        /// <returns>Returns true if the headset can currently see the controller within the given radius threshold.</returns>
        public bool LeftControllerGlanced()
        {
            return leftControllerGlance;
        }

        /// <summary>
        /// the RightControllerGlanced method returns the state of if the headset is currently looking at the right controller or not.
        /// </summary>
        /// <returns>Returns true if the headset can currently see the controller within the given radius threshold.</returns>
        public bool RightControllerGlanced()
        {
            return rightControllerGlance;
        }

        private void OnEnable()
        {
            VRTK_ObjectCache.registeredHeadsetControllerAwareness = this;
            headset = VRTK_DeviceFinder.HeadsetTransform();
            leftController = VRTK_DeviceFinder.GetControllerLeftHand();
            rightController = VRTK_DeviceFinder.GetControllerRightHand();
        }

        private void OnDisable()
        {
            VRTK_ObjectCache.registeredHeadsetControllerAwareness = null;
            leftController = null;
            rightController = null;
        }

        private void Update()
        {
            if (trackLeftController)
            {
                RayCastToController(leftController, customLeftControllerOrigin, ref leftControllerObscured, ref leftControllerLastState);
            }
            if (trackRightController)
            {
                RayCastToController(rightController, customRightControllerOrigin, ref rightControllerObscured, ref rightControllerLastState);
            }

            CheckHeadsetView(leftController, customLeftControllerOrigin, ref leftControllerGlance, ref leftControllerGlanceLastState);
            CheckHeadsetView(rightController, customRightControllerOrigin, ref rightControllerGlance, ref rightControllerGlanceLastState);
        }

        private HeadsetControllerAwareEventArgs SetHeadsetControllerAwareEvent(RaycastHit raycastHit, uint controllerIndex)
        {
            HeadsetControllerAwareEventArgs e;
            e.raycastHit = raycastHit;
            e.controllerIndex = controllerIndex;
            return e;
        }

        private void RayCastToController(GameObject controller, Transform customDestination, ref bool obscured, ref bool lastState)
        {
            obscured = false;
            if (controller && controller.activeInHierarchy)
            {
                var destination = (customDestination ? customDestination.position : controller.transform.position);
                RaycastHit hitInfo;
                if (Physics.Linecast(headset.position, destination, out hitInfo))
                {
                    obscured = true;
                }

                if (lastState != obscured)
                {
                    ObscuredStateChanged(controller, obscured, hitInfo);
                }

                lastState = obscured;
            }
        }

        private void ObscuredStateChanged(GameObject controller, bool obscured, RaycastHit hitInfo)
        {
            if (obscured)
            {
                OnControllerObscured(SetHeadsetControllerAwareEvent(hitInfo, VRTK_DeviceFinder.GetControllerIndex(controller)));
            }
            else
            {
                OnControllerUnobscured(SetHeadsetControllerAwareEvent(hitInfo, VRTK_DeviceFinder.GetControllerIndex(controller)));
            }
        }

        private void CheckHeadsetView(GameObject controller, Transform customDestination, ref bool controllerGlance, ref bool controllerGlanceLastState)
        {
            controllerGlance = false;
            if (controller && controller.activeInHierarchy)
            {
                var controllerPosition = (customDestination ? customDestination.position : controller.transform.position);
                var distanceFromHeadsetToController = Vector3.Distance(headset.position, controllerPosition);
                var lookPoint = headset.position + (headset.forward * distanceFromHeadsetToController);

                if (Vector3.Distance(controllerPosition, lookPoint) <= controllerGlanceRadius)
                {
                    controllerGlance = true;
                }

                if (controllerGlanceLastState != controllerGlance)
                {
                    GlanceStateChanged(controller, controllerGlance);
                }

                controllerGlanceLastState = controllerGlance;
            }
        }

        private void GlanceStateChanged(GameObject controller, bool glance)
        {
            var emptyHit = new RaycastHit();
            if (glance)
            {
                OnControllerGlanceEnter(SetHeadsetControllerAwareEvent(emptyHit, VRTK_DeviceFinder.GetControllerIndex(controller)));
            }
            else
            {
                OnControllerGlanceExit(SetHeadsetControllerAwareEvent(emptyHit, VRTK_DeviceFinder.GetControllerIndex(controller)));
            }
        }
    }
}