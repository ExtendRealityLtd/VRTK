// Object Follow|Utilities|90060
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Abstract class that allows to change one game object's properties to follow another game object.
    /// </summary>
    public abstract class VRTK_ObjectFollow : MonoBehaviour
    {
        [Tooltip("The game object to follow. The followed property values will be taken from this one.")]
        public GameObject gameObjectToFollow;
        [Tooltip("The game object to change the property values of. If left empty the game object this script is attached to will be changed.")]
        public GameObject gameObjectToChange;

        [Tooltip("Whether to follow the position of the given game object.")]
        public bool followsPosition = true;
        [Tooltip("Whether to smooth the position when following `gameObjectToFollow`.")]
        public bool smoothsPosition;
        [Tooltip("The maximum allowed distance between the unsmoothed source and the smoothed target per frame to use for smoothing.")]
        public float maxAllowedPerFrameDistanceDifference = 0.003f;
        /// <summary>
        /// The position that results by following `gameObjectToFollow`.
        /// </summary>
        public Vector3 targetPosition { get; private set; }

        [Tooltip("Whether to follow the rotation of the given game object.")]
        public bool followsRotation = true;
        [Tooltip("Whether to smooth the rotation when following `gameObjectToFollow`.")]
        public bool smoothsRotation;
        [Tooltip("The maximum allowed angle between the unsmoothed source and the smoothed target per frame to use for smoothing.")]
        public float maxAllowedPerFrameAngleDifference = 1.5f;
        /// <summary>
        /// The rotation that results by following `gameObjectToFollow`.
        /// </summary>
        public Quaternion targetRotation { get; private set; }

        [Tooltip("Whether to follow the scale of the given game object.")]
        public bool followsScale = true;
        [Tooltip("Whether to smooth the scale when following `gameObjectToFollow`.")]
        public bool smoothsScale;
        [Tooltip("The maximum allowed size between the unsmoothed source and the smoothed target per frame to use for smoothing.")]
        public float maxAllowedPerFrameSizeDifference = 0.003f;
        /// <summary>
        /// The scale that results by following `gameObjectToFollow`.
        /// </summary>
        public Vector3 targetScale { get; private set; }

        /// <summary>
        /// Follow `gameObjectToFollow` using the current settings.
        /// </summary>
        public virtual void Follow()
        {
            if (gameObjectToFollow == null)
            {
                return;
            }

            if (followsPosition)
            {
                FollowPosition();
            }

            if (followsRotation)
            {
                FollowRotation();
            }

            if (followsScale)
            {
                FollowScale();
            }
        }

        protected virtual void OnEnable()
        {
            gameObjectToChange = gameObjectToChange != null ? gameObjectToChange : gameObject;
        }

        protected virtual void OnValidate()
        {
            maxAllowedPerFrameDistanceDifference = Mathf.Max(0.0001f, maxAllowedPerFrameDistanceDifference);
            maxAllowedPerFrameAngleDifference = Mathf.Max(0.0001f, maxAllowedPerFrameAngleDifference);
            maxAllowedPerFrameSizeDifference = Mathf.Max(0.0001f, maxAllowedPerFrameSizeDifference);
        }

        protected abstract Vector3 GetPositionToFollow();

        protected abstract void SetPositionOnGameObject(Vector3 newPosition);

        protected abstract Quaternion GetRotationToFollow();

        protected abstract void SetRotationOnGameObject(Quaternion newRotation);

        protected virtual Vector3 GetScaleToFollow()
        {
            return gameObjectToFollow.transform.localScale;
        }

        protected virtual void SetScaleOnGameObject(Vector3 newScale)
        {
            gameObjectToChange.transform.localScale = newScale;
        }

        protected virtual void FollowPosition()
        {
            var positionToFollow = GetPositionToFollow();
            Vector3 newPosition;

            if (smoothsPosition)
            {
                var alpha = Mathf.Clamp01(Vector3.Distance(targetPosition, positionToFollow) / maxAllowedPerFrameDistanceDifference);
                newPosition = Vector3.Lerp(targetPosition, positionToFollow, alpha);
            }
            else
            {
                newPosition = positionToFollow;
            }

            targetPosition = newPosition;
            SetPositionOnGameObject(newPosition);
        }

        protected virtual void FollowRotation()
        {
            var rotationToFollow = GetRotationToFollow();
            Quaternion newRotation;

            if (smoothsRotation)
            {
                var alpha = Mathf.Clamp01(Quaternion.Angle(targetRotation, rotationToFollow) / maxAllowedPerFrameAngleDifference);
                newRotation = Quaternion.Lerp(targetRotation, rotationToFollow, alpha);
            }
            else
            {
                newRotation = rotationToFollow;
            }

            targetRotation = newRotation;
            SetRotationOnGameObject(newRotation);
        }

        protected virtual void FollowScale()
        {
            var scaleToFollow = GetScaleToFollow();
            Vector3 newScale;

            if (smoothsScale)
            {
                var alpha = Mathf.Clamp01(Vector3.Distance(targetScale, scaleToFollow) / maxAllowedPerFrameSizeDifference);
                newScale = Vector3.Lerp(targetScale, scaleToFollow, alpha);
            }
            else
            {
                newScale = scaleToFollow;
            }

            targetScale = newScale;
            SetScaleOnGameObject(newScale);
        }
    }
}