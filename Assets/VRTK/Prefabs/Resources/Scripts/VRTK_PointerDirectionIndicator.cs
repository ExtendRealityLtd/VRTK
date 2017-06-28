// Pointer Direction Indicator|Prefabs|0057
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    public delegate void PointerDirectionIndicatorEventHandler(object sender);

    /// <summary>
    /// The Pointer Direction Indicator is used to determine a given world rotation that can be used by a Destiantion Marker.
    /// </summary>
    /// <remarks>
    /// The Pointer Direction Indicator can be attached to a VRTK_Pointer in the `Direction Indicator` parameter and will the be used to send rotation data when the destination marker events are emitted.
    ///
    /// This can be useful for rotating the play area upon teleporting to face the user in a new direction without expecting them to physically turn in the play space.
    /// </remarks>
    public class VRTK_PointerDirectionIndicator : MonoBehaviour
    {
        [Header("Appearance Settings")]

        [Tooltip("If this is checked then the reported rotation will include the offset of the headset rotation in relation to the play area.")]
        public bool includeHeadsetOffset = true;
        [Tooltip("If this is checked then the direction indicator will be displayed when the location is invalid.")]
        public bool displayOnInvalidLocation = true;
        [Tooltip("If this is checked then the pointer valid/invalid colours will also be used to change the colour of the direction indicator.")]
        public bool usePointerColor = false;

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
            validLocation.SetActive(validity);
            invalidLocation.SetActive((displayOnInvalidLocation ? !validity : validity));

            if (usePointerColor)
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.color = color;
                }
            }
        }

        protected virtual void Awake()
        {
            validLocation = transform.Find("ValidLocation").gameObject;
            invalidLocation = transform.Find("InvalidLocation").gameObject;
            gameObject.SetActive(false);
        }

        protected virtual void Update()
        {
            if (controllerEvents != null)
            {
                float touchpadAngle = controllerEvents.GetTouchpadAxisAngle();
                float angle = ((touchpadAngle > 180) ? touchpadAngle -= 360 : touchpadAngle) + headset.eulerAngles.y;
                transform.localEulerAngles = new Vector3(0f, angle, 0f);
            }
        }
    }
}