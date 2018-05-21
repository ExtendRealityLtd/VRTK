namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using ControllerHand = SDK_BaseController.ControllerHand;
    using AllowedController = VRTK_InteractableObject.AllowedController;

    [CustomEditor(typeof(VRTK_ControllerActions), true)]
    public class VRTK_ControllerActionsEditor : Editor
    {
        protected GameObject controller;

        private void OnEnable()
        {
            controller = ((VRTK_ControllerActions)target).gameObject;
        }

        public override void OnInspectorGUI()
        {
            // Only check for interactable objects that apply to this controller hand
            var hand = VRTK_DeviceFinder.GetControllerHand(controller);
            Func<AllowedController, bool> isHand = (AllowedController allowedController) =>
            {
                if (allowedController == AllowedController.Both)
                {
                    return true;
                }
                if (hand == ControllerHand.Left && allowedController == AllowedController.Left_Only)
                {
                    return true;
                }
                if (hand == ControllerHand.Right && allowedController == AllowedController.Right_Only)
                {
                    return true;
                }
                return false;
            };

            if (controller.GetComponent<VRTK_InteractTouch>() == null
                && DoesSceneContainInteractableObjects((interactableObject) => isHand(interactableObject.allowedTouchControllers)))
            {
                EditorGUILayout.HelpBox("VRTK_InteractTouch is missing, controller will be unable to touch interactable objects", MessageType.Warning);
            }

            if (controller.GetComponent<VRTK_InteractGrab>() == null
                && DoesSceneContainInteractableObjects((interactableObject) => interactableObject.isGrabbable && isHand(interactableObject.allowedGrabControllers)))
            {
                EditorGUILayout.HelpBox("VRTK_InteractGrab is missing, controller will be unable to grab interactable objects", MessageType.Warning);
            }

            if (controller.GetComponent<VRTK_InteractUse>() == null
                && DoesSceneContainInteractableObjects((interactableObject) => interactableObject.isUsable && isHand(interactableObject.allowedUseControllers)))
            {
                EditorGUILayout.HelpBox("VRTK_InteractUse is missing, controller will be unable to use interactable objects", MessageType.Warning);
            }

            DrawDefaultInspector();
        }

        /// <summary>
        /// Check the scene for the presence of any `VRTK_InteractableObject` components.
        /// </summary>
        /// <param name="filter">A lambda that may return false to exclude an interactable object from the search</param>
        /// <returns>Whether the scene contains any objects that are interactable and pass the filter</returns>
        protected bool DoesSceneContainInteractableObjects(Func<VRTK_InteractableObject, bool> filter)
        {
            VRTK_InteractableObject[] interactableObjects = FindObjectsOfType<VRTK_InteractableObject>();
            foreach (var interactableObject in interactableObjects)
            {
                if (!filter(interactableObject))
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}