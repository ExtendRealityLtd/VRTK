// Pointer Direction Indicator|Prefabs|0057
namespace VRTK
{
    using UnityEngine;

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
        [Tooltip("If this is checked then the reported rotation will include the offset of the headset rotation in relation to the play area.")]
        public bool includeHeadsetOffset = true;

        protected VRTK_ControllerEvents controllerEvents;
        protected Transform playArea;
        protected Transform headset;

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
            gameObject.SetActive(active);
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

        protected virtual void Awake()
        {
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