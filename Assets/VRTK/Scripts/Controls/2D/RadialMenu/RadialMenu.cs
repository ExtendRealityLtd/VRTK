// Radial Menu|Prefabs|0040
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public delegate void HapticPulseEventHandler(float strength);

    /// <summary>
    /// This adds a UI element into the world space that can be dropped into a Controller object and used to create and use Radial Menus from the touchpad.
    /// </summary>
    /// <remarks>
    /// If the RadialMenu is placed inside a controller, it will automatically find a `VRTK_ControllerEvents` in its parent to use at the input. However, a `VRTK_ControllerEvents` can be defined explicitly by setting the `Events` parameter of the `Radial Menu Controller` script also attached to the prefab.
    ///
    /// The RadialMenu can also be placed inside a `VRTK_InteractableObject` for the RadialMenu to be anchored to a world object instead of the controller. The `Events Manager` parameter will automatically be set if the RadialMenu is a child of an InteractableObject, but it can also be set manually in the inspector. Additionally, for the RadialMenu to be anchored in the world, the `RadialMenuController` script in the prefab must be replaced with `VRTK_IndependentRadialMenuController`. See the script information for further details on making the RadialMenu independent of the controllers.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/030_Controls_RadialTouchpadMenu` displays a radial menu for each controller. The left controller uses the `Hide On Release` variable, so it will only be visible if the left touchpad is being touched. It also uses the `Execute On Unclick` variable to delay execution until the touchpad button is unclicked. The example scene also contains a demonstration of anchoring the RadialMenu to an interactable cube instead of a controller.
    /// </example>
    [ExecuteInEditMode]
    public class RadialMenu : MonoBehaviour
    {
        [System.Serializable]
        public class RadialMenuButton
        {
            public Sprite ButtonIcon;
            public UnityEvent OnClick = new UnityEvent();
            public UnityEvent OnHold = new UnityEvent();
            public UnityEvent OnHoverEnter = new UnityEvent();
            public UnityEvent OnHoverExit = new UnityEvent();
        }

        public enum ButtonEvent
        {
            hoverOn,
            hoverOff,
            click,
            unclick
        }

        [Tooltip("An array of Buttons that define the interactive buttons required to be displayed as part of the radial menu.")]
        public List<RadialMenuButton> buttons;
        [Tooltip("The base for each button in the menu, by default set to a dynamic circle arc that will fill up a portion of the menu.")]
        public GameObject buttonPrefab;
        [Tooltip("If checked, then the buttons will be auto generated on awake.")]
        public bool generateOnAwake = true;
        [Tooltip("Percentage of the menu the buttons should fill, 1.0 is a pie slice, 0.1 is a thin ring.")]
        [Range(0f, 1f)]
        public float buttonThickness = 0.5f;
        [Tooltip("The background colour of the buttons, default is white.")]
        public Color buttonColor = Color.white;
        [Tooltip("The distance the buttons should move away from the centre. This creates space between the individual buttons.")]
        public float offsetDistance = 1;
        [Tooltip("The additional rotation of the Radial Menu.")]
        [Range(0, 359)]
        public float offsetRotation;
        [Tooltip("Whether button icons should rotate according to their arc or be vertical compared to the controller.")]
        public bool rotateIcons;
        [Tooltip("The margin in pixels that the icon should keep within the button.")]
        public float iconMargin;
        [Tooltip("Whether the buttons are shown")]
        public bool isShown;
        [Tooltip("Whether the buttons should be visible when not in use.")]
        public bool hideOnRelease;
        [Tooltip("Whether the button action should happen when the button is released, as opposed to happening immediately when the button is pressed.")]
        public bool executeOnUnclick;
        [Tooltip("The base strength of the haptic pulses when the selected button is changed, or a button is pressed. Set to zero to disable.")]
        [Range(0, 1)]
        public float baseHapticStrength;

        public event HapticPulseEventHandler FireHapticPulse;

        //Has to be public to keep state from editor -> play mode?
        [Tooltip("The actual GameObjects that make up the radial menu.")]
        public List<GameObject> menuButtons;

        protected int currentHover = -1;
        protected int currentPress = -1;

        /// <summary>
        /// The HoverButton method is used to set the button hover at a given angle.
        /// </summary>
        /// <param name="angle">The angle on the radial menu.</param>
        public virtual void HoverButton(float angle)
        {
            InteractButton(angle, ButtonEvent.hoverOn);
        }

        /// <summary>
        /// The ClickButton method is used to set the button click at a given angle.
        /// </summary>
        /// <param name="angle">The angle on the radial menu.</param>
        public virtual void ClickButton(float angle)
        {
            InteractButton(angle, ButtonEvent.click);
        }

        /// <summary>
        /// The UnClickButton method is used to set the button unclick at a given angle.
        /// </summary>
        /// <param name="angle">The angle on the radial menu.</param>
        public virtual void UnClickButton(float angle)
        {
            InteractButton(angle, ButtonEvent.unclick);
        }

        /// <summary>
        /// The ToggleMenu method is used to show or hide the radial menu.
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
        /// The StopTouching method is used to stop touching the menu.
        /// </summary>
        public virtual void StopTouching()
        {
            if (currentHover != -1)
            {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(menuButtons[currentHover], pointer, ExecuteEvents.pointerExitHandler);
                buttons[currentHover].OnHoverExit.Invoke();
                currentHover = -1;
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
                StopCoroutine("TweenMenuScale");
                StartCoroutine("TweenMenuScale", isShown);
            }
        }

        /// <summary>
        /// The GetButton method is used to get a button from the menu.
        /// </summary>
        /// <param name="id">The id of the button to retrieve.</param>
        /// <returns>The found radial menu button.</returns>
        public virtual RadialMenuButton GetButton(int id)
        {
            if (id < buttons.Count)
            {
                return buttons[id];
            }
            return null;
        }

        /// <summary>
        /// The HideMenu method is used to hide the menu.
        /// </summary>
        /// <param name="force">If true then the menu is always hidden.</param>
        public virtual void HideMenu(bool force)
        {
            if (isShown && (hideOnRelease || force))
            {
                isShown = false;
                StopCoroutine("TweenMenuScale");
                StartCoroutine("TweenMenuScale", isShown);
            }
        }

        /// <summary>
        /// The RegenerateButtons method creates all the button arcs and populates them with desired icons.
        /// </summary>
        public void RegenerateButtons()
        {
            RemoveAllButtons();
            for (int i = 0; i < buttons.Count; i++)
            {
                // Initial placement/instantiation
                GameObject newButton = Instantiate(buttonPrefab);
                newButton.transform.SetParent(transform);
                newButton.transform.localScale = Vector3.one;
                newButton.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                newButton.GetComponent<RectTransform>().offsetMin = Vector2.zero;

                //Setup button arc
                UICircle circle = newButton.GetComponent<UICircle>();
                if (buttonThickness == 1)
                {
                    circle.fill = true;
                }
                else
                {
                    circle.thickness = (int)(buttonThickness * (GetComponent<RectTransform>().rect.width / 2f));
                }
                int fillPerc = (int)(100f / buttons.Count);
                circle.fillPercent = fillPerc;
                circle.color = buttonColor;

                //Final placement/rotation
                float angle = ((360 / buttons.Count) * i) + offsetRotation;
                newButton.transform.localEulerAngles = new Vector3(0, 0, angle);
                newButton.layer = 4; //UI Layer
                newButton.transform.localPosition = Vector3.zero;
                if (circle.fillPercent < 55)
                {
                    float angleRad = (angle * Mathf.PI) / 180f;
                    Vector2 angleVector = new Vector2(-Mathf.Cos(angleRad), -Mathf.Sin(angleRad));
                    newButton.transform.localPosition += (Vector3)angleVector * offsetDistance;
                }

                //Place and populate Button Icon
                GameObject buttonIcon = newButton.GetComponentInChildren<RadialButtonIcon>().gameObject;
                if (buttons[i].ButtonIcon == null)
                {
                    buttonIcon.SetActive(false);
                }
                else
                {
                    buttonIcon.GetComponent<Image>().sprite = buttons[i].ButtonIcon;
                    buttonIcon.transform.localPosition = new Vector2(-1 * ((newButton.GetComponent<RectTransform>().rect.width / 2f) - (circle.thickness / 2f)), 0);
                    //Min icon size from thickness and arc
                    float scale1 = Mathf.Abs(circle.thickness);
                    float R = Mathf.Abs(buttonIcon.transform.localPosition.x);
                    float bAngle = (359f * circle.fillPercent * 0.01f * Mathf.PI) / 180f;
                    float scale2 = (R * 2 * Mathf.Sin(bAngle / 2f));
                    if (circle.fillPercent > 24) //Scale calc doesn't work for > 90 degrees
                    {
                        scale2 = float.MaxValue;
                    }

                    float iconScale = Mathf.Min(scale1, scale2) - iconMargin;
                    buttonIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(iconScale, iconScale);
                    //Rotate icons all vertically if desired
                    if (!rotateIcons)
                    {
                        buttonIcon.transform.eulerAngles = GetComponentInParent<Canvas>().transform.eulerAngles;
                    }
                }
                menuButtons.Add(newButton);

            }
        }

        /// <summary>
        /// The AddButton method is used to add a new button to the menu.
        /// </summary>
        /// <param name="newButton">The button to add.</param>
        public void AddButton(RadialMenuButton newButton)
        {
            buttons.Add(newButton);
            RegenerateButtons();
        }

        protected virtual void Awake()
        {
            if (Application.isPlaying)
            {
                if (!isShown)
                {
                    transform.localScale = Vector3.zero;
                }
                if (generateOnAwake)
                {
                    RegenerateButtons();
                }
            }
        }

        protected virtual void Update()
        {
            //Keep track of pressed button and constantly invoke Hold event
            if (currentPress != -1)
            {
                buttons[currentPress].OnHold.Invoke();
            }
        }

        //Turns and Angle and Event type into a button action
        protected virtual void InteractButton(float angle, ButtonEvent evt) //Can't pass ExecuteEvents as parameter? Unity gives error
        {
            //Get button ID from angle
            float buttonAngle = 360f / buttons.Count; //Each button is an arc with this angle
            angle = VRTK_SharedMethods.Mod((angle + -offsetRotation), 360); //Offset the touch coordinate with our offset

            int buttonID = (int)VRTK_SharedMethods.Mod(((angle + (buttonAngle / 2f)) / buttonAngle), buttons.Count); //Convert angle into ButtonID (This is the magic)
            var pointer = new PointerEventData(EventSystem.current); //Create a new EventSystem (UI) Event

            //If we changed buttons while moving, un-hover and un-click the last button we were on
            if (currentHover != buttonID && currentHover != -1)
            {
                ExecuteEvents.Execute(menuButtons[currentHover], pointer, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(menuButtons[currentHover], pointer, ExecuteEvents.pointerExitHandler);
                buttons[currentHover].OnHoverExit.Invoke();
                if (executeOnUnclick && currentPress != -1)
                {
                    ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerDownHandler);
                    AttempHapticPulse(baseHapticStrength * 1.666f);
                }
            }
            if (evt == ButtonEvent.click) //Click button if click, and keep track of current press (executes button action)
            {
                ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerDownHandler);
                currentPress = buttonID;
                if (!executeOnUnclick)
                {
                    buttons[buttonID].OnClick.Invoke();
                    AttempHapticPulse(baseHapticStrength * 2.5f);
                }
            }
            else if (evt == ButtonEvent.unclick) //Clear press id to stop invoking OnHold method (hide menu)
            {
                ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerUpHandler);
                currentPress = -1;

                if (executeOnUnclick)
                {
                    AttempHapticPulse(baseHapticStrength * 2.5f);
                    buttons[buttonID].OnClick.Invoke();
                }
            }
            else if (evt == ButtonEvent.hoverOn && currentHover != buttonID) // Show hover UI event (darken button etc). Show menu
            {
                ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerEnterHandler);
                buttons[buttonID].OnHoverEnter.Invoke();
                AttempHapticPulse(baseHapticStrength);
            }
            currentHover = buttonID; //Set current hover ID, need this to un-hover if selected button changes
        }

        //Simple tweening for menu, scales linearly from 0 to 1 and 1 to 0
        protected virtual IEnumerator TweenMenuScale(bool show)
        {
            float targetScale = 0;
            Vector3 Dir = -1 * Vector3.one;
            if (show)
            {
                targetScale = 1;
                Dir = Vector3.one;
            }
            int i = 0; //Sanity check for infinite loops
            while (i < 250 && ((show && transform.localScale.x < targetScale) || (!show && transform.localScale.x > targetScale)))
            {
                transform.localScale += Dir * Time.deltaTime * 4f; //Tweening function - currently 0.25 second linear
                yield return true;
                i++;
            }
            transform.localScale = Dir * targetScale;
            StopCoroutine("TweenMenuScale");
        }

        protected virtual void AttempHapticPulse(float strength)
        {
            if (strength > 0 && FireHapticPulse != null)
            {
                FireHapticPulse(strength);
            }
        }

        protected virtual void RemoveAllButtons()
        {
            if (menuButtons == null)
            {
                menuButtons = new List<GameObject>();
            }
            for (int i = 0; i < menuButtons.Count; i++)
            {
                DestroyImmediate(menuButtons[i]);
            }
            menuButtons = new List<GameObject>();
        }
    }
}