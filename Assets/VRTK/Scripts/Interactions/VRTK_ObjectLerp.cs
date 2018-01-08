using System.Collections;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="originTransform">The origin tranform</param>
    /// <param name="targetTransform">The target transform</param>
    /// <param name="targetAttachPointTransform">The target attach point transform</param>
    public struct ObjectLerpEventArgs
    {
        public Transform originTransform;
        public Transform targetTransform;
        public Transform targetAttachPointTransform;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ObjectLerpEventArgs"/></param>
    public delegate void ObjectLerpEventHandler(object sender, ObjectLerpEventArgs e);

    /// <summary>
    /// The Object Lerp script is usually applied to an Interactable Object and provides a mechanism for moving the GameObject in a lerped translation when grabbed.
    /// </summary>
    public class VRTK_ObjectLerp : MonoBehaviour
    {
        #region Variables

        /// <summary>
        /// Emitted when the lerp of the provided object starts
        /// </summary>
        public event ObjectLerpEventHandler ObjectLerpStartTranslation;

        /// <summary>
        /// Emitted when the lerp of the provided object is successfully completed
        /// </summary>
        public event ObjectLerpEventHandler ObjectLerpCompletedTranslation;

        /// <summary>
        /// Emitted when the lerp of the provided object is cancelled
        /// </summary>
        public event ObjectLerpEventHandler ObjectLerpCancelledTranslation;

        public float lerpDuration = 0.25f;
        public AnimationCurve lerpAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        protected bool isLerping = false;

        #endregion Variables

        #region Properties

        /// <summary>
        /// Determines if this object is currently lerping
        /// </summary>
        /// <returns>whether or not the script is currently lerping</returns>
        public bool IsLerping
        {
            get { return isLerping; }
            set { isLerping = value; }
        }

        #endregion Properties

        #region Events

        public virtual void OnObjectLerpStartTranslation(ObjectLerpEventArgs e)
        {
            if (ObjectLerpStartTranslation != null)
            {
                ObjectLerpStartTranslation(this, e);
            }
        }

        public virtual void OnObjectLerpCompletedTranslation(ObjectLerpEventArgs e)
        {
            if (ObjectLerpCompletedTranslation != null)
            {
                ObjectLerpCompletedTranslation(this, e);
            }
        }

        public virtual void OnObjectLerpCancelledTranslation(ObjectLerpEventArgs e)
        {
            if (ObjectLerpCancelledTranslation != null)
            {
                ObjectLerpCancelledTranslation(this, e);
            }
        }

        #endregion Events

        #region Object Translation

        /// <summary>
        /// Start the object translation
        /// </summary>
        /// <param name="originTransform">The origin tranform</param>
        /// <param name="targetTransform">The target transform</param>
        /// <param name="targetAttachPointTransform">The target attach point transform</param>
        /// <param name="durationOverride">The duration override for the lerp translation, used instead of the global lerp duration</param>
        public virtual void StartObjectTranslation(Transform originTransform, Transform targetTransform,
            Transform targetAttachPointTransform = null, float durationOverride = 0f)
        {
            float selectedDuration = durationOverride > 0f ? durationOverride : lerpDuration;
            StartCoroutine(DoObjectTranslation(originTransform, targetTransform, targetAttachPointTransform, selectedDuration));
        }

        /// <summary>
        /// Cancel current object translation
        /// </summary>
        public void CancelObjectTranslation()
        {
            isLerping = false;
        }

        /// <summary>
        /// Do the object translation
        /// </summary>
        /// <param name="originTransform">The origin tranform</param>
        /// <param name="targetTransform">The target transform</param>
        /// <param name="targetAttachPointTransform">The target attach point transform</param>
        /// <param name="duration">The required duration for the lerp translation</param>
        /// <returns></returns>
        public virtual IEnumerator DoObjectTranslation(Transform originTransform, Transform targetTransform,
            Transform targetAttachPointTransform = null, float duration = 0f)
        {
            isLerping = true;

            Vector3 startPosition = originTransform.position;
            Quaternion startRotation = originTransform.rotation;

            bool move = true;
            float moveTime = 0f;

            ObjectLerpEventArgs currentObjectLerpEventArgs = SetObjectLerpEventArgs(originTransform, targetTransform, targetAttachPointTransform);
            OnObjectLerpStartTranslation(currentObjectLerpEventArgs);

            while (isLerping && move)
            {
                moveTime += Time.deltaTime;
                var movePercent = Mathf.Clamp01(moveTime / duration);
                move = movePercent < 1f;
                float moveCurve = lerpAnimationCurve.Evaluate(movePercent);

                Vector3 endPosition;
                Quaternion endRotation;

                if (targetAttachPointTransform != null)
                {
                    endPosition = targetTransform.position - (targetAttachPointTransform.position - originTransform.position);
                    endRotation = targetTransform.rotation * Quaternion.Euler(targetAttachPointTransform.localEulerAngles);
                }
                else
                {
                    endPosition = targetTransform.position;
                    endRotation = targetTransform.rotation;
                }

                originTransform.position = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0.0f, 1.0f, moveCurve));
                originTransform.rotation = Quaternion.Slerp(startRotation, endRotation, moveCurve);

                yield return null;
            }


            if (isLerping)
            {
                OnObjectLerpCompletedTranslation(currentObjectLerpEventArgs);
                isLerping = false;
            }
            else
            {
                OnObjectLerpCancelledTranslation(currentObjectLerpEventArgs);
            }
        }

        #endregion Object Translation

        #region Utility

        /// <summary>
        /// Sets the ObjectLerpEventArgs object, ready to use in events
        /// </summary>
        /// <param name="originTransform">The origin tranform</param>
        /// <param name="targetTransform">The target transform</param>
        /// <param name="targetAttachPointTransform">The target attach point transform</param>
        /// <returns></returns>
        protected virtual ObjectLerpEventArgs SetObjectLerpEventArgs(Transform originTransform, Transform targetTransform,
            Transform targetAttachPointTransform = null)
        {
            ObjectLerpEventArgs e;
            e.originTransform = originTransform;
            e.targetTransform = targetTransform;
            e.targetAttachPointTransform = targetAttachPointTransform;

            return e;
        }

        #endregion Utility
    }
}