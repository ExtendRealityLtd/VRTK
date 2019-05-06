namespace VRTK.Prefabs.Locomotion.DestinationLocations
{
    using UnityEngine;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Extension;
    using Zinnia.Data.Type;

    /// <summary>
    /// Handles receiving and dispatching appropriate actions to the relevant <see cref="DestinationLocation"/>.
    /// </summary>
    public class ActionDispatcher : MonoBehaviour
    {
        /// <summary>
        /// The currently selected <see cref="DestinationLocation"/>.
        /// </summary>
        public DestinationLocation SelectedLocation { get; protected set; }

        /// <summary>
        /// Handles <see cref="SurfaceData"/> potentially entering a <see cref="DestinationLocation"/>.
        /// </summary>
        /// <param name="data">The potential interaction of something entering a <see cref="DestinationLocation"/>.</param>
        [RequiresBehaviourState]
        public void Enter(SurfaceData data)
        {
            DestinationLocation location = GetLocation(data);
            if (location == null)
            {
                return;
            }

            location.Enter(data);
        }

        /// <summary>
        /// Handles <see cref="SurfaceData"/> potentially exiting a <see cref="DestinationLocation"/>.
        /// </summary>
        /// <param name="data">The potential interaction of something exiting a <see cref="DestinationLocation"/>.</param>
        [RequiresBehaviourState]
        public void Exit(SurfaceData data)
        {
            DestinationLocation location = GetLocation(data);
            if (location == null)
            {
                return;
            }

            location.Exit(data);
        }

        /// <summary>
        /// Handles <see cref="SurfaceData"/> potentially making a selection on a <see cref="DestinationLocation"/>.
        /// </summary>
        /// <param name="data">The potential interaction of something selecting a <see cref="DestinationLocation"/>.</param>
        [RequiresBehaviourState]
        public void Select(SurfaceData data)
        {
            if (SelectedLocation != null && data != null)
            {
                SelectedLocation.Deselect();
            }

            DestinationLocation location = GetLocation(data);
            if (location == null)
            {
                return;
            }

            location.Select(data);
            SelectedLocation = location;
        }

        /// <summary>
        /// Gets a <see cref="DestinationLocation"/> if one exists in the given <see cref="SurfaceData"/> colliding transform or parent.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <returns>The found <see cref="DestinationLocation"/>.</returns>
        protected virtual DestinationLocation GetLocation(SurfaceData data)
        {
            if (data == null || data.CollisionData.transform == null)
            {
                return null;
            }

            return data.CollisionData.transform.gameObject.TryGetComponent<DestinationLocation>(false, true);
        }
    }
}