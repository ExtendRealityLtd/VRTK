namespace VRTK.Examples
{
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Tilia.Interactions.Interactables.Interactables;
    using Tilia.Interactions.Interactables.Interactors;
    using Tilia.Interactions.SnapZone;
    using UnityEngine;
    using Zinnia.Extension;
    using Zinnia.Tracking.Collision;

    /// <summary>
    /// Controls the operation of the archery bow.
    /// </summary>
    public class BowController : MonoBehaviour
    {
        #region Bow Settings
        /// <summary>
        /// The actual Interactable bow.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Bow Settings"), DocumentedByXml]
        public InteractableFacade BowInteractable { get; set; }
        /// <summary>
        /// The colliders associated with the bow.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Collider[] BowColliders { get; set; }
        /// <summary>
        /// The snap zone used for nocking the arrow to the bow.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public SnapZoneFacade ArrowSnapZone { get; set; }
        /// <summary>
        /// The animation scrubber that scrubs through the bow flex animation.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public AnimatorScrubber FlexAnimationScrubber { get; set; }
        #endregion

        #region String Settings
        /// <summary>
        /// The <see cref="LineRenderer"/> used to draw the bow string.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("String Settings"), DocumentedByXml]
        public LineRenderer StringRenderer { get; set; }
        /// <summary>
        /// The points used to draw the string between.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Transform[] StringPoints { get; set; } = new Transform[3];
        /// <summary>
        /// The container holding the nocking point on the string.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject StringNock { get; set; }
        /// <summary>
        /// The speed in which the string returns to the idle position.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float StringReturnSpeed { get; set; } = 150f;
        /// <summary>
        /// The maximum length the string can be pulled back.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float MaxStringPull { get; set; } = 0.75f;
        /// <summary>
        /// The power produced to propel the arrow forward by the string when fully pulled back.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float StringPower { get; set; } = 20f;
        #endregion

        /// <summary>
        /// The normalized value of the current pull distance of the string from the nock resting point.
        /// </summary>
        public float NormalizedPullDistance
        {
            get
            {
                return Mathf.InverseLerp(0, MaxStringPull + minStringPull, Vector3.Distance(nockRestingPosition, StringNock.transform.localPosition));
            }
        }

        /// <summary>
        /// Whether the string is being grabbed.
        /// </summary>
        protected bool isStringGrabbed;
        /// <summary>
        /// The value of the string when it is not being pulled.
        /// </summary>
        protected float minStringPull;
        /// <summary>
        /// The resting position of the nock.
        /// </summary>
        protected Vector3 nockRestingPosition;
        /// <summary>
        /// The secondary grabbing interactor.
        /// </summary>
        protected InteractorFacade secondaryInteractor;
        /// <summary>
        /// The controller for the nocked arrow.
        /// </summary>
        protected ArrowController nockedArrow;
        /// <summary>
        /// The interactor grabbing the arrow.
        /// </summary>
        protected InteractorFacade arrowInteractor;

        /// <summary>
        /// Grabs the string.
        /// </summary>
        public virtual void GrabString()
        {
            isStringGrabbed = true;
        }

        /// <summary>
        /// Ungrabs the string.
        /// </summary>
        public virtual void UngrabString()
        {
            if (isStringGrabbed)
            {
                AttemptFireArrow();
            }

            isStringGrabbed = false;
        }

        /// <summary>
        /// Attempts to nock the arrow.
        /// </summary>
        /// <param name="data">The collision data which should contain an arrow.</param>
        public virtual void AttemptArrowNock(CollisionNotifier.EventData data)
        {
            if (ArrowSnapZone.SnappedGameObject != null || !data.ColliderData.name.Equals("ArrowNockPoint"))
            {
                return;
            }

            InteractableFacade arrow = data.ColliderData.GetAttachedRigidbody().GetComponent<InteractableFacade>();
            if (arrow == null || arrow.GrabbingInteractors.Count == 0)
            {
                return;
            }

            arrowInteractor = arrow.GrabbingInteractors.Count > 0 ? arrow.GrabbingInteractors[0] : null;

            if (arrowInteractor == null)
            {
                return;
            }

            arrowInteractor.Ungrab();
            arrowInteractor.SimulateUntouch(arrow);
            arrowInteractor.Grab(BowInteractable);
            ArrowSnapZone.Snap(arrow);

            nockedArrow = arrow.GetComponentInParent<ArrowController>();
            ToggleColliders(BowColliders, nockedArrow.ArrowColliders, true);
            nockedArrow.ToggleColliderTrigger(true);
        }

        /// <summary>
        /// Attempts to fire the arrow.
        /// </summary>
        public virtual void AttemptFireArrow()
        {
            if (arrowInteractor == null || nockedArrow == null || !nockedArrow.ArrowInteractable.gameObject.Equals(ArrowSnapZone.SnappedGameObject))
            {
                return;
            }

            ArrowSnapZone.Unsnap();
            BowInteractable.Ungrab(1);
            arrowInteractor.SimulateUntouch(BowInteractable);
            nockedArrow.Fire(NormalizedPullDistance * StringPower);
        }

        protected virtual void OnEnable()
        {
            StringRenderer.gameObject.SetActive(true);
            nockRestingPosition = StringNock.transform.localPosition;
            minStringPull = nockRestingPosition.z;
        }

        protected virtual void Update()
        {
            if (!isStringGrabbed)
            {
                if (!StringNock.transform.localPosition.ApproxEquals(nockRestingPosition))
                {
                    StringNock.transform.localPosition = Vector3.MoveTowards(StringNock.transform.localPosition, nockRestingPosition, StringReturnSpeed * Time.deltaTime);
                }
            }
            else
            {
                secondaryInteractor = BowInteractable.GrabbingInteractors.Count > 1 ? BowInteractable.GrabbingInteractors[1] : null;
                if (secondaryInteractor != null)
                {
                    StringNock.transform.position = secondaryInteractor.GrabAttachPoint.transform.position;
                    StringNock.transform.localPosition = Vector3.forward * Mathf.Clamp(StringNock.transform.localPosition.z, MaxStringPull * -1, minStringPull);
                }
            }

            FlexAnimationScrubber.Scrub(Mathf.Clamp01(NormalizedPullDistance - 0.001f));
            for (int linePoint = 0; linePoint < StringPoints.Length; linePoint++)
            {
                StringRenderer.SetPosition(linePoint, StringPoints[linePoint].position);
            }
        }

        /// <summary>
        /// Toggles the collisions bvetween the sources and the targets.
        /// </summary>
        /// <param name="sources">The source colliders to reference.</param>
        /// <param name="targets">The target colliders to reference.</param>
        /// <param name="ignore">Whether to ignore or resume the collisions between the references.</param>
        protected virtual void ToggleColliders(Collider[] sources, Collider[] targets, bool ignore)
        {
            foreach (Collider source in sources)
            {
                foreach (Collider target in targets)
                {
                    Physics.IgnoreCollision(source, target, ignore);
                }
            }
        }
    }
}