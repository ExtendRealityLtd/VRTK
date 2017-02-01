// Rotate Touchpad Control Action|TouchpadControlActions|25020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Rotate Touchpad Control Action script is used to rotate the controlled GameObject around the up vector when changing the touchpad axis.
    /// </summary>
    /// <remarks>
    /// The effect is a smooth rotation to simulate turning.
    /// </remarks>
    public class VRTK_RotateTouchpadControlAction : VRTK_BaseTouchpadControlAction
    {
        [Tooltip("The maximum speed the controlled object can be rotated based on the position of the touchpad axis.")]
        public float maximumRotationSpeed = 3f;
        [Tooltip("The rotation multiplier to be applied when the modifier button is pressed.")]
        public float rotationMultiplier = 1.5f;

        private Collider centerCollider;
        private Transform controlledTransform;
        private Transform playArea;

        /// <summary>
        /// The ProcessFixedUpdate method is run for every FixedUpdate on the Touchpad Control script.
        /// </summary>
        /// <param name="controlledGameObject">The GameObject that is going to be affected.</param>
        /// <param name="directionDevice">The device that is used for the direction.</param>
        /// <param name="axisDirection">The axis that is being affected from the touchpad.</param>
        /// <param name="axis">The value of the current touchpad touch point based across the axis direction.</param>
        /// <param name="deadzone">The value of the deadzone based across the axis direction.</param>
        /// <param name="currentlyFalling">Whether the controlled GameObject is currently falling.</param>
        /// <param name="modifierActive">Whether the modifier button is pressed.</param>
        public override void ProcessFixedUpdate(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
        {
            float angle = Rotate(axis, modifierActive);
            if (angle != 0f)
            {
                RotateAroundPlayer(controlledGameObject, angle);
            }
        }

        protected virtual void OnEnable()
        {
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
        }

        protected virtual void RotateAroundPlayer(GameObject controlledGameObject, float angle)
        {
            Vector3 objectCenter = GetObjectCenter(controlledGameObject.transform);
            Vector3 objectPosition = controlledGameObject.transform.TransformPoint(objectCenter);
            controlledGameObject.transform.Rotate(Vector3.up, angle);
            objectPosition -= controlledGameObject.transform.TransformPoint(objectCenter);
            controlledGameObject.transform.position += objectPosition;
        }

        protected virtual float Rotate(float axis, bool modifierActive)
        {
            return axis * maximumRotationSpeed * Time.deltaTime * (modifierActive ? rotationMultiplier : 1) * 10;
        }

        protected virtual Vector3 GetObjectCenter(Transform checkObject)
        {
            if (centerCollider == null || checkObject != controlledTransform)
            {
                controlledTransform = checkObject;

                if (checkObject == playArea)
                {
                    CapsuleCollider playAreaCollider = playArea.GetComponent<CapsuleCollider>();
                    centerCollider = playAreaCollider;
                    return playAreaCollider.center;
                }
                else
                {
                    centerCollider = checkObject.GetComponent<Collider>();
                }
            }

            return Vector3.zero;
        }
    }
}