// Slide Object Control Action|ObjectControlActions|25010
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Slide Object Control Action script is used to slide the controlled GameObject around the scene when changing the axis.
    /// </summary>
    /// <remarks>
    /// The effect is a smooth sliding motion in forward and sideways directions to simulate walking.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    ///
    /// To enable the Slide Object Control Action, ensure one of the `TouchpadControlOptions` children (located under the Controller script alias) has the `Slide` control script active.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/Object Control Actions/VRTK_SlideObjectControlAction")]
    public class VRTK_SlideObjectControlAction : VRTK_BaseObjectControlAction
    {
        [Header("Slide Settings")]

        [Tooltip("The maximum speed the controlled object can be moved in based on the position of the axis.")]
        public float maximumSpeed = 3f;
        [Tooltip("The rate of speed deceleration when the axis is no longer being changed.")]
        public float deceleration = 0.1f;
        [Tooltip("The rate of speed deceleration when the axis is no longer being changed and the object is falling.")]
        public float fallingDeceleration = 0.01f;
        [Tooltip("The speed multiplier to be applied when the modifier button is pressed.")]
        public float speedMultiplier = 1.5f;

        [Header("Custom Settings")]

        [Tooltip("An optional Body Physics script to check for potential collisions in the moving direction. If any potential collision is found then the move will not take place. This can help reduce collision tunnelling.")]
        public VRTK_BodyPhysics bodyPhysics;

        protected float currentSpeed = 0f;

        protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
        {
            currentSpeed = CalculateSpeed(axis, currentlyFalling, modifierActive);
            Move(controlledGameObject, directionDevice, axisDirection);
        }

        protected virtual float CalculateSpeed(float inputValue, bool currentlyFalling, bool modifierActive)
        {
            float speed = currentSpeed;
            if (inputValue != 0f)
            {
                speed = (maximumSpeed * inputValue);
                speed = (modifierActive ? (speed * speedMultiplier) : speed);
            }
            else
            {
                speed = Decelerate(speed, currentlyFalling);
            }

            return speed;
        }

        protected virtual float Decelerate(float speed, bool currentlyFalling)
        {
            float decelerationSpeed = (currentlyFalling ? fallingDeceleration : deceleration);
            if (speed > 0)
            {
                speed -= Mathf.Lerp(decelerationSpeed, maximumSpeed, 0f);
            }
            else if (speed < 0)
            {
                speed += Mathf.Lerp(decelerationSpeed, -maximumSpeed, 0f);
            }
            else
            {
                speed = 0;
            }

            if (speed < decelerationSpeed && speed > -decelerationSpeed)
            {
                speed = 0;
            }

            return speed;
        }

        protected virtual void Move(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection)
        {
            if (directionDevice && directionDevice.gameObject.activeInHierarchy && controlledGameObject.activeInHierarchy)
            {
                float storeYPosition = controlledGameObject.transform.position.y;
                Vector3 updatedPosition = axisDirection * currentSpeed * Time.deltaTime;
                Vector3 finalPosition = controlledGameObject.transform.position + updatedPosition;
                finalPosition = new Vector3(finalPosition.x, storeYPosition, finalPosition.z);

                if (CanMove(bodyPhysics, controlledGameObject.transform.position, finalPosition))
                {
                    controlledGameObject.transform.position = finalPosition;
                }
            }
        }
    }
}