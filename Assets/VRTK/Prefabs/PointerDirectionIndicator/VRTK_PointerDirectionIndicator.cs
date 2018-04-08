// Pointer Direction Indicator|Prefabs|0100
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    public delegate void PointerDirectionIndicatorEventHandler(object sender);

    /// <summary>
    /// Adds a Pointer Direction Indicator to a pointer renderer and determines a given world rotation that can be used by a Destiantion Marker.
    /// </summary>
    /// <remarks>
    /// **Prefab Usage:**
    ///  * Place the `VRTK/Prefabs/PointerDirectionIndicator/PointerDirectionIndicator` prefab into the scene hierarchy.
    ///  * Attach the `PointerDirectionIndicator` scene GameObejct to the `Direction Indicator` inspector parameter on a `VRTK_BasePointerRenderer` component.
    ///
    ///   > This can be useful for rotating the play area upon teleporting to face the user in a new direction without expecting them to physically turn in the play space.
    /// </remarks>
    public class VRTK_PointerDirectionIndicator : MonoBehaviour
    {
        /// <summary>
        /// States of Direction Indicator Visibility.
        /// </summary>
        public enum VisibilityState
        {
            /// <summary>
            /// Only shows the direction indicator when the pointer is active.
            /// </summary>
            OnWhenPointerActive,
            /// <summary>
            /// Only shows the direction indicator when the pointer cursor is visible or if the cursor is hidden and the pointer is active.
            /// </summary>
            AlwaysOnWithPointerCursor
        }

        [Header("Control Settings")]

        [Tooltip("The touchpad axis needs to be above this deadzone for it to register as a valid touchpad angle.")]
        public Vector2 touchpadDeadzone = Vector2.zero;
        [Tooltip("The axis to use for the direction coordinates.")]
        public VRTK_ControllerEvents.Vector2AxisAlias coordinateAxis = VRTK_ControllerEvents.Vector2AxisAlias.Touchpad;

        [Header("Appearance Settings")]

        [Tooltip("If this is checked then the reported rotation will include the offset of the headset rotation in relation to the play area.")]
        public bool includeHeadsetOffset = true;
        [Tooltip("If this is checked then the direction indicator will be displayed when the location is invalid.")]
        public bool displayOnInvalidLocation = true;
        [Tooltip("If this is checked then the pointer valid/invalid colours will also be used to change the colour of the direction indicator.")]
        public bool usePointerColor = false;
        [Tooltip("Determines when the direction indicator will be visible.")]
        public VisibilityState indicatorVisibility = VisibilityState.OnWhenPointerActive;

        [HideInInspector]
        public bool isActive = true;

        /// <summary>
        /// Emitted when the object tooltip is reset.
        /// </summary>
        public event PointerDirectionIndicatorEventHandler PointerDirectionIndicatorPositionSet;

        protected VRTK_ControllerEvents controllerEvents;
        protected Transform playArea;
        protected Transform headset;
        protected GameObject validLocation;
        protected GameObject invalidLocation;

        public virtual void OnPointerDirectionIndicatorPositionSet()
        {
            if (PointerDirectionIndicatorPositionSet != null)
            {
                PointerDirectionIndicatorPositionSet(this);
            }
        }

        /// <summary>
        /// The Initialize method is used to set up the direction indicator.
        /// </summary>
        /// <param name="events">The Controller Events script that is used to control the direction indicator's rotation.</param>
        public virtual void Initialize(VRTK_ControllerEvents events)
        {
            controllerEvents = events;
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            headset = VRTK_DeviceFinder.HeadsetTransform();
        }

        /// <summary>
        /// The SetPosition method is used to set the world position of the direction indicator.
        /// </summary>
        /// <param name="active">Determines if the direction indicator GameObject should be active or not.</param>
        /// <param name="position">The position to set the direction indicator to.</param>
        public virtual void SetPosition(bool active, Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive((isActive && active));
            OnPointerDirectionIndicatorPositionSet();
        }

        /// <summary>
        /// The GetRotation method returns the current reported rotation of the direction indicator.
        /// </summary>
        /// <returns>The reported rotation of the direction indicator.</returns>
        public virtual Quaternion GetRotation()
        {
            float offset = (includeHeadsetOffset ? playArea.eulerAngles.y - headset.eulerAngles.y : 0f);
            return Quaternion.Euler(0f, transform.localEulerAngles.y + offset, 0f);
        }

        /// <summary>
        /// The SetMaterialColor method sets the current material colour on the direction indicator.
        /// </summary>
        /// <param name="color">The colour to update the direction indicatormaterial to.</param>
        /// <param name="validity">Determines if the colour being set is based from a valid location or invalid location.</param>
        public virtual void SetMaterialColor(Color color, bool validity)
        {
            if (validLocation != null)
            {
                validLocation.SetActive(validity);
            }

            if (invalidLocation != null)
            {
                invalidLocation.SetActive((displayOnInvalidLocation ? !validity : validity));
            }

            if (usePointerColor)
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.color = color;
                }
            }
        }

        /// <summary>
        /// The GetControllerEvents method returns the associated Controller Events script with the Pointer Direction Indicator script.
        /// </summary>
        /// <returns>The associated Controller Events script.</returns>
        public virtual VRTK_ControllerEvents GetControllerEvents()
        {
            return controllerEvents;
        }

        protected virtual void Awake()
        {
            validLocation = transform.Find("ValidLocation").gameObject;
            invalidLocation = transform.Find("InvalidLocation").gameObject;
            gameObject.SetActive(false);
        }

        protected virtual void Update()
        {
            if (controllerEvents != null && controllerEvents.GetAxisState(coordinateAxis, SDK_BaseController.ButtonPressTypes.Touch) && !InsideDeadzone(controllerEvents.GetAxis(coordinateAxis)))
            {
                float touchpadAngle = controllerEvents.GetAxisAngle(coordinateAxis);
                float angle = ((touchpadAngle > 180) ? touchpadAngle -= 360 : touchpadAngle) + headset.eulerAngles.y;
                transform.localEulerAngles = new Vector3(0f, angle, 0f);
            }
        }

        protected virtual bool InsideDeadzone(Vector2 currentAxis)
        {
            return (currentAxis == Vector2.zero || (Mathf.Abs(currentAxis.x) <= touchpadDeadzone.x && Mathf.Abs(currentAxis.y) <= touchpadDeadzone.y));
        }
    }
}