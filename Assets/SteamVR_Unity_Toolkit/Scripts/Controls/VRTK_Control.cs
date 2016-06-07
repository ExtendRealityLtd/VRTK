namespace VRTK {
    using UnityEngine;

    [ExecuteInEditMode]
    public abstract class VRTK_Control : MonoBehaviour {
        private static Color COLOR_OK = Color.yellow;
        private static Color COLOR_ERROR = Color.red;

        public delegate void ValueChange(float value);
        public event ValueChange OnValueChanged;

        abstract protected void initRequiredComponents();
        abstract protected bool detectSetup();
        abstract protected void handleUpdate();

        protected Bounds bounds;
        protected float value;

        protected bool setupSuccessful = true;

        public float getValue() {
            return value;
        }

        public virtual void Start() {
            if (Application.isPlaying) {
                initRequiredComponents();
            }
            setupSuccessful = detectSetup();
        }

        public virtual void Update() {
            if (!Application.isPlaying) {
                setupSuccessful = detectSetup();
            } else if (setupSuccessful) {
                float oldValue = value;
                handleUpdate();

                // trigger events
                if (value != oldValue) {
                    if (OnValueChanged != null) {
                        OnValueChanged(getValue());
                    }
                }
            }
        }

        public virtual void OnDrawGizmos() {
            bounds = Utilities.getBounds(transform);
            Gizmos.color = (setupSuccessful) ? COLOR_OK : COLOR_ERROR;

            if (setupSuccessful) {
                Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
            } else {
                Gizmos.DrawCube(bounds.center, bounds.extents * 2);
            }
        }

    }
}