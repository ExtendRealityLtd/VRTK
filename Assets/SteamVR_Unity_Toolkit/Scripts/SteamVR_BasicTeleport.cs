//====================================================================================
//
// Purpose: Provide basic teleportation of VR CameraRig
//
// This script must be attached to the [CameraRig] Prefab
//
// A GameObject must have the SteamVR_WorldPointer attached to it to listen for the
// updated world position to teleport to.
//
//====================================================================================

using UnityEngine;
using System.Collections;

public class SteamVR_BasicTeleport : MonoBehaviour {
    public float blinkTransitionSpeed = 0.6f;

    private int listenerInitTries = 5;
    private Transform eyeCamera;

    void Start()
    {
        InitPointerListeners();
        eyeCamera = this.GetComponentInChildren<SteamVR_GameView>().GetComponent<Transform>();
    }

    void InitPointerListeners()
    {
        SteamVR_WorldPointer[] worldPointers = GameObject.FindObjectsOfType<SteamVR_WorldPointer>();

        // If the WorldPointer Object isn't initialised yet then retry in a quarter of a second
        // Because the Controller is a child of the CameraRig (and the WorldPointer is usually attached
        // to the Controller) then it is likely the WorldPointer object isn't available at start.
        if (worldPointers.Length == 0)
        {
            if (listenerInitTries > 0)
            {
                Invoke("InitPointerListeners", 0.25f);
            } else
            {
                Debug.LogError("A GameObject must exist with a SteamVR_WorldPointer script attached to it");
                return;
            }
        }

        foreach (SteamVR_WorldPointer worldPointer in worldPointers)
        {
            worldPointer.WorldPointerDestinationSet += new WorldPointerEventHandler(DoTeleport);
        }
    }

    void DoTeleport(object sender, WorldPointerEventArgs e)
    {
        SteamVR_Fade.Start(Color.black, 0);
        SteamVR_Fade.Start(Color.clear, blinkTransitionSpeed);
        this.transform.position = new Vector3(e.tipPosition.x - eyeCamera.localPosition.x, this.transform.position.y, e.tipPosition.z - eyeCamera.localPosition.z);
    }
}
