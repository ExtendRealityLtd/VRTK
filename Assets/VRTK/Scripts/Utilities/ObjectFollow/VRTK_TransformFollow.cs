// Transform Follow|Utilities|90062
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Changes one game object's transform to follow another game object's transform.
    /// </summary>
    public class VRTK_TransformFollow : VRTK_ObjectFollow
    {
        /// <summary>
        /// The moment at which to follow.
        /// </summary>
        /// <param name="OnUpdate">Follow in the Update method.</param>
        /// <param name="OnLateUpdate">Follow in the LateUpdate method.</param>
        /// <param name="OnPreRender">Follow in the OnPreRender method. (This script doesn't have to be attached to a camera.)</param>
        public enum FollowMoment
        {
            OnUpdate,
            OnLateUpdate,
            OnPreRender
        }

        [Tooltip("The moment at which to follow.")]
        public FollowMoment moment = FollowMoment.OnPreRender;

        private Transform transformToFollow;
        private Transform transformToChange;
        private bool isListeningToOnPreRender;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (gameObjectToFollow == null)
            {
                return;
            }

            transformToFollow = gameObjectToFollow.transform;
            transformToChange = gameObjectToChange.transform;

            if (moment == FollowMoment.OnPreRender)
            {
                Camera.onPreRender += OnCamPreRender;
                isListeningToOnPreRender = true;
            }
        }

        protected virtual void OnDisable()
        {
            transformToFollow = null;
            transformToChange = null;

            if (isListeningToOnPreRender)
            {
                Camera.onPreRender -= OnCamPreRender;
                isListeningToOnPreRender = false;
            }
        }

        protected virtual void Update()
        {
            if (moment == FollowMoment.OnUpdate)
            {
                Follow();
            }
        }

        protected virtual void LateUpdate()
        {
            if (moment == FollowMoment.OnLateUpdate)
            {
                Follow();
            }
        }

        //this method shouldn't be called `OnPreRender` to prevent a name clash with `MonoBehaviour.OnPreRender` since that is used when this script is attached to a camera
        protected virtual void OnCamPreRender(Camera cam)
        {
            if (cam.gameObject.transform == VRTK_SDK_Bridge.GetHeadsetCamera())
            {
                Follow();
            }
        }

        protected override Vector3 GetPositionToFollow()
        {
            return transformToFollow.position;
        }

        protected override void SetPositionOnGameObject(Vector3 newPosition)
        {
            transformToChange.position = newPosition;
        }

        protected override Quaternion GetRotationToFollow()
        {
            return transformToFollow.rotation;
        }

        protected override void SetRotationOnGameObject(Quaternion newRotation)
        {
            transformToChange.rotation = newRotation;
        }
    }
}