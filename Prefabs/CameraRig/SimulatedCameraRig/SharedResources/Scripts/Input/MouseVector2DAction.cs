namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig.Input
{
    using UnityEngine;
    using Zinnia.Action;

    /// <summary>
    /// Listens for input from the mouse and converts into Vector2 data.
    /// </summary>
    public class MouseVector2DAction : Vector2Action
    {
        /// <summary>
        /// The named x axis of the mouse.
        /// </summary>
        [Tooltip("The named x axis of the mouse.")]
        public string xAxisName = "Mouse X";
        /// <summary>
        /// The named y axis of the mouse.
        /// </summary>
        [Tooltip("The named y axis of the mouse.")]
        public string yAxisName = "Mouse Y";
        [Tooltip("Determines whether to lock the cursor in the game window."), SerializeField]
        private bool _lockCursor = false;
        /// <summary>
        /// Determines whether to lock the cursor in the game window.
        /// </summary>
        public bool LockCursor
        {
            get { return _lockCursor; }
            set
            {
                _lockCursor = value;
            }
        }
        /// <summary>
        /// Multiplies the speed at which the unlocked cursor moves the axis.
        /// </summary>
        [Tooltip("Multiplies the speed at which the unlocked cursor moves the axis.")]
        public float cursorMultiplier = 1f;
        /// <summary>
        /// Multiplies the speed at which the locked cursor moves the axis.
        /// </summary>
        [Tooltip("Multiplies the speed at which the locked cursor moves the axis.")]
        public float lockedCursorMultiplier = 2f;

        /// <summary>
        /// The previous axis position of the mouse pointer.
        /// </summary>
        protected Vector3 previousMousePosition;

        protected override void OnEnable()
        {
            previousMousePosition = Input.mousePosition;
            base.OnEnable();
        }

        protected virtual void Update()
        {
            Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Vector3 mouseData = GetMouseDelta();
            Receive(new Vector2(mouseData.x, mouseData.y));
        }

        /// <summary>
        /// Gets the difference in axis position of the mouse between the previous frame and current frame.
        /// </summary>
        /// <returns>The difference in mouse axis position.</returns>
        protected virtual Vector3 GetMouseDelta()
        {
            Vector3 difference = Input.mousePosition - previousMousePosition;
            previousMousePosition = Input.mousePosition;

            return Cursor.lockState == CursorLockMode.Locked
                ? new Vector3(Input.GetAxis(xAxisName), Input.GetAxis(yAxisName)) * lockedCursorMultiplier
                : difference * cursorMultiplier;
        }
    }
}