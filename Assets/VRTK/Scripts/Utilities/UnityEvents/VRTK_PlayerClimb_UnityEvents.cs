namespace VRTK.UnityEventHelper
{
    using UnityEngine.Events;
    using System;

    public sealed class VRTK_PlayerClimb_UnityEvents : VRTK_UnityEvents<VRTK_PlayerClimb>
    {
        [Serializable]
        public sealed class PlayerClimbEvent : UnityEvent<object, PlayerClimbEventArgs> { }

        public PlayerClimbEvent OnPlayerClimbStarted = new PlayerClimbEvent();
        public PlayerClimbEvent OnPlayerClimbEnded = new PlayerClimbEvent();

        protected override void AddListeners(VRTK_PlayerClimb component)
        {
            component.PlayerClimbStarted += PlayerClimbStarted;
            component.PlayerClimbEnded += PlayerClimbEnded;
        }

        protected override void RemoveListeners(VRTK_PlayerClimb component)
        {
            component.PlayerClimbStarted -= PlayerClimbStarted;
            component.PlayerClimbEnded -= PlayerClimbEnded;
        }

        private void PlayerClimbStarted(object o, PlayerClimbEventArgs e)
        {
            OnPlayerClimbStarted.Invoke(o, e);
        }

        private void PlayerClimbEnded(object o, PlayerClimbEventArgs e)
        {
            OnPlayerClimbEnded.Invoke(o, e);
        }
    }
}