namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public struct PlayerPhysicsEventArgs
    {
    }

    public delegate void PlayerPhysicsEventHandler(object sender, PlayerPhysicsEventArgs e);

    public class VRTK_PlayerPhysics : MonoBehaviour
    {
        public event PlayerPhysicsEventHandler PhysicsFallStarted;
        public event PlayerPhysicsEventHandler PhysicsFallEnded;

        private Rigidbody rb;
        private BoxCollider bc;

        private Transform headCamera;

        private bool isFalling = false;

        public virtual void OnPhysicsFallStarted(PlayerPhysicsEventArgs e)
        {
            if (PhysicsFallStarted != null)
                PhysicsFallStarted(this, e);
        }

        public virtual void OnPhysicsFallEnded(PlayerPhysicsEventArgs e)
        {
            if (PhysicsFallEnded != null)
                PhysicsFallEnded(this, e);
        }

        protected PlayerPhysicsEventArgs SetPlayerPhysicsEvent()
        {
            PlayerPhysicsEventArgs e;

            return e;
        }

        public bool IsFalling()
        {
            return isFalling;
        }

        private void Start()
        {
            headCamera = GameObject.FindObjectOfType<SteamVR_GameView>().GetComponent<Transform>();

            CreateCollider();
        }

        private void CreateCollider()
        {
            rb = this.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
                rb = this.gameObject.AddComponent<Rigidbody>();
            rb.mass = 80;
            rb.freezeRotation = true;
            rb.isKinematic = true;

            bc = this.gameObject.GetComponent<BoxCollider>();
            if (bc == null)
                bc = this.gameObject.AddComponent<BoxCollider>();

            bc.center = new Vector3(0f, 0.25f, 0f);
            bc.size = new Vector3(0.5f, 0.5f, 0.5f);
            bc.isTrigger = true;

            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        public void StartPhysicsFall(Vector3 velocity)
        {
            if (!isFalling)
            {
                OnPhysicsFallStarted(SetPlayerPhysicsEvent());

                isFalling = true;
                rb.isKinematic = false;
                bc.isTrigger = false;
                rb.velocity = velocity + new Vector3(0.0f, -0.001f, 0.0f);
            }
        }

        public void StopPhysicsFall()
        {
            OnPhysicsFallEnded(SetPlayerPhysicsEvent());

            isFalling = false;
            rb.isKinematic = true;
            bc.isTrigger = true;
        }

        private void FixedUpdate()
        {
            Vector3 newCenter = new Vector3(headCamera.transform.localPosition.x, bc.center.y, headCamera.transform.localPosition.z);
            bc.center = newCenter;
        }

        private void Update()
        {
            if (isFalling)
            {
                if (rb.velocity == Vector3.zero)
                {
                    StopPhysicsFall();
                }
            }
        }
    }
}