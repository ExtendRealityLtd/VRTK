using UnityEngine;
using UnityEngine.Events;
using VRTK;

[RequireComponent(typeof(VRTK_ControllerEvents))]
public class VRTK_ControllerEvents_UnityEvents : MonoBehaviour
{
    private VRTK_ControllerEvents ce;

    [System.Serializable]
    public class UnityObjectEvent : UnityEvent<ControllerInteractionEventArgs> { };
    public UnityObjectEvent OnTriggerPressed;
    public UnityObjectEvent OnTriggerReleased;
    public UnityObjectEvent OnTriggerTouchStart;
    public UnityObjectEvent OnTriggerTouchEnd;
    public UnityObjectEvent OnTriggerHairlineStart;
    public UnityObjectEvent OnTriggerHairlineEnd;
    public UnityObjectEvent OnTriggerClicked;
    public UnityObjectEvent OnTriggerUnclicked;
    public UnityObjectEvent OnTriggerAxisChanged;
    public UnityObjectEvent OnApplicationMenuPressed;
    public UnityObjectEvent OnApplicationMenuReleased;
    public UnityObjectEvent OnGripPressed;
    public UnityObjectEvent OnGripReleased;
    public UnityObjectEvent OnTouchpadPressed;
    public UnityObjectEvent OnTouchpadReleased;
    public UnityObjectEvent OnTouchpadTouchStart;
    public UnityObjectEvent OnTouchpadTouchEnd;
    public UnityObjectEvent OnTouchpadAxisChanged;
    public UnityObjectEvent OnAliasPointerOn;
    public UnityObjectEvent OnAliasPointerOff;
    public UnityObjectEvent OnAliasPointerSet;
    public UnityObjectEvent OnAliasGrabOn;
    public UnityObjectEvent OnAliasGrabOff;
    public UnityObjectEvent OnAliasUseOn;
    public UnityObjectEvent OnAliasUseOff;
    public UnityObjectEvent OnAliasUIClickOn;
    public UnityObjectEvent OnAliasUIClickOff;
    public UnityObjectEvent OnAliasMenuOn;
    public UnityObjectEvent OnAliasMenuOff;

    private void SetControllerEvents()
    {
        if (ce == null)
        {
            ce = GetComponent<VRTK_ControllerEvents>();
        }
    }

    private void OnEnable()
    {
        SetControllerEvents();
        if (ce == null)
        {
            Debug.LogError("The VRTK_ControllerEvents_UnityEvents script requires to be attached to a GameObject that contains a VRTK_ControllerEvents script");
            return;
        }

        ce.TriggerPressed += TriggerPressed;
        ce.TriggerReleased += TriggerReleased;
        ce.TriggerTouchStart += TriggerTouchStart;
        ce.TriggerTouchEnd += TriggerTouchEnd;
        ce.TriggerHairlineStart += TriggerHairlineStart;
        ce.TriggerHairlineEnd += TriggerHairlineEnd;
        ce.TriggerClicked += TriggerClicked;
        ce.TriggerUnclicked += TriggerUnclicked;
        ce.TriggerAxisChanged += TriggerAxisChanged;
        ce.ApplicationMenuPressed += ApplicationMenuPressed;
        ce.ApplicationMenuReleased += ApplicationMenuReleased;
        ce.GripPressed += GripPressed;
        ce.GripReleased += GripReleased;
        ce.TouchpadPressed += TouchpadPressed;
        ce.TouchpadReleased += TouchpadReleased;
        ce.TouchpadTouchStart += TouchpadTouchStart;
        ce.TouchpadTouchEnd += TouchpadTouchEnd;
        ce.TouchpadAxisChanged += TouchpadAxisChanged;
        ce.AliasPointerOn += AliasPointerOn;
        ce.AliasPointerOff += AliasPointerOff;
        ce.AliasPointerSet += AliasPointerSet;
        ce.AliasGrabOn += AliasGrabOn;
        ce.AliasGrabOff += AliasGrabOff;
        ce.AliasUseOn += AliasUseOn;
        ce.AliasUseOff += AliasUseOff;
        ce.AliasUIClickOn += AliasUIClickOn;
        ce.AliasUIClickOff += AliasUIClickOff;
        ce.AliasMenuOn += AliasMenuOn;
        ce.AliasMenuOff += AliasMenuOff;
    }

    private void TriggerPressed(object o, ControllerInteractionEventArgs e)
    {
        OnTriggerPressed.Invoke(e);
    }

    private void TriggerReleased(object o, ControllerInteractionEventArgs e)
    {
        OnTriggerReleased.Invoke(e);
    }

    private void TriggerTouchStart(object o, ControllerInteractionEventArgs e)
    {
        OnTriggerTouchStart.Invoke(e);
    }

    private void TriggerTouchEnd(object o, ControllerInteractionEventArgs e)
    {
        OnTriggerTouchEnd.Invoke(e);
    }

    private void TriggerHairlineStart(object o, ControllerInteractionEventArgs e)
    {
        OnTriggerHairlineStart.Invoke(e);
    }

    private void TriggerHairlineEnd(object o, ControllerInteractionEventArgs e)
    {
        OnTriggerHairlineEnd.Invoke(e);
    }

