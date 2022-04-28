namespace VRTK.Examples
{
    using UnityEngine;
    using Zinnia.Extension;

    /// <summary>
    /// Plays a Random <see cref="AudioClip"/> from the given selection.
    /// </summary>
    public class RandomAudioClipPlayer : MonoBehaviour
    {
        [Tooltip("The AudioSource to play the clip through.")]
        [SerializeField]
        private AudioSource audioSource;
        /// <summary>
        /// The <see cref="AudioSource"/> to play the clip through.
        /// </summary>
        public AudioSource AudioSource
        {
            get
            {
                return audioSource;
            }
            set
            {
                audioSource = value;
            }
        }
        [Tooltip("The AudioClip pool to choose a sound to play from.")]
        [SerializeField]
        private AudioClip[] clipPool;
        /// <summary>
        /// The <see cref="AudioClip"/> pool to choose a sound to play from.
        /// </summary>
        public AudioClip[] ClipPool
        {
            get
            {
                return clipPool;
            }
            set
            {
                clipPool = value;
            }
        }
        [Tooltip("Whether to stop the previous playing AudioClip.")]
        [SerializeField]
        private bool stopPreviousClip = true;
        /// <summary>
        /// Whether to stop the previous playing <see cref="AudioClip"/>.
        /// </summary>
        public bool StopPreviousClip
        {
            get
            {
                return stopPreviousClip;
            }
            set
            {
                stopPreviousClip = value;
            }
        }

        /// <summary>
        /// Plays a random <see cref="AudioClip"/> from the <see cref="ClipPool"/>.
        /// </summary>
        public virtual void PlayRandomOneShot()
        {
            if (!this.IsValidState())
            {
                return;
            }

            PlayOneShot(Random.Range(0, ClipPool.Length - 1));
        }

        /// <summary>
        /// Plays the <see cref="AudioClip"/> at the given location.
        /// </summary>
        /// <param name="clipIndex">The clip to play.</param>
        public virtual void PlayOneShot(int clipIndex)
        {
            if (!this.IsValidState())
            {
                return;
            }

            if (StopPreviousClip)
            {
                AudioSource.Stop();
            }
            AudioSource.PlayOneShot(ClipPool[clipIndex]);
        }
    }
}