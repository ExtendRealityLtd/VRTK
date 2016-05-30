using UnityEngine;
using System.Collections;

public class VRTK_ControllerActions : MonoBehaviour {
    private bool controllerVisible = true;
    private ushort hapticPulseStrength;
    private int hapticPulseCountdown;

    private uint controllerIndex;
    private SteamVR_TrackedObject trackedController;
    private SteamVR_Controller.Device device;
    private ushort maxHapticVibration = 3999;

    public bool IsControllerVisible()
    {
        return controllerVisible;
    }

    public void ToggleControllerModel(bool on, GameObject grabbedChildObject)
    {
        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            if (renderer.gameObject != grabbedChildObject && (grabbedChildObject == null || !renderer.transform.IsChildOf(grabbedChildObject.transform)))
            {
                renderer.enabled = on;
            }
        }

        foreach (SkinnedMeshRenderer renderer in this.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (renderer.gameObject != grabbedChildObject && (grabbedChildObject == null || !renderer.transform.IsChildOf(grabbedChildObject.transform)))
            {
                renderer.enabled = on;
            }
        }
        controllerVisible = on;
    }

    public void TriggerHapticPulse(int duration, ushort strength)
    {
        hapticPulseCountdown = duration;
        hapticPulseStrength = (strength <= maxHapticVibration ? strength : maxHapticVibration);
    }

    private void Awake()
    {
        trackedController = GetComponent<SteamVR_TrackedObject>();
    }

    private void Update()
    {
        controllerIndex = (uint)trackedController.index;
        device = SteamVR_Controller.Input((int)controllerIndex);

        if (hapticPulseCountdown > 0)
        {
            device.TriggerHapticPulse(hapticPulseStrength);
            hapticPulseCountdown -= 1;
        }
    }
}
