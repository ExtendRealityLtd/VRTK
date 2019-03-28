namespace VRTK.Prefabs.Pointers
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Cast;
    using Zinnia.Action;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Follow;

    /// <summary>
    /// Sets up the Pointer Prefab based on the provided user settings.
    /// </summary>
    public class PointerConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public PointerFacade Facade { get; protected set; }
        #endregion

        #region Object Follow Settings
        /// <summary>
        /// The <see cref="ObjectFollower"/> component for the Pointer.
        /// </summary>
        [Serialized]
        [field: Header("Object Follow Settings"), DocumentedByXml, Restricted]
        public ObjectFollower ObjectFollow { get; protected set; }
        #endregion

        #region Cast Settings
        /// <summary>
        /// The <see cref="PointsCast"/> component for the Pointer.
        /// </summary>
        [Serialized]
        [field: Header("Cast Settings"), DocumentedByXml, Restricted]
        public PointsCast Caster { get; protected set; }
        #endregion

        #region Action Settings
        /// <summary>
        /// The <see cref="BooleanAction"/> that will activate/deactivate the pointer.
        /// </summary>
        [Serialized]
        [field: Header("Action Settings"), DocumentedByXml, Restricted]
        public BooleanAction ActivationAction { get; protected set; }
        /// <summary>
        /// The <see cref="BooleanAction"/> that initiates the pointer selection when the action is activated.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public BooleanAction SelectOnActivatedAction { get; protected set; }
        /// <summary>
        /// The <see cref="BooleanAction"/> that initiates the pointer selection when the action is deactivated.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public BooleanAction SelectOnDeactivatedAction { get; protected set; }
        #endregion

        /// <summary>
        /// Configures the target validity based on the facade settings.
        /// </summary>
        public virtual void ConfigureTargetValidity()
        {
            Caster.TargetValidity = Facade.TargetValidity;
        }

        /// <summary>
        /// Configures the object follow sources based on the facade settings.
        /// </summary>
        public virtual void ConfigureFollowSources()
        {
            if (Facade.FollowSource != null)
            {
                ObjectFollow.Sources.RunWhenActiveAndEnabled(() => ObjectFollow.Sources.Clear());
                ObjectFollow.Sources.RunWhenActiveAndEnabled(() => ObjectFollow.Sources.Add(Facade.FollowSource));
            }
        }

        /// <summary>
        /// Configures the selection action on the facade settings.
        /// </summary>
        public virtual void ConfigureSelectionAction()
        {
            if (Facade.SelectionAction != null)
            {
                SelectOnActivatedAction.RunWhenActiveAndEnabled(() => SelectOnActivatedAction.ClearSources());
                SelectOnActivatedAction.RunWhenActiveAndEnabled(() => SelectOnActivatedAction.AddSource(Facade.SelectionAction));
                SelectOnDeactivatedAction.RunWhenActiveAndEnabled(() => SelectOnDeactivatedAction.ClearSources());
                SelectOnDeactivatedAction.RunWhenActiveAndEnabled(() => SelectOnDeactivatedAction.AddSource(Facade.SelectionAction));
            }
        }

        /// <summary>
        /// Configures the activation action based on the facade settings.
        /// </summary>
        public virtual void ConfigureActivationAction()
        {
            if (Facade.ActivationAction != null)
            {
                ActivationAction.RunWhenActiveAndEnabled(() => ActivationAction.ClearSources());
                ActivationAction.RunWhenActiveAndEnabled(() => ActivationAction.AddSource(Facade.ActivationAction));
            }
        }

        /// <summary>
        /// Configures the selection type based on the facade settings.
        /// </summary>
        public virtual void ConfigureSelectionType()
        {
            switch (Facade.SelectionMethod)
            {
                case PointerFacade.SelectionType.SelectOnActivate:
                    SelectOnActivatedAction.gameObject.SetActive(true);
                    SelectOnDeactivatedAction.gameObject.SetActive(false);
                    break;
                case PointerFacade.SelectionType.SelectOnDeactivate:
                    SelectOnActivatedAction.gameObject.SetActive(false);
                    SelectOnDeactivatedAction.gameObject.SetActive(true);
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