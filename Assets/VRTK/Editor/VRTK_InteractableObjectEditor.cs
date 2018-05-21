namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using AllowedController = VRTK_InteractableObject.AllowedController;

    [CustomEditor(typeof(VRTK_InteractableObject), true)]
    public class VRTK_InteractableObjectEditor : Editor
    {
        private SerializedProperty isGrabbable;
        private SerializedProperty isUsable;
        private SerializedProperty allowedTouchControllers;
        private SerializedProperty allowedGrabControllers;
        private SerializedProperty allowedUseControllers;

        protected VRTK_InteractableObject interactableObject;

        private void OnEnable()
        {
            interactableObject = (VRTK_InteractableObject)target;

            isGrabbable = serializedObject.FindProperty("isGrabbable");
            isUsable = serializedObject.FindProperty("isUsable");
            allowedTouchControllers = serializedObject.FindProperty("allowedTouchControllers");
            allowedGrabControllers = serializedObject.FindProperty("allowedGrabControllers");
            allowedUseControllers = serializedObject.FindProperty("allowedUseControllers");
        }

        public override void OnInspectorGUI()
        {
            // Check for issues with the ability to touch objects
            RequireInteractScriptOnController<VRTK_InteractTouch>(
                allowedControllers: (AllowedController)allowedTouchControllers.intValue,
                noControllersMessage: "Controller aliases cannot be found in the current scene, make sure prefabs use the VRTK_InteractTouch script",
                missingComponentsMessage: "VRTK_InteractTouch is missing from the controller aliases, they will be unable to touch this object",
                missingOneComponentMessage: "VRTK_InteractTouch is missing from the \"{0}\" controller alias, it will be unable to touch this object"
            );

            // Check for issues with the ability to grab objects
            if (isGrabbable.boolValue)
            {
                RequireInteractScriptOnController<VRTK_InteractGrab>(
                    allowedControllers: (AllowedController)allowedGrabControllers.intValue,
                    noControllersMessage: "Controller aliases cannot be found in the current scene, make sure prefabs use the VRTK_InteractGrab script",
                    missingComponentsMessage: "VRTK_InteractGrab is missing from the controller aliases, they will be unable to grab this object",
                    missingOneComponentMessage: "VRTK_InteractGrab is missing from the \"{0}\" controller alias, it will be unable to grab this object"
                );
            }

            // Check for issues with the ability to use objects
            if (isUsable.boolValue)
            {
                RequireInteractScriptOnController<VRTK_InteractUse>(
                    allowedControllers: (AllowedController)allowedUseControllers.intValue,
                    noControllersMessage: "Controller aliases cannot be found in the current scene, make sure prefabs use the VRTK_InteractUse script",
                    missingComponentsMessage: "VRTK_InteractUse is missing from the controller aliases, they will be unable to use this object",
                    missingOneComponentMessage: "VRTK_InteractUse is missing from the \"{0}\" controller alias, it will be unable to use this object"
                );
            }

            DrawDefaultInspector();
        }

        /// <summary>
        /// Ensure that the controller aliases contain a VRTK_Interact* script
        /// </summary>
        /// <typeparam name="T">The VRTK_Interact* script type</typeparam>
        /// <param name="allowedControllers">The AllowedControllers value for this type of interaction</param>
        /// <param name="noControllersMessage">The message to use when required controllers are not present in the scene</param>
        /// <param name="missingComponentsMessage">The message to use when both controllers are missing the script</param>
        /// <param name="missingOneComponentMessage">The message to use when one controller is missing the script</param>
        protected void RequireInteractScriptOnController<T>(AllowedController allowedControllers, string noControllersMessage, string missingComponentsMessage, string missingOneComponentMessage) where T : MonoBehaviour
        {
            GameObject leftController = VRTK_DeviceFinder.GetControllerLeftHand();
            GameObject rightController = VRTK_DeviceFinder.GetControllerRightHand();

            bool areControllersMissing = false;

            switch (allowedControllers)
            {
                case AllowedController.Left_Only:
                    areControllersMissing = leftController == null;
                    rightController = null;
                    break;
                case AllowedController.Right_Only:
                    areControllersMissing = rightController == null;
                    leftController = null;
                    break;
                default:
                    areControllersMissing = leftController == null && rightController == null;
                    break;
            }

            if (areControllersMissing)
            {
                // If there are no "Controllers" they may be inserted from prefabs or another scene
                EditorGUILayout.HelpBox(noControllersMessage, MessageType.Info);
                return;
            }

            // Valid = Does not require script or has script
            bool isLeftControllerValid = leftController == null || leftController.GetComponent<T>() != null;
            bool isRightControllerValid = rightController == null || rightController.GetComponent<T>() != null;

            // If a "Controller" exists but no Interact* scripts can be found it was probably forgotten
            if (!isLeftControllerValid && !isRightControllerValid)
            {
                // Both controllers are missing the interaction script
                EditorGUILayout.HelpBox(missingComponentsMessage, MessageType.Warning);
            }
            else if (!isLeftControllerValid || !isRightControllerValid)
            {
                // One required controller is missing the interaction script
                string missingOnHand = isLeftControllerValid ? "right" : "left";
                string message = string.Format(missingOneComponentMessage, missingOnHand);
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }
        }
    }
}