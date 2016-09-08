namespace VRTK.Examples
{
    using UnityEngine;

    public class ButtonReactor : MonoBehaviour
    {
        public GameObject go;
        public Transform dispenseLocation;

        private void Start()
        {
            GetComponent<VRTK_Button>().events.OnPush.AddListener(handlePush);
        }

        private void handlePush()
        {
            Debug.Log("Pushed");

            GameObject newGo = (GameObject)Instantiate(go, dispenseLocation.position, Quaternion.identity);
            Destroy(newGo, 10f);
        }
    }
}