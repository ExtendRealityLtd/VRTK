namespace VRTK.Prefabs.Interactions.Interactors
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Collision;
    using Zinnia.Tracking.Collision.Active;
    using Zinnia.Tracking.Collision.Active.Operation;

    /// <summary>
    /// Sets up the Interactor Prefab touch settings based on the provided user settings.
    /// </summary>
    public class TouchInteractorInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected InteractorFacade facade;
        #endregion

        #region Touch Settings
        /// <summary>
        /// The <see cref="ActiveCollisionsContainer"/> that holds all current collisions.
        /// </summary>
        [Header("Touch Settings"), Tooltip("The ActiveCollisionsContainer that holds all current collisions."), InternalSetting, SerializeField]
        protected ActiveCollisionsContainer activeCollisionsContainer;
        /// <summary>
        /// The <see cref="Slicer"/> that holds the current active collision.
        /// </summary>
        [Tooltip("The Slicer that holds the current active collision."), InternalSetting, SerializeField]
        protected Slicer currentActiveCollision;
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid start touching collisions.
        /// </summary>
        [Tooltip("The ActiveCollisionPublisher for checking valid start touching collisions."), InternalSetting, SerializeField]
        protected ActiveCollisionPublisher startTouchingPublisher;
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid stop touching collisions.
        /// </summary>
        [Tooltip("The ActiveCollisionPublisher for checking valid stop touching collisions."), InternalSetting, SerializeField]
        protected ActiveCollisionPublisher stopTouchingPublisher;
        #endregion

        /// <summary>
        /// A collection of currently touched GameObjects.
        /// </summary>
        public List<GameObject> TouchedObjects => GetTouchedObjects();
        /// <summary>
        /// The currently active touched GameObject.
        /// </summary>
        public GameObject ActiveTouchedObject => GetActiveTouchedObject();

        /// <summary>
        /// Configures the <see cref="ActiveCollisionPublisher"/> components for touching and untouching.
        /// </summary>
        public virtual void ConfigurePublishers()
        {
            if (startTouchingPublisher != null)
            {
                startTouchingPublisher.payload.sourceContainer = facade.gameObject;
            }

            if (stopTouchingPublisher != null)
            {
                stopTouchingPublisher.payload.sourceContainer = facade.gameObject;
            }
        }

        protected virtual void OnEnable()
        {
            ConfigurePublishers();
        }

        protected virtual void OnDisable()
        {
            stopTouchingPublisher.ForceSetActiveCollisions(startTouchingPublisher.payload);
            stopTouchingPublisher.ForcePublish();
        }

        /// <summary>
        /// Retrieves a collection of currently touched GameObjects.
        /// </summary>
        /// <returns>The currently touched GameObjects.</returns>
        protected virtual List<GameObject> GetTouchedObjects()
        {
            List<GameObject> returnList = new List<GameObject>();

            if (activeCollisionsContainer == null)
            {
                return returnList;
            }

            foreach (CollisionNotifier.EventData element in activeCollisionsContainer.Elements)
            {
                returnList.Add(element.collider.GetContainingTransform().gameObject);
            }

            return returnList;
        }

        /// <summary>
        /// Retrieves the currently active touched GameObject.
        /// </summary>
        /// <returns>The currently active touched GameObject.</returns>
        protected virtual GameObject GetActiveTouchedObject()
        {
            if (currentActiveCollision == null || currentActiveCollision.SlicedList.activeCollisions.Count == 0)
            {
                return null;
            }

            return currentActiveCollision.SlicedList.activeCollisions[0].collider.GetContainingTransform().gameObject;
        }
    }
}