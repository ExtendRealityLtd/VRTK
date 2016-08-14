namespace VRTK
{
    using UnityEngine;

    public struct HeadsetCollisionEventArgs
    {
        public Collider collider;
        public Transform currentTransform;
    }

    public delegate void HeadsetCollisionEventHandler(object sender, HeadsetCollisionEventArgs e);

    public abstract class VRTK_HeadsetCollisionFadeBase : MonoBehaviour
    {
        public float blinkTransitionSpeedEnter = 0.1f;
        public float blinkTransitionSpeedExit = 0.1f;
        public Color fadeColor = Color.black;

        public event HeadsetCollisionEventHandler HeadsetCollisionDetect;
        public event HeadsetCollisionEventHandler HeadsetCollisionEnded;

        protected virtual void Start()
        {
            Utilities.AddCameraFade();
            if (gameObject.GetComponentInChildren<SteamVR_Fade>() == null)
            {
                Debug.LogWarning("This 'VRTK_HeadsetCollisionFade' script needs a SteamVR_Fade script on the camera eye.");
            }

            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Headset);
        }

        public virtual void OnHeadsetCollisionDetect(HeadsetCollisionEventArgs e)
        {
            if (HeadsetCollisionDetect != null)
            {
                HeadsetCollisionDetect(this, e);
            }
            SteamVR_Fade.Start(fadeColor, blinkTransitionSpeedEnter);
        }

        public virtual void OnHeadsetCollisionEnded(HeadsetCollisionEventArgs e)
        {
            if (HeadsetCollisionEnded != null)
            {
                HeadsetCollisionEnded(this, e);
            }
            SteamVR_Fade.Start(Color.clear, blinkTransitionSpeedExit);
        }

        protected HeadsetCollisionEventArgs SetHeadsetCollisionEvent(Collider collider, Transform currentTransform)
        {
            HeadsetCollisionEventArgs e;
            e.collider = collider;
            e.currentTransform = currentTransform;
            return e;
        }
    }

    public class VRTK_HeadsetCollisionFade : VRTK_HeadsetCollisionFadeBase
    {
        public string ignoreTargetWithTagOrClass;

        protected override void Start()
        {
            base.Start();

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(0.1f, 0.1f, 0.1f);

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        protected virtual bool ValidTarget(Transform target)
        {
            return (target && target.tag != ignoreTargetWithTagOrClass && target.GetComponent(ignoreTargetWithTagOrClass) == null);
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            if (enabled && !collider.GetComponent<VRTK_PlayerObject>() && ValidTarget(collider.transform))
                OnHeadsetCollisionDetect(SetHeadsetCollisionEvent(collider, transform));
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            if (!collider.GetComponent<VRTK_PlayerObject>())
                OnHeadsetCollisionEnded(SetHeadsetCollisionEvent(collider, transform));
        }
    }
}