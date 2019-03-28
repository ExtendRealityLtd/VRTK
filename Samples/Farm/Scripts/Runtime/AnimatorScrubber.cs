namespace VRTK.Examples
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;

    /// <summary>
    /// Scrubs through an animation timeline to a specific normalized point along the timeline.
    /// </summary>
    public class AnimatorScrubber : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="Animator"/> to scrub.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Animator Timeline { get; set; }
        /// <summary>
        /// The the name of the <see cref="Animation"/> to scrub.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public string AnimationName { get; set; }

        /// <summary>
        /// Scrubs through the <see cref="Timeline"/> to the given value.
        /// </summary>
        /// <param name="value">The normalized position to scrub to.</param>
        [RequiresBehaviourState]
        public virtual void Scrub(float value)
        {
            Timeline.speed = 0;
            Timeline.Play(AnimationName, -1, value);
        }

        protected virtual void OnEnable()
        {
            Timeline.speed = 0;
        }
    }
}