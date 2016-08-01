using UnityEngine;
using VRTK;

[ExecuteInEditMode]
public class Setup_Scene_034 : MonoBehaviour
{
    private bool initalised = false;
#if UNITY_EDITOR
    private void Update()
    {
        if (!initalised)
        {
            var headset = VRTK_DeviceFinder.HeadsetTransform();
            var controllerManager = FindObjectOfType<SteamVR_ControllerManager>();
            var controllerEvents = controllerManager.left.GetComponent<VRTK_ControllerEvents>();

            if (!headset.GetComponent<VRTK_SimplePointer>())
            {
                var pointer = headset.gameObject.AddComponent<VRTK_SimplePointer>();

                pointer.controller = controllerEvents;
                pointer.enableTeleport = false;
                pointer.pointerVisibility = VRTK_WorldPointer.pointerVisibilityStates.Always_Off;
                pointer.pointerLength = 100f;
                pointer.showPointerTip = true;
            }

            if (!headset.GetComponent<VRTK_UIPointer>())
            {
                var uiPointer = headset.gameObject.AddComponent<VRTK_UIPointer>();
                uiPointer.controller = controllerEvents;
                uiPointer.ignoreCanvasWithTagOrClass = "ExcludeTeleport";
            }
            initalised = true;
        }
    }
#endif
}