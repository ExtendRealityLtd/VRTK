using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void HapticPulseEventHandler(ushort strength);

[ExecuteInEditMode] //Lets us set up buttons from inspector option
public class RadialMenu : MonoBehaviour
{
    #region Variables
    public List<RadialMenuButton> buttons;
    public GameObject buttonPrefab;
    [Range(0f, 1f)]
    public float buttonThickness = 0.5f;
    public Color buttonColor = Color.white;
    public float offsetDistance = 1;
    [Range(0, 359)]
    public float offsetRotation;
    public bool rotateIcons;
    public float iconMargin;
    public bool isShown;
    public bool hideOnRelease;
    public bool executeOnUnclick;
    [Range(0, 1599)]
    public ushort baseHapticStrength;

    public event HapticPulseEventHandler FireHapticPulse;

    //Has to be public to keep state from editor -> play mode?
    public List<GameObject> menuButtons;

    private int currentHover = -1;
    private int currentPress = -1;
    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Application.isPlaying)
        {
            if (!isShown)
            {
                transform.localScale = Vector3.zero;
            }
            RegenerateButtons();
        }
    }

    private void Update()
    {
        //Keep track of pressed button and constantly invoke Hold event
        if (currentPress != -1)
        {
            buttons[currentPress].OnHold.Invoke();
        }
    }

    #endregion

    #region Interaction

    //Turns and Angle and Event type into a button action
    private void InteractButton(float angle, ButtonEvent evt) //Can't pass ExecuteEvents as parameter? Unity gives error
    {
        //Get button ID from angle
        float buttonAngle = 360f / buttons.Count; //Each button is an arc with this angle
        angle = mod((angle + offsetRotation), 360); //Offset the touch coordinate with our offset

        int buttonID = (int)mod(((angle + (buttonAngle / 2f)) / buttonAngle), buttons.Count); //Convert angle into ButtonID (This is the magic)
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
                AttempHapticPulse ((ushort)(baseHapticStrength * 1.666f));
            }
        }
        if (evt == ButtonEvent.click) //Click button if click, and keep track of current press (executes button action)
        {
            ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerDownHandler);
            currentPress = buttonID;
            if (!executeOnUnclick)
            {
                buttons[buttonID].OnClick.Invoke ();
                AttempHapticPulse ((ushort)(baseHapticStrength * 2.5f));
            }
        }
        else if (evt == ButtonEvent.unclick) //Clear press id to stop invoking OnHold method (hide menu)
        {
            ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerUpHandler);
            currentPress = -1;

            if (executeOnUnclick)
            {
                AttempHapticPulse ((ushort)(baseHapticStrength * 2.5f));
                buttons[buttonID].OnClick.Invoke ();
            }
        }
        else if (evt == ButtonEvent.hoverOn && currentHover != buttonID) // Show hover UI event (darken button etc). Show menu
        {
            ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerEnterHandler);
            buttons[buttonID].OnHoverEnter.Invoke();
            AttempHapticPulse (baseHapticStrength);
        }
        currentHover = buttonID; //Set current hover ID, need this to un-hover if selected button changes
    }

    /*
    * Public methods to call Interact
    */

    public void HoverButton(float angle)
    {
        InteractButton(angle, ButtonEvent.hoverOn);
    }

    public void ClickButton(float angle)
    {
        InteractButton(angle, ButtonEvent.click);
    }

    public void UnClickButton(float angle)
    {
        InteractButton(angle, ButtonEvent.unclick);
    }

    public void ToggleMenu()
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

    public void StopTouching()
    {
        if (currentHover != -1)
        {
            var pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(menuButtons[currentHover], pointer, ExecuteEvents.pointerExitHandler);
            buttons[currentHover].OnHoverExit.Invoke();
            currentHover = -1;
        }
    }

    /*
    * Public methods to Show/Hide menu
    */
    public void ShowMenu()
    {
        if (!isShown)
        {
            isShown = true;
            StopCoroutine("TweenMenuScale");
            StartCoroutine("TweenMenuScale", isShown);
        }
    }

    public RadialMenuButton GetButton(int id)
    {
        if (id < buttons.Count)
        {
            return buttons[id];
        }
        return null;
    }

    public void HideMenu(bool force)
    {
        if (isShown && (hideOnRelease || force))
        {
            isShown = false;
            StopCoroutine("TweenMenuScale");
            StartCoroutine("TweenMenuScale", isShown);
        }
    }

    //Simple tweening for menu, scales linearly from 0 to 1 and 1 to 0
    private IEnumerator TweenMenuScale(bool show)
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

    private void AttempHapticPulse(ushort strength)
    {
        if (strength > 0 && FireHapticPulse != null)
        {
            FireHapticPulse (strength);
        }
    }

    #endregion

    #region Generation

    //Creates all the button Arcs and populates them with desired icons
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

    public void AddButton(RadialMenuButton newButton)
    {
        buttons.Add(newButton);
        RegenerateButtons();
    }

    private void RemoveAllButtons()
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

    #endregion

    #region Utility

    private float mod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    #endregion
}

[System.Serializable]
public class RadialMenuButton
{
    public Sprite ButtonIcon;

    public UnityEvent OnClick;
    public UnityEvent OnHold;
    public UnityEvent OnHoverEnter;
    public UnityEvent OnHoverExit;
}

public enum ButtonEvent
{
    hoverOn,
    hoverOff,
    click,
    unclick
}