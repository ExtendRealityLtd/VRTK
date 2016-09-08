namespace VRTK.Examples
{
    using UnityEngine;

    public class Sphere_Spawner : MonoBehaviour
    {
        private GameObject spawnMe;
        private Vector3 position;

        private void Start()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            GetComponent<VRTK_ControllerEvents>().TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
            GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
            spawnMe = GameObject.Find("SpawnMe");
            position = spawnMe.transform.position;
        }

        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            Instantiate(spawnMe, position, Quaternion.identity);
        }

        private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                Instantiate(spawnMe, position, Quaternion.identity);
            }
        }
    }
}