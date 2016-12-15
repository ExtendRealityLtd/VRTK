// Interact Controller Appearance|Interactions|30080
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interact Controller Appearance script is used to determine whether the controller model should be visible or hidden on touch, grab or use.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/008_Controller_UsingAGrabbedObject` shows that the controller can be hidden when touching, grabbing and using an object.
    /// </example>
    public class VRTK_InteractControllerAppearance : MonoBehaviour
    {
        [Header("Touch Visibility")]
        [Tooltip("Hides the controller model when a valid touch occurs.")]
        public bool hideControllerOnTouch = false;
        [Tooltip("The amount of seconds to wait before hiding the controller on touch.")]
        public float hideDelayOnTouch = 0f;

        [Header("Grab Visibility")]
        [Tooltip("Hides the controller model when a valid grab occurs.")]
        public bool hideControllerOnGrab = false;
        [Tooltip("The amount of seconds to wait before hiding the controller on grab.")]
        public float hideDelayOnGrab = 0f;

        [Header("Use Visibility")]
        [Tooltip("Hides the controller model when a valid use occurs.")]
        public bool hideControllerOnUse = false;
        [Tooltip("The amount of seconds to wait before hiding the controller on use.")]
        public float hideDelayOnUse = 0f;

        private VRTK_ControllerActions storedControllerActions;
        private GameObject storedCurrentObject;
        private bool touchControllerShow = true;
        private bool grabControllerShow = true;

        /// <summary>
        /// The ToggleControllerOnTouch method determines whether the controller should be shown or hidden when touching an interactable object.
        /// </summary>
        /// <param name="showController">If true then the controller will attempt to be made visible when no longer touching, if false then the controller will be hidden on touch.</param>
        /// <param name="controllerActions">The controller to apply the visibility state to.</param>
        /// <param name="obj">The object that is currently being interacted with by the controller which is passed through to the visibility to prevent the object from being hidden as well.</param>
        public void ToggleControllerOnTouch(bool showController, VRTK_ControllerActions controllerActions, GameObject obj)
        {
            if (hideControllerOnTouch)
            {
                touchControllerShow = showController;
                ToggleController(showController, controllerActions, obj.gameObject, hideDelayOnTouch);
            }
        }

        /// <summary>
        /// The ToggleControllerOnGrab method determines whether the controller should be shown or hidden when grabbing an interactable object.
        /// </summary>
        /// <param name="showController">If true then the controller will attempt to be made visible when no longer grabbing, if false then the controller will be hidden on grab.</param>
        /// <param name="controllerActions">The controller to apply the visibility state to.</param>
        /// <param name="obj">The object that is currently being interacted with by the controller which is passed through to the visibility to prevent the object from being hidden as well.</param>
        public void ToggleControllerOnGrab(bool showController, VRTK_ControllerActions controllerActions, GameObject obj)
        {
            if (hideControllerOnGrab)
            {
                var objScript = (obj ? obj.GetComponentInParent<VRTK_InteractableObject>() : null);

                //if attempting to show the controller but it's touched and the touch should hide the controller
                if (showController && !touchControllerShow && objScript && objScript.IsTouched())
                {
                    return;
                }
                grabControllerShow = showController;
                ToggleController(showController, controllerActions, obj.gameObject, hideDelayOnGrab);
            }
        }

        /// <summary>
        /// The ToggleControllerOnUse method determines whether the controller should be shown or hidden when using an interactable object.
        /// </summary>
        /// <param name="showController">If true then the controller will attempt to be made visible when no longer using, if false then the controller will be hidden on use.</param>
        /// <param name="controllerActions">The controller to apply the visibility state to.</param>
        /// <param name="obj">The object that is currently being interacted with by the controller which is passed through to the visibility to prevent the object from being hidden as well.</param>
        public void ToggleControllerOnUse(bool showController, VRTK_ControllerActions controllerActions, GameObject obj)
        {
            if (hideControllerOnUse)
            {
                var objScript = (obj ? obj.GetComponentInParent<VRTK_InteractableObject>() : null);

                //if attempting to show the controller but it's grabbed and the grab should hide the controller
                if (showController && ((!grabControllerShow && objScript && objScript.IsGrabbed()) || (!touchControllerShow && objScript && objScript.IsTouched())))
                {
                    return;
                }
                ToggleController(showController, controllerActions, obj.gameObject, hideDelayOnUse);
            }
        }

        private void ToggleController(bool showController, VRTK_ControllerActions controllerActions, GameObject obj, float touchDelay)
        {
            if (showController)
            {
                ShowController(controllerActions, obj);
            }
            else
            {
                storedControllerActions = controllerActions;
                storedCurrentObject = obj;
                Invoke("HideController", touchDelay);
            }
        }

        private void ShowController(VRTK_ControllerActions controllerActions, GameObject obj)
        {
            CancelInvoke("HideController");
            controllerActions.ToggleControllerModel(true, obj);
        }

        private void HideController()
        {
            if (storedCurrentObject != null)
            {
                storedControllerActions.ToggleControllerModel(false, storedCurrentObject);
            }
        }
    }
}