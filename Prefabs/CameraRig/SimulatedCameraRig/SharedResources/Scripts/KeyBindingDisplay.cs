namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using UnityEngine.UI;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.CameraRig.UnityXRCameraRig.Input;

    /// <summary>
    /// Sets up the key binding display.
    /// </summary>
    public class KeyBindingDisplay : MonoBehaviour
    {
        [Tooltip("The Text component to apply the instructions text to."), InternalSetting, SerializeField]
        protected Text keyBindingText;
        [Tooltip("The action for handling forward."), InternalSetting, SerializeField]
        protected UnityButtonAction forward;
        [Tooltip("The action for handling backward."), InternalSetting, SerializeField]
        protected UnityButtonAction backward;
        [Tooltip("The action for handling strafeLeft."), InternalSetting, SerializeField]
        protected UnityButtonAction strafeLeft;
        [Tooltip("The action for handling strafeRight."), InternalSetting, SerializeField]
        protected UnityButtonAction strafeRight;
        [Tooltip("The action for handling button1."), InternalSetting, SerializeField]
        protected UnityButtonAction button1;
        [Tooltip("The action for handling button2."), InternalSetting, SerializeField]
        protected UnityButtonAction button2;
        [Tooltip("The action for handling button3."), InternalSetting, SerializeField]
        protected UnityButtonAction button3;
        [Tooltip("The action for handling switchToPlayer."), InternalSetting, SerializeField]
        protected UnityButtonAction switchToPlayer;
        [Tooltip("The action for handling switchToLeftController."), InternalSetting, SerializeField]
        protected UnityButtonAction switchToLeftController;
        [Tooltip("The action for handling switchToRightController."), InternalSetting, SerializeField]
        protected UnityButtonAction switchToRightController;
        [Tooltip("The action for handling resetPlayer."), InternalSetting, SerializeField]
        protected UnityButtonAction resetPlayer;
        [Tooltip("The action for handling resetControllers."), InternalSetting, SerializeField]
        protected UnityButtonAction resetControllers;
        [Tooltip("The action for handling toggleInstructions."), InternalSetting, SerializeField]
        protected UnityButtonAction toggleInstructions;
        [Tooltip("The action for handling lockCursorToggle."), InternalSetting, SerializeField]
        protected UnityButtonAction lockCursorToggle;

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
            keyBindingText.text = string.Format(
                Instructions,
                forward.keyCode,
                backward.keyCode,
                strafeLeft.keyCode,
                strafeRight.keyCode,
                button1.keyCode,
                button2.keyCode,
                button3.keyCode,
                switchToPlayer.keyCode,
                switchToLeftController.keyCode,
                switchToRightController.keyCode,
                resetPlayer.keyCode,
                resetControllers.keyCode,
                toggleInstructions.keyCode,
                lockCursorToggle.keyCode
                );
        }
    }
}