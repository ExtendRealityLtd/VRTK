namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEventHelper;

    public class ButtonReactor : MonoBehaviour
    {
        public GameObject go;
        public Transform dispenseLocation;

        private VRTK_Button_UnityEvents buttonEvents;

        private void Start()
        {
            buttonEvents = GetComponent<VRTK_Button_UnityEvents>();
            if (buttonEvents == null)
            {
                buttonEvents = gameObject.AddComponent<VRTK_Button_UnityEvents>();
            }
            buttonEvents.OnPushed.AddListener(handlePush);
        }

        private void handlePush(object sender, Control3DEventArgs e)
        {
            Debug.Log("Pushed");

            GameObject newGo = (GameObject)Instantiate(go, dispenseLocation.position, Quaternion.identity);
            Destroy(newGo, 10f);
        }
    }
}