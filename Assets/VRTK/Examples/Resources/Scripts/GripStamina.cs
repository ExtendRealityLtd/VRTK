using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using VRTK;

public class GripStamina : MonoBehaviour {

    public enum Orientation
    {
        ClockWise,
        CounterClockWise    
    }

    public VRTK_PlayerClimbStamina staminaComponent;
    public Sprite defaultImage;    
    public int numIcons = 5;
    public bool rotateIcons = true;
    public bool isShown = true;
    public Orientation orientation = Orientation.CounterClockWise;

    private List<GameObject> gripIcons = new List<GameObject>();
    private Color iconColor = Color.green;
    private float normalizedCurrentStamina;
    
    private void Awake()
    {
        if (Application.isPlaying)
        {
            if (!isShown)
            {
                transform.localScale = Vector3.zero;
            }
            GenerateIcons();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        normalizedCurrentStamina = Mathf.Clamp01(staminaComponent.CurrentStamina / staminaComponent.maxStamina);
        if (normalizedCurrentStamina < 1f)
        {
            transform.localScale = Vector3.one;
            RefreshIcons();
        } else
        {
            transform.localScale = Vector3.zero;
        }
    }

    private void RefreshIcons()
    {
        // Update Icon Color
        iconColor.r = 2 * (1 - normalizedCurrentStamina);
        iconColor.g = 2 * (normalizedCurrentStamina);
        iconColor.b = 0f;

        // Update Icons
        for (int i = 0; i < gripIcons.Count; i++)
        {
            GameObject icon = gripIcons[i];
            icon.GetComponent<Image>().color = iconColor;

            //Integer division caused issues here, converting to float fixed it
            float index = (float)i;
            if (normalizedCurrentStamina >= (index + 1f) / numIcons)
            {
                icon.transform.localScale = Vector3.one / (numIcons * 1.5f);
            }
            else
            {                
                float percentScale = Mathf.Clamp01((normalizedCurrentStamina - index / numIcons) / ((index + 1f) / numIcons - index / numIcons));
                icon.transform.localScale = Vector3.one / (numIcons * 1.5f) * percentScale;
            }
        }
    }

    //Creates all the button Arcs and populates them with desired icons
    private void GenerateIcons()
    {        
        for (int i = 0; i < numIcons; i++)
        {
            // Initial placement/instantiation
            GameObject newButton = new GameObject();
            newButton.layer = 4;

            Image image = newButton.AddComponent<Image>();
            image.sprite= defaultImage;
            image.color = iconColor;

            newButton.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            newButton.GetComponent<RectTransform>().offsetMin = Vector2.zero;

            newButton.transform.SetParent(transform);
            newButton.transform.localScale = Vector3.one / (numIcons * 1.5f);
            
            float angle = ((180 / numIcons) * i);
            newButton.transform.localEulerAngles = new Vector3(0, 0, angle);

            if (orientation == Orientation.ClockWise)
            {
                angle *= -1;                 
            }

            float angleRad = (angle * Mathf.PI) / 180f;
            Vector2 angleVector = new Vector2(-Mathf.Cos(angleRad), -Mathf.Sin(angleRad));
            newButton.transform.localPosition = Vector3.zero + (Vector3)angleVector * 100f;            

            newButton.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            newButton.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            
            //Rotate icons all vertically if desired
            if (!rotateIcons)
            {
                newButton.transform.eulerAngles = GetComponentInParent<Canvas>().transform.eulerAngles;
            }            
            
            gripIcons.Add(newButton);
        }
    }    
}
