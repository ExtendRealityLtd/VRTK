namespace VRTK
{
    using UnityEngine;
    using System;

    [Serializable]
    public class VRTK_ControllerModelElementPaths
    {
        [Tooltip("The overall shape of the controller.")]
        public string bodyModelPath = "";
        [Tooltip("The model that represents the trigger button.")]
        public string triggerModelPath = "";
        [Tooltip("The model that represents the left grip button.")]
        public string leftGripModelPath = "";
        [Tooltip("The model that represents the right grip button.")]
        public string rightGripModelPath = "";
        [Tooltip("The model that represents the touchpad.")]
        public string touchpadModelPath = "";
        [Tooltip("The model that represents button one.")]
        public string buttonOneModelPath = "";
        [Tooltip("The model that represents button two.")]
        public string buttonTwoModelPath = "";
        [Tooltip("The model that represents the system menu button.")]
        public string systemMenuModelPath = "";
        [Tooltip("The model that represents the start menu button.")]
        public string startMenuModelPath = "";
    }
}