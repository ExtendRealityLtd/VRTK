namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using UnityEngine.UI;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.CameraRig.UnityXRCameraRig.Input;
    using Malimbe.PropertySerializationAttribute;

    /// <summary>
    /// Sets up the key binding display.
    /// </summary>
    public class KeyBindingDisplay : MonoBehaviour
    {
        /// <summary>
        /// The Text component to apply the instructions text to.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public Text KeyBindingText { get; protected set; }
        /// <summary>
        /// The action for handling forward.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction Forward { get; protected set; }
        /// <summary>
        /// The action for handling backward.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction Backward { get; protected set; }
        /// <summary>
        /// The action for handling strafeLeft.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction StrafeLeft { get; protected set; }
        /// <summary>
        /// The action for handling strafeRight.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction StrafeRight { get; protected set; }
        /// <summary>
        /// The action for handling button1.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction Button1 { get; protected set; }
        /// <summary>
        /// The action for handling button2.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction Button2 { get; protected set; }
        /// <summary>
        /// The action for handling button3.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction Button3 { get; protected set; }
        /// <summary>
        /// The action for handling switchToPlayer.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction SwitchToPlayer { get; protected set; }
        /// <summary>
        /// The action for handling switchToLeftController.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction SwitchToLeftController { get; protected set; }
        /// <summary>
        /// The action for handling switchToRightController.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction SwitchToRightController { get; protected set; }
        /// <summary>
        /// The action for handling resetPlayer.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction ResetPlayer { get; protected set; }
        /// <summary>
        /// The action for handling resetControllers.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction ResetControllers { get; protected set; }
        /// <summary>
        /// The action for handling toggleInstructions.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction ToggleInstructions { get; protected set; }
        /// <summary>
        /// The action for handling lockCursorToggle.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public UnityButtonAction LockCursorToggle { get; protected set; }

        /// <summary>
        /// The instructions text.
        /// </summary>
        protected const string Instructions = @"<b>Simulator Key Bindings</b>

<b>Movement:</b>
Forward: {0}
Backward: {1}
Strafe Left: {2}
Strafe Right: {3}

<b>Buttons</b>
Button 1: {4}
Button 2: {5}
Button 3: {6}

<b>Object Control</b>
Move PlayArea: {7}
Move Left Controller: {8}
Move Right Controller: {9}
Reset Player: Position {10}
Reset Controller Position: {11}

<b>Misc</b>
Toggle Help Window: {12}
Lock Mouse Cursor: {13}";

        protected virtual void OnEnable()
        {
            KeyBindingText.text = string.Format(
                Instructions,
                Forward.KeyCode,
                Backward.KeyCode,
                StrafeLeft.KeyCode,
                StrafeRight.KeyCode,
                Button1.KeyCode,
                Button2.KeyCode,
                Button3.KeyCode,
                SwitchToPlayer.KeyCode,
                SwitchToLeftController.KeyCode,
                SwitchToRightController.KeyCode,
                ResetPlayer.KeyCode,
                ResetControllers.KeyCode,
                ToggleInstructions.KeyCode,
                LockCursorToggle.KeyCode
                );
        }
    }
}