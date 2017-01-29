namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_SnapDropZone))]
    public class VRTK_SnapDropZone_UnityEvents : MonoBehaviour
    {
        private VRTK_SnapDropZone sdz;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, SnapDropZoneEventArgs> { };

        /// <summary>
        /// Emits the ObjectEnteredSnapDropZone class event.
        /// </summary>
        public UnityObjectEvent OnObjectEnteredSnapDropZone = new UnityObjectEvent();
        /// <summary>
        /// Emits the ObjectExitedSnapDropZone class event.
        /// </summary>
        public UnityObjectEvent OnObjectExitedSnapDropZone = new UnityObjectEvent();
        /// <summary>
        /// Emits the ObjectSnappedToDropZone class event.
        /// </summary>
        public UnityObjectEvent OnObjectSnappedToDropZone = new UnityObjectEvent();
        /// <summary>
        /// Emits the ObjectUnsnappedFromDropZone class event.
        /// </summary>
        public UnityObjectEvent OnObjectUnsnappedFromDropZone = new UnityObjectEvent();

        private void SetSnapDropZone()
        {
            if (sdz == null)
            {
                sdz = GetComponent<VRTK_SnapDropZone>();
            }
        }

        private void OnEnable()
        {
            SetSnapDropZone();
            if (sdz == null)
            {
                Debug.LogError("The VRTK_SnapDropZone_UnityEvents script requires to be attached to a GameObject that contains a VRTK_SnapDropZone script");
                return;
            }

            sdz.ObjectEnteredSnapDropZone += ObjectEnteredSnapDropZone;
            sdz.ObjectExitedSnapDropZone += ObjectExitedSnapDropZone;
            sdz.ObjectSnappedToDropZone += ObjectSnappedToDropZone;
            sdz.ObjectUnsnappedFromDropZone += ObjectUnsnappedFromDropZone;
        }

        private void ObjectEnteredSnapDropZone(object o, SnapDropZoneEventArgs e)
        {
            OnObjectEnteredSnapDropZone.Invoke(o, e);
        }

        private void ObjectExitedSnapDropZone(object o, SnapDropZoneEventArgs e)
        {
            OnObjectExitedSnapDropZone.Invoke(o, e);
        }

        private void ObjectSnappedToDropZone(object o, SnapDropZoneEventArgs e)
        {
            OnObjectSnappedToDropZone.Invoke(o, e);
        }

        private void ObjectUnsnappedFromDropZone(object o, SnapDropZoneEventArgs e)
        {
            OnObjectUnsnappedFromDropZone.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (sdz == null)
            {
                return;
            }

            sdz.ObjectEnteredSnapDropZone -= ObjectEnteredSnapDropZone;
            sdz.ObjectExitedSnapDropZone -= ObjectExitedSnapDropZone;
            sdz.ObjectSnappedToDropZone -= ObjectSnappedToDropZone;
            sdz.ObjectUnsnappedFromDropZone -= ObjectUnsnappedFromDropZone;
        }
    }
}