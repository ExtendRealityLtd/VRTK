namespace VRTK.Examples.Physics
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Ignores collisions between two collections of <see cref="GameObject"/>s.
    /// </summary>
    public class CollisionIgnorer : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="GameObject"/> collection to ignore collisions from.
        /// </summary>
        [Tooltip("The GameObjects to ignore collisions from.")]
        public List<GameObject> ignoreCollisionsFrom = new List<GameObject>();
        /// <summary>
        /// The <see cref="GameObject"/> collection  to ignore collisions with.
        /// </summary>
        [Tooltip("The GameObjects to ignore collisions with.")]
        public List<GameObject> ignoreCollisionsWith = new List<GameObject>();
        /// <summary>
        /// Whether to ignore collisions on component enable.
        /// </summary>
        [Tooltip("Whether to ignore collisions on component enable.")]
        public bool ignoreCollisionsOnEnable = true;

        /// <summary>
        /// Ignores the collisions between the colliders found in the <see cref="ignoreCollisionsFrom"/> and the <see cref="ignoreCollisionsWith"/>.
        /// </summary>
        public virtual void IgnoreCollisions()
        {
            ToggleCollisions(true);
        }

        /// <summary>
        /// Restores the collisions between the colliders found in the <see cref="ignoreCollisionsFrom"/> and the <see cref="ignoreCollisionsWith"/>.
        /// </summary>
        public virtual void RestoreCollisions()
        {
            ToggleCollisions(false);
        }

        protected virtual void OnEnable()
        {
            if (ignoreCollisionsOnEnable)
            {
                IgnoreCollisions();
            }
        }

        /// <summary>
        /// Toggles the collision state between the colliders found in the <see cref="ignoreCollisionsFrom"/> and the <see cref="ignoreCollisionsWith"/>.
        /// </summary>
        /// <param name="ignore">Whether to ignore the collisions or restore the collisions.</param>
        protected virtual void ToggleCollisions(bool ignore)
        {
            List<Collider> collidersFrom = new List<Collider>();
            List<Collider> collidersWith = new List<Collider>();
            foreach (GameObject ignoreCollisionFrom in ignoreCollisionsFrom)
            {
                collidersFrom.AddRange(ignoreCollisionFrom.GetComponentsInChildren<Collider>());
            }
            foreach (GameObject ignoreCollisionWith in ignoreCollisionsWith)
            {
                collidersWith.AddRange(ignoreCollisionWith.GetComponentsInChildren<Collider>());
            }

            foreach (Collider ignoreCollisionFromCollider in collidersFrom)
            {
                foreach (Collider ignoreCollisionWithCollider in collidersWith)
                {
                    Physics.IgnoreCollision(ignoreCollisionFromCollider, ignoreCollisionWithCollider, ignore);
                }
            }

        }
    }
}