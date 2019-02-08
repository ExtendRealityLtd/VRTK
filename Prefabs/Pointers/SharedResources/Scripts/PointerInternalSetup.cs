namespace VRTK.Prefabs.Pointers
{
    using UnityEngine;
    using Zinnia.Cast;
    using Zinnia.Action;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Follow;

    /// <summary>
    /// Sets up the Pointer Prefab based on the provided user settings.
    /// </summary>
    public class PointerInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected PointerFacade facade;
        #endregion

        #region Object Follow Settings
        /// <summary>
        /// The <see cref="ObjectFollower"/> component for the Pointer.
        /// </summary>
        [Header("Object Follow Settings"), Tooltip("The ObjectFollower component for the Pointer."), InternalSetting, SerializeField]
        protected ObjectFollower objectFollow;
        #endregion

        #region Cast Settings
        /// <summary>
        /// The <see cref="PointsCast"/> component for the Pointer.
        /// </summary>
        [Header("Cast Settings"), Tooltip("The PointsCast component for the Pointer."), InternalSetting, SerializeField]
        protected PointsCast caster;
        #endregion

        #region Action Settings
        /// <summary>
        /// The <see cref="BooleanAction"/> that will activate/deactivate the pointer.
        /// </summary>
        [Header("Action Settings"), Tooltip("The BooleanAction that will activate/deactivate the pointer."), InternalSetting, SerializeField]
        protected BooleanAction activationAction;
        /// <summary>
        /// The <see cref="BooleanAction"/> that initiates the pointer selection when the action is activated.
        /// </summary>
        [Tooltip("The BooleanAction that initiates the pointer selection when the action is activated."), InternalSetting, SerializeField]
        protected BooleanAction selectOnActivatedAction;
        /// <summary>
        /// The <see cref="BooleanAction"/> that initiates the pointer selection when the action is deactivated.
        /// </summary>
        [Tooltip("The BooleanAction that initiates the pointer selection when the action is deactivated."), InternalSetting, SerializeField]
        protected BooleanAction selectOnDeactivatedAction;
        #endregion

        /// <summary>
        /// Configures the target validity based on the facade settings.
        /// </summary>
        public virtual void ConfigureTargetValidity()
        {
            caster.targetValidity = facade.TargetValidity;
        }

        /// <summary>
        /// Configures the object follow sources based on the facade settings.
        /// </summary>
        public virtual void ConfigureFollowSources()
        {
            if (facade.FollowSource != null)
            {
                objectFollow.ClearSources();
                objectFollow.AddSource(facade.FollowSource);
            }
        }

        /// <summary>
        /// Configures the selection action on the facade settings.
        /// </summary>
        public virtual void ConfigureSelectionAction()
        {
            if (facade.SelectionAction != null)
            {
                selectOnActivatedAction.ClearSources();
                selectOnActivatedAction.AddSource(facade.SelectionAction);
                selectOnDeactivatedAction.ClearSources();
                selectOnDeactivatedAction.AddSource(facade.SelectionAction);
            }
        }

        /// <summary>
        /// Configures the activation action based on the facade settings.
        /// </summary>
        public virtual void ConfigureActivationAction()
        {
            if (facade.ActivationAction != null)
            {
                activationAction.ClearSources();
                activationAction.AddSource(facade.ActivationAction);
            }
        }

        /// <summary>
        /// Configures the selection type based on the facade settings.
        /// </summary>
        public virtual void ConfigureSelectionType()
        {
            switch (facade.SelectionMethod)
            {
                case PointerFacade.SelectionType.SelectOnActivate:
                    selectOnActivatedAction.gameObject.SetActive(true);
                    selectOnDeactivatedAction.gameObject.SetActive(false);
                    break;
                case PointerFacade.SelectionType.SelectOnDeactivate:
                    selectOnActivatedAction.gameObject.SetActive(false);
                    selectOnDeactivatedAction.gameObject.SetActive(true);
                    break;
            }
        }

        protected virtual void OnEnable()
        {
            ConfigureTargetValidity();
            ConfigureFollowSources();
            ConfigureSelectionAction();
            ConfigureActivationAction();
            ConfigureSelectionType();
        }
    }
}