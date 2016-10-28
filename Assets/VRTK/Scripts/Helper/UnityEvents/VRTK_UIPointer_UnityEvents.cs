namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_UIPointer))]
    public class VRTK_UIPointer_UnityEvents : MonoBehaviour
    {
        private VRTK_UIPointer uip;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, UIPointerEventArgs> { };

        /// <summary>
        /// Emits the UIPointerElementEnter class event.
        /// </summary>
        public UnityObjectEvent OnUIPointerElementEnter;
        /// <summary>
        /// Emits the UIPointerElementExit class event.
        /// </summary>
        public UnityObjectEvent OnUIPointerElementExit;
        /// <summary>
        /// Emits the UIPointerElementClick class event.
        /// </summary>
        public UnityObjectEvent OnUIPointerElementClick;
        /// <summary>
        /// Emits the UIPointerElementDragStart class event.
        /// </summary>
        public UnityObjectEvent OnUIPointerElementDragStart;
        /// <summary>
        /// Emits the UIPointerElementDragEnd class event.
        /// </summary>
        public UnityObjectEvent OnUIPointerElementDragEnd;

        private void SetUIPointer()
        {
            if (uip == null)
            {
                uip = GetComponent<VRTK_UIPointer>();
            }
        }

        private void OnEnable()
        {
            SetUIPointer();
            if (uip == null)
            {
                Debug.LogError("The VRTK_UIPointer_UnityEvents script requires to be attached to a GameObject that contains a VRTK_UIPointer script");
                return;
            }
            uip.UIPointerElementEnter += UIPointerElementEnter;
            uip.UIPointerElementExit += UIPointerElementExit;
            uip.UIPointerElementClick += UIPointerElementClick;
            uip.UIPointerElementDragStart += UIPointerElementDragStart;
            uip.UIPointerElementDragEnd += UIPointerElementDragEnd;
        }

        private void UIPointerElementEnter(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementEnter.Invoke(o, e);
        }

        private void UIPointerElementExit(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementExit.Invoke(o, e);
        }

        private void UIPointerElementClick(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementClick.Invoke(o, e);
        }

        private void UIPointerElementDragStart(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementDragStart.Invoke(o, e);
        }

        private void UIPointerElementDragEnd(object o, UIPointerEventArgs e)
        {
            OnUIPointerElementDragEnd.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (uip == null)
            {
                return;
            }

            uip.UIPointerElementEnter -= UIPointerElementEnter;
            uip.UIPointerElementExit -= UIPointerElementExit;
            uip.UIPointerElementClick -= UIPointerElementClick;
            uip.UIPointerElementDragStart -= UIPointerElementDragStart;
            uip.UIPointerElementDragEnd -= UIPointerElementDragEnd;
        }
    }
}