namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using VRTK.Controllables;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_BaseControllable_UnityEvents")]
    public sealed class VRTK_BaseControllable_UnityEvents : VRTK_UnityEvents<VRTK_BaseControllable>
    {
        [Serializable]
        public sealed class BaseControllablEvent : UnityEvent<object, ControllableEventArgs> { }

        public BaseControllablEvent OnValueChanged = new BaseControllablEvent();
        public BaseControllablEvent OnRestingPointReached = new BaseControllablEvent();
        public BaseControllablEvent OnMinLimitReached = new BaseControllablEvent();
        public BaseControllablEvent OnMinLimitExited = new BaseControllablEvent();
        public BaseControllablEvent OnMaxLimitReached = new BaseControllablEvent();
        public BaseControllablEvent OnMaxLimitExited = new BaseControllablEvent();

        protected override void AddListeners(VRTK_BaseControllable component)
        {
            component.ValueChanged += ValueChanged;
            component.RestingPointReached += RestingPointReached;
            component.MinLimitReached += MinLimitReached;
            component.MinLimitExited += MinLimitExited;
            component.MaxLimitReached += MaxLimitReached;
            component.MaxLimitExited += MaxLimitExited;
        }

        protected override void RemoveListeners(VRTK_BaseControllable component)
        {
            component.ValueChanged -= ValueChanged;
            component.RestingPointReached -= RestingPointReached;
            component.MinLimitReached -= MinLimitReached;
            component.MinLimitExited -= MinLimitExited;
            component.MaxLimitReached -= MaxLimitReached;
            component.MaxLimitExited -= MaxLimitExited;
        }

        private void ValueChanged(object o, ControllableEventArgs e)
        {
            OnValueChanged.Invoke(o, e);
        }

        private void RestingPointReached(object o, ControllableEventArgs e)
        {
            OnRestingPointReached.Invoke(o, e);
        }

        private void MinLimitReached(object o, ControllableEventArgs e)
        {
            OnMinLimitReached.Invoke(o, e);
        }

        private void MinLimitExited(object o, ControllableEventArgs e)
        {
            OnMinLimitExited.Invoke(o, e);
        }

        private void MaxLimitReached(object o, ControllableEventArgs e)
        {
            OnMaxLimitReached.Invoke(o, e);
        }

        private void MaxLimitExited(object o, ControllableEventArgs e)
        {
            OnMaxLimitExited.Invoke(o, e);
        }
    }
}