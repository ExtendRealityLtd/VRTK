// Headset Collision|Scripts|0090
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="collider">The Collider of the game object the headset has collided with.</param>
    /// <param name="currentTransform">The current Transform of the object that the Headset Collision Fade script is attached to (Camera).</param>
    public struct HeadsetCollisionEventArgs
    {
        public Collider collider;
        public Transform currentTransform;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="HeadsetCollisionEventArgs"/></param>
    public delegate void HeadsetCollisionEventHandler(object sender, HeadsetCollisionEventArgs e);

    /// <summary>
    /// The purpose of the Headset Collision is to detect when the user's VR headset collides with another game object.
    /// </summary>
    /// <remarks>
    ///   > **Unity Version Information**
    ///   > * If using `Unity 5.3` or older then the Headset Collision script is attached to the `Camera(head)` object within the `[CameraRig]` prefab.
    ///   > * If using `Unity 5.4` or newer then the Headset Collision script is attached to the `Camera(eye)` object within the `[CameraRig]->Camera(head)` prefab.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.
    /// </example>
    public class VRTK_HeadsetCollision : MonoBehaviour
    {
        [Tooltip("A string that specifies an object Tag or the name of a Script attached to an object and will be ignored on headset collision.")]
        public string ignoreTargetWithTagOrClass;
        [Tooltip("A specified VRTK_TagOrScriptPolicyList to use to determine whether any objects will be acted upon by the Headset Collision. If a list is provided then the 'Ignore Target With Tag Or Class' parameter will be ignored.")]
        public VRTK_TagOrScriptPolicyList targetTagOrScriptListPolicy;

        /// <summary>
        /// Emitted when the user's headset collides with another game object.
        /// </summary>
        public event HeadsetCollisionEventHandler HeadsetCollisionDetect;
        /// <summary>
        /// Emitted when the user's headset stops colliding with a game object.
        /// </summary>
        public event HeadsetCollisionEventHandler HeadsetCollisionEnded;

        private bool headsetColliding = false;
        private Collider collidingWith = null;

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

        /// <summary>
        /// The IsColliding method is used to determine if the headset is currently colliding with a valid game object and returns true if it is and false if it is not colliding with anything or an invalid game object.
        /// </summary>
        /// <returns>Returns true if the headset is currently colliding with a valid game object.</returns>
        public virtual bool IsColliding()
        {
            return headsetColliding;
        }

        private void OnEnable()
        {
            VRTK_ObjectCache.registeredHeadsetCollider = this;
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
            EndCollision(collidingWith);
            VRTK_ObjectCache.registeredHeadsetCollider = null;
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
            return (target && !(Utilities.TagOrScriptCheck(target.gameObject, targetTagOrScriptListPolicy, ignoreTargetWithTagOrClass)));
        }

        private void OnTriggerStay(Collider collider)
        {
            if (enabled && !collider.GetComponent<VRTK_PlayerObject>() && ValidTarget(collider.transform))
            {
                headsetColliding = true;
                collidingWith = collider;
                OnHeadsetCollisionDetect(SetHeadsetCollisionEvent(collider, transform));
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            EndCollision(collider);
        }

        private void Update()
        {
            if (headsetColliding && (!collidingWith || !collidingWith.gameObject.activeInHierarchy))
            {
                EndCollision(collidingWith);
            }
        }

        private void EndCollision(Collider collider)
        {
            if (!collider || !collider.GetComponent<VRTK_PlayerObject>())
            {
                headsetColliding = false;
                collidingWith = null;
                OnHeadsetCollisionEnded(SetHeadsetCollisionEvent(collider, transform));
            }
        }
    }
}