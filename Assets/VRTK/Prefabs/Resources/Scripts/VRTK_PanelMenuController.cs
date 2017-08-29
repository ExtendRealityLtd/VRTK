// Panel Menu Controller|Prefabs|0070
namespace VRTK
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Purpose: top-level controller class to handle the display of up to four child PanelMenuItemController items which are displayed as a canvas UI panel.
    /// </summary>
    /// <remarks>
    /// This script should be attached to a VRTK_InteractableObject > first child GameObject [PanelMenuController].
    /// The [PanelMenuController] must have a child GameObject [panel items container].
    /// The [panel items container] must have a Canvas component.
    /// A [panel items container] can have up to four child GameObject, each of these contains the UI for a panel that can be displayed by [PanelMenuController].
    /// They also have the [PanelMenuItemController] script attached to them. The [PanelMenuItemController] script intercepts the controller events sent from this [PanelMenuController] and passes them onto additional custom event subscriber scripts, which then carry out the required custom UI actions.
    /// To show / hide a UI panel, you must first pick up the VRTK_InteractableObject and then by pressing the touchpad top/bottom/left/right you can open/close the child UI panel that has been assigned via the Unity Editor panel. Button type UI actions are handled by a trigger press when the panel is open.
    /// </remarks>
    /// <example>
    /// `040_Controls_Panel_Menu` contains three basic interactive object examples of the PanelMenu in use.
    /// </example>
    public class VRTK_PanelMenuController : MonoBehaviour
    {
        public enum TouchpadPressPosition
        {
            None,
            Top,
            Bottom,
            Left,
            Right
        }

        [Tooltip("The GameObject the panel should rotate towards, which is the Camera (eye) by default.")]
        public GameObject rotateTowards;
        [Tooltip("The scale multiplier, which relates to the scale of parent interactable object.")]
        public float zoomScaleMultiplier = 1f;
        [Tooltip("The top PanelMenuItemController, which is triggered by pressing up on the controller touchpad.")]
        public VRTK_PanelMenuItemController topPanelMenuItemController;
        [Tooltip("The bottom PanelMenuItemController, which is triggered by pressing down on the controller touchpad.")]
        public VRTK_PanelMenuItemController bottomPanelMenuItemController;
        [Tooltip("The left PanelMenuItemController, which is triggered by pressing left on the controller touchpad.")]
        public VRTK_PanelMenuItemController leftPanelMenuItemController;
        [Tooltip("The right PanelMenuItemController, which is triggered by pressing right on the controller touchpad.")]
        public VRTK_PanelMenuItemController rightPanelMenuItemController;

        // Relates to scale of canvas on panel items.
        protected const float CanvasScaleSize = 0.001f;

        // Swipe sensitivity / detection.
        protected const float AngleTolerance = 30f;
        protected const float SwipeMinDist = 0.2f;
        protected const float SwipeMinVelocity = 4.0f;

        protected VRTK_ControllerEvents controllerEvents;
        protected VRTK_PanelMenuItemController currentPanelMenuItemController;
        protected GameObject interactableObject;
        protected GameObject canvasObject;
        protected readonly Vector2 xAxis = new Vector2(1, 0);
        protected readonly Vector2 yAxis = new Vector2(0, 1);
        protected Vector2 touchStartPosition;
        protected Vector2 touchEndPosition;
        protected float touchStartTime;
        protected float currentAngle;
        protected bool isTrackingSwipe = false;
        protected bool isPendingSwipeCheck = false;
        protected bool isGrabbed = false;
        protected bool isShown = false;
        protected Coroutine tweenMenuScaleRoutine;

        /// <summary>
        /// The ToggleMenu method is used to show or hide the menu.
        /// </summary>
        public virtual void ToggleMenu()
        {
            if (isShown)
            {
                HideMenu(true);
            }
            else
            {
                ShowMenu();
            }
        }

        /// <summary>
        /// The ShowMenu method is used to show the menu.
        /// </summary>
        public virtual void ShowMenu()
        {
            if (!isShown)
            {
                isShown = true;
                InitTweenMenuScale(isShown);
            }
        }

        /// <summary>
        /// The HideMenu method is used to hide the menu.
        /// </summary>
        /// <param name="force">If true then the menu is always hidden.</param>
        public virtual void HideMenu(bool force)
        {
            if (isShown && force)
            {
                isShown = false;
                InitTweenMenuScale(isShown);
            }
        }

        /// <summary>
        /// The HideMenuImmediate method is used to immediately hide the menu.
        /// </summary>
        public virtual void HideMenuImmediate()
        {
            if (currentPanelMenuItemController != null && isShown)
            {
                HandlePanelMenuItemControllerVisibility(currentPanelMenuItemController);
            }
            transform.localScale = Vector3.zero;
            canvasObject.transform.localScale = Vector3.zero;
            isShown = false;
        }

        protected virtual void Awake()
        {
            Initialize();
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Start()
        {
            interactableObject = gameObject.transform.parent.gameObject;
            if (interactableObject == null || interactableObject.GetComponent<VRTK_InteractableObject>() == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PanelMenuController", "VRTK_InteractableObject", "a parent"));
                return;
            }

            interactableObject.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += new InteractableObjectEventHandler(DoInteractableObjectIsGrabbed);
            interactableObject.GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += new InteractableObjectEventHandler(DoInteractableObjectIsUngrabbed);

            canvasObject = gameObject.transform.GetChild(0).gameObject;
            if (canvasObject == null || canvasObject.GetComponent<Canvas>() == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PanelMenuController", "Canvas", "a child"));
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            if (interactableObject != null)
            {
                if (rotateTowards == null)
                {
                    rotateTowards = VRTK_DeviceFinder.HeadsetTransform().gameObject;
                    if (rotateTowards == null)
                    {
                        VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.COULD_NOT_FIND_OBJECT_FOR_ACTION, "PanelMenuController", "an object", "rotate towards"));
                    }
                }

                if (isShown)
                {
                    if (rotateTowards != null)
                    {
                        transform.rotation = Quaternion.LookRotation((rotateTowards.transform.position - transform.position) * -1, Vector3.up);
                    }
                }

                if (isPendingSwipeCheck)
                {
                    CalculateSwipeAction();
                }
            }
        }

        protected virtual void Initialize()
        {
            if (Application.isPlaying)
            {
                if (!isShown)
                {
                    transform.localScale = Vector3.zero;
                }
            }

            if (controllerEvents == null)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();
            }
        }

        protected virtual void BindControllerEvents()
        {
            controllerEvents.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPress);
            controllerEvents.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouched);
            controllerEvents.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadUntouched);
            controllerEvents.TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
            controllerEvents.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        }

        protected virtual void UnbindControllerEvents()
        {
            controllerEvents.TouchpadPressed -= new ControllerInteractionEventHandler(DoTouchpadPress);
            controllerEvents.TouchpadTouchStart -= new ControllerInteractionEventHandler(DoTouchpadTouched);
            controllerEvents.TouchpadTouchEnd -= new ControllerInteractionEventHandler(DoTouchpadUntouched);
            controllerEvents.TouchpadAxisChanged -= new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
            controllerEvents.TriggerPressed -= new ControllerInteractionEventHandler(DoTriggerPressed);
        }

        protected virtual void HandlePanelMenuItemControllerVisibility(VRTK_PanelMenuItemController targetPanelItemController)
        {
            if (isShown)
            {
                if (currentPanelMenuItemController == targetPanelItemController)
                {
                    targetPanelItemController.Hide(interactableObject);
                    currentPanelMenuItemController = null;
                    HideMenu(true);
                }
                else
                {
                    currentPanelMenuItemController.Hide(interactableObject);
                    currentPanelMenuItemController = targetPanelItemController;
                }
            }
            else
            {
                currentPanelMenuItemController = targetPanelItemController;
            }

            if (currentPanelMenuItemController != null)
            {
                currentPanelMenuItemController.Show(interactableObject);
                ShowMenu();
            }
        }

        protected virtual void InitTweenMenuScale(bool show)
        {
            if (tweenMenuScaleRoutine != null)
            {
                StopCoroutine(tweenMenuScaleRoutine);
            }
            if (enabled)
            {
                tweenMenuScaleRoutine = StartCoroutine(TweenMenuScale(show));
            }
        }

        protected virtual IEnumerator TweenMenuScale(bool show)
        {
            float targetScale = 0;
            Vector3 direction = -1 * Vector3.one;
            if (show)
            {
                canvasObject.transform.localScale = new Vector3(CanvasScaleSize, CanvasScaleSize, CanvasScaleSize);
                targetScale = zoomScaleMultiplier;
                direction = Vector3.one;
            }
            int i = 0;
            while (i < 250 && ((show && transform.localScale.x < targetScale) || (!show && transform.localScale.x > targetScale)))
            {
                transform.localScale += direction * Time.deltaTime * 4f * zoomScaleMultiplier;
                yield return true;
                i++;
            }
            transform.localScale = direction * targetScale;

            if (!show)
            {
                canvasObject.transform.localScale = Vector3.zero;
            }
        }

        protected virtual void DoInteractableObjectIsGrabbed(object sender, InteractableObjectEventArgs e)
        {
            controllerEvents = e.interactingObject.GetComponentInParent<VRTK_ControllerEvents>();
            if (controllerEvents != null)
            {
                BindControllerEvents();
            }
            isGrabbed = true;
        }

        protected virtual void DoInteractableObjectIsUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            isGrabbed = false;
            if (isShown)
            {
                HideMenuImmediate();
            }

            if (controllerEvents != null)
            {
                UnbindControllerEvents();
                controllerEvents = null;
            }
        }

        protected virtual void DoTouchpadPress(object sender, ControllerInteractionEventArgs e)
        {
            if (isGrabbed)
            {
                TouchpadPressPosition pressPosition = CalculateTouchpadPressPosition();
                switch (pressPosition)
                {
                    case TouchpadPressPosition.Top:
                        if (topPanelMenuItemController != null)
                        {
                            HandlePanelMenuItemControllerVisibility(topPanelMenuItemController);
                        }
                        break;

                    case TouchpadPressPosition.Bottom:
                        if (bottomPanelMenuItemController != null)
                        {
                            HandlePanelMenuItemControllerVisibility(bottomPanelMenuItemController);
                        }
                        break;

                    case TouchpadPressPosition.Left:
                        if (leftPanelMenuItemController != null)
                        {
                            HandlePanelMenuItemControllerVisibility(leftPanelMenuItemController);
                        }
                        break;

                    case TouchpadPressPosition.Right:
                        if (rightPanelMenuItemController != null)
                        {
                            HandlePanelMenuItemControllerVisibility(rightPanelMenuItemController);
                        }
                        break;
                }
            }
        }

        protected virtual void DoTouchpadTouched(object sender, ControllerInteractionEventArgs e)
        {
            touchStartPosition = new Vector2(e.touchpadAxis.x, e.touchpadAxis.y);
            touchStartTime = Time.time;
            isTrackingSwipe = true;
        }

        protected virtual void DoTouchpadUntouched(object sender, ControllerInteractionEventArgs e)
        {
            isTrackingSwipe = false;
            isPendingSwipeCheck = true;
        }

        protected virtual void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            ChangeAngle(CalculateAngle(e));

            if (isTrackingSwipe)
            {
                touchEndPosition = new Vector2(e.touchpadAxis.x, e.touchpadAxis.y);
            }
        }

        protected virtual void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (isGrabbed)
            {
                OnTriggerPressed();
            }
        }

        protected virtual void ChangeAngle(float angle, object sender = null)
        {
            currentAngle = angle;
        }

        protected virtual void CalculateSwipeAction()
        {
            isPendingSwipeCheck = false;

            float deltaTime = Time.time - touchStartTime;
            Vector2 swipeVector = touchEndPosition - touchStartPosition;
            float velocity = swipeVector.magnitude / deltaTime;

            if ((velocity > SwipeMinVelocity) && (swipeVector.magnitude > SwipeMinDist))
            {
                swipeVector.Normalize();
                float angleOfSwipe = Vector2.Dot(swipeVector, xAxis);
                angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;

                // Left / right
                if (angleOfSwipe < AngleTolerance)
                {
                    OnSwipeRight();
                }
                else if ((180.0f - angleOfSwipe) < AngleTolerance)
                {
                    OnSwipeLeft();
                }
                else
                {
                    // Top / bottom
                    angleOfSwipe = Vector2.Dot(swipeVector, yAxis);
                    angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                    if (angleOfSwipe < AngleTolerance)
                    {
                        OnSwipeTop();
                    }
                    else if ((180.0f - angleOfSwipe) < AngleTolerance)
                    {
                        OnSwipeBottom();
                    }
                }
            }
        }

        protected virtual TouchpadPressPosition CalculateTouchpadPressPosition()
        {
            if (CheckAnglePosition(currentAngle, AngleTolerance, 0))
            {
                return TouchpadPressPosition.Top;
            }
            else if (CheckAnglePosition(currentAngle, AngleTolerance, 180))
            {
                return TouchpadPressPosition.Bottom;
            }
            else if (CheckAnglePosition(currentAngle, AngleTolerance, 270))
            {
                return TouchpadPressPosition.Left;
            }
            else if (CheckAnglePosition(currentAngle, AngleTolerance, 90))
            {
                return TouchpadPressPosition.Right;
            }

            return TouchpadPressPosition.None;
        }

        protected virtual void OnSwipeLeft()
        {
            if (currentPanelMenuItemController != null)
            {
                currentPanelMenuItemController.SwipeLeft(interactableObject);
            }
        }

        protected virtual void OnSwipeRight()
        {
            if (currentPanelMenuItemController != null)
            {
                currentPanelMenuItemController.SwipeRight(interactableObject);
            }
        }

        protected virtual void OnSwipeTop()
        {
            if (currentPanelMenuItemController != null)
            {
                currentPanelMenuItemController.SwipeTop(interactableObject);
            }
        }

        protected virtual void OnSwipeBottom()
        {
            if (currentPanelMenuItemController != null)
            {
                currentPanelMenuItemController.SwipeBottom(interactableObject);
            }
        }

        protected virtual void OnTriggerPressed()
        {
            if (currentPanelMenuItemController != null)
            {
                currentPanelMenuItemController.TriggerPressed(interactableObject);
            }
        }

        protected virtual float CalculateAngle(ControllerInteractionEventArgs e)
        {
            return e.touchpadAngle;
        }

        protected virtual float NormAngle(float currentDegree, float maxAngle = 360)
        {
            if (currentDegree < 0) currentDegree = currentDegree + maxAngle;
            return currentDegree % maxAngle;
        }

        protected virtual bool CheckAnglePosition(float currentDegree, float tolerance, float targetDegree)
        {
            float lowerBound = NormAngle(currentDegree - tolerance);
            float upperBound = NormAngle(currentDegree + tolerance);

            if (lowerBound > upperBound)
            {
                return targetDegree >= lowerBound || targetDegree <= upperBound;
            }
            return targetDegree >= lowerBound && targetDegree <= upperBound;
        }
    }
}
