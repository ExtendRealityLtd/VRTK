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

        public override void StartUsing(GameObject usingObject)
        {
            base.StartUsing(usingObject);
            spinSpeed = activeSpinSpeed;
        }

        public override void StopUsing(GameObject usingObject)
        {
            base.StopUsing(usingObject);
            spinSpeed = idleSpinSpeed;
        }

        protected override void Start()
        {
            base.Start();
            if (rotatingObject == null)
            {
                rotatingObject = this.transform;
            }
            spinSpeed = idleSpinSpeed;
        }

        protected override void Update()
        {
            rotatingObject.Rotate(rotationAxis, spinSpeed * Time.deltaTime);
        }
    }
}