namespace VRTK.Prefabs.Interactions.SnapDropZone
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.PropertySetterMethod;
    using Malimbe.PropertyValidationMethod;
    using Malimbe.XmlDocumentationAttribute;

    /// <summary>
    /// Enables and disables an object's <see cref="Rigidbody.isKinematic"/> state.
    /// </summary>
    public class RigidbodyKinematicMutator : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="Rigidbody"/> to change the kinematic state on.
        /// </summary>
        [Serialized, Validated, Cleared]
        [field: DocumentedByXml]
        public Rigidbody Target { get; set; }
        /// <summary>
        /// Whether <see cref="Target"/> should be kinematic.
        /// </summary>
        [Serialized, Validated]
        [field: DocumentedByXml]
        public bool IsKinematic { get; set; }
        /// <summary>
        /// Whether to restore the kinematic state of the previous <see cref="Target"/> whenever it changes.
        /// </summary>
        [Serialized, Validated]
        [field: DocumentedByXml]
        public bool RestoresPreviousKinematicState { get; set; } = true;

        /// <summary>
        /// Whether <see cref="Target"/> was kinematic before changing the kinematic state.
        /// </summary>
        protected bool? wasKinematic;

        /// <summary>
        /// Handles changes to <see cref="Target"/>.
        /// </summary>
        /// <param name="previousValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        [CalledBySetter(nameof(Target))]
        public virtual void OnTargetChange(Rigidbody previousValue, ref Rigidbody newValue)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (previousValue != null && RestoresPreviousKinematicState && wasKinematic != null)
            {
                previousValue.isKinematic = wasKinematic.Value;
            }

            if (newValue == null)
            {
                wasKinematic = null;
                return;
            }

            wasKinematic = newValue.isKinematic;
            newValue.isKinematic = IsKinematic;
        }

        /// <summary>
        /// Handles changes to <see cref="IsKinematic"/>.
        /// </summary>
        /// <param name="previousValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        [CalledBySetter(nameof(IsKinematic))]
        public virtual void OnIsKinematicChange(bool previousValue, ref bool newValue)
        {
            if (!Application.isPlaying || Target == null)
            {
                return;
            }

            Target.isKinematic = newValue;
        }
    }
}
