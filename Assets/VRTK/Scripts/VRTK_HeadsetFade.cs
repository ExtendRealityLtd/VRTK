namespace VRTK
{
    using UnityEngine;

    public struct HeadsetFadeEventArgs
    {
        public float timeTillComplete;
        public Transform currentTransform;
    }

    public delegate void HeadsetFadeEventHandler(object sender, HeadsetFadeEventArgs e);

    public class VRTK_HeadsetFade : MonoBehaviour
    {
        public event HeadsetFadeEventHandler HeadsetFadeStart;
        public event HeadsetFadeEventHandler HeadsetFadeComplete;
        public event HeadsetFadeEventHandler HeadsetUnfadeStart;
        public event HeadsetFadeEventHandler HeadsetUnfadeComplete;

        private bool isTransitioning = false;
        private bool isFaded = false;

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

        public virtual bool IsFaded()
        {
            return isFaded;
        }

        public virtual bool IsTransitioning()
        {
            return isTransitioning;
        }

        public virtual void Fade(Color color, float duration)
        {
            isFaded = false;
            isTransitioning = true;
            VRTK_SDK_Bridge.HeadsetFade(color, duration);
            OnHeadsetFadeStart(SetHeadsetFadeEvent(transform, duration));
            CancelInvoke("UnfadeComplete");
            Invoke("FadeComplete", duration);
        }

        public virtual void Unfade(float duration)
        {
            isFaded = true;
            isTransitioning = true;
            VRTK_SDK_Bridge.HeadsetFade(Color.clear, duration);
            OnHeadsetUnfadeStart(SetHeadsetFadeEvent(transform, duration));
            CancelInvoke("FadeComplete");
            Invoke("UnfadeComplete", duration);
        }

        private void Start()
        {
            isTransitioning = false;
            isFaded = false;

            Utilities.AddCameraFade();
            if (!VRTK_SDK_Bridge.HasHeadsetFade(gameObject))
            {
                Debug.LogWarning("This 'VRTK_HeadsetCollisionFade' script needs a compatible fade script on the camera eye.");
            }
        }

        private HeadsetFadeEventArgs SetHeadsetFadeEvent(Transform currentTransform, float duration)
        {
            HeadsetFadeEventArgs e;
            e.timeTillComplete = duration;
            e.currentTransform = currentTransform;
            return e;
        }

        private void FadeComplete()
        {
            isFaded = true;
            isTransitioning = false;
            OnHeadsetFadeComplete(SetHeadsetFadeEvent(transform, 0f));
        }

        private void UnfadeComplete()
        {
            isFaded = false;
            isTransitioning = false;
            OnHeadsetUnfadeComplete(SetHeadsetFadeEvent(transform, 0f));
        }
    }
}