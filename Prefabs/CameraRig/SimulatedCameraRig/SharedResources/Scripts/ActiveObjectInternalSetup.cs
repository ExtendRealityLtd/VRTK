namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Data.Type.Transformation;
    using VRTK.Prefabs.CameraRig.SimulatedCameraRig.Input;

    /// <summary>
    /// Sets up the active controllable objects.
    /// </summary>
    public class ActiveObjectInternalSetup : MonoBehaviour
    {
        [Tooltip("The speed to move the free mouse cursor."), SerializeField]
        private float _freeCursorRotationSpeed = 0.2f;
        /// <summary>
        /// The speed to move the free mouse cursor.
        /// </summary>
        public float FreeCursorRotationSpeed
        {
            get { return _freeCursorRotationSpeed; }
            set
            {
                _freeCursorRotationSpeed = value;
                ConfigureMouseInput();
            }
        }

        [Tooltip("The speed to move the locked mouse cursor."), SerializeField]
        private float _lockedCursorRotationSpeed = 3f;
        /// <summary>
        /// The speed to move the locked mouse cursor.
        /// </summary>
        public float LockedCursorRotationSpeed
        {
            get { return _lockedCursorRotationSpeed; }
            set
            {
                _lockedCursorRotationSpeed = value;
                ConfigureMouseInput();
            }
        }

        [Tooltip("The speed to move the object at."), SerializeField]
        private float _movementSpeed = 0.025f;
        /// <summary>
        /// The speed to move the object at.
        /// </summary>
        public float MovementSpeed
        {
            get { return _movementSpeed; }
            set
            {
                _movementSpeed = value;
                RefreshPositiveMultipliers();
                RefreshNegativeMultipliers();
            }
        }

        [Tooltip("The mouse input action to update."), SerializeField]
        private MouseVector2DAction _mouseInput;
        /// <summary>
        /// The mouse input action to update.
        /// </summary>
        public MouseVector2DAction MouseInput
        {
            get { return _mouseInput; }
            set
            {
                _mouseInput = value;
                ConfigureMouseInput();
            }
        }

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
                positiveMultiplier.SetElement(1, MovementSpeed);
                positiveMultiplier.CurrentIndex = 0;
            }
        }

        /// <summary>
        /// Refreshes the negative multiplier collection.
        /// </summary>
        public virtual void RefreshNegativeMultipliers()
        {
            foreach (FloatMultiplier negativeMultiplier in negativeMultipliers)
            {
                negativeMultiplier.SetElement(1, -MovementSpeed);
                negativeMultiplier.CurrentIndex = 0;
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
                MouseInput.cursorMultiplier = FreeCursorRotationSpeed;
                MouseInput.lockedCursorMultiplier = LockedCursorRotationSpeed;
            }
        }
    }
}