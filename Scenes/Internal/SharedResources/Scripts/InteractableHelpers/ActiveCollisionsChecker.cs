namespace VRTK.Examples.InteractableHelper
{
    using UnityEngine;
    using UnityEngine.Events;
    using VRTK.Core.Tracking.Collision.Active;

    public class ActiveCollisionsChecker : MonoBehaviour
    {
        public ActiveCollisionsContainer collisionContainer;

        public UnityEvent CollisionsFound = new UnityEvent();
        public UnityEvent CollisionsNotFound = new UnityEvent();

        public virtual void HasCollisions()
        {
            if (collisionContainer.Elements.Count == 0)
            {
                CollisionsNotFound?.Invoke();
            }
            else
            {
                CollisionsFound?.Invoke();
            }
        }
    }
}