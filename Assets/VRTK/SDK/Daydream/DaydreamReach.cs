#if VRTK_SDK_DAYDREAM
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// DaydreamReach component uses touchpad to extend controller position out in front of user
    /// along the current controller orientation plane
    /// </summary>
    public class DaydreamReach : MonoBehaviour
    {
        [Tooltip("Controller to track, defaults to ./Controller but probably want a Pointer Joint like GvrControllerPointer/Laser")]
        public Transform controller;
        [Tooltip("Maximum reach distance from controller origin.")]
        public float reachDistance = 0.5f;

        private Vector3 positionOrigin;

        protected virtual void Start()
        {
            if (controller == null)
            {
                controller = transform.FindChild("Controller");
            }
            positionOrigin = transform.position;
        }

        protected virtual void Update()
        {
            if (!GvrController.IsTouching)
            {
                // snap back to origin
                transform.position = positionOrigin;
            }
            else
            {
                transform.position = ReachForwardByDistance();
            }
        }

        private Vector3 ReachForwardByDistance()
        {
            Vector3 pos = positionOrigin;
            Vector3 offset = Vector3.zero;
            Vector2 touch = GetTouch(5);

            offset.z = touch.y * reachDistance;
            offset = controller.transform.rotation * offset;
            pos += offset;
            return pos;
        }

        /// <summary>
        /// GetTouch gets touch position adjusted coordinates. Abs(x) and abs(y) always 0 to 1
        /// </summary>
        /// <param name="origin">like a numeric keypad, 1= lower left, 2=lower-center, 5=center etc</param>
        /// <returns></returns>
        private Vector2 GetTouch(int origin = 5)
        {
            if (!GvrController.IsTouching)
            {
                return Vector2.zero;
            }

            Vector2 touch = GvrController.TouchPos;
            // raw: 0,0 is top left, x is left-right, y is front-back
            // invert y axis
            touch.y = 1.0f - touch.y;

            switch (origin)
            {
                case 1:
                    break;
                case 2:
                    touch.x = touch.x * 2f - 1f;
                    break;
                case 5:
                    touch.y = touch.y * 2f - 1f;
                    touch.x = touch.x * 2f - 1f;
                    break;
            }

            return touch;
        }
    }
}
#endif