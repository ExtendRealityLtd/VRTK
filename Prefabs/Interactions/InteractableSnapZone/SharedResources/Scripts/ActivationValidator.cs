namespace VRTK.Prefabs.Interactions.InteractableSnapZone
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Interactions.Interactables;

    /// <summary>
    /// Determines if the collided SnapZone is valid for activation based on whether another snap zone is already holding the activated state.
    /// </summary>
    public class ActivationValidator : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the <see cref="GameObject"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<GameObject> { }

        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public SnapZoneFacade Facade { get; protected set; }
        #endregion

        /// <summary>
        /// Emitted when the SnapZone activation is validated.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Validated = new UnityEvent();

        /// <summary>
        /// Determines if the SnapZone is currently activated.
        /// </summary>
        public bool IsActivated { get; protected set; }

        /// <summary>
        /// A unique reference for a listener based upon the interactable and SnapZone being activated.
        /// </summary>
        protected struct ListenerKey
        {
            private readonly GameObject interactable;
            private readonly SnapZoneActivator zone;

            public ListenerKey(GameObject interactable, SnapZoneActivator zone)
            {
                this.interactable = interactable;
                this.zone = zone;
            }
        }

        /// <summary>
        /// The <see cref="GameObject"/> that is currently activating this SnapZone.
        /// </summary>
        protected GameObject currentActivatingGameObject;
        /// <summary>
        /// The <see cref="InteractableFacade"/> associated to the <see cref="GameObject"/> that is currently activating this SnapZone.
        /// </summary>
        protected InteractableFacade currentActivatingInteractable;
        /// <summary>
        /// A collection of listeners registered with the SnapZone that is being activated by a given interactable <see cref="GameObject"/>.
        /// </summary>
        protected Dictionary<ListenerKey, UnityAction<GameObject>> activatingZoneListeners = new Dictionary<ListenerKey, UnityAction<GameObject>>();
        /// <summary>
        /// A collection of listeners registered with this SnapZone.
        /// </summary>
        protected Dictionary<ListenerKey, UnityAction<GameObject>> currentZoneListeners = new Dictionary<ListenerKey, UnityAction<GameObject>>();

        /// <summary>
        /// Attempts to activate the SnapZone if the colliding <see cref="GameObject"/> is not already activating another SnapZone.
        /// </summary>
        /// <param name="activator">The colliding interactable.</param>
        public virtual void Activate(GameObject activator)
        {
            IsActivated = false;
            TrySetInteractableFacade(activator);
            if (currentActivatingInteractable == null || Facade.Configuration.ActivationArea == null || !Facade.Configuration.CollidingObjectsList.Contains(currentActivatingInteractable.gameObject))
            {
                return;
            }

            currentActivatingInteractable.ActiveCollisions.AddUnique(Facade.Configuration.ActivationArea.gameObject);

            foreach (GameObject collidingObject in currentActivatingInteractable.ActiveCollisions.SubscribableElements)
            {
                if (collidingObject == null)
                {
                    continue;
                }

                SnapZoneActivator activatingZone = collidingObject.GetComponent<SnapZoneActivator>();
                if (activatingZone == null)
                {
                    continue;
                }

                if (activatingZone == Facade.Configuration.ActivationArea)
                {
                    if (!IsActivated)
                    {
                        Facade.Configuration.EmitActivated(activator);
                    }
                    IsActivated = true;
                    Validated?.Invoke(activator);
                    break;
                }
                else
                {
                    ListenerKey listenerKey = new ListenerKey(activator, activatingZone);

                    if (!activatingZoneListeners.ContainsKey(listenerKey))
                    {
                        UnityAction<GameObject> onExitActivatingZoneListener = activatingInteractable => AttemptReactivation(activatingInteractable, activatingZone);
                        activatingZone.Facade.Exited.AddListener(onExitActivatingZoneListener);
                        activatingZoneListeners.Add(listenerKey, onExitActivatingZoneListener);
                    }

                    if (!currentZoneListeners.ContainsKey(listenerKey))
                    {
                        UnityAction<GameObject> onExitCurrentZoneListener = activatingInteractable => CancelAttemptReactivation(activatingInteractable, activatingZone);
                        Facade.Exited.AddListener(onExitCurrentZoneListener);
                        currentZoneListeners.Add(listenerKey, onExitCurrentZoneListener);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Attempts to Deactivate the SnapZone if it is already activated.
        /// </summary>
        /// <param name="deactivator">The interactable that is no longer colliding with the SnapZone.</param>
        public virtual void Deactivate(GameObject deactivator)
        {
            if (IsActivated)
            {
                IsActivated = false;
                Facade.Deactivated?.Invoke(deactivator);
            }
        }

        /// <summary>
        /// Attempts activate this SnapZone with the given <see cref="GameObject"/>.
        /// </summary>
        /// <param name="activator">The colliding interactable.</param>
        /// <param name="activatingZone">The SnapZone that was previously being activated by the colliding interactable.</param>
        protected virtual void AttemptReactivation(GameObject activator, SnapZoneActivator activatingZone)
        {
            activator.TryGetComponent<InteractableFacade>(true, true).ActiveCollisions.Remove(activatingZone.Facade.Configuration.ActivationArea.gameObject);
            ListenerKey listenerKey = new ListenerKey(activator, activatingZone);
            activatingZoneListeners.TryGetValue(listenerKey, out UnityAction<GameObject> activatingZoneListener);
            if (activatingZoneListener != null)
            {
                activatingZone.Facade.Exited.RemoveListener(activatingZoneListener);
            }
            activatingZoneListeners.Remove(listenerKey);
            Activate(activator);
        }

        /// <summary>
        /// Cancels the attempt to activate the SnapZone unpon the previous activating SnapZone becoming deactivated.
        /// </summary>
        /// <param name="activator">The colliding interactable.</param>
        /// <param name="activatingZone">The SnapZone that was previously being activated by the colliding interactable.</param>
        protected virtual void CancelAttemptReactivation(GameObject activator, SnapZoneActivator activatingZone)
        {
            ListenerKey listenerKey = new ListenerKey(activator, activatingZone);

            activatingZoneListeners.TryGetValue(listenerKey, out UnityAction<GameObject> onExitActivatingZoneListener);
            if (onExitActivatingZoneListener != null)
            {
                activatingZone.Facade.Exited.RemoveListener(onExitActivatingZoneListener);
                activatingZoneListeners.Remove(listenerKey);
            }

            currentZoneListeners.TryGetValue(listenerKey, out UnityAction<GameObject> onExitCurrentZoneListener);
            if (onExitCurrentZoneListener != null)
            {
                Facade.Exited.RemoveListener(onExitCurrentZoneListener);
                currentZoneListeners.Remove(listenerKey);
            }
        }

        /// <summary>
        /// Tries and retrieves the <see cref="InteractableFacade"/> associated with the given valid snappable <see cref="GameObject"/>.
        /// </summary>
        /// <param name="container">The colliding interactable.</param>
        protected virtual void TrySetInteractableFacade(GameObject container)
        {
            if ((container != null && container != currentActivatingGameObject) || currentActivatingInteractable == null)
            {
                currentActivatingGameObject = container;
                currentActivatingInteractable = currentActivatingGameObject.TryGetComponent<InteractableFacade>(true, true);
            }

            if (currentActivatingInteractable == null)
            {
                throw new NullReferenceException("The given container must contain an InteractableFacade.");
            }
        }
    }
}