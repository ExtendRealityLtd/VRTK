using UnityEngine;
using System.Collections;

public class Menu_Container_Object_Colors : SteamVR_InteractableObject
{
    private Color selectedColor;

    protected override void Start()
    {
        base.Start();
        SetSelectedColor(Color.red);
    }

    public void SetSelectedColor(Color color)
    {
        foreach(Menu_Object_Spawner menuObjectSpawner in this.gameObject.GetComponentsInChildren<Menu_Object_Spawner>())
        {
            menuObjectSpawner.SetSelectedColor(color);
        }
    }
}
