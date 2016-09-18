namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_ControllerEvents))]
    public class VRTK_ControllerEvents_UnityEvents : MonoBehaviour
    {
        private VRTK_ControllerEvents ce;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, ControllerInteractionEventArgs> { };

        /// <summary>
        /// Emits the TriggerPressed class event.
        /// </summary>
        public UnityObjectEvent OnTriggerPressed;
        /// <summary>
        /// Emits the TriggerReleased class event.
        /// </summary>
        public UnityObjectEvent OnTriggerReleased;
        /// <summary>
        /// Emits the TriggerTouchStart class event.
        /// </summary>
        public UnityObjectEvent OnTriggerTouchStart;
        /// <summary>
        /// Emits the TriggerTouchEnd class event.
        /// </summary>
        public UnityObjectEvent OnTriggerTouchEnd;
        /// <summary>
        /// Emits the TriggerHairlineStart class event.
        /// </summary>
        public UnityObjectEvent OnTriggerHairlineStart;
        /// <summary>
        /// Emits the TriggerHairlineEnd class event.
        /// </summary>
        public UnityObjectEvent OnTriggerHairlineEnd;
        /// <summary>
        /// Emits the TriggerClicked class event.
        /// </summary>
        public UnityObjectEvent OnTriggerClicked;
        /// <summary>
        /// Emits the TriggerUnclicked class event.
        /// </summary>
        public UnityObjectEvent OnTriggerUnclicked;
        /// <summary>
        /// Emits the TriggerAxisChanged class event.
        /// </summary>
        public UnityObjectEvent OnTriggerAxisChanged;
        /// <summary>
        /// Emits the ApplicationMenuPressed class event.
        /// </summary>
        public UnityObjectEvent OnApplicationMenuPressed;
        /// <summary>
        /// Emits the ApplicationMenuReleased class event.
        /// </summary>
        public UnityObjectEvent OnApplicationMenuReleased;
        /// <summary>
        /// Emits the GripPressed class event.
        /// </summary>
        public UnityObjectEvent OnGripPressed;
        /// <summary>
        /// Emits the GripReleased class event.
        /// </summary>
        public UnityObjectEvent OnGripReleased;
        /// <summary>
        /// Emits the TouchpadPressed class event.
        /// </summary>
        public UnityObjectEvent OnTouchpadPressed;
        /// <summary>
        /// Emits the TouchpadReleased class event.
        /// </summary>
        public UnityObjectEvent OnTouchpadReleased;
        /// <summary>
        /// Emits the TouchpadTouchStart class event.
        /// </summary>
        public UnityObjectEvent OnTouchpadTouchStart;
        /// <summary>
        /// Emits the TouchpadTouchEnd class event.
        /// </summary>
        public UnityObjectEvent OnTouchpadTouchEnd;
        /// <summary>
        /// Emits the TouchpadAxisChanged class event.
        /// </summary>
        public UnityObjectEvent OnTouchpadAxisChanged;
        /// <summary>
        /// Emits the AliasPointerOn class event.
        /// </summary>
        public UnityObjectEvent OnAliasPointerOn;
        /// <summary>
        /// Emits the AliasPointerOff class event.
        /// </summary>
        public UnityObjectEvent OnAliasPointerOff;
        /// <summary>
        /// Emits the AliasPointerSet class event.
        /// </summary>
        public UnityObjectEvent OnAliasPointerSet;
        /// <summary>
        /// Emits the AliasGrabOn class event.
        /// </summary>
        public UnityObjectEvent OnAliasGrabOn;
        /// <summary>
        /// Emits the AliasGrabOff class event.
        /// </summary>
        public UnityObjectEvent OnAliasGrabOff;
        /// <summary>
        /// Emits the AliasUseOn class event.
        /// </summary>
        public UnityObjectEvent OnAliasUseOn;
        /// <summary>
        /// Emits the AliasUseOff class event.
        /// </summary>
        public UnityObjectEvent OnAliasUseOff;
        /// <summary>
        /// Emits the AliasMenuOn class event.
        /// </summary>
        public UnityObjectEvent OnAliasUIClickOn;
        /// <summary>
        /// Emits the AliasMenuOff class event.
        /// </summary>
        public UnityObjectEvent OnAliasUIClickOff;
        /// <summary>
        /// Emits the AliasUIClickOn class event.
        /// </summary>
        public UnityObjectEvent OnAliasMenuOn;
        /// <summary>
        /// Emits the AliasUIClickOff class event.
        /// </summary>
        public UnityObjectEvent OnAliasMenuOff;
        /// <summary>
        /// Emits the ControllerEnabled class event.
        /// </summary>
        public UnityObjectEvent OnControllerEnabled;
        /// <summary>
        /// Emits the ControllerDisabled class event.
        /// </summary>
        public UnityObjectEvent OnControllerDisabled;

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
            ce.ControllerEnabled += ControllerEnabled;
            ce.ControllerDisabled += ControllerDisabled;
        }

        private void TriggerPressed(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerPressed.Invoke(o, e);
        }

        private void TriggerReleased(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerReleased.Invoke(o, e);
        }

        private void TriggerTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerTouchStart.Invoke(o, e);
        }

        private void TriggerTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerTouchEnd.Invoke(o, e);
        }

        private void TriggerHairlineStart(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerHairlineStart.Invoke(o, e);
        }

        private void TriggerHairlineEnd(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerHairlineEnd.Invoke(o, e);
        }

        private void TriggerClicked(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerClicked.Invoke(o, e);
        }

        private void TriggerUnclicked(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerUnclicked.Invoke(o, e);
        }

        private void TriggerAxisChanged(object o, ControllerInteractionEventArgs e)
        {
            OnTriggerAxisChanged.Invoke(o, e);
        }

        private void ApplicationMenuPressed(object o, ControllerInteractionEventArgs e)
        {
            OnApplicationMenuPressed.Invoke(o, e);
        }

        private void ApplicationMenuReleased(object o, ControllerInteractionEventArgs e)
        {
            OnApplicationMenuReleased.Invoke(o, e);
        }

        private void GripPressed(object o, ControllerInteractionEventArgs e)
        {
            OnGripPressed.Invoke(o, e);
        }

        private void GripReleased(object o, ControllerInteractionEventArgs e)
        {
            OnGripReleased.Invoke(o, e);
        }

        private void TouchpadPressed(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadPressed.Invoke(o, e);
        }

        private void TouchpadReleased(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadReleased.Invoke(o, e);
        }

        private void TouchpadTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadTouchStart.Invoke(o, e);
        }

        private void TouchpadTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadTouchEnd.Invoke(o, e);
        }

        private void TouchpadAxisChanged(object o, ControllerInteractionEventArgs e)
        {
            OnTouchpadAxisChanged.Invoke(o, e);
        }

        private void AliasPointerOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasPointerOn.Invoke(o, e);
        }

        private void AliasPointerOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasPointerOff.Invoke(o, e);
        }

        private void AliasPointerSet(object o, ControllerInteractionEventArgs e)
        {
            OnAliasPointerSet.Invoke(o, e);
        }

        private void AliasGrabOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasGrabOn.Invoke(o, e);
        }

        private void AliasGrabOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasGrabOff.Invoke(o, e);
        }

        private void AliasUseOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasUseOn.Invoke(o, e);
        }

        private void AliasUseOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasUseOff.Invoke(o, e);
        }

        private void AliasUIClickOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasUIClickOn.Invoke(o, e);
        }

        private void AliasUIClickOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasUIClickOff.Invoke(o, e);
        }

        private void AliasMenuOn(object o, ControllerInteractionEventArgs e)
        {
            OnAliasMenuOn.Invoke(o, e);
        }

        private void AliasMenuOff(object o, ControllerInteractionEventArgs e)
        {
            OnAliasMenuOff.Invoke(o, e);
        }

        private void ControllerEnabled(object o, ControllerInteractionEventArgs e)
        {
            OnControllerEnabled.Invoke(o, e);
        }

        private void ControllerDisabled(object o, ControllerInteractionEventArgs e)
        {
            OnControllerDisabled.Invoke(o, e);
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
            ce.ControllerEnabled -= ControllerEnabled;
            ce.ControllerDisabled -= ControllerDisabled;
        }
    }
}