namespace VRTK.Examples
{
    using Tilia.Interactions.Interactables.Interactables;
    using UnityEngine;
    using Zinnia.Extension;
    using Zinnia.Tracking.Collision;

    /// <summary>
    /// Controls the operation of the archery arrow.
    /// </summary>
    public class ArrowController : MonoBehaviour
    {
        [Tooltip("The actual Interactable arrow.")]
        [SerializeField]
        private InteractableFacade arrowInteractable;
        /// <summary>
        /// The actual Interactable arrow.
        /// </summary>
        public InteractableFacade ArrowInteractable
        {
            get
            {
                return arrowInteractable;
            }
            set
            {
                arrowInteractable = value;
            }
        }
        [Tooltip("The amount of time to pass before destroying the arrow after being fired.")]
        [SerializeField]
        private float timeTillDestroy = 20f;
        /// <summary>
        /// The amount of time to pass before destroying the arrow after being fired.
        /// </summary>
        public float TimeTillDestroy
        {
            get
            {
                return timeTillDestroy;
            }
            set
            {
                timeTillDestroy = value;
            }
        }
        [Tooltip("The actions to enable upon the arrow hitting a valid object.")]
        [SerializeField]
        private GameObject hitAction;
        /// <summary>
        /// The actions to enable upon the arrow hitting a valid object.
        /// </summary>
        public GameObject HitAction
        {
            get
            {
                return hitAction;
            }
            set
            {
                hitAction = value;
            }
        }
        [Tooltip("The colliders associated with the arrow.")]
        [SerializeField]
        private Collider[] arrowColliders;
        /// <summary>
        /// The colliders associated with the arrow.
        /// </summary>
        public Collider[] ArrowColliders
        {
            get
            {
                return arrowColliders;
            }
            set
            {
                arrowColliders = value;
            }
        }

        /// <summary>
        /// Whether the arrow is in flight;
        /// </summary>
        protected bool inFlight;

        /// <summary>
        /// Clears the <see cref="ArrowInteractable"/>.
        /// </summary>
        public virtual void ClearArrowInteractable()
        {
            if(!this.IsValidState())
            {
                return;
            }

            ArrowInteractable = default;
        }

        /// <summary>
        /// Clears the <see cref="HitAction"/>.
        /// </summary>
        public virtual void ClearHitAction()
        {
            if (!this.IsValidState())
            {
                return;
            }

            HitAction = default;
        }

        /// <summary>
        /// Checks to see if the arrow has hit something.
        /// </summary>
        /// <param name="data">The hit data.</param>
        public virtual void CheckHit(CollisionNotifier.EventData data)
        {
            int ignoreLayer = LayerMask.GetMask("Ignore Raycast");
            if (!inFlight || 
                data.ColliderData.attachedRigidbody == ArrowInteractable.InteractableRigidbody || 
                ignoreLayer == (ignoreLayer | (1 << data.ColliderData.gameObject.layer)))
            {
                return;
            }

            inFlight = false;
            HitAction.transform.position = ArrowInteractable.transform.position;
            ArrowInteractable.gameObject.SetActive(false);
            HitAction.SetActive(true);
            Destroy(gameObject, 2f);
        }

        /// <summary>
        /// Fires the arrow.
        /// </summary>
        /// <param name="force">The force to propel the arrow with.</param>
        public virtual void Fire(float force)
        {
            ArrowInteractable.InteractableRigidbody.isKinematic = false;
            ArrowInteractable.InteractableRigidbody.velocity = ArrowInteractable.transform.forward * force;
            inFlight = true;
            Destroy(gameObject, TimeTillDestroy);
        }

        /// <summary>
        /// Toggles the colliders on the arrow to/from trigger colliders.
        /// </summary>
        /// <param name="isTrigger">Whether to make the colliders a trigger.</param>
        public virtual void ToggleColliderTrigger(bool isTrigger)
        {
            for (int i = 1; i < ArrowColliders.Length; i++)
            {
                ArrowColliders[i].isTrigger = isTrigger;
            }
        }

        protected virtual void FixedUpdate()
        {
            if (inFlight && ArrowInteractable != null)
            {
                ArrowInteractable.transform.LookAt(ArrowInteractable.transform.position + ArrowInteractable.InteractableRigidbody.velocity);
            }
        }
    }
}