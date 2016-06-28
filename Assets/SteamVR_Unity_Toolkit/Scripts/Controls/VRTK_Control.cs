namespace VRTK
{
    using UnityEngine;

    [ExecuteInEditMode]
    public abstract class VRTK_Control : MonoBehaviour
    {
        private static Color COLOR_OK = Color.yellow;
        private static Color COLOR_ERROR = new Color(1, 0, 0, 0.9f);

        public delegate void ValueChange(float value);
        public event ValueChange OnValueChanged;

        abstract protected void InitRequiredComponents();
        abstract protected bool DetectSetup();
        abstract protected void HandleUpdate();

        protected Bounds bounds;
        protected float value;

        protected bool setupSuccessful = true;

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
                    if (OnValueChanged != null)
                    {
                        OnValueChanged(getValue());
                    }
                }
            }
        }

        public virtual void OnDrawGizmos()
        {
            bounds = Utilities.GetBounds(transform);
            Gizmos.color = (setupSuccessful) ? COLOR_OK : COLOR_ERROR;

            if (setupSuccessful)
            {
                Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
            }
            else
            {
                Gizmos.DrawCube(bounds.center, bounds.extents * 2.01f); // draw slightly bigger to eliminate flickering
            }
        }
    }
}