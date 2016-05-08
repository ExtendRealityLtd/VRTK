using UnityEngine;
using System.Collections;

public class Sphere_Spawner : MonoBehaviour {
    private GameObject spawnMe;
    private Vector3 position;

	void Start () {
        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_ControllerEvents_ListenerExample is required to be attached to a SteamVR Controller that has the SteamVR_ControllerEvents script attached to it");
            return;
        }

        GetComponent<SteamVR_ControllerEvents>().TriggerClicked += new ControllerClickedEventHandler(DoTriggerClicked);
        GetComponent<SteamVR_ControllerEvents>().TouchpadClicked += new ControllerClickedEventHandler(DoTouchpadClicked);
        spawnMe = GameObject.Find("SpawnMe");
        position = spawnMe.transform.position;
    }

    void DoTriggerClicked(object sender, ControllerClickedEventArgs e)
    {
        Instantiate(spawnMe, position, Quaternion.identity);
    }

    void DoTouchpadClicked(object sender, ControllerClickedEventArgs e)
    {
        for (int i = 0; i < 20; i++)
        {
            Instantiate(spawnMe, position, Quaternion.identity);
        }
    }
}
