// Headset Controller Aware|Presence|70040
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="raycastHit">The Raycast Hit struct of item that is obscuring the path to the controller.</param>
    /// <param name="controllerIndex">**OBSOLETE** The index of the controller that is being or has been obscured or being or has been glanced.</param>
    /// <param name="controllerReference">The reference to the controller that is being or has been obscured or being or has been glanced.</param>
    public struct HeadsetControllerAwareEventArgs
    {
        public RaycastHit raycastHit;
        [System.Obsolete("`HeadsetControllerAwareEventArgs.controllerIndex` has been replaced with `HeadsetControllerAwareEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
        public uint controllerIndex;
        public VRTK_ControllerReference controllerReference;
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
    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_HeadsetControllerAware")]
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
        [Tooltip("A custom raycaster to use when raycasting to find controllers.")]
        public VRTK_CustomRaycast customRaycast;

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

        protected GameObject leftController;
        protected GameObject rightController;
        protected Transform headset;

        protected bool leftControllerObscured = false;
        protected bool rightControllerObscured = false;
        protected bool leftControllerLastState = false;
        protected bool rightControllerLastState = false;

        protected bool leftControllerGlance = false;
        protected bool rightControllerGlance = false;
        protected bool leftControllerGlanceLastState = false;
        protected bool rightControllerGlanceLastState = false;

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
        public virtual bool LeftControllerObscured()
        {
            return leftControllerObscured;
        }

        /// <summary>
        /// The RightControllerObscured method returns the state of if the right controller is being obscured from the path of the headset.
        /// </summary>
        /// <returns>Returns true if the path between the headset and the controller is obscured.</returns>
        public virtual bool RightControllerObscured()
        {
            return rightControllerObscured;
        }

        /// <summary>
        /// the LeftControllerGlanced method returns the state of if the headset is currently looking at the left controller or not.
        /// </summary>
        /// <returns>Returns true if the headset can currently see the controller within the given radius threshold.</returns>
        public virtual bool LeftControllerGlanced()
        {
            return leftControllerGlance;
        }

        /// <summary>
        /// the RightControllerGlanced method returns the state of if the headset is currently looking at the right controller or not.
        /// </summary>
        /// <returns>Returns true if the headset can currently see the controller within the given radius threshold.</returns>
        public virtual bool RightControllerGlanced()
        {
            return rightControllerGlance;
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            VRTK_ObjectCache.registeredHeadsetControllerAwareness = this;
            headset = VRTK_DeviceFinder.HeadsetTransform();
            leftController = VRTK_DeviceFinder.GetControllerLeftHand();
            rightController = VRTK_DeviceFinder.GetControllerRightHand();
        }

        protected virtual void OnDisable()
        {
            VRTK_ObjectCache.registeredHeadsetControllerAwareness = null;
            leftController = null;
            rightController = null;
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
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

        protected virtual HeadsetControllerAwareEventArgs SetHeadsetControllerAwareEvent(RaycastHit raycastHit, VRTK_ControllerReference controllerReference)
        {
            HeadsetControllerAwareEventArgs e;
            e.raycastHit = raycastHit;
#pragma warning disable 0618
            e.controllerIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
#pragma warning restore 0618
            e.controllerReference = controllerReference;
            return e;
        }

        protected virtual void RayCastToController(GameObject controller, Transform customDestination, ref bool obscured, ref bool lastState)
        {
            obscured = false;
            if (controller && controller.gameObject.activeInHierarchy)
            {
                var destination = (customDestination ? customDestination.position : controller.transform.position);
                RaycastHit hitInfo;
                if (VRTK_CustomRaycast.Linecast(customRaycast, headset.position, destination, out hitInfo, new LayerMask(), QueryTriggerInteraction.Ignore))
                {
                    obscured = true;
                }

                if (lastState != obscured)
                {
                    ObscuredStateChanged(controller.gameObject, obscured, hitInfo);
                }

                lastState = obscured;
            }
        }

        protected virtual void ObscuredStateChanged(GameObject controller, bool obscured, RaycastHit hitInfo)
        {
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(controller);
            if (obscured)
            {
                OnControllerObscured(SetHeadsetControllerAwareEvent(hitInfo, controllerReference));
            }
            else
            {
                OnControllerUnobscured(SetHeadsetControllerAwareEvent(hitInfo, controllerReference));
            }
        }

        protected virtual void CheckHeadsetView(GameObject controller, Transform customDestination, ref bool controllerGlance, ref bool controllerGlanceLastState)
        {
            controllerGlance = false;
            if (controller && controller.gameObject.activeInHierarchy)
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
                    GlanceStateChanged(controller.gameObject, controllerGlance);
                }

                controllerGlanceLastState = controllerGlance;
            }
        }

        protected virtual void GlanceStateChanged(GameObject controller, bool glance)
        {
            RaycastHit emptyHit = new RaycastHit();
            VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(controller);
            if (glance)
            {
                OnControllerGlanceEnter(SetHeadsetControllerAwareEvent(emptyHit, controllerReference));
            }
            else
            {
                OnControllerGlanceExit(SetHeadsetControllerAwareEvent(emptyHit, controllerReference));
            }
        }
    }
}