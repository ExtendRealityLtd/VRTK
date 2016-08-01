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
        public class ValueChangedEvent : UnityEvent<float> { }

        [System.Serializable]
        public class DefaultControlEvents
        {
            public ValueChangedEvent OnValueChanged;
        }

        private static Color COLOR_OK = Color.yellow;
        private static Color COLOR_ERROR = new Color(1, 0, 0, 0.9f);
        private static float MIN_OPENING_DISTANCE = 20f; // percentage how far open something needs to be in order to activate it

        public DefaultControlEvents defaultEvents;

        abstract protected void InitRequiredComponents();
        abstract protected bool DetectSetup();
        abstract protected void HandleUpdate();

        protected Bounds bounds;
        protected float value;
        protected bool setupSuccessful = true;

        private GameObject controlContent;
        private bool hideControlContent = false;

        public float getValue()
        {
            return value;
        }

        public virtual void Start()
        {
            if (Application.isPlaying)
            {
                InitRequiredComponents();
            }

            setupSuccessful = DetectSetup();

            if (Application.isPlaying)
            {
                HandleInteractables();
            }
        }

        public virtual void Update()
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
                    defaultEvents.OnValueChanged.Invoke(getValue());
                }
            }
        }

        public virtual void OnDrawGizmos()
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

        public void SetContent(GameObject content, bool hideContent)
        {
            controlContent = content;
            hideControlContent = hideContent;
        }

        public GameObject GetContent()
        {
            return controlContent;
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