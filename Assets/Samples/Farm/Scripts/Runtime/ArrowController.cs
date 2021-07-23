namespace VRTK.Examples
{
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Tilia.Interactions.Interactables.Interactables;
    using UnityEngine;
    using Zinnia.Tracking.Collision;

    /// <summary>
    /// Controls the operation of the archery arrow.
    /// </summary>
    public class ArrowController : MonoBehaviour
    {
        /// <summary>
        /// The actual Interactable arrow.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public InteractableFacade ArrowInteractable { get; set; }
        /// <summary>
        /// The amount of time to pass before destroying the arrow after being fired.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float TimeTillDestroy { get; set; } = 20f;
        /// <summary>
        /// The actions to enable upon the arrow hitting a valid object.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject HitAction { get; set; }
        /// <summary>
        /// The colliders associated with the arrow.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Collider[] ArrowColliders { get; set; }

        /// <summary>
        /// Whether the arrow is in flight;
        /// </summary>
        protected bool inFlight;

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