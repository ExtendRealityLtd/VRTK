// Headset Fade|Presence|70020
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="timeTillComplete">A float that is the duration for the fade/unfade process has remaining.</param>
    /// <param name="currentTransform">The current Transform of the object that the Headset Fade script is attached to (Camera).</param>
    public struct HeadsetFadeEventArgs
    {
        public float timeTillComplete;
        public Transform currentTransform;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="HeadsetFadeEventArgs"/></param>
    public delegate void HeadsetFadeEventHandler(object sender, HeadsetFadeEventArgs e);

    /// <summary>
    /// Provides the ability to change the colour of the headset view to a specified colour over a given duration.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_HeadsetFade` script on any active scene GameObject.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_HeadsetFade")]
    public class VRTK_HeadsetFade : MonoBehaviour
    {
        /// <summary>
        /// Emitted when the user's headset begins to fade to a given colour.
        /// </summary>
        public event HeadsetFadeEventHandler HeadsetFadeStart;
        /// <summary>
        /// Emitted when the user's headset has completed the fade and is now fully at the given colour.
        /// </summary>
        public event HeadsetFadeEventHandler HeadsetFadeComplete;
        /// <summary>
        /// Emitted when the user's headset begins to unfade back to a transparent colour.
        /// </summary>
        public event HeadsetFadeEventHandler HeadsetUnfadeStart;
        /// <summary>
        /// Emitted when the user's headset has completed unfading and is now fully transparent again.
        /// </summary>
        public event HeadsetFadeEventHandler HeadsetUnfadeComplete;

        protected Transform headset;
        protected bool isTransitioning = false;
        protected bool isFaded = false;

        public virtual void OnHeadsetFadeStart(HeadsetFadeEventArgs e)
        {
            if (HeadsetFadeStart != null)
            {
                HeadsetFadeStart(this, e);
            }
        }

        public virtual void OnHeadsetFadeComplete(HeadsetFadeEventArgs e)
        {
            if (HeadsetFadeComplete != null)
            {
                HeadsetFadeComplete(this, e);
            }
        }

        public virtual void OnHeadsetUnfadeStart(HeadsetFadeEventArgs e)
        {
            if (HeadsetUnfadeStart != null)
            {
                HeadsetUnfadeStart(this, e);
            }
        }

        public virtual void OnHeadsetUnfadeComplete(HeadsetFadeEventArgs e)
        {
            if (HeadsetUnfadeComplete != null)
            {
                HeadsetUnfadeComplete(this, e);
            }
        }

        /// <summary>
        /// The IsFaded method returns true if the headset is currently fading or has completely faded and returns false if it is completely unfaded.
        /// </summary>
        /// <returns>Returns `true` if the headset is currently fading or faded.</returns>
        public virtual bool IsFaded()
        {
            return isFaded;
        }

        /// <summary>
        /// The IsTransitioning method returns true if the headset is currently fading or unfading and returns false if it is completely faded or unfaded.
        /// </summary>
        /// <returns>Returns `true` if the headset is currently in the process of fading or unfading.</returns>
        public virtual bool IsTransitioning()
        {
            return isTransitioning;
        }

        /// <summary>
        /// The Fade method initiates a change in the colour of the headset view to the given colour over a given duration.
        /// </summary>
        /// <param name="color">The colour to fade the headset view to.</param>
        /// <param name="duration">The time in seconds to take to complete the fade transition.</param>
        public virtual void Fade(Color color, float duration)
        {
            isFaded = false;
            isTransitioning = true;
            VRTK_SDK_Bridge.HeadsetFade(color, duration);
            OnHeadsetFadeStart(SetHeadsetFadeEvent(headset, duration));
            CancelInvoke("UnfadeComplete");
            Invoke("FadeComplete", duration);
        }

        /// <summary>
        /// The Unfade method initiates the headset to change colour back to a transparent colour over a given duration.
        /// </summary>
        /// <param name="duration">The time in seconds to take to complete the unfade transition.</param>
        public virtual void Unfade(float duration)
        {
            isFaded = true;
            isTransitioning = true;
            VRTK_SDK_Bridge.HeadsetFade(Color.clear, duration);
            OnHeadsetUnfadeStart(SetHeadsetFadeEvent(headset, duration));
            CancelInvoke("FadeComplete");
            Invoke("UnfadeComplete", duration);
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            headset = VRTK_DeviceFinder.HeadsetCamera();
            isTransitioning = false;
            isFaded = false;

            VRTK_SharedMethods.AddCameraFade();
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual HeadsetFadeEventArgs SetHeadsetFadeEvent(Transform currentTransform, float duration)
        {
            HeadsetFadeEventArgs e;
            e.timeTillComplete = duration;
            e.currentTransform = currentTransform;
            return e;
        }

        protected virtual void FadeComplete()
        {
            isFaded = true;
            isTransitioning = false;
            OnHeadsetFadeComplete(SetHeadsetFadeEvent(headset, 0f));
        }

        protected virtual void UnfadeComplete()
        {
            isFaded = false;
            isTransitioning = false;
            OnHeadsetUnfadeComplete(SetHeadsetFadeEvent(headset, 0f));
        }
    }
}