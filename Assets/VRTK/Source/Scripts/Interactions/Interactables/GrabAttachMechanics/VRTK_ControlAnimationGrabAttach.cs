// Control Animation Grab Attach|GrabAttachMechanics|50100
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactingObject">The GameObject that is performing the interaction (e.g. a controller).</param>
    /// <param name="currentFrame">The current frame the animation is on.</param>
    public struct ControlAnimationGrabAttachEventArgs
    {
        public GameObject interactingObject;
        public float currentFrame;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ControlAnimationGrabAttachEventArgs"/></param>
    public delegate void ControlAnimationGrabAttachEventHandler(object sender, ControlAnimationGrabAttachEventArgs e);

    /// <summary>
    /// Scrubs through the given animation based on the distance from the grabbing object to the original grabbing point.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_ControlAnimationGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    ///    * Create and apply an animation via:
    ///      * `Animation Timeline` parameter takes a legacy `Animation` component to use as the timeline to scrub through. The animation must be marked as `legacy` via the inspector in debug mode.
    ///      * `Animator Timeline` parameter takes an Animator component to use as the timeline to scrub through.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_ControlAnimationGrabAttach")]
    public class VRTK_ControlAnimationGrabAttach : VRTK_BaseGrabAttach
    {
        [Tooltip("The maximum distance the grabbing object is away from the Interactable Object before it is automatically released.")]
        public float detachDistance = 1f;

        [Header("Animation Settings", order = 2)]

        [Tooltip("An Animation with the timeline to scrub through on grab. If this is set then the `Animator Timeline` will be ignored if it is also set.")]
        public Animation animationTimeline;
        [Tooltip("An Animator with the timeline to scrub through on grab.")]
        public Animator animatorTimeline;
        [Tooltip("The maximum amount of frames in the timeline.")]
        public float maxFrames = 1f;
        [Tooltip("An amount to multiply the distance by to determine the scrubbed frame to be on.")]
        public float distanceMultiplier = 1f;
        [Tooltip("If this is checked then the animation will rewind to the start on ungrab.")]
        public bool rewindOnRelease = false;
        [Tooltip("The speed in which the animation rewind will be multiplied by.")]
        public float rewindSpeedMultplier = 1f;

        /// <summary>
        /// Emitted when the Animation Frame is at the start.
        /// </summary>
        public event ControlAnimationGrabAttachEventHandler AnimationFrameAtStart;
        /// <summary>
        /// Emitted when the Animation Frame is at the end.
        /// </summary>
        public event ControlAnimationGrabAttachEventHandler AnimationFrameAtEnd;
        /// <summary>
        /// Emitted when the Animation Frame has changed.
        /// </summary>
        public event ControlAnimationGrabAttachEventHandler AnimationFrameChanged;

        protected float animationSpeed = 0f;
        protected float frameOffset = 0f;
        protected float currentFrame = 0f;
        protected Coroutine resetTimelineRoutine;
        protected bool atEnd = false;
        protected string animationName = "";

        public virtual void OnAnimationFrameChanged(ControlAnimationGrabAttachEventArgs e)
        {
            if (AnimationFrameChanged != null)
            {
                AnimationFrameChanged(this, e);
            }
        }

        public virtual void OnAnimationFrameAtStart(ControlAnimationGrabAttachEventArgs e)
        {
            if (AnimationFrameAtStart != null)
            {
                AnimationFrameAtStart(this, e);
            }
        }

        public virtual void OnAnimationFrameAtEnd(ControlAnimationGrabAttachEventArgs e)
        {
            if (AnimationFrameAtEnd != null)
            {
                AnimationFrameAtEnd(this, e);
            }
        }

        /// <summary>
        /// The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed.
        /// </summary>
        /// <param name="grabbingObject">The GameObject that is doing the grabbing.</param>
        /// <param name="givenGrabbedObject">The GameObject that is being grabbed.</param>
        /// <param name="givenControllerAttachPoint">The point on the grabbing object that the grabbed object should be attached to after grab occurs.</param>
        /// <returns>Returns `true` if the grab is successful, `false` if the grab is unsuccessful.</returns>
        public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
        {
            CancelResetTimeline();
            atEnd = false;
            return base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint);
        }

        /// <summary>
        /// The StopGrab method ends the grab of the current Interactable Object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If `true` will apply the current velocity of the grabbing object to the grabbed object on release.</param>
        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            base.StopGrab(applyGrabbingObjectVelocity);
            frameOffset = currentFrame;
            if (rewindOnRelease)
            {
                RewindAnimation();
            }
        }

        /// <summary>
        /// The CreateTrackPoint method sets up the point of grab to track on the grabbed object.
        /// </summary>
        /// <param name="controllerPoint">The point on the controller where the grab was initiated.</param>
        /// <param name="currentGrabbedObject">The GameObject that is currently being grabbed.</param>
        /// <param name="currentGrabbingObject">The GameObject that is currently doing the grabbing.</param>
        /// <param name="customTrackPoint">A reference to whether the created track point is an auto generated custom object.</param>
        /// <returns>The Transform of the created track point.</returns>
        public override Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)
        {
            Transform returnTrackpoint = null;
            customTrackPoint = true;
            returnTrackpoint = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, currentGrabbedObject.name, "ControlAnimation", "AttachPoint")).transform;
            returnTrackpoint.SetParent(null);
            returnTrackpoint.position = (precisionGrab ? controllerPoint.position : currentGrabbedObject.transform.position);
            return returnTrackpoint;
        }

        /// <summary>
        /// The ProcessUpdate method is run in every Update method on the Interactable Object.
        /// </summary>
        public override void ProcessUpdate()
        {
            if (trackPoint != null)
            {
                float distance = Vector3.Distance(trackPoint.position, initialAttachPoint.position);
                if (distance > detachDistance && grabbedObjectScript.IsDroppable())
                {
                    ForceReleaseGrab();
                }
                else
                {
                    float grabDistance = Vector3.Distance(trackPoint.position, controllerAttachPoint.transform.position);
                    SetFrame(grabDistance + frameOffset);
                }
            }
        }

        /// <summary>
        /// The SetFrame method scrubs to the specific frame of the Animator timeline.
        /// </summary>
        /// <param name="frame">The frame to scrub to.</param>
        public virtual void SetFrame(float frame)
        {
            float setFrame = frame * distanceMultiplier;
            SetTimelineSpeed(animationSpeed);
            if (setFrame < maxFrames)
            {
                SetTimelinePosition(setFrame);
                if (setFrame == 0)
                {
                    OnAnimationFrameAtStart(SetEventPayload(setFrame));
                }
                OnAnimationFrameChanged(SetEventPayload(setFrame));
                currentFrame = frame;
                atEnd = false;
            }
            else if (!atEnd)
            {
                OnAnimationFrameAtEnd(SetEventPayload(setFrame));
                atEnd = true;
            }
        }

        /// <summary>
        /// The RewindAnimation method will force the animation to rewind to the start frame.
        /// </summary>
        public virtual void RewindAnimation()
        {
            CancelResetTimeline();
            resetTimelineRoutine = StartCoroutine(ResetTimeline(currentFrame));
        }

        protected virtual void OnDisable()
        {
            CancelResetTimeline();
        }

        protected override void Initialise()
        {
            tracked = false;
            climbable = false;
            kinematic = true;
            InitTimeline();
        }

        protected virtual void InitTimeline()
        {
            animatorTimeline = (animatorTimeline != null ? animatorTimeline : GetComponent<Animator>());
            animationTimeline = (animationTimeline != null ? animationTimeline : GetComponent<Animation>());
            if (animationTimeline != null)
            {
                if (!animationTimeline.clip.legacy)
                {
                    VRTK_Logger.Error("The `VRTK_ControlAnimationGrabAttach` script is using an `Animation Timeline` that has not been set to `Legacy Animation`. Only legacy animations are supported.");
                }

                foreach (AnimationState currentClip in animationTimeline)
                {
                    animationName = currentClip.name;
                    break;
                }
            }
            SetTimelineSpeed(animationSpeed);
        }

        protected virtual void SetTimelineSpeed(float speed)
        {
            if (animationTimeline != null)
            {
                animationTimeline[animationName].speed = speed;
            }
            else if (animatorTimeline != null)
            {
                animatorTimeline.speed = speed;
            }
        }

        protected virtual void SetTimelinePosition(float framePosition)
        {
            if (animationTimeline != null)
            {
                animationTimeline[animationName].time = framePosition;
                animationTimeline.Play(animationName);
            }
            else if (animatorTimeline != null)
            {
                animatorTimeline.Play(0, 0, framePosition);
            }
        }

        protected virtual void CancelResetTimeline()
        {
            if (resetTimelineRoutine != null)
            {
                StopCoroutine(resetTimelineRoutine);
            }
        }

        protected virtual IEnumerator ResetTimeline(float frame)
        {
            while (frame > 0f)
            {
                SetFrame(frame);
                frame -= Time.fixedDeltaTime * rewindSpeedMultplier;
                frameOffset = currentFrame;
                yield return null;
            }
            SetFrame(0f);
        }

        protected virtual ControlAnimationGrabAttachEventArgs SetEventPayload(float frame)
        {
            ControlAnimationGrabAttachEventArgs e;
            e.interactingObject = (grabbedObjectScript != null ? grabbedObjectScript.GetGrabbingObject() : null);
            e.currentFrame = frame;
            return e;
        }
    }
}