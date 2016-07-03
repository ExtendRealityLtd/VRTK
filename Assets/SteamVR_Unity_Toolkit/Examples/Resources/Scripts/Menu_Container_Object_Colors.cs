using UnityEngine;
using System.Collections;
using VRTK;

public class Menu_Container_Object_Colors : VRTK_InteractableObject
{
    public void SetSelectedColor(Color color)
    {
        foreach(Menu_Object_Spawner menuObjectSpawner in this.gameObject.GetComponentsInChildren<Menu_Object_Spawner>())
        {
            menuObjectSpawner.SetSelectedColor(color);
        }
    }

    protected override void Start()
    {
        base.Start();
        SetSelectedColor(Color.red);
    }
}