    private void TriggerClicked(object o, ControllerInteractionEventArgs e)
    {
        OnTriggerClicked.Invoke(e);
    }

    private void TriggerUnclicked(object o, ControllerInteractionEventArgs e)
    {
        OnTriggerUnclicked.Invoke(e);
    }

    private void TriggerAxisChanged(object o, ControllerInteractionEventArgs e)
    {
        OnTriggerAxisChanged.Invoke(e);
    }

    private void ApplicationMenuPressed(object o, ControllerInteractionEventArgs e)
    {
        OnApplicationMenuPressed.Invoke(e);
    }

    private void ApplicationMenuReleased(object o, ControllerInteractionEventArgs e)
    {
        OnApplicationMenuReleased.Invoke(e);
    }

    private void GripPressed(object o, ControllerInteractionEventArgs e)
    {
        OnGripPressed.Invoke(e);
    }

    private void GripReleased(object o, ControllerInteractionEventArgs e)
    {
        OnGripReleased.Invoke(e);
    }

    private void TouchpadPressed(object o, ControllerInteractionEventArgs e)
    {
        OnTouchpadPressed.Invoke(e);
    }

    private void TouchpadReleased(object o, ControllerInteractionEventArgs e)
    {
        OnTouchpadReleased.Invoke(e);
    }

    private void TouchpadTouchStart(object o, ControllerInteractionEventArgs e)
    {
        OnTouchpadTouchStart.Invoke(e);
    }

    private void TouchpadTouchEnd(object o, ControllerInteractionEventArgs e)
    {
        OnTouchpadTouchEnd.Invoke(e);
    }

    private void TouchpadAxisChanged(object o, ControllerInteractionEventArgs e)
    {
        OnTouchpadAxisChanged.Invoke(e);
    }

    private void AliasPointerOn(object o, ControllerInteractionEventArgs e)
    {
        OnAliasPointerOn.Invoke(e);
    }

    private void AliasPointerOff(object o, ControllerInteractionEventArgs e)
    {
        OnAliasPointerOff.Invoke(e);
    }

    private void AliasPointerSet(object o, ControllerInteractionEventArgs e)
    {
        OnAliasPointerSet.Invoke(e);
    }

    private void AliasGrabOn(object o, ControllerInteractionEventArgs e)
    {
        OnAliasGrabOn.Invoke(e);
    }

    private void AliasGrabOff(object o, ControllerInteractionEventArgs e)
    {
        OnAliasGrabOff.Invoke(e);
    }

    private void AliasUseOn(object o, ControllerInteractionEventArgs e)
    {
        OnAliasUseOn.Invoke(e);
    }

    private void AliasUseOff(object o, ControllerInteractionEventArgs e)
    {
        OnAliasUseOff.Invoke(e);
    }

    private void AliasUIClickOn(object o, ControllerInteractionEventArgs e)
    {
        OnAliasUIClickOn.Invoke(e);
    }

    private void AliasUIClickOff(object o, ControllerInteractionEventArgs e)
    {
        OnAliasUIClickOff.Invoke(e);
    }

    private void AliasMenuOn(object o, ControllerInteractionEventArgs e)
    {
        OnAliasMenuOn.Invoke(e);
    }

    private void AliasMenuOff(object o, ControllerInteractionEventArgs e)
    {
        OnAliasMenuOff.Invoke(e);
    }

    private void OnDisable()
    {
        if (ce == null)
        {
            return;
        }

        ce.TriggerPressed -= TriggerPressed;
        ce.TriggerReleased -= TriggerReleased;
        ce.TriggerTouchStart -= TriggerTouchStart;
        ce.TriggerTouchEnd -= TriggerTouchEnd;
        ce.TriggerHairlineStart -= TriggerHairlineStart;
        ce.TriggerHairlineEnd -= TriggerHairlineEnd;
        ce.TriggerClicked -= TriggerClicked;
        ce.TriggerUnclicked -= TriggerUnclicked;
        ce.TriggerAxisChanged -= TriggerAxisChanged;
        ce.ApplicationMenuPressed -= ApplicationMenuPressed;
        ce.ApplicationMenuReleased -= ApplicationMenuReleased;
        ce.GripPressed -= GripPressed;
        ce.GripReleased -= GripReleased;
        ce.TouchpadPressed -= TouchpadPressed;
        ce.TouchpadReleased -= TouchpadReleased;
        ce.TouchpadTouchStart -= TouchpadTouchStart;
        ce.TouchpadTouchEnd -= TouchpadTouchEnd;
        ce.TouchpadAxisChanged -= TouchpadAxisChanged;
        ce.AliasPointerOn -= AliasPointerOn;
        ce.AliasPointerOff -= AliasPointerOff;
        ce.AliasPointerSet -= AliasPointerSet;
        ce.AliasGrabOn -= AliasGrabOn;
        ce.AliasGrabOff -= AliasGrabOff;
        ce.AliasUseOn -= AliasUseOn;
        ce.AliasUseOff -= AliasUseOff;
        ce.AliasUIClickOn -= AliasUIClickOn;
        ce.AliasUIClickOff -= AliasUIClickOff;
        ce.AliasMenuOn -= AliasMenuOn;
        ce.AliasMenuOff -= AliasMenuOff;
    }
}