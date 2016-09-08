// Control|Controls3D|0010
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// All 3D controls extend the `VRTK_Control` abstract class which provides a default set of methods and events that all of the subsequent controls expose.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class VRTK_Control : MonoBehaviour
    {
        public enum Direction
        {
            autodetect, x, y, z
        }

        [System.Serializable]
        public class ValueChangedEvent : UnityEvent<float, float> { }

        [System.Serializable]
        public class DefaultControlEvents
        {
            /// <summary>
            /// Emitted when the control is interacted with.
            /// </summary>
            public ValueChangedEvent OnValueChanged;
        }

        // to be registered by each control to support value normalization
        public struct ControlValueRange
        {
            public float controlMin;
            public float controlMax;
        }

        private static Color COLOR_OK = Color.yellow;
        private static Color COLOR_ERROR = new Color(1, 0, 0, 0.9f);
        private static float MIN_OPENING_DISTANCE = 20f; // percentage how far open something needs to be in order to activate it

        public DefaultControlEvents defaultEvents;

        abstract protected void InitRequiredComponents();
        abstract protected bool DetectSetup();
        abstract protected ControlValueRange RegisterValueRange();
        abstract protected void HandleUpdate();
        protected Bounds bounds;
        protected bool setupSuccessful = true;

        protected float value;

        private ControlValueRange cvr;
        private GameObject controlContent;
        private bool hideControlContent = false;

        /// <summary>
        /// The GetValue method returns the current value/position/setting of the control depending on the control that is extending this abstract class.
        /// </summary>
        /// <returns>The current value of the control.</returns>
        public float GetValue()
        {
            return value;
        }

        /// <summary>
        /// The GetNormalizedValue method returns the current value mapped onto a range between 0 and 100.
        /// </summary>
        /// <returns>The current normalized value of the control.</returns>
        public float GetNormalizedValue()
        {
            return Mathf.Abs(Mathf.Round((value - cvr.controlMin) / (cvr.controlMax - cvr.controlMin) * 100));
        }

        /// <summary>
        /// The SetContent method sets the given game object as the content of the control. This will then disable and optionally hide the content when a control is obscuring its view to prevent interacting with content within a control.
        /// </summary>
        /// <param name="content">The content to be considered within the control.</param>
        /// <param name="hideContent">When true the content will be hidden in addition to being non-interactable in case the control is fully closed.</param>
        public void SetContent(GameObject content, bool hideContent)
        {
            controlContent = content;
            hideControlContent = hideContent;
        }

        /// <summary>
        /// The GetContent method returns the current game object of the control's content.
        /// </summary>
        /// <returns>The currently stored content for the control.</returns>
        public GameObject GetContent()
        {
            return controlContent;
        }

        protected virtual void Awake()
        {
            if (Application.isPlaying)
            {
                InitRequiredComponents();
            }

            setupSuccessful = DetectSetup();

            if (Application.isPlaying)
            {
                cvr = RegisterValueRange();
                HandleInteractables();
            }
        }

        protected virtual void Update()
        {
            if (!Application.isPlaying)
            {
                setupSuccessful = DetectSetup();
            }
            else if (setupSuccessful)
            {
                float oldValue = value;
                HandleUpdate();

                // trigger events
                if (value != oldValue)
                {
                    HandleInteractables();
                    defaultEvents.OnValueChanged.Invoke(GetValue(), GetNormalizedValue());
                }
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (!enabled)
            {
                return;
            }

            bounds = Utilities.GetBounds(transform);
            Gizmos.color = (setupSuccessful) ? COLOR_OK : COLOR_ERROR;

            if (setupSuccessful)
            {
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            else
            {
                Gizmos.DrawCube(bounds.center, bounds.size * 1.01f); // draw slightly bigger to eliminate flickering
            }
        }

        protected Vector3 getThirdDirection(Vector3 axis1, Vector3 axis2)
        {
            bool xTaken = axis1.x != 0 || axis2.x != 0;
            bool yTaken = axis1.y != 0 || axis2.y != 0;
            bool zTaken = axis1.z != 0 || axis2.z != 0;

            if (xTaken && yTaken)
            {
                return Vector3.forward;
            }
            else if (xTaken && zTaken)
            {
                return Vector3.up;
            }
            else
            {
                return Vector3.right;
            }

        }

        private void HandleInteractables()
        {
            if (controlContent == null)
            {
                return;
            }

            if (hideControlContent)
            {
                controlContent.SetActive(value > 0);
            }

            // do not cache objects since otherwise they would still be made inactive once taken out of the content
            foreach (VRTK_InteractableObject io in controlContent.GetComponentsInChildren<VRTK_InteractableObject>(true))
            {
                io.enabled = value > MIN_OPENING_DISTANCE;
            }
        }
    }
}