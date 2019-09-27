namespace VRTK.Prefabs.Locomotion.Movement.SpatialManipulation
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Action;
    using Zinnia.Process;

    /// <summary>
    /// Provides a basis for manipulating an spatial object.
    /// </summary>
    public abstract class SpatialManipulator : MonoBehaviour, IProcessable
    {
        #region Object Settings
        /// <summary>
        /// The primary source to track positional and rotational data on to apply to the manipulator.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Object Settings"), DocumentedByXml]
        public GameObject PrimarySource { get; set; }
        /// <summary>
        /// The secondary source to track positional and rotational data on to apply to the manipulator.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject SecondarySource { get; set; }
        /// <summary>
        /// The target to apply the spatial manipulation to.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Target { get; set; }
        /// <summary>
        /// An optional offset to take into consideration when manipulating the target.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Offset { get; set; }
        /// <summary>
        /// Multiplies the result of the manupulation operation.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float Multiplier { get; set; } = 1f;
        #endregion

        #region Activation Settings
        /// <summary>
        /// The action that will enable the activation state.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Activation Settings"), DocumentedByXml]
        public BooleanAction ActivationAction { get; set; }
        /// <summary>
        /// The minimum value required to be considered active.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float ActivationThreshold { get; set; }
        #endregion

        /// <summary>
        /// Determines whether the manipulator was activated last frame.
        /// </summary>
        protected bool wasActivated;

        /// <summary>
        /// Processes the manipulation operation.
        /// </summary>
        public abstract void Process();

        /// <summary>
        /// Determines if the given object is valid.
        /// </summary>
        /// <param name="source">The object to check.</param>
        /// <returns>Whether the object is valid.</returns>
        protected virtual bool IsObjectValid(GameObject source)
        {
            return source != null && source.activeInHierarchy;
        }

        /// <summary>
        /// Gets the local position of the given source.
        /// </summary>
        /// <param name="source">The source to get the local position for.</param>
        /// <returns>The local position.</returns>
        protected virtual Vector3 GetLocalPosition(GameObject source)
        {
            return IsObjectValid(source) ? source.transform.localPosition : Vector3.zero;
        }
    }
}