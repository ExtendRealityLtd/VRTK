namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(VRTK_BodyPhysics))]
    public class VRTK_BodyPhysics_UnityEvents : MonoBehaviour
    {
        private VRTK_BodyPhysics bp;

        [System.Serializable]
        public class UnityObjectEvent : UnityEvent<object, BodyPhysicsEventArgs> { };

        /// <summary>
        /// Emits the StartFalling class event.
        /// </summary>
        public UnityObjectEvent OnStartFalling = new UnityObjectEvent();
        /// <summary>
        /// Emits the StopFalling class event.
        /// </summary>
        public UnityObjectEvent OnStopFalling = new UnityObjectEvent();
        /// <summary>
        /// Emits the StartMoving class event.
        /// </summary>
        public UnityObjectEvent OnStartMoving = new UnityObjectEvent();
        /// <summary>
        /// Emits the StopMoving class event.
        /// </summary>
        public UnityObjectEvent OnStopMoving = new UnityObjectEvent();
        /// <summary>
        /// Emits the StartColliding class event.
        /// </summary>
        public UnityObjectEvent OnStartColliding = new UnityObjectEvent();
        /// <summary>
        /// Emits the StopColliding class event.
        /// </summary>
        public UnityObjectEvent OnStopColliding = new UnityObjectEvent();

        private void SetBodyPhysics()
        {
            if (bp == null)
            {
                bp = GetComponent<VRTK_BodyPhysics>();
            }
        }

        private void OnEnable()
        {
            SetBodyPhysics();
            if (bp == null)
            {
                Debug.LogError("The VRTK_BodyPhysics_UnityEvents script requires to be attached to a GameObject that contains a VRTK_BodyPhysics script");
                return;
            }

            bp.StartFalling += StartFalling;
            bp.StopFalling += StopFalling;
            bp.StartMoving += StartMoving;
            bp.StopMoving += StopMoving;
            bp.StartColliding += StartColliding;
            bp.StopColliding += StopColliding;
        }

        private void StartFalling(object o, BodyPhysicsEventArgs e)
        {
            OnStartFalling.Invoke(o, e);
        }

        private void StopFalling(object o, BodyPhysicsEventArgs e)
        {
            OnStopFalling.Invoke(o, e);
        }

        private void StartMoving(object o, BodyPhysicsEventArgs e)
        {
            OnStartMoving.Invoke(o, e);
        }

        private void StopMoving(object o, BodyPhysicsEventArgs e)
        {
            OnStopMoving.Invoke(o, e);
        }

        private void StartColliding(object o, BodyPhysicsEventArgs e)
        {
            OnStartColliding.Invoke(o, e);
        }

        private void StopColliding(object o, BodyPhysicsEventArgs e)
        {
            OnStopColliding.Invoke(o, e);
        }

        private void OnDisable()
        {
            if (bp == null)
            {
                return;
            }

            bp.StartFalling -= StartFalling;
            bp.StopFalling -= StopFalling;
            bp.StartMoving -= StartMoving;
            bp.StopMoving -= StopMoving;
            bp.StartColliding -= StartColliding;
            bp.StopColliding -= StopColliding;
        }
    }
}