// Base Touchpad Control Action|TouchpadControlActions|25000
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Base Touchpad Control Action script is an abstract class that all touchpad control action scripts inherit.
    /// </summary>
    /// <remarks>
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseTouchpadControlAction : MonoBehaviour
    {
        public enum AxisDescriptions
        {
            XAxis,
            YAxis
        }

        [Tooltip("A helper parameter to easily identify which axis this Touchpad Control Action is for.")]
        public AxisDescriptions axisDescription;

        protected Collider centerCollider;
        protected Vector3 colliderCenter = Vector3.zero;
        protected float colliderRadius = 0f;
        protected float colliderHeight = 0f;
        protected Transform controlledTransform;
        protected Transform playArea;

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
        public abstract void ProcessFixedUpdate(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive);

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

        protected virtual void Blink(float blinkSpeed)
        {
            if (blinkSpeed > 0f)
            {
                VRTK_SDK_Bridge.HeadsetFade(Color.black, 0);
                ReleaseBlink(blinkSpeed);
            }
        }

        protected virtual void ReleaseBlink(float blinkSpeed)
        {
            VRTK_SDK_Bridge.HeadsetFade(Color.clear, blinkSpeed);
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
                    colliderRadius = playAreaCollider.radius;
                    colliderHeight = playAreaCollider.height;
                    colliderCenter = playAreaCollider.center;
                }
                else
                {
                    centerCollider = checkObject.GetComponent<Collider>();
                    colliderRadius = 0.1f;
                    colliderHeight = 0.1f;
                }
            }

            return colliderCenter;
        }

        protected virtual int GetAxisDirection(float axis)
        {
            int axisDirection = 0;
            if (axis < 0)
            {
                axisDirection = -1;
            }
            else if (axis > 0)
            {
                axisDirection = 1;
            }

            return axisDirection;
        }
    }
}