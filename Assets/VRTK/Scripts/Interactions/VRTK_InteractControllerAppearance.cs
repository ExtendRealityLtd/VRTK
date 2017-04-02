// Interact Controller Appearance|Interactions|30080
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// The Interact Controller Appearance script is attached on the same GameObject as an Interactable Object script and is used to determine whether the controller model should be visible or hidden on touch, grab or use.
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

        protected bool touchControllerShow = true;
        protected bool grabControllerShow = true;
        protected Coroutine hideControllerRoutine;

        /// <summary>
        /// The ToggleControllerOnTouch method determines whether the controller should be shown or hidden when touching an interactable object.
        /// </summary>
        /// <param name="showController">If true then the controller will attempt to be made visible when no longer touching, if false then the controller will be hidden on touch.</param>
        /// <param name="touchingObject">The touching object to apply the visibility state to.</param>
        /// <param name="ignoredObject">The object that is currently being interacted with by the touching object which is passed through to the visibility to prevent the object from being hidden as well.</param>
        public virtual void ToggleControllerOnTouch(bool showController, GameObject touchingObject, GameObject ignoredObject)
        {
            if (hideControllerOnTouch)
            {
                touchControllerShow = showController;
                ToggleController(showController, touchingObject, ignoredObject, hideDelayOnTouch);
            }
        }

        /// <summary>
        /// The ToggleControllerOnGrab method determines whether the controller should be shown or hidden when grabbing an interactable object.
        /// </summary>
        /// <param name="showController">If true then the controller will attempt to be made visible when no longer grabbing, if false then the controller will be hidden on grab.</param>
        /// <param name="grabbingObject">The grabbing object to apply the visibility state to.</param>
        /// <param name="ignoredObject">The object that is currently being interacted with by the grabbing object which is passed through to the visibility to prevent the object from being hidden as well.</param>
        public virtual void ToggleControllerOnGrab(bool showController, GameObject grabbingObject, GameObject ignoredObject)
        {
            if (hideControllerOnGrab)
            {
                var objScript = (ignoredObject != null ? ignoredObject.GetComponentInParent<VRTK_InteractableObject>() : null);

                //if attempting to show the controller but it's touched and the touch should hide the controller
                if (showController && !touchControllerShow && objScript && objScript.IsTouched())
                {
                    return;
                }
                grabControllerShow = showController;
                ToggleController(showController, grabbingObject, ignoredObject, hideDelayOnGrab);
            }
        }

        /// <summary>
        /// The ToggleControllerOnUse method determines whether the controller should be shown or hidden when using an interactable object.
        /// </summary>
        /// <param name="showController">If true then the controller will attempt to be made visible when no longer using, if false then the controller will be hidden on use.</param>
        /// <param name="usingObject">The using object to apply the visibility state to.</param>
        /// <param name="ignoredObject">The object that is currently being interacted with by the using object which is passed through to the visibility to prevent the object from being hidden as well.</param>
        public virtual void ToggleControllerOnUse(bool showController, GameObject usingObject, GameObject ignoredObject)
        {
            if (hideControllerOnUse)
            {
                var objScript = (ignoredObject != null ? ignoredObject.GetComponentInParent<VRTK_InteractableObject>() : null);

                //if attempting to show the controller but it's grabbed and the grab should hide the controller
                if (showController && ((!grabControllerShow && objScript && objScript.IsGrabbed()) || (!touchControllerShow && objScript && objScript.IsTouched())))
                {
                    return;
                }
                ToggleController(showController, usingObject, ignoredObject, hideDelayOnUse);
            }
        }

        protected virtual void OnEnable()
        {
            if (!GetComponent<VRTK_InteractableObject>())
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractControllerAppearance", "VRTK_InteractableObject", "the same"));
            }
        }

        protected virtual void OnDisable()
        {
            if (hideControllerRoutine != null)
            {
                StopCoroutine(hideControllerRoutine);
            }
        }

        protected virtual void ToggleController(bool showController, GameObject interactingObject, GameObject ignoredObject, float delayTime)
        {
            if (showController)
            {
                ShowController(interactingObject, ignoredObject);
            }
            else
            {
                hideControllerRoutine = StartCoroutine(HideController(interactingObject, ignoredObject, delayTime));
            }
        }

        protected virtual void ShowController(GameObject interactingObject, GameObject ignoredObject)
        {
            if (hideControllerRoutine != null)
            {
                StopCoroutine(hideControllerRoutine);
            }
            VRTK_SharedMethods.SetRendererVisible(interactingObject, ignoredObject);
        }

        protected virtual IEnumerator HideController(GameObject interactingObject, GameObject ignoredObject, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            VRTK_SharedMethods.SetRendererHidden(interactingObject, ignoredObject);
        }
    }
}