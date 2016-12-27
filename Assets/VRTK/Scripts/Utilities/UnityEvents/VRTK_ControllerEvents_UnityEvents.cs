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
        /// Emits the GripPressed class event.
        /// </summary>
        public UnityObjectEvent OnGripPressed;
        /// <summary>
        /// Emits the GripReleased class event.
        /// </summary>
        public UnityObjectEvent OnGripReleased;
        /// <summary>
        /// Emits the GripTouchStart class event.
        /// </summary>
        public UnityObjectEvent OnGripTouchStart;
        /// <summary>
        /// Emits the GripTouchEnd class event.
        /// </summary>
        public UnityObjectEvent OnGripTouchEnd;
        /// <summary>
        /// Emits the GripHairlineStart class event.
        /// </summary>
        public UnityObjectEvent OnGripHairlineStart;
        /// <summary>
        /// Emits the GripHairlineEnd class event.
        /// </summary>
        public UnityObjectEvent OnGripHairlineEnd;
        /// <summary>
        /// Emits the GripClicked class event.
        /// </summary>
        public UnityObjectEvent OnGripClicked;
        /// <summary>
        /// Emits the GripUnclicked class event.
        /// </summary>
        public UnityObjectEvent OnGripUnclicked;
        /// <summary>
        /// Emits the GripAxisChanged class event.
        /// </summary>
        public UnityObjectEvent OnGripAxisChanged;

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
        /// Emits the ButtonOnePressed class event.
        /// </summary>
        public UnityObjectEvent OnButtonOnePressed;
        /// <summary>
        /// Emits the ButtonOneReleased class event.
        /// </summary>
        public UnityObjectEvent OnButtonOneReleased;
        /// <summary>
        /// Emits the ButtonOneTouchStart class event.
        /// </summary>
        public UnityObjectEvent OnButtonOneTouchStart;
        /// <summary>
        /// Emits the ButtonOneTouchEnd class event.
        /// </summary>
        public UnityObjectEvent OnButtonOneTouchEnd;

        /// <summary>
        /// Emits the ButtonTwoPressed class event.
        /// </summary>
        public UnityObjectEvent OnButtonTwoPressed;
        /// <summary>
        /// Emits the ButtonTwoReleased class event.
        /// </summary>
        public UnityObjectEvent OnButtonTwoReleased;
        /// <summary>
        /// Emits the ButtonTwoTouchStart class event.
        /// </summary>
        public UnityObjectEvent OnButtonTwoTouchStart;
        /// <summary>
        /// Emits the ButtonTwoTouchEnd class event.
        /// </summary>
        public UnityObjectEvent OnButtonTwoTouchEnd;

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
        /// <summary>
        /// Emits the ControllerIndexChanged class event.
        /// </summary>
        public UnityObjectEvent OnControllerIndexChanged;

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

            ce.GripPressed += GripPressed;
            ce.GripReleased += GripReleased;
            ce.GripTouchStart += GripTouchStart;
            ce.GripTouchEnd += GripTouchEnd;
            ce.GripHairlineStart += GripHairlineStart;
            ce.GripHairlineEnd += GripHairlineEnd;
            ce.GripClicked += GripClicked;
            ce.GripUnclicked += GripUnclicked;
            ce.GripAxisChanged += GripAxisChanged;

            ce.TouchpadPressed += TouchpadPressed;
            ce.TouchpadReleased += TouchpadReleased;
            ce.TouchpadTouchStart += TouchpadTouchStart;
            ce.TouchpadTouchEnd += TouchpadTouchEnd;
            ce.TouchpadAxisChanged += TouchpadAxisChanged;

            ce.ButtonOnePressed += ButtonOnePressed;
            ce.ButtonOneReleased += ButtonOneReleased;
            ce.ButtonOneTouchStart += ButtonOneTouchStart;
            ce.ButtonOneTouchEnd += ButtonOneTouchEnd;

            ce.ButtonTwoPressed += ButtonTwoPressed;
            ce.ButtonTwoReleased += ButtonTwoReleased;
            ce.ButtonTwoTouchStart += ButtonTwoTouchStart;
            ce.ButtonTwoTouchEnd += ButtonTwoTouchEnd;

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
            ce.ControllerIndexChanged += ControllerIndexChanged;
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

        private void GripPressed(object o, ControllerInteractionEventArgs e)
        {
            OnGripPressed.Invoke(o, e);
        }

        private void GripReleased(object o, ControllerInteractionEventArgs e)
        {
            OnGripReleased.Invoke(o, e);
        }

        private void GripTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnGripTouchStart.Invoke(o, e);
        }

        private void GripTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnGripTouchEnd.Invoke(o, e);
        }

        private void GripHairlineStart(object o, ControllerInteractionEventArgs e)
        {
            OnGripHairlineStart.Invoke(o, e);
        }

        private void GripHairlineEnd(object o, ControllerInteractionEventArgs e)
        {
            OnGripHairlineEnd.Invoke(o, e);
        }

        private void GripClicked(object o, ControllerInteractionEventArgs e)
        {
            OnGripClicked.Invoke(o, e);
        }

        private void GripUnclicked(object o, ControllerInteractionEventArgs e)
        {
            OnGripUnclicked.Invoke(o, e);
        }

        private void GripAxisChanged(object o, ControllerInteractionEventArgs e)
        {
            OnGripAxisChanged.Invoke(o, e);
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

        private void ButtonOnePressed(object o, ControllerInteractionEventArgs e)
        {
            OnButtonOnePressed.Invoke(o, e);
        }

        private void ButtonOneReleased(object o, ControllerInteractionEventArgs e)
        {
            OnButtonOneReleased.Invoke(o, e);
        }

        private void ButtonOneTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnButtonOneTouchStart.Invoke(o, e);
        }

        private void ButtonOneTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnButtonOneTouchEnd.Invoke(o, e);
        }

        private void ButtonTwoPressed(object o, ControllerInteractionEventArgs e)
        {
            OnButtonTwoPressed.Invoke(o, e);
        }

        private void ButtonTwoReleased(object o, ControllerInteractionEventArgs e)
        {
            OnButtonTwoReleased.Invoke(o, e);
        }

        private void ButtonTwoTouchStart(object o, ControllerInteractionEventArgs e)
        {
            OnButtonTwoTouchStart.Invoke(o, e);
        }

        private void ButtonTwoTouchEnd(object o, ControllerInteractionEventArgs e)
        {
            OnButtonTwoTouchEnd.Invoke(o, e);
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

        private void ControllerIndexChanged(object o, ControllerInteractionEventArgs e)
        {
            OnControllerIndexChanged.Invoke(o, e);
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

            ce.GripPressed -= GripPressed;
            ce.GripReleased -= GripReleased;
            ce.GripTouchStart -= GripTouchStart;
            ce.GripTouchEnd -= GripTouchEnd;
            ce.GripHairlineStart -= GripHairlineStart;
            ce.GripHairlineEnd -= GripHairlineEnd;
            ce.GripClicked -= GripClicked;
            ce.GripUnclicked -= GripUnclicked;
            ce.GripAxisChanged -= GripAxisChanged;

            ce.TouchpadPressed -= TouchpadPressed;
            ce.TouchpadReleased -= TouchpadReleased;
            ce.TouchpadTouchStart -= TouchpadTouchStart;
            ce.TouchpadTouchEnd -= TouchpadTouchEnd;
            ce.TouchpadAxisChanged -= TouchpadAxisChanged;

            ce.ButtonOnePressed -= ButtonOnePressed;
            ce.ButtonOneReleased -= ButtonOneReleased;
            ce.ButtonOneTouchStart -= ButtonOneTouchStart;
            ce.ButtonOneTouchEnd -= ButtonOneTouchEnd;

            ce.ButtonTwoPressed -= ButtonTwoPressed;
            ce.ButtonTwoReleased -= ButtonTwoReleased;
            ce.ButtonTwoTouchStart -= ButtonTwoTouchStart;
            ce.ButtonTwoTouchEnd -= ButtonTwoTouchEnd;

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
            ce.ControllerIndexChanged -= ControllerIndexChanged;
        }
    }
}