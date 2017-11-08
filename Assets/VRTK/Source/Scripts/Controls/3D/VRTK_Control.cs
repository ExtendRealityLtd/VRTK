// Control|Controls3D|100010
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="value">The current value being reported by the control.</param>
    /// <param name="normalizedValue">The normalized value being reported by the control.</param>
    public struct Control3DEventArgs
    {
        public float value;
        public float normalizedValue;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="Control3DEventArgs"/></param>
    public delegate void Control3DEventHandler(object sender, Control3DEventArgs e);

    /// <summary>
    /// All 3D controls extend the `VRTK_Control` abstract class which provides a default set of methods and events that all of the subsequent controls expose.
    /// </summary>
    [ExecuteInEditMode]
    [System.Obsolete("`VRTK_Control` has been deprecated. This script will be removed in a future version of VRTK.")]
    public abstract class VRTK_Control : MonoBehaviour
    {
        /// <summary>
        /// The ControlValueRange struct provides a way for each inherited control to support value normalization.
        /// </summary>
        public struct ControlValueRange
        {
            public float controlMin;
            public float controlMax;
        }

        /// <summary>
        /// 3D Control Directions
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Attempt to auto detect the axis.
            /// </summary>
            autodetect,
            /// <summary>
            /// The world x direction.
            /// </summary>
            x,
            /// <summary>
            /// The world y direction.
            /// </summary>
            y,
            /// <summary>
            /// The world z direction.
            /// </summary>
            z
        }

        [Tooltip("If active the control will react to the controller without the need to push the grab button.")]
        public bool interactWithoutGrab = false;

        /// <summary>
        /// Emitted when the 3D Control value has changed.
        /// </summary>
        public event Control3DEventHandler ValueChanged;

        abstract protected void InitRequiredComponents();
        abstract protected bool DetectSetup();
        abstract protected ControlValueRange RegisterValueRange();

        protected Bounds bounds;
        protected bool setupSuccessful = true;
        protected VRTK_ControllerRigidbodyActivator autoTriggerVolume;

        protected float value;
        protected static Color COLOR_OK = Color.yellow;
        protected static Color COLOR_ERROR = new Color(1, 0, 0, 0.9f);
        protected const float MIN_OPENING_DISTANCE = 20f; // percentage how far open something needs to be in order to activate it
        protected ControlValueRange valueRange;
        protected GameObject controlContent;
        protected bool hideControlContent = false;

        public virtual void OnValueChanged(Control3DEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        /// <summary>
        /// The GetValue method returns the current value/position/setting of the control depending on the control that is extending this abstract class.
        /// </summary>
        /// <returns>The current value of the control.</returns>
        public virtual float GetValue()
        {
            return value;
        }

        /// <summary>
        /// The GetNormalizedValue method returns the current value mapped onto a range between 0 and 100.
        /// </summary>
        /// <returns>The current normalized value of the control.</returns>
        public virtual float GetNormalizedValue()
        {
            return Mathf.Abs(Mathf.Round((value - valueRange.controlMin) / (valueRange.controlMax - valueRange.controlMin) * 100));
        }

        /// <summary>
        /// The SetContent method sets the given game object as the content of the control. This will then disable and optionally hide the content when a control is obscuring its view to prevent interacting with content within a control.
        /// </summary>
        /// <param name="content">The content to be considered within the control.</param>
        /// <param name="hideContent">When true the content will be hidden in addition to being non-interactable in case the control is fully closed.</param>
        public virtual void SetContent(GameObject content, bool hideContent)
        {
            controlContent = content;
            hideControlContent = hideContent;
        }

        /// <summary>
        /// The GetContent method returns the current game object of the control's content.
        /// </summary>
        /// <returns>The currently stored content for the control.</returns>
        public virtual GameObject GetContent()
        {
            return controlContent;
        }

        abstract protected void HandleUpdate();

        protected virtual void Awake()
        {
            if (Application.isPlaying)
            {
                InitRequiredComponents();
                if (interactWithoutGrab)
                {
                    CreateTriggerVolume();
                }
            }

            setupSuccessful = DetectSetup();

            if (Application.isPlaying)
            {
                valueRange = RegisterValueRange();
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
                    OnValueChanged(SetControlEvent());
                }
            }
        }

        protected virtual Control3DEventArgs SetControlEvent()
        {
            Control3DEventArgs e;
            e.value = GetValue();
            e.normalizedValue = GetNormalizedValue();
            return e;
        }

        protected virtual void OnDrawGizmos()
        {
            if (!enabled)
            {
                return;
            }

            bounds = VRTK_SharedMethods.GetBounds(transform);
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

        protected virtual void CreateTriggerVolume()
        {
            GameObject autoTriggerVolumeGO = new GameObject(name + "-Trigger");
            autoTriggerVolumeGO.transform.SetParent(transform);
            autoTriggerVolume = autoTriggerVolumeGO.AddComponent<VRTK_ControllerRigidbodyActivator>();

            // calculate bounding box
            Bounds triggerBounds = VRTK_SharedMethods.GetBounds(transform);
            triggerBounds.Expand(triggerBounds.size * 0.2f);
            autoTriggerVolumeGO.transform.position = triggerBounds.center;
            BoxCollider triggerCollider = autoTriggerVolumeGO.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = triggerBounds.size;
        }

        protected Vector3 GetThirdDirection(Vector3 axis1, Vector3 axis2)
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

        protected virtual void HandleInteractables()
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
            VRTK_InteractableObject[] foundInteractableObjects = controlContent.GetComponentsInChildren<VRTK_InteractableObject>(true);
            for (int i = 0; i < foundInteractableObjects.Length; i++)
            {
                foundInteractableObjects[i].enabled = value > MIN_OPENING_DISTANCE;
            }
        }
    }
}