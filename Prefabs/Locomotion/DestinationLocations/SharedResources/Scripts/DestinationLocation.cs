namespace VRTK.Prefabs.Locomotion.DestinationLocations
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Rule;
    using Zinnia.Data.Type;
    using Zinnia.Extension;

    /// <summary>
    /// Represents an intersectable volume that can defer its <see cref="Transform"/> properties to another location.
    /// </summary>
    public class DestinationLocation : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the specified <see cref="SurfaceData"/>.
        /// </summary>
        [Serializable]
        public class SurfaceDataUnityEvent : UnityEvent<SurfaceData> { }
        /// <summary>
        /// Defines the event with the specified <see cref="TransformData"/>.
        /// </summary>
        [Serializable]
        public class TransformDataUnityEvent : UnityEvent<TransformData> { }

        #region Location Settings
        /// <summary>
        /// The destination to defer the selected action output to.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Location Settings"), DocumentedByXml]
        public GameObject Destination { get; set; }
        /// <summary>
        /// The overriding origin of where to lock any destination points to.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Origin { get; set; }
        /// <summary>
        /// Apply the rotation of the <see cref="Destination"/> to the selected action output.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool ApplyDestinationRotation { get; set; } = true;
        /// <summary>
        /// Emits the <see cref="Exited"/> event for all <see cref="HoveringElements"/> when <see cref="Select(SurfaceData)"/> is executed.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool EmitExitOnSelect { get; set; } = true;
        /// <summary>
        /// Allows to optionally determine which <see cref="SurfaceData"/> sources can affect the location.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public RuleContainer SourceValidity { get; set; }
        #endregion

        #region Location Events
        /// <summary>
        /// Emitted when the Destination Location is entered for the first time.
        /// </summary>
        [Header("Location Events"), DocumentedByXml]
        public UnityEvent HoverActivated = new UnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is entered.
        /// </summary>
        [DocumentedByXml]
        public SurfaceDataUnityEvent Entered = new SurfaceDataUnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is exited.
        /// </summary>
        [DocumentedByXml]
        public SurfaceDataUnityEvent Exited = new SurfaceDataUnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is exited for the last time.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent HoverDeactivated = new UnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is activated.
        /// </summary>
        [DocumentedByXml]
        public TransformDataUnityEvent Activated = new TransformDataUnityEvent();
        /// <summary>
        /// Emitted when the Destination Location is deactivated.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Deactivated = new UnityEvent();
        #endregion

        /// <summary>
        /// Whether the Destination Location is currently being hovered over.
        /// </summary>
        public bool IsHovered { get; protected set; }
        /// <summary>
        /// Whether the Destination Location is currently activated.
        /// </summary>
        public bool IsActivated { get; protected set; }

        /// <summary>
        /// A collection of <see cref="SurfaceData"/> elements that are currently hovering over the Destination Location.
        /// </summary>
        public List<SurfaceData> HoveringElements { get; protected set; } = new List<SurfaceData>();

        /// <summary>
        /// The payload to emit when the Destination Location is selected.
        /// </summary>
        protected TransformData selectedPayload = new TransformData();
        /// <summary>
        /// A temporary container to store and mutate <see cref="RaycastHit"/> data from the hovering <see cref="SurfaceData"/>.
        /// </summary>
        protected RaycastHit hoverHit = new RaycastHit();

        /// <summary>
        /// Handles a new enter action.
        /// </summary>
        /// <param name="data">The data that is entering the location.</param>
        [RequiresBehaviourState]
        public virtual void Enter(SurfaceData data)
        {
            if (data.Transform == null || !SourceValidity.Accepts(data.Transform))
            {
                return;
            }

            IsHovered = true;
            HoveringElements.Add(data);
            if (HoveringElements.Count == 1)
            {
                HoverActivated?.Invoke();
            }
            Entered?.Invoke(CreateHoverPayload(data));
        }

        /// <summary>
        /// Handles a new exit action.
        /// </summary>
        /// <param name="data">The data that is exiting the location.</param>
        [RequiresBehaviourState]
        public virtual void Exit(SurfaceData data)
        {
            if (data.Transform == null || !SourceValidity.Accepts(data.Transform) || !HoveringElements.Contains(data))
            {
                return;
            }

            IsHovered = false;
            HoveringElements.Remove(data);
            Exited?.Invoke(CreateHoverPayload(data));
            if (HoveringElements.Count == 0)
            {
                HoverDeactivated?.Invoke();
            }
        }

        /// <summary>
        /// Handles a new select action.
        /// </summary>
        /// <param name="data">The data that is selecting the location.</param>
        [RequiresBehaviourState]
        public virtual void Select(SurfaceData data)
        {
            if (data.Transform == null || !SourceValidity.Accepts(data.Transform) || !HoveringElements.Contains(data))
            {
                return;
            }

            IsActivated = true;
            Activated?.Invoke(CreateSelectedPayload(data));

            if (!EmitExitOnSelect)
            {
                return;
            }

            foreach (SurfaceData element in HoveringElements.ToArray())
            {
                Exit(element);
            }
            HoveringElements.Clear();
        }

        /// <summary>
        /// Handles a new deselect action.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void Deselect()
        {
            IsActivated = false;
            Deactivated?.Invoke();
        }

        /// <summary>
        /// Creates the payload to emit on the hovering events of <see cref="Entered"/> and <see cref="Exited"/>.
        /// </summary>
        /// <param name="data">The data that is mutating the hover state.</param>
        /// <returns>The data to emit.</returns>
        protected virtual SurfaceData CreateHoverPayload(SurfaceData data)
        {
            if (Origin == null || !Origin.activeInHierarchy)
            {
                return data;
            }

            hoverHit = data.CollisionData;
            hoverHit.point = Origin.transform.position;
            data.CollisionData = hoverHit;
            return data;
        }

        /// <summary>
        /// Creates the payload to emit on the <see cref="Activated"/> event.
        /// </summary>
        /// <param name="data">The default data to potentially mutate.</param>
        /// <returns>The data to emit.</returns>
        protected virtual TransformData CreateSelectedPayload(SurfaceData data)
        {
            if (Destination == null || !Destination.activeInHierarchy)
            {
                return data;
            }

            selectedPayload.Clear();
            selectedPayload.Transform = Destination.transform;
            if (!ApplyDestinationRotation)
            {
                selectedPayload.RotationOverride = data.Rotation;
            }
            return selectedPayload;
        }
    }
}