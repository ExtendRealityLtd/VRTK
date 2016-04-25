//=====================================================================================
//
// Purpose: Provide a mechanism for determining if a game world object is interactable
//
// This script should be attached to any object the player is required to grab or touch
//
// An optional highlight color can be set to change the object's appearance if it is
// invoked.
//
//=====================================================================================

using UnityEngine;
using System.Collections;

public class SteamVR_InteractableObject : MonoBehaviour
{
    public bool isGrabbable = true;
    public bool highlightOnTouch = true;
    public Color touchHighlightColor;

    private bool isGrabbed = false;
    private Color[] originalObjectColours;

    void Start()
    {
        originalObjectColours = StoreOriginalColors();
    }

    Renderer[] GetRendererArray()
    {
        return (GetComponents<Renderer>().Length > 0 ? GetComponents<Renderer>() : GetComponentsInChildren<Renderer>());
    }

    Color[] StoreOriginalColors()
    {
        Renderer[] rendererArray = GetRendererArray();
        int length = rendererArray.Length;
        Color[] colors = new Color[length];

        for (int i = 0; i < length; i++)
        {
            var renderer = rendererArray[i];
            colors[i] = renderer.material.color;
        }
        return colors;
    }

    Color[] BuildHighlightColorArray(Color color)
    {
        Renderer[] rendererArray = GetRendererArray();
        int length = rendererArray.Length;
        Color[] colors = new Color[length];

        for (int i = 0; i < length; i++)
        {
            colors[i] = color;
        }
        return colors;
    }

    void ChangeColor(Color[] colors)
    {
        Renderer[] rendererArray = GetRendererArray();
        int i = 0;
        foreach (Renderer renderer in rendererArray)
        {
            renderer.material.color = colors[i];
            i++;
        }
    }

    public bool IsGrabbed()
    {
        return isGrabbed;
    }

    public void OnGrab(GameObject grabbingObject)
    {
        isGrabbed = true;
    }

    public void OnUngrab(GameObject previousGrabbingObject)
    {
        isGrabbed = false;
    }

    public void ToggleHighlight(bool toggle)
    {
        if(highlightOnTouch)
        {
            if (toggle)
            {
                var colorArray = BuildHighlightColorArray(touchHighlightColor);
                ChangeColor(colorArray);
            } else
            {
                ChangeColor(originalObjectColours);
            }
        }
    }
}