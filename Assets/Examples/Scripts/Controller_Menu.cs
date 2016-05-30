using UnityEngine;
using System.Collections;

public class Controller_Menu : MonoBehaviour {
    public GameObject menuObject;

    private GameObject clonedMenuObject;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool menuInit = false;
    private bool menuActive = false;

    void Start()
    {        
        GetComponent<VRTK_ControllerEvents>().AliasMenuOn += new ControllerClickedEventHandler(DoMenuOn);
        GetComponent<VRTK_ControllerEvents>().AliasMenuOff += new ControllerClickedEventHandler(DoMenuOff);
        menuInit = false;
        menuActive = false;
    }

    void InitMenu()
    {
        clonedMenuObject = Instantiate(menuObject, this.transform.position, Quaternion.identity) as GameObject;
        clonedMenuObject.SetActive(true);
        menuInit = true;
    }

    void DoMenuOn(object sender, ControllerClickedEventArgs e)
    {
        if (!menuInit)
        {
            InitMenu();
        }
        clonedMenuObject.SetActive(true);
        menuActive = true;
    }

    void DoMenuOff(object sender, ControllerClickedEventArgs e)
    {
        clonedMenuObject.SetActive(false);
        menuActive = false;
    }

    void Update()
    {
        if(menuActive)
        {
            clonedMenuObject.transform.rotation = this.transform.rotation;
            clonedMenuObject.transform.position = this.transform.position;
        }
    }
}
