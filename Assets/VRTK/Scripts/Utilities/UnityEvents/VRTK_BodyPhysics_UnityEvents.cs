namespace VRTK.UnityEventHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_BodyPhysics_UnityEvents")]
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

        public BodyPhysicsEvent OnStartLeaning = new BodyPhysicsEvent();
        public BodyPhysicsEvent OnStopLeaning = new BodyPhysicsEvent();

        public BodyPhysicsEvent OnStartTouchingGround = new BodyPhysicsEvent();
        public BodyPhysicsEvent OnStopTouchingGround = new BodyPhysicsEvent();

        protected override void AddListeners(VRTK_BodyPhysics component)
        {
            component.StartFalling += StartFalling;
            component.StopFalling += StopFalling;

            component.StartMoving += StartMoving;
            component.StopMoving += StopMoving;

            component.StartColliding += StartColliding;
            component.StopColliding += StopColliding;

            component.StartLeaning += StartLeaning;
            component.StopLeaning += StopLeaning;

            component.StartTouchingGround += StartTouchingGround;
            component.StopTouchingGround += StopTouchingGround;
        }

        protected override void RemoveListeners(VRTK_BodyPhysics component)
        {
            component.StartFalling -= StartFalling;
            component.StopFalling -= StopFalling;

            component.StartMoving -= StartMoving;
            component.StopMoving -= StopMoving;

            component.StartColliding -= StartColliding;
            component.StopColliding -= StopColliding;

            component.StartLeaning -= StartLeaning;
            component.StopLeaning -= StopLeaning;

            component.StartTouchingGround -= StartTouchingGround;
            component.StopTouchingGround -= StopTouchingGround;
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

        private void StartLeaning(object o, BodyPhysicsEventArgs e)
        {
            OnStartLeaning.Invoke(o, e);
        }

        private void StopLeaning(object o, BodyPhysicsEventArgs e)
        {
            OnStopLeaning.Invoke(o, e);
        }

        private void StartTouchingGround(object o, BodyPhysicsEventArgs e)
        {
            OnStartTouchingGround.Invoke(o, e);
        }

        private void StopTouchingGround(object o, BodyPhysicsEventArgs e)
        {
            OnStopTouchingGround.Invoke(o, e);
        }
    }
}