namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig.Input
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Action;

    /// <summary>
    /// Listens for input from the mouse and converts into Vector2 data.
    /// </summary>
    public class MouseVector2DAction : Vector2Action
    {
        /// <summary>
        /// The named x axis of the mouse.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public string XAxisName { get; set; } = "Mouse X";
        /// <summary>
        /// The named y axis of the mouse.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public string YAxisName { get; set; } = "Mouse Y";
        /// <summary>
        /// Determines whether to lock the cursor in the game window.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool LockCursor { get; set; }
        /// <summary>
        /// Multiplies the speed at which the unlocked cursor moves the axis.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float CursorMultiplier { get; set; } = 1f;
        /// <summary>
        /// Multiplies the speed at which the locked cursor moves the axis.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float LockedCursorMultiplier { get; set; } = 2f;

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
                ? new Vector3(Input.GetAxis(XAxisName), Input.GetAxis(YAxisName)) * LockedCursorMultiplier
                : difference * CursorMultiplier;
        }
    }
}