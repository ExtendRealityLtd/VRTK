using UnityEngine;
using System.Collections;

public class VRTK_ControllerActions : MonoBehaviour {
    private bool controllerVisible = true;
    private float hapticPulseCountdown;

    private uint controllerIndex;
    private SteamVR_TrackedObject trackedController;
    private SteamVR_Controller.Device device;

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

    {
        hapticPulseCountdown = durationMilliseconds;
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
            // 1000 microseconds = 1 millisecond
            device.TriggerHapticPulse(1000);
            hapticPulseCountdown -= Time.deltaTime * 1000;
        }
    }
}