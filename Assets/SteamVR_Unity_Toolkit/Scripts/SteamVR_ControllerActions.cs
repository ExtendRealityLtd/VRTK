using UnityEngine;
using System.Collections;

public class SteamVR_ControllerActions : MonoBehaviour {
    bool controllerVisible = true;

    public bool IsControllerVisible()
    {
        return controllerVisible;
    }

    public void ToggleControllerModel(bool on)
    {
        foreach (MeshRenderer renderer in this.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = on;
        }
        controllerVisible = on;
    }
}
