// Third Person Camera|Utilities|90090
namespace VRTK
{
    using UnityEngine;
    /// <summary>
    /// This script will follow and switch between camera targets that are attached to a third person character. Attach to `[CameraRig]` and then set the array of game objects that should be attached to the empty root object of the 3rd person character
    /// </summary>
    /// <remarks>
    /// This script is intended to be paired with GazeDrivenLocomotion
    /// </remarks>
    public class VRTK_ThirdPersonCamera : MonoBehaviour
    {
        [Tooltip("The speed the camera should move at it's max speed")]
        public float cameraMoveSpeed = 1.0f;
        [Tooltip("An array of transforms that represent different camera angles. Must have at least 1 transform for this script to work. It is important to attach the transform to the empty root object that is above the character mesh.")]
        public Transform[] targetArray;
        [Tooltip("Makes the speed of camera acceleration and deceleration dependent on an animation curve. The animation curve can make the acceleration curve slower to curb simulator sickness by having the 'Y' axis grow slower.")]
        [SerializeField]
        private AnimationCurve animationCurve;
        [Tooltip("Determines whether to use animation curve or standard lerp mechanism. Don't enable unless you've created an animation curve")]
        [SerializeField]
        private bool useAnimationCurve;
        [Tooltip("Used to switch camera angles when the grip button is pressed. Remember to assign VRTK_ControllerEvents as a component of your left controller.")]
        [SerializeField]
        private VRTK_ControllerEvents leftControllerEvents;
        [Tooltip("Used to switch camera angles when the grip button is pressed. Remember to assign VRTK_ControllerEvents as a component of your right controller.")]
        [SerializeField]
        private VRTK_ControllerEvents rightControllerEvents;
        private Vector3 targetPreviousPosition;
        private Transform target;
        private float animationCurvePercentage = 0;
        private GameObject currentTransformObject;
        private int currentIndex = 0;
        private Transform hmd;

        // Finds HMD position, then sets the transform of the camera
        protected void Start()
        {
            hmd = VRTK_DeviceFinder.HeadsetCamera();
            //if no cameras set, then find the player gameobject.
            if (targetArray.Length == 0)
            {
                Debug.Log("No default cameras discovered, please add some empty game objects to the scene and assign them to the array..");
            }
            if (leftControllerEvents == null || rightControllerEvents == null)
            {
                Debug.Log("Requires VRTK_ControllerEvents to be attached to controllers and then assigned.");
            }
            target = GetTargetPosition(targetArray[0]);
            transform.position = target.position;
            leftControllerEvents.GripPressed += ChangeCameraAngle;
            rightControllerEvents.GripPressed += ChangeCameraAngle;
        }

        protected void Update()
        {
            FollowTarget();
        }

        //Offset target position by where the user was when the view switched, to always get standardized camera view
        private Transform GetTargetPosition(Transform targetPosition)
        {
            if (currentTransformObject != null)
            {
                Destroy(currentTransformObject);
            }
            currentTransformObject = Instantiate(targetPosition.gameObject, targetPosition.parent) as GameObject;
            currentTransformObject.transform.position = currentTransformObject.transform.position - transform.InverseTransformPoint(hmd.position);
            return currentTransformObject.transform;

        }

        private void FollowTarget()
        {
            if (target == null)
            {
                return;
            }
            //Use animation curve to ramp up acceleration of camera. This isn't exact math, rather meant to change acceleration in discrete controllable steps.
            if (useAnimationCurve)
            {
                // same position, means we probably haven't moved and player hasn't moved, so we are stationary.
                if (targetPreviousPosition == target.position)
                {
                    animationCurvePercentage = Mathf.Clamp(animationCurvePercentage - (Time.deltaTime), 0, 1);
                }
                // target is moving, and so is the camera or target is moving and camera is stopped, either imply that we should continue along the curve
                if (targetPreviousPosition != target.position)
                {
                    animationCurvePercentage = Mathf.Clamp(animationCurvePercentage + Time.deltaTime, 0, 1);
                }
                transform.position = Vector3.Lerp(transform.position, target.position, animationCurve.Evaluate(animationCurvePercentage) * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * cameraMoveSpeed);
            }
            targetPreviousPosition = target.position;
        }

        private void ChangeCameraAngle(object sender, VRTK.ControllerInteractionEventArgs e)
        {
            //cycle through different camera angles.
            if ((targetArray.Length - 1) > currentIndex)
            {
                currentIndex++;
            }
            else
            {
                currentIndex = 0;
            }
            target = GetTargetPosition(targetArray[currentIndex]);
            targetPreviousPosition = target.position;
            transform.position = target.position;
        }
    }
}