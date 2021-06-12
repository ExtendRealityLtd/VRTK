namespace VRTK.Examples
{
    using Malimbe.BehaviourStateRequirementMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using UnityEngine;

    /// <summary>
    /// Plays a Random <see cref="AudioClip"/> from the given selection.
    /// </summary>
    public class RandomAudioClipPlayer : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="AudioSource"/> to play the clip through.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public AudioSource AudioSource { get; set; }
        /// <summary>
        /// The <see cref="AudioClip"/> pool to choose a sound to play from.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public AudioClip[] ClipPool { get; set; }
        /// <summary>
        /// Whether to stop the previous playing <see cref="AudioClip"/>.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool StopPreviousClip { get; set; } = true;

        /// <summary>
        /// Plays a random <see cref="AudioClip"/> from the <see cref="ClipPool"/>.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void PlayRandomOneShot()
        {
            PlayOneShot(Random.Range(0, ClipPool.Length - 1));
        }

        /// <summary>
        /// Plays the <see cref="AudioClip"/> at the given location.
        /// </summary>
        /// <param name="clipIndex">The clip to play.</param>
        [RequiresBehaviourState]
        public virtual void PlayOneShot(int clipIndex)
        {
            if (StopPreviousClip)
            {
                AudioSource.Stop();
            }
            AudioSource.PlayOneShot(ClipPool[clipIndex]);
        }
    }
}