using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ViveControllerInput : BaseInputModule
{
    public static ViveControllerInput Instance;

    [Header(" [Cursor setup]")]
    public Sprite CursorSprite;
    public Material CursorMaterial;
    public float NormalCursorScale = 0.00025f;

    [Space(10)]

    [Header(" [Runtime variables]")]
    [Tooltip("Indicates whether or not the gui was hit by any controller this frame")]
    public bool GuiHit;

    [Tooltip("Indicates whether or not a button was used this frame")]
    public bool ButtonUsed;

    [Tooltip("Generated cursors")]
    public RectTransform[] Cursors;

    private GameObject[] CurrentPoint;
    private GameObject[] CurrentPressed;
    private GameObject[] CurrentDragging;

    private PointerEventData[] PointEvents;

    private bool Initialized = false;

    [Tooltip("Generated non rendering camera (used for raycasting ui)")]
    public Camera ControllerCamera;

    private SteamVR_ControllerManager ControllerManager;
    private SteamVR_TrackedObject[] Controllers;
    private SteamVR_Controller.Device[] ControllerDevices;

    protected override void Start()
    {
        base.Start();

        if (Initialized == false)
        {
            Instance = this;

            ControllerCamera = new GameObject("Controller UI Camera").AddComponent<Camera>();
            ControllerCamera.clearFlags = CameraClearFlags.Nothing; //CameraClearFlags.Depth;
            ControllerCamera.cullingMask = 0; // 1 << LayerMask.NameToLayer("UI"); 

            ControllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();
            Controllers = new SteamVR_TrackedObject[] { ControllerManager.left.GetComponent<SteamVR_TrackedObject>(), ControllerManager.right.GetComponent<SteamVR_TrackedObject>() };
            ControllerDevices = new SteamVR_Controller.Device[Controllers.Length];
            Cursors = new RectTransform[Controllers.Length];

            for (int index = 0; index < Cursors.Length; index++)
            {
                GameObject cursor = new GameObject("Cursor " + index);
                Canvas canvas = cursor.AddComponent<Canvas>();
                cursor.AddComponent<CanvasRenderer>();
                cursor.AddComponent<CanvasScaler>();
                cursor.AddComponent<UIIgnoreRaycast>();
                cursor.AddComponent<GraphicRaycaster>();

                canvas.renderMode = RenderMode.WorldSpace;
                canvas.sortingOrder = 1000; //set to be on top of everything

                Image image = cursor.AddComponent<Image>();
                image.sprite = CursorSprite;
                image.material = CursorMaterial;


                if (CursorSprite == null)
                    Debug.LogError("Set CursorSprite on " + this.gameObject.name + " to the sprite you want to use as your cursor.", this.gameObject);

                Cursors[index] = cursor.GetComponent<RectTransform>();
            }

            CurrentPoint = new GameObject[Cursors.Length];
            CurrentPressed = new GameObject[Cursors.Length];
            CurrentDragging = new GameObject[Cursors.Length];
            PointEvents = new PointerEventData[Cursors.Length];

            Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                canvas.worldCamera = ControllerCamera;
            }

            Initialized = true;
        }
    }

    // use screen midpoint as locked pointer location, enabling look location to be the "mouse"
    private bool GetLookPointerEventData(int index)
    {
        if (PointEvents[index] == null)
            PointEvents[index] = new PointerEventData(base.eventSystem);
        else
            PointEvents[index].Reset();

        PointEvents[index].delta = Vector2.zero;
        PointEvents[index].position = new Vector2(Screen.width / 2, Screen.height / 2);
        PointEvents[index].scrollDelta = Vector2.zero;

        base.eventSystem.RaycastAll(PointEvents[index], m_RaycastResultCache);
        PointEvents[index].pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        if (PointEvents[index].pointerCurrentRaycast.gameObject != null)
        {
            GuiHit = true; //gets set to false at the beginning of the process event
        }

        m_RaycastResultCache.Clear();

        return true;
    }

    // update the cursor location and whether it is enabled
    // this code is based on Unity's DragMe.cs code provided in the UI drag and drop example
    private void UpdateCursor(int index, PointerEventData pointData)
    {
        if (PointEvents[index].pointerCurrentRaycast.gameObject != null)
        {
            Cursors[index].gameObject.SetActive(true);

            if (pointData.pointerEnter != null)
            {
                RectTransform draggingPlane = pointData.pointerEnter.GetComponent<RectTransform>();
                Vector3 globalLookPos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, pointData.position, pointData.enterEventCamera, out globalLookPos))
                {
                    Cursors[index].position = globalLookPos;
                    Cursors[index].rotation = draggingPlane.rotation;

                    // scale cursor based on distance to camera
                    float lookPointDistance = (Cursors[index].position - Camera.main.transform.position).magnitude;
                    float cursorScale = lookPointDistance * NormalCursorScale;
                    if (cursorScale < NormalCursorScale)
                    {
                        cursorScale = NormalCursorScale;
                    }

                    Cursors[index].localScale = Vector3.one * cursorScale;
                }
            }
        }
        else
        {
            Cursors[index].gameObject.SetActive(false);
        }
    }

    // clear the current selection
    public void ClearSelection()
    {
        if (base.eventSystem.currentSelectedGameObject)
        {
            base.eventSystem.SetSelectedGameObject(null);
        }
    }

    // select a game object
    private void Select(GameObject go)
    {
        ClearSelection();

        if (ExecuteEvents.GetEventHandler<ISelectHandler>(go))
        {
            base.eventSystem.SetSelectedGameObject(go);
        }
    }

    // send update event to selected object
    // needed for InputField to receive keyboard input
    private bool SendUpdateEventToSelectedObject()
    {
        if (base.eventSystem.currentSelectedGameObject == null)
            return false;

        BaseEventData data = GetBaseEventData();

        ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);

        return data.used;
    }

    private void UpdateCameraPosition(int index)
    {
        ControllerCamera.transform.position = Controllers[index].transform.position;
        ControllerCamera.transform.forward = Controllers[index].transform.forward;
    }

    private void InitializeControllers()
    {
        for (int index = 0; index < Controllers.Length; index++)
        {
            if (Controllers[index] != null && Controllers[index].index != SteamVR_TrackedObject.EIndex.None)
            {
                ControllerDevices[index] = SteamVR_Controller.Input((int)Controllers[index].index);
            }
            else
            {
                ControllerDevices[index] = null;
            }
        }
    }

    // Process is called by UI system to process events
    public override void Process()
    {
        InitializeControllers();

        GuiHit = false;
        ButtonUsed = false;

        // send update events if there is a selected object - this is important for InputField to receive keyboard events
        SendUpdateEventToSelectedObject();

        // see if there is a UI element that is currently being looked at
        for (int index = 0; index < Cursors.Length; index++)
        {
            if (Controllers[index].gameObject.activeInHierarchy == false)
            {
                if (Cursors[index].gameObject.activeInHierarchy == true)
                {
                    Cursors[index].gameObject.SetActive(false);
                }
                continue;
            }

            UpdateCameraPosition(index);

            bool hit = GetLookPointerEventData(index);
            if (hit == false)
                continue;

            CurrentPoint[index] = PointEvents[index].pointerCurrentRaycast.gameObject;

            // handle enter and exit events (highlight)
            base.HandlePointerExitAndEnter(PointEvents[index], CurrentPoint[index]);

            // update cursor
            UpdateCursor(index, PointEvents[index]);

            if (Controllers[index] != null)
            {
                if (ButtonDown(index))
                {
                    ClearSelection();

                    PointEvents[index].pressPosition = PointEvents[index].position;
                    PointEvents[index].pointerPressRaycast = PointEvents[index].pointerCurrentRaycast;
                    PointEvents[index].pointerPress = null;

                    if (CurrentPoint[index] != null)
                    {
                        CurrentPressed[index] = CurrentPoint[index];

                        GameObject newPressed = ExecuteEvents.ExecuteHierarchy(CurrentPressed[index], PointEvents[index], ExecuteEvents.pointerDownHandler);

                        if (newPressed == null)
                        {
                            // some UI elements might only have click handler and not pointer down handler
                            newPressed = ExecuteEvents.ExecuteHierarchy(CurrentPressed[index], PointEvents[index], ExecuteEvents.pointerClickHandler);
                            if (newPressed != null)
                            {
                                CurrentPressed[index] = newPressed;
                            }
                        }
                        else
                        {
                            CurrentPressed[index] = newPressed;
                            // we want to do click on button down at same time, unlike regular mouse processing
                            // which does click when mouse goes up over same object it went down on
                            // reason to do this is head tracking might be jittery and this makes it easier to click buttons
                            ExecuteEvents.Execute(newPressed, PointEvents[index], ExecuteEvents.pointerClickHandler);
                        }

                        if (newPressed != null)
                        {
                            PointEvents[index].pointerPress = newPressed;
                            CurrentPressed[index] = newPressed;
                            Select(CurrentPressed[index]);
                            ButtonUsed = true;
                        }

                        ExecuteEvents.Execute(CurrentPressed[index], PointEvents[index], ExecuteEvents.beginDragHandler);
                        PointEvents[index].pointerDrag = CurrentPressed[index];
                        CurrentDragging[index] = CurrentPressed[index];
                    }
                }
                
                if (ButtonUp(index))
                {
                    if (CurrentDragging[index])
                    {
                        ExecuteEvents.Execute(CurrentDragging[index], PointEvents[index], ExecuteEvents.endDragHandler);
                        if (CurrentPoint[index] != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(CurrentPoint[index], PointEvents[index], ExecuteEvents.dropHandler);
                        }
                        PointEvents[index].pointerDrag = null;
                        CurrentDragging[index] = null;
                    }
                    if (CurrentPressed[index])
                    {
                        ExecuteEvents.Execute(CurrentPressed[index], PointEvents[index], ExecuteEvents.pointerUpHandler);
                        PointEvents[index].rawPointerPress = null;
                        PointEvents[index].pointerPress = null;
                        CurrentPressed[index] = null;
                    }
                }

                // drag handling
                if (CurrentDragging[index] != null)
                {
                    ExecuteEvents.Execute(CurrentDragging[index], PointEvents[index], ExecuteEvents.dragHandler);
                }
            }
        }
    }

    private bool ButtonDown(int index)
    {
        return (ControllerDevices[index] != null && ControllerDevices[index].GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) == true);
    }

    private bool ButtonUp(int index)
    {
        return (ControllerDevices[index] != null && ControllerDevices[index].GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) == true);
    }
}
