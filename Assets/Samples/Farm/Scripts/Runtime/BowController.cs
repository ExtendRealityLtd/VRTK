namespace VRTK.Examples
{
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
        [Header("Bow Settings")]
        [Tooltip("The actual Interactable bow.")]
        [SerializeField]
        private InteractableFacade bowInteractable;
        /// <summary>
        /// The actual Interactable bow.
        /// </summary>
        public InteractableFacade BowInteractable
        {
            get
            {
                return bowInteractable;
            }
            set
            {
                bowInteractable = value;
            }
        }
        [Tooltip("The colliders associated with the bow.")]
        [SerializeField]
        private Collider[] bowColliders;
        /// <summary>
        /// The colliders associated with the bow.
        /// </summary>
        public Collider[] BowColliders
        {
            get
            {
                return bowColliders;
            }
            set
            {
                bowColliders = value;
            }
        }
        [Tooltip("The snap zone used for nocking the arrow to the bow.")]
        [SerializeField]
        private SnapZoneFacade arrowSnapZone;
        /// <summary>
        /// The snap zone used for nocking the arrow to the bow.
        /// </summary>
        public SnapZoneFacade ArrowSnapZone
        {
            get
            {
                return arrowSnapZone;
            }
            set
            {
                arrowSnapZone = value;
            }
        }
        [Tooltip("The animation scrubber that scrubs through the bow flex animation.")]
        [SerializeField]
        private AnimatorScrubber flexAnimationScrubber;
        /// <summary>
        /// The animation scrubber that scrubs through the bow flex animation.
        /// </summary>
        public AnimatorScrubber FlexAnimationScrubber
        {
            get
            {
                return flexAnimationScrubber;
            }
            set
            {
                flexAnimationScrubber = value;
            }
        }
        #endregion

        #region String Settings
        [Header("String Settings")]
        [Tooltip("The LineRenderer used to draw the bow string.")]
        [SerializeField]
        private LineRenderer stringRenderer;
        /// <summary>
        /// The <see cref="LineRenderer"/> used to draw the bow string.
        /// </summary>
        public LineRenderer StringRenderer
        {
            get
            {
                return stringRenderer;
            }
            set
            {
                stringRenderer = value;
            }
        }
        [Tooltip("The points used to draw the string between.")]
        [SerializeField]
        private Transform[] stringPoints = new Transform[3];
        /// <summary>
        /// The points used to draw the string between.
        /// </summary>
        public Transform[] StringPoints
        {
            get
            {
                return stringPoints;
            }
            set
            {
                stringPoints = value;
            }
        }
        [Tooltip("The container holding the nocking point on the string.")]
        [SerializeField]
        private GameObject stringNock;
        /// The container holding the nocking point on the string.
        /// </summary>
        public GameObject StringNock
        {
            get
            {
                return stringNock;
            }
            set
            {
                stringNock = value;
            }
        }
        [Tooltip("The speed in which the string returns to the idle position.")]
        [SerializeField]
        private float stringReturnSpeed = 150f;
        /// <summary>
        /// The speed in which the string returns to the idle position.
        /// </summary>
        public float StringReturnSpeed
        {
            get
            {
                return stringReturnSpeed;
            }
            set
            {
                stringReturnSpeed = value;
            }
        }
        [Tooltip("The maximum length the string can be pulled back.")]
        [SerializeField]
        private float maxStringPull = 0.75f;
        /// <summary>
        /// The maximum length the string can be pulled back.
        /// </summary>
        public float MaxStringPull
        {
            get
            {
                return maxStringPull;
            }
            set
            {
                maxStringPull = value;
            }
        }
        [Tooltip("The power produced to propel the arrow forward by the string when fully pulled back.")]
        [SerializeField]
        private float stringPower = 20f;
        /// <summary>
        /// The power produced to propel the arrow forward by the string when fully pulled back.
        /// </summary>
        public float StringPower
        {
            get
            {
                return stringPower;
            }
            set
            {
                stringPower = value;
            }
        }
        #endregion

        /// <summary>
        /// Clears the <see cref="BowInteractable"/>.
        /// </summary>
        public virtual void ClearBowInteractable()
        {
            if (!this.IsValidState())
            {
                return;
            }

            BowInteractable = default;
        }

        /// <summary>
        /// Clears the <see cref="ArrowSnapZone"/>.
        /// </summary>
        public virtual void ClearArrowSnapZone()
        {
            if (!this.IsValidState())
            {
                return;
            }

            ArrowSnapZone = default;
        }

        /// <summary>
        /// Clears the <see cref="FlexAnimationScrubber"/>.
        /// </summary>
        public virtual void ClearFlexAnimationScrubber()
        {
            if (!this.IsValidState())
            {
                return;
            }

            FlexAnimationScrubber = default;
        }

        /// <summary>
        /// Clears the <see cref="StringRenderer"/>.
        /// </summary>
        public virtual void ClearStringRenderer()
        {
            if (!this.IsValidState())
            {
                return;
            }

            StringRenderer = default;
        }

        /// <summary>
        /// Clears the <see cref="StringNock"/>.
        /// </summary>
        public virtual void ClearStringNock()
        {
            if (!this.IsValidState())
            {
                return;
            }

            StringNock = default;
        }

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