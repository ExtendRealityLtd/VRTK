using UnityEngine;
using System.Collections;
using VRTK;

public class Openable_Door : VRTK_InteractableObject
{
    public bool flipped = false;
    public bool rotated = false;
    float sideFlip = -1;
    float side = -1;
    float smooth = 270.0f;
    float doorOpenAngle = -90f;
    bool open = false;

    Vector3 defaultRotation;
    Vector3 openRotation;

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);
        SetDoorRotation(usingObject.transform.position);
        SetRotation();
        open = !open;
    }

    //Ensure to override the start method and call the parent start method
    protected override void Start () {
        base.Start();
        defaultRotation = transform.eulerAngles;
        SetRotation();
        sideFlip = (flipped ? 1 : -1);
    }

    void SetRotation()
    {
        openRotation = new Vector3(defaultRotation.x, defaultRotation.y + (doorOpenAngle * (sideFlip * side)), defaultRotation.z);
    }

    void SetDoorRotation(Vector3 interacterPosition)
    {
        side = ((rotated == false && interacterPosition.z > transform.position.z) || (rotated == true && interacterPosition.x > transform.position.x) ? -1 : 1);
    }

    protected override void Update () {
        if (open)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(openRotation), Time.deltaTime * smooth);
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(defaultRotation), Time.deltaTime * smooth);
        }
    }
}
