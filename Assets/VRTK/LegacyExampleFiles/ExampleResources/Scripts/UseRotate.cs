namespace VRTK.Examples
{
    using UnityEngine;
    using VRTK;

    public class UseRotate : VRTK_InteractableObject
    {
        [Header("Rotation when in use")]
        [SerializeField]
        [Tooltip("Rotation speed when not in use (deg/sec)")]
        private float idleSpinSpeed = 0f;
        [SerializeField]
        [Tooltip("Rotation speed when in use (deg/sec)")]
        private float activeSpinSpeed = 360f;
        [Tooltip("Game object to rotate\n(leave empty to use this object)")]
        [SerializeField]
        private Transform rotatingObject;
        [SerializeField]
        private Vector3 rotationAxis = Vector3.up;

        private float spinSpeed = 0f;

        public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
        {
            base.StartUsing(currentUsingObject);
            spinSpeed = activeSpinSpeed;
        }

        public override void StopUsing(VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
        {
            base.StopUsing(previousUsingObject, resetUsingObjectState);
            spinSpeed = idleSpinSpeed;
        }

        protected void Start()
        {
            if (rotatingObject == null)
            {
                rotatingObject = transform;
            }
            spinSpeed = idleSpinSpeed;
        }

        protected override void Update()
        {
            base.Update();
            rotatingObject.Rotate(rotationAxis, spinSpeed * Time.deltaTime);
        }
    }
}