namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_HeadsetCollision_UnityEvents")]
    public sealed class VRTK_HeadsetCollision_UnityEvents : VRTK_UnityEvents<VRTK_HeadsetCollision>
    {
        [Serializable]
        public sealed class HeadsetCollisionEvent : UnityEvent<object, HeadsetCollisionEventArgs> { }

        public HeadsetCollisionEvent OnHeadsetCollisionDetect = new HeadsetCollisionEvent();
        public HeadsetCollisionEvent OnHeadsetCollisionEnded = new HeadsetCollisionEvent();

        protected override void AddListeners(VRTK_HeadsetCollision component)
        {
            component.HeadsetCollisionDetect += HeadsetCollisionDetect;
            component.HeadsetCollisionEnded += HeadsetCollisionEnded;
        }

        protected override void RemoveListeners(VRTK_HeadsetCollision component)
        {
            component.HeadsetCollisionDetect -= HeadsetCollisionDetect;
            component.HeadsetCollisionEnded -= HeadsetCollisionEnded;
        }

        private void HeadsetCollisionDetect(object o, HeadsetCollisionEventArgs e)
        {
            OnHeadsetCollisionDetect.Invoke(o, e);
        }

        private void HeadsetCollisionEnded(object o, HeadsetCollisionEventArgs e)
        {
            OnHeadsetCollisionEnded.Invoke(o, e);
        }
    }
}