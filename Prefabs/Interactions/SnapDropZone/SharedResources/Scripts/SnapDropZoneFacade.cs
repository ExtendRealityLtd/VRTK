namespace VRTK.Prefabs.Interactions.SnapDropZone
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.PropertySetterMethod;
    using Malimbe.PropertyValidationMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into the SnapDropZone prefab.
    /// </summary>
    public class SnapDropZoneFacade : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the specified <see cref="GameObject"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<GameObject>
        {
        }

        #region SnapDropZone Settings
        /// <summary>
        /// The currently snapped object.
        /// </summary>
        [Serialized, Validated, Cleared]
        [field: Header("SnapDropZone Settings"), DocumentedByXml]
        public GameObject SnappedObject { get; set; }
        /// <summary>
        /// Whether to clone <see cref="SnappedObject"/> on raise of <see cref="Unsnapped"/>.
        /// </summary>
        [Serialized, Validated]
        [field: DocumentedByXml]
        public bool ClonesOnUnsnap { get; set; }
        #endregion

        #region SnapDropZone Events
        /// <summary>
        /// Emitted when <see cref="SnappedObject"/> changes to another (non-<see langword="null"/>) <see cref="GameObject"/>.
        /// </summary>
        [Header("SnapDropZone Events"), DocumentedByXml]
        public UnityEvent Snapped = new UnityEvent();
        /// <summary>
        /// Emitted when <see cref="SnappedObject"/> changes to <see langword="null"/>.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Unsnapped = new UnityEvent();
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked <see cref="SnapDropZoneInternalSetup"/>.
        /// </summary>
        [Serialized, Validated, Cleared]
        [field: Header("Internal Settings"), DocumentedByXml, InternalSetting]
        public SnapDropZoneInternalSetup InternalSetup { get; set; }
        #endregion

        /// <summary>
        /// Sets <see cref="SnappedObject"/> to the given <see cref="GameObject"/> if there currently is no snapped object.
        /// </summary>
        /// <param name="snappedObject">The object to snap.</param>
        public virtual void SetSnappedObjectIfEmpty(GameObject snappedObject)
        {
            if (SnappedObject == null)
            {
                SnappedObject = snappedObject;
            }
        }

        /// <summary>
        /// Clears <see cref="SnappedObject"/> if the given object is equal to it.
        /// </summary>
        /// <param name="snappedObject">The object to unsnap.</param>
        public virtual void ClearSnappedObjectIfEqual(GameObject snappedObject)
        {
            if (SnappedObject == snappedObject)
            {
                SnappedObject = null;
            }
        }

        /// <summary>
        /// Handles changes to <see cref="SnappedObject"/>.
        /// </summary>
        /// <param name="previousValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        [CalledBySetter(nameof(SnappedObject))]
        protected virtual void OnSnappedObjectChange(GameObject previousValue, ref GameObject newValue)
        {
            if (!Application.isPlaying || InternalSetup == null)
            {
                return;
            }

            if (previousValue != null && previousValue != newValue)
            {
                Unsnapped?.Invoke(previousValue);
            }

            InternalSetup.ConfigureForSnappedObject(newValue);

            if (newValue != null && previousValue != newValue)
            {
                Snapped?.Invoke(newValue);
            }
        }

        /// <summary>
        /// Handles changes to <see cref="ClonesOnUnsnap"/>.
        /// </summary>
        /// <param name="previousValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        [CalledBySetter(nameof(ClonesOnUnsnap))]
        protected virtual void OnClonesOnUnsnapChange(bool previousValue, ref bool newValue)
        {
            if (!Application.isPlaying || InternalSetup == null)
            {
                return;
            }

            InternalSetup.ConfigureCloning(newValue);
        }
    }
}
