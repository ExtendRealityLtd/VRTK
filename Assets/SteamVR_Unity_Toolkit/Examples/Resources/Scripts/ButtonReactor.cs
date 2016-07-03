using UnityEngine;
using VRTK;

public class ButtonReactor : MonoBehaviour
{
    public GameObject go;
    public Transform dispenseLocation;

    private void Start()
    {
        GetComponent<VRTK_Button>().OnPushed += handlePush;
    }

    private void handlePush()
    {
        Debug.Log("Pushed");

        GameObject newGo = (GameObject)Instantiate(go, dispenseLocation.position, Quaternion.identity);
        Destroy(newGo, 10f);
    }
}