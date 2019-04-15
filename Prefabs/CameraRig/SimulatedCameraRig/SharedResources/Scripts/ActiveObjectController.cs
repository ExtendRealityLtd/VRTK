namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using System.Collections.Generic;
    using Malimbe.MemberChangeMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;
    using Zinnia.Data.Type.Transformation.Aggregation;
    using VRTK.Prefabs.CameraRig.SimulatedCameraRig.Input;

    /// <summary>
    /// Sets up the active controllable objects.
    /// </summary>
    public class ActiveObjectController : MonoBehaviour
    {
        /// <summary>
        /// The speed to move the free mouse cursor.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float FreeCursorRotationSpeed { get; set; } = 0.2f;
        /// <summary>
        /// The speed to move the locked mouse cursor.
        /// </summary>
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float LockedCursorRotationSpeed { get; set; } = 3f;
        /// <summary>
        /// The speed to move the object at.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float MovementSpeed { get; set; } = 0.025f;
        /// <summary>
        /// The mouse input action to update.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public MouseVector2DAction MouseInput { get; set; }

        /// <summary>
        /// A collection of multipliers that deal with positive floats.
        /// </summary>
        [Tooltip("A collection of multipliers that deal with positive floats.")]
        public List<FloatMultiplier> positiveMultipliers = new List<FloatMultiplier>();
        /// <summary>
        /// A collection of multipliers that deal with negative floats.
        /// </summary>
        [Tooltip("A collection of multipliers that deal with negative floats.")]
        public List<FloatMultiplier> negativeMultipliers = new List<FloatMultiplier>();

        /// <summary>
        /// Refreshes the positive multiplier collection.
        /// </summary>
        public virtual void RefreshPositiveMultipliers()
        {
            foreach (FloatMultiplier positiveMultiplier in positiveMultipliers)
            {
                positiveMultiplier.Collection.RunWhenActiveAndEnabled(() => positiveMultiplier.Collection.SetAt(MovementSpeed, 1));
                positiveMultiplier.Collection.RunWhenActiveAndEnabled(() => positiveMultiplier.Collection.CurrentIndex = 0);
            }
        }

        /// <summary>
        /// Refreshes the negative multiplier collection.
        /// </summary>
        public virtual void RefreshNegativeMultipliers()
        {
            foreach (FloatMultiplier negativeMultiplier in negativeMultipliers)
            {
                negativeMultiplier.Collection.RunWhenActiveAndEnabled(() => negativeMultiplier.Collection.SetAt(-MovementSpeed, 1));
                negativeMultiplier.Collection.RunWhenActiveAndEnabled(() => negativeMultiplier.Collection.CurrentIndex = 0);
            }
        }

        protected virtual void OnEnable()
        {
            ConfigureMouseInput();
            RefreshPositiveMultipliers();
            RefreshNegativeMultipliers();
        }

        /// <summary>
        /// Configures the mouse input settings.
        /// </summary>
        protected virtual void ConfigureMouseInput()
        {
            if (MouseInput != null)
            {
                MouseInput.CursorMultiplier = FreeCursorRotationSpeed;
                MouseInput.LockedCursorMultiplier = LockedCursorRotationSpeed;
            }
        }

        /// <summary>
        /// Called after <see cref="FreeCursorRotationSpeed"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(FreeCursorRotationSpeed))]
        protected virtual void OnAfterFreeCursorRotationSpeedChange()
        {
            ConfigureMouseInput();
        }

        /// <summary>
        /// Called after <see cref="LockedCursorRotationSpeed"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(LockedCursorRotationSpeed))]
        protected virtual void OnAfterLockedCursorRotationSpeedChange()
        {
            ConfigureMouseInput();
        }

        /// <summary>
        /// Called after <see cref="MovementSpeed"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(MovementSpeed))]
        protected virtual void OnAfterMovementSpeedChange()
        {
            RefreshPositiveMultipliers();
            RefreshNegativeMultipliers();
        }

        /// <summary>
        /// Called after <see cref="MouseInput"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(MouseInput))]
        protected virtual void OnAfterMouseInputChange()
        {
            ConfigureMouseInput();
        }
    }
}