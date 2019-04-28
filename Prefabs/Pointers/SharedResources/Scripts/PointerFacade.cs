namespace VRTK.Prefabs.Pointers
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.MemberChangeMethod;
    using Zinnia.Rule;
    using Zinnia.Action;
    using Zinnia.Pointer;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into the Pointer Prefab.
    /// </summary>
    public class PointerFacade : MonoBehaviour
    {
        /// <summary>
        /// The pointer selection type.
        /// </summary>
        public enum SelectionType
        {
            /// <summary>
            /// Initiates the select action when the selection action is activated (e.g. button pressed).
            /// </summary>
            SelectOnActivate,
            /// <summary>
            /// Initiates the select action when the selection action is deactivated (e.g. button released).
            /// </summary>
            SelectOnDeactivate
        }

        #region Pointer Settings
        /// <summary>
        /// The source for the pointer origin to follow.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Pointer Settings"), DocumentedByXml]
        public GameObject FollowSource { get; set; }
        /// <summary>
        /// The <see cref="BooleanAction"/> that will activate/deactivate the pointer.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public BooleanAction ActivationAction { get; set; }
        /// <summary>
        /// The <see cref="BooleanAction"/> that initiates the pointer selection.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public BooleanAction SelectionAction { get; set; }
        /// <summary>
        /// The action moment when to initiate the select action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public SelectionType SelectionMethod { get; set; }
        /// <summary>
        /// Allows to optionally determine targets based on the set rules.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public RuleContainer TargetValidity { get; set; }
        #endregion

        #region Pointer Events
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> becomes active.
        /// </summary>
        [Header("Pointer Events"), DocumentedByXml]
        public ObjectPointer.UnityEvent Activated = new ObjectPointer.UnityEvent();
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> is deactivated.
        /// </summary>
        [DocumentedByXml]
        public ObjectPointer.UnityEvent Deactivated = new ObjectPointer.UnityEvent();
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> collides with a new target.
        /// </summary>
        [DocumentedByXml]
        public ObjectPointer.UnityEvent Entered = new ObjectPointer.UnityEvent();
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> stops colliding with an existing target.
        /// </summary>
        [DocumentedByXml]
        public ObjectPointer.UnityEvent Exited = new ObjectPointer.UnityEvent();
        /// <summary>
        /// Emitted when the <see cref="ObjectPointer"/> changes its hovering position over an existing target.
        /// </summary>
        [DocumentedByXml]
        public ObjectPointer.UnityEvent HoverChanged = new ObjectPointer.UnityEvent();
        /// <summary>
        /// Emitted whenever <see cref="ObjectPointer.Select"/> is called.
        /// </summary>
        [DocumentedByXml]
        public ObjectPointer.UnityEvent Selected = new ObjectPointer.UnityEvent();
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public PointerConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Called after <see cref="FollowSource"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(FollowSource))]
        protected virtual void OnAfterFollowSourceChange()
        {
            Configuration.ConfigureFollowSources();
        }

        /// <summary>
        /// Called after <see cref="ActivationAction"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(ActivationAction))]
        protected virtual void OnAfterActivationActionChange()
        {
            Configuration.ConfigureActivationAction();
        }

        /// <summary>
        /// Called after <see cref="SelectionAction"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SelectionAction))]
        protected virtual void OnAfterSelectionActionChange()
        {
            Configuration.ConfigureSelectionAction();
        }

        /// <summary>
        /// Called after <see cref="SelectionMethod"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SelectionMethod))]
        protected virtual void OnAfterSelectionMethodChange()
        {
            Configuration.ConfigureSelectionType();
        }

        /// <summary>
        /// Called after <see cref="TargetValidity"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(TargetValidity))]
        protected virtual void OnAfterTargetValidityChange()
        {
            Configuration.ConfigureTargetValidity();
        }
    }
}