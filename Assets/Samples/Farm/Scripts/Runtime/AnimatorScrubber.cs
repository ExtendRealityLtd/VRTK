namespace VRTK.Examples
{
    using UnityEngine;
    using Zinnia.Extension;

    /// <summary>
    /// Scrubs through an animation timeline to a specific normalized point along the timeline.
    /// </summary>
    public class AnimatorScrubber : MonoBehaviour
    {
        [Tooltip("The Animator to scrub.")]
        [SerializeField]
        private Animator timeline;
        /// <summary>
        /// The <see cref="Animator"/> to scrub.
        /// </summary>
        public Animator Timeline
        {
            get
            {
                return timeline;
            }
            set
            {
                timeline = value;
            }
        }
        [Tooltip("The name of the Animation to scrub.")]
        [SerializeField]
        private string animationName;
        /// <summary>
        /// The name of the <see cref="Animation"/> to scrub.
        /// </summary>
        public string AnimationName
        {
            get
            {
                return animationName;
            }
            set
            {
                animationName = value;
            }
        }

        /// <summary>
        /// Scrubs through the <see cref="Timeline"/> to the given value.
        /// </summary>
        /// <param name="value">The normalized position to scrub to.</param>
        public virtual void Scrub(float value)
        {
            if (!this.IsValidState())
            {
                return;
            }

            Timeline.speed = 0;
            Timeline.Play(AnimationName, -1, value);
        }

        protected virtual void OnEnable()
        {
            Timeline.speed = 0;
        }
    }
}