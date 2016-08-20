namespace VRTK
{
    using UnityEngine;
    using UnityEngine.Events;

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

        public float GetValue()
        {
            return value;
        }

        /// <summary>
        /// Returns the current value mapped onto a range between 0% and 100%.
        /// </summary>
        /// <returns>Normalized Value</returns>
        public float GetNormalizedValue()
        {
            return Mathf.Abs(Mathf.Round((value - cvr.controlMin) / (cvr.controlMax - cvr.controlMin) * 100));
        }

        public void SetContent(GameObject content, bool hideContent)
        {
            controlContent = content;
            hideControlContent = hideContent;
        }

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