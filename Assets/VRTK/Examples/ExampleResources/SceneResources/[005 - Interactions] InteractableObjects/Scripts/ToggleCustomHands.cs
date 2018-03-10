namespace VRTK.Examples
{
    using UnityEngine;

    public class ToggleCustomHands : MonoBehaviour
    {
        public VRTK_ControllerEvents leftController;
        public VRTK_ControllerEvents rightController;

        public GameObject leftHandAvatar;
        public GameObject rightHandAvatar;

        protected bool state;

        protected virtual void OnEnable()
        {
            state = false;
            if (leftController != null)
            {
                leftController.ButtonTwoPressed += ToggleHands;
            }

            if (rightController != null)
            {
                rightController.ButtonTwoPressed += ToggleHands;
            }
            ToggleVisibility();
        }

        protected virtual void OnDisable()
        {
            if (leftController != null)
            {
                leftController.ButtonTwoPressed -= ToggleHands;
            }

            if (rightController != null)
            {
                rightController.ButtonTwoPressed -= ToggleHands;
            }
        }

        protected virtual void ToggleHands(object sender, ControllerInteractionEventArgs e)
        {
            state = !state;
            ToggleVisibility();
        }

        protected virtual void ToggleVisibility()
        {
            ToggleAvatarVisibility();
            ToggleSDKVisibility();
            ToggleScriptAlias();
        }

        protected virtual void ToggleAvatarVisibility()
        {
            if (leftHandAvatar != null)
            {
                leftHandAvatar.SetActive(state);
            }
            if (rightHandAvatar != null)
            {
                rightHandAvatar.SetActive(state);
            }
        }

        protected virtual void ToggleSDKVisibility()
        {
            VRTK_SDKSetup sdkType = VRTK_SDKManager.GetLoadedSDKSetup();
            if (sdkType != null)
            {
                VRTK_ControllerReference leftController = VRTK_ControllerReference.GetControllerReference(VRTK_DeviceFinder.GetControllerLeftHand(true));
                VRTK_ControllerReference rightController = VRTK_ControllerReference.GetControllerReference(VRTK_DeviceFinder.GetControllerRightHand(true));
                switch (sdkType.name)
                {
                    case "SteamVR":
                        ToggleControllerRenderer(leftController.actual, "Model");
                        ToggleControllerRenderer(rightController.actual, "Model");
                        break;
                    case "Oculus":
                        ToggleControllerRenderer(leftController.model);
                        ToggleControllerRenderer(rightController.model);
                        break;
                    case "WindowsMR":
                        ToggleControllerRenderer(leftController.model, "glTFController");
                        ToggleControllerRenderer(rightController.model, "glTFController");
                        break;
                }
            }
        }

        protected virtual void ToggleControllerRenderer(GameObject controller, string findPath = "")
        {
            if (controller != null)
            {
                if (findPath == "")
                {
                    controller.SetActive(!state);
                }
                else
                {
                    Transform childModel = controller.transform.Find(findPath);
                    if (childModel != null)
                    {
                        childModel.gameObject.SetActive(!state);
                    }
                }
            }
        }

        protected virtual void ToggleScriptAlias()
        {
            GameObject scriptLeft = VRTK_DeviceFinder.GetControllerLeftHand(false);
            GameObject scriptRight = VRTK_DeviceFinder.GetControllerRightHand(false);
            CycleScriptAlias(scriptLeft, leftHandAvatar);
            CycleScriptAlias(scriptRight, rightHandAvatar);
        }

        protected virtual void CycleScriptAlias(GameObject controller, GameObject avatar)
        {
            if (controller != null)
            {
                VRTK_InteractTouch touch = controller.GetComponentInChildren<VRTK_InteractTouch>();
                VRTK_InteractGrab grab = controller.GetComponentInChildren<VRTK_InteractGrab>();
                touch.enabled = false;
                grab.enabled = false;

                touch.customColliderContainer = null;
                grab.ForceControllerAttachPoint(null);

                if (avatar != null && state)
                {
                    touch.customColliderContainer = avatar.transform.Find("HandColliders").gameObject;
                    grab.ForceControllerAttachPoint(avatar.transform.Find("GrabAttachPoint").GetComponent<Rigidbody>());
                }
                touch.enabled = true;
                grab.enabled = true;
            }
        }
    }
}