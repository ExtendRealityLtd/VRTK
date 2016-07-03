using UnityEngine;
using System.Collections;
using VRTK;

public class Controller_Hand : MonoBehaviour
{
    public enum Hands
    {
        Right,
        Left
    }

    public Hands hand = Hands.Right;

    private Transform pointerFinger;
    private Transform gripFingers;
    private float maxRotation = 90f;
    private float originalPointerRotation;
    private float originalGripRotation;
    private float targetPointerRotation;
    private float targetGripRotation;

    private void Start()
    {
        this.GetComponentInParent<VRTK_ControllerEvents>().AliasGrabOn += new ControllerInteractionEventHandler(DoGrabOn);
        this.GetComponentInParent<VRTK_ControllerEvents>().AliasGrabOff += new ControllerInteractionEventHandler(DoGrabOff);
        this.GetComponentInParent<VRTK_ControllerEvents>().AliasUseOn += new ControllerInteractionEventHandler(DoUseOn);
        this.GetComponentInParent<VRTK_ControllerEvents>().AliasUseOff += new ControllerInteractionEventHandler(DoUseOff);

        pointerFinger = this.transform.Find("Container/PointerFingerContainer");
        gripFingers = this.transform.Find("Container/GripFingerContainer");

        if (hand == Hands.Left)
        {
            InversePosition(pointerFinger);
            InversePosition(gripFingers);
            InversePosition(this.transform.Find("Container/Palm"));
            InversePosition(this.transform.Find("Container/Thumb"));
        }

        originalPointerRotation = pointerFinger.localEulerAngles.y;
        originalGripRotation = gripFingers.localEulerAngles.y;

        targetPointerRotation = originalPointerRotation;
        targetGripRotation = originalGripRotation;
    }

    private void InversePosition(Transform givenTransform)
    {
        givenTransform.localPosition = new Vector3(givenTransform.localPosition.x * -1, givenTransform.localPosition.y, givenTransform.localPosition.z);
        givenTransform.localEulerAngles = new Vector3(givenTransform.localEulerAngles.x, givenTransform.localEulerAngles.y * -1, givenTransform.localEulerAngles.z);
    }

    private void DoGrabOn(object sender, ControllerInteractionEventArgs e)
    {
        targetGripRotation = maxRotation;
    }

    private void DoGrabOff(object sender, ControllerInteractionEventArgs e)
    {
        targetGripRotation = originalGripRotation;
    }

    private void DoUseOn(object sender, ControllerInteractionEventArgs e)
    {
        targetPointerRotation = maxRotation;
    }

    private void DoUseOff(object sender, ControllerInteractionEventArgs e)
    {
        targetPointerRotation = originalPointerRotation;
    }

    private void Update()
    {
        pointerFinger.localEulerAngles = new Vector3(targetPointerRotation, 0f, 0f);
        gripFingers.localEulerAngles = new Vector3(targetGripRotation, 0f, 0f);
    }
}
