namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_PointerDirectionIndicator_UnityEvents")]
    public sealed class VRTK_PointerDirectionIndicator_UnityEvents : VRTK_UnityEvents<VRTK_PointerDirectionIndicator>
    {
        [Serializable]
        public sealed class PointerDirectionIndicatorEvent : UnityEvent<object> { }

        public PointerDirectionIndicatorEvent OnPointerDirectionIndicatorPositionSet = new PointerDirectionIndicatorEvent();

        protected override void AddListeners(VRTK_PointerDirectionIndicator component)
        {
            component.PointerDirectionIndicatorPositionSet += PointerDirectionIndicatorPositionSet;
        }

        protected override void RemoveListeners(VRTK_PointerDirectionIndicator component)
        {
            component.PointerDirectionIndicatorPositionSet -= PointerDirectionIndicatorPositionSet;
        }

        private void PointerDirectionIndicatorPositionSet(object o)
        {
            OnPointerDirectionIndicatorPositionSet.Invoke(o);
        }
    }
}