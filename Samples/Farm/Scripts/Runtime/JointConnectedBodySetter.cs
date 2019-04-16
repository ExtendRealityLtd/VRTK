namespace VRTK.Examples
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;

    public class JointConnectedBodySetter : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="Rigidbody"/> to use as the source of the connected body.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public Rigidbody Source { get; set; }
        /// <summary>
        /// The <see cref="Joint"/> to set the connected body property on.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public Joint Target { get; set; }

        /// <summary>
        /// Sets the <see cref="Source"/> parameter from <see cref="GameObject"/> data.
        /// </summary>
        /// <param name="source">The <see cref="GameObject"/> to retrieve the <see cref="Rigidbody"/> from.</param>
        [RequiresBehaviourState]
        public virtual void SetSource(GameObject source)
        {
            Source = source != null ? source.GetComponent<Rigidbody>() : null;
        }

        /// <summary>
        /// Connects the <see cref="Source"/> to the <see cref="Target"/> connected body.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void ConnectSourceToTarget()
        {
            Target.connectedBody = Source;
        }
    }
}