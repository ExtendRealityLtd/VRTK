namespace VRTK.Examples
{
    using UnityEngine;
    using Zinnia.Extension;

    public class JointConnectedBodySetter : MonoBehaviour
    {
        [Tooltip("The Rigidbody to use as the source of the connected body.")]
        [SerializeField]
        private Rigidbody source;
        /// <summary>
        /// The <see cref="Rigidbody"/> to use as the source of the connected body.
        /// </summary>
        public Rigidbody Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }
        [Tooltip("The Joint to set the connected body property on.")]
        [SerializeField]
        private Joint target;
        /// <summary>
        /// The <see cref="Joint"/> to set the connected body property on.
        /// </summary>
        public Joint Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="Source"/> parameter from <see cref="GameObject"/> data.
        /// </summary>
        /// <param name="source">The <see cref="GameObject"/> to retrieve the <see cref="Rigidbody"/> from.</param>
        public virtual void SetSource(GameObject source)
        {
            if (!this.IsValidState())
            {
                return;
            }

            Source = source != null ? source.GetComponent<Rigidbody>() : null;
        }

        /// <summary>
        /// Connects the <see cref="Source"/> to the <see cref="Target"/> connected body.
        /// </summary>
        public virtual void ConnectSourceToTarget()
        {
            if (!this.IsValidState())
            {
                return;
            }

            Target.connectedBody = Source;
        }

        /// <summary>
        /// Clears the <see cref="Source"/>.
        /// </summary>
        public virtual void ClearSource()
        {
            if (!this.IsValidState())
            {
                return;
            }

            Source = default;
        }

        /// <summary>
        /// Clears the <see cref="Target"/>.
        /// </summary>
        public virtual void ClearTarget()
        {
            if (!this.IsValidState())
            {
                return;
            }

            Target = default;
        }
    }
}