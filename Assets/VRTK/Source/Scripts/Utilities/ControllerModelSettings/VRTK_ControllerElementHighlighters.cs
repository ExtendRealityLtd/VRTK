namespace VRTK
{
    using UnityEngine;
    using System;
    using Highlighters;

    [Serializable]
    public class VRTK_ControllerElementHighlighters
    {
        [Tooltip("The highlighter to use on the overall shape of the controller.")]
        public VRTK_BaseHighlighter body;
        [Tooltip("The highlighter to use on the trigger button.")]
        public VRTK_BaseHighlighter trigger;
        [Tooltip("The highlighter to use on the left grip button.")]
        public VRTK_BaseHighlighter gripLeft;
        [Tooltip("The highlighter to use on the right grip button.")]
        public VRTK_BaseHighlighter gripRight;
        [Tooltip("The highlighter to use on the touchpad.")]
        public VRTK_BaseHighlighter touchpad;
        [Tooltip("The highlighter to use on the touchpad two.")]
        public VRTK_BaseHighlighter touchpadTwo;
        [Tooltip("The highlighter to use on button one.")]
        public VRTK_BaseHighlighter buttonOne;
        [Tooltip("The highlighter to use on button two.")]
        public VRTK_BaseHighlighter buttonTwo;
        [Tooltip("The highlighter to use on the system menu button.")]
        public VRTK_BaseHighlighter systemMenu;
        [Tooltip("The highlighter to use on the start menu button.")]
        public VRTK_BaseHighlighter startMenu;
    }
}