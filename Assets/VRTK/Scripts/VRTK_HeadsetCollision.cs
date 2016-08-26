namespace VRTK
{
    using UnityEngine;

    public struct HeadsetCollisionEventArgs
    {
        public Collider collider;
        public Transform currentTransform;
    }

    public delegate void HeadsetCollisionEventHandler(object sender, HeadsetCollisionEventArgs e);

    public class VRTK_HeadsetCollision : MonoBehaviour
    {
        public string ignoreTargetWithTagOrClass;

        public event HeadsetCollisionEventHandler HeadsetCollisionDetect;
        public event HeadsetCollisionEventHandler HeadsetCollisionEnded;

        private bool headsetColliding = false;

        public virtual void OnHeadsetCollisionDetect(HeadsetCollisionEventArgs e)
        {
            if (HeadsetCollisionDetect != null)
            {
                HeadsetCollisionDetect(this, e);
            }
        }

        public virtual void OnHeadsetCollisionEnded(HeadsetCollisionEventArgs e)
        {
            if (HeadsetCollisionEnded != null)
            {
                HeadsetCollisionEnded(this, e);
            }
        }

        public virtual bool IsColliding()
        {
            return headsetColliding;
        }

        private void OnEnable()
        {
            headsetColliding = false;
            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Headset);

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(0.1f, 0.1f, 0.1f);

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        private void OnDisable()
        {
            headsetColliding = false;
            Destroy(gameObject.GetComponent<BoxCollider>());
            Destroy(gameObject.GetComponent<Rigidbody>());
        }

        private HeadsetCollisionEventArgs SetHeadsetCollisionEvent(Collider collider, Transform currentTransform)
        {
            HeadsetCollisionEventArgs e;
            e.collider = collider;
            e.currentTransform = currentTransform;
            return e;
        }

        private bool ValidTarget(Transform target)
        {
            return (target && target.tag != ignoreTargetWithTagOrClass && target.GetComponent(ignoreTargetWithTagOrClass) == null);
        }

        private void OnTriggerStay(Collider collider)
        {
            if (enabled && !collider.GetComponent<VRTK_PlayerObject>() && ValidTarget(collider.transform))
            {
                headsetColliding = true;
                OnHeadsetCollisionDetect(SetHeadsetCollisionEvent(collider, transform));
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!collider.GetComponent<VRTK_PlayerObject>())
            {
                headsetColliding = false;
                OnHeadsetCollisionEnded(SetHeadsetCollisionEvent(collider, transform));
            }
        }
    }
}