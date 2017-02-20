namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_Button))]
    public class VRTK_Button_UnityEvents : MonoBehaviour
    {
        private VRTK_Button b3d;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, Control3DEventArgs> { };

        /// <summary>
        /// Emits the Pushed class event.
        /// </summary>
        public UnityObjectEvent OnPushed = new UnityObjectEvent();

        private void SetButton3D()
        {
            if (b3d == null)
            {
                b3d = GetComponent<VRTK_Button>();
            }
        }

        private void OnEnable()
        {
            SetButton3D();
            if (b3d == null)
            {
                Debug.LogError("The VRTK_Button_UnityEvents script requires to be attached to a GameObject that contains a VRTK_Button script");
                return;
            }

            b3d.Pushed += Pushed;
        }

        private void Pushed(object o, Control3DEventArgs e)
        {
            OnPushed.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (b3d == null)
            {
                return;
            }

            b3d.Pushed -= Pushed;
        }
    }
}