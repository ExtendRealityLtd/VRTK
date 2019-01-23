namespace VRTK.Prefabs.Pointers
{
    using UnityEngine;
    using Zinnia.Rule;
    using Zinnia.Action;
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
        [Header("Pointer Settings"), Tooltip("The source for the pointer origin to follow."), SerializeField]
        private GameObject _followSource;
        /// <summary>
        /// The source for the pointer origin to follow.
        /// </summary>
        public GameObject FollowSource
        {
            get { return _followSource; }
            set
            {
                _followSource = value;
                internalSetup.ConfigureFollowSources();
            }
        }

        [Tooltip("The BooleanAction that will activate/deactivate the pointer."), SerializeField]
        private BooleanAction _activationAction;
        /// <summary>
        /// The <see cref="BooleanAction"/> that will activate/deactivate the pointer.
        /// </summary>
        public BooleanAction ActivationAction
        {
            get { return _activationAction; }
            set
            {
                _activationAction = value;
                internalSetup.ConfigureActivationAction();
            }
        }

        [Tooltip("The BooleanAction that initiates the pointer selection."), SerializeField]
        private BooleanAction _selectionAction;
        /// <summary>
        /// The <see cref="BooleanAction"/> that initiates the pointer selection.
        /// </summary>
        public BooleanAction SelectionAction
        {
            get { return _selectionAction; }
            set
            {
                _selectionAction = value;
                internalSetup.ConfigureSelectionAction();
            }
        }

        [Tooltip("The action moment when to initiate the select action."), SerializeField]
        private SelectionType _selectionMethod = SelectionType.SelectOnActivate;
        /// <summary>
        /// The action moment when to initiate the select action.
        /// </summary>
        public SelectionType SelectionMethod
        {
            get { return _selectionMethod; }
            set
            {
                _selectionMethod = value;
                internalSetup.ConfigureSelectionType();
            }
        }

        /// <summary>
        /// Allows to optionally determine targets based on the set rules.
        /// </summary>
        [Tooltip("Allows to optionally determine targets based on the set rules."), SerializeField]
        private RuleContainer _targetValidity;
        public RuleContainer TargetValidity
        {
            get { return _targetValidity; }
            set
            {
                _targetValidity = value;
                internalSetup.ConfigureTargetValidity();
            }
        }
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected PointerInternalSetup internalSetup;
        #endregion

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            internalSetup.ConfigureTargetValidity();
            internalSetup.ConfigureFollowSources();
            internalSetup.ConfigureSelectionAction();
            internalSetup.ConfigureActivationAction();
            internalSetup.ConfigureSelectionType();
        }
    }
}