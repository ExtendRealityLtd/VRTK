namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_BodyPhysics_UnityEvents : VRTK_UnityEvents<VRTK_BodyPhysics>
    {
        [Serializable]
        public sealed class BodyPhysicsEvent : UnityEvent<object, BodyPhysicsEventArgs> { }

        public BodyPhysicsEvent OnStartFalling = new BodyPhysicsEvent();
        public BodyPhysicsEvent OnStopFalling = new BodyPhysicsEvent();

        public BodyPhysicsEvent OnStartMoving = new BodyPhysicsEvent();
        public BodyPhysicsEvent OnStopMoving = new BodyPhysicsEvent();

        public BodyPhysicsEvent OnStartColliding = new BodyPhysicsEvent();
        public BodyPhysicsEvent OnStopColliding = new BodyPhysicsEvent();

        protected override void AddListeners(VRTK_BodyPhysics component)
        {
            component.StartFalling += StartFalling;
            component.StopFalling += StopFalling;

            component.StartMoving += StartMoving;
            component.StopMoving += StopMoving;

            component.StartColliding += StartColliding;
            component.StopColliding += StopColliding;
        }

        protected override void RemoveListeners(VRTK_BodyPhysics component)
        {
            component.StartFalling -= StartFalling;
            component.StopFalling -= StopFalling;

            component.StartMoving -= StartMoving;
            component.StopMoving -= StopMoving;

            component.StartColliding -= StartColliding;
            component.StopColliding -= StopColliding;
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
    }
}