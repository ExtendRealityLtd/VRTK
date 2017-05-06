// Headset Collision|Presence|70010
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
    /// The Headset Collision script will automatically create a script on the headset to deal with the collision events.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_HeadsetCollision")]
    public class VRTK_HeadsetCollision : MonoBehaviour
    {
        [Tooltip("The radius of the auto generated sphere collider for detecting collisions on the headset.")]
        public float colliderRadius = 0.1f;
        [Tooltip("A specified VRTK_PolicyList to use to determine whether any objects will be acted upon by the Headset Collision.")]
        public VRTK_PolicyList targetListPolicy;

        /// <summary>
        /// Emitted when the user's headset collides with another game object.
        /// </summary>
        public event HeadsetCollisionEventHandler HeadsetCollisionDetect;
        /// <summary>
        /// Emitted when the user's headset stops colliding with a game object.
        /// </summary>
        public event HeadsetCollisionEventHandler HeadsetCollisionEnded;

        /// <summary>
        /// Determines if the headset is currently colliding with another object.
        /// </summary>
        [HideInInspector]
        public bool headsetColliding = false;
        /// <summary>
        /// Stores the collider of what the headset is colliding with.
        /// </summary>
        [HideInInspector]
        public Collider collidingWith = null;

        protected Transform headset;
        protected VRTK_HeadsetCollider headsetColliderScript;
        protected GameObject headsetColliderContainer;
        protected bool generateCollider = false;
        protected bool generateRigidbody = false;

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

        protected virtual void OnEnable()
        {
            VRTK_ObjectCache.registeredHeadsetCollider = this;
            headset = VRTK_DeviceFinder.HeadsetTransform();
            if (headset)
            {
                headsetColliding = false;
                SetupHeadset();
                VRTK_PlayerObject.SetPlayerObject(headsetColliderContainer.gameObject, VRTK_PlayerObject.ObjectTypes.Headset);
            }
        }

        protected virtual void OnDisable()
        {
            if (headset)
            {
                headsetColliderScript.EndCollision(collidingWith);
                VRTK_ObjectCache.registeredHeadsetCollider = null;
                TearDownHeadset();
            }
        }

        protected virtual void Update()
        {
            if (headsetColliderContainer && headsetColliderContainer.transform.parent != headset)
            {
                headsetColliderContainer.transform.SetParent(headset);
                headsetColliderContainer.transform.localPosition = Vector3.zero;
                headsetColliderContainer.transform.localRotation = headset.localRotation;
            }
        }

        protected virtual void CreateHeadsetColliderContainer()
        {
            if (!headsetColliderContainer)
            {
                headsetColliderContainer = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, "HeadsetColliderContainer"));
                headsetColliderContainer.transform.position = Vector3.zero;
                headsetColliderContainer.transform.localRotation = headset.localRotation;
                headsetColliderContainer.transform.localScale = Vector3.one;
                headsetColliderContainer.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }

        protected virtual void SetupHeadset()
        {
            var headsetRigidbody = headset.GetComponentInChildren<Rigidbody>();
            if (!headsetRigidbody)
            {
                CreateHeadsetColliderContainer();
                headsetRigidbody = headsetColliderContainer.AddComponent<Rigidbody>();
                headsetRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                generateRigidbody = true;
            }
            headsetRigidbody.isKinematic = true;
            headsetRigidbody.useGravity = false;

            var headsetCollider = headset.GetComponentInChildren<Collider>();
            if (!headsetCollider)
            {
                CreateHeadsetColliderContainer();
                var newCollider = headsetColliderContainer.gameObject.AddComponent<SphereCollider>();
                newCollider.radius = colliderRadius;
                headsetCollider = newCollider;
                generateCollider = true;
            }
            headsetCollider.isTrigger = true;

            if (!headsetColliderScript)
            {
                var attachTo = (headsetColliderContainer ? headsetColliderContainer : headset.gameObject);
                headsetColliderScript = attachTo.AddComponent<VRTK_HeadsetCollider>();
                headsetColliderScript.SetParent(gameObject);
                headsetColliderScript.SetIgnoreTarget(targetListPolicy);
            }
        }

        protected virtual void TearDownHeadset()
        {
            if (generateCollider)
            {
                Destroy(headset.gameObject.GetComponent<BoxCollider>());
            }
            if (generateRigidbody)
            {
                Destroy(headset.gameObject.GetComponent<Rigidbody>());
            }
            if (headsetColliderScript)
            {
                Destroy(headsetColliderScript);
            }
            if (headsetColliderContainer)
            {
                Destroy(headsetColliderContainer);
            }
        }
    }

    public class VRTK_HeadsetCollider : MonoBehaviour
    {
        protected VRTK_HeadsetCollision parent;
        protected VRTK_PolicyList targetListPolicy;

        public virtual void SetParent(GameObject setParent)
        {
            parent = setParent.GetComponent<VRTK_HeadsetCollision>();
        }

        public virtual void SetIgnoreTarget(VRTK_PolicyList list = null)
        {
            targetListPolicy = list;
        }

        public virtual void EndCollision(Collider collider)
        {
            if (!collider || !VRTK_PlayerObject.IsPlayerObject(collider.gameObject))
            {
                parent.headsetColliding = false;
                parent.collidingWith = null;
                parent.OnHeadsetCollisionEnded(SetHeadsetCollisionEvent(collider, transform));
            }
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            if (enabled && !VRTK_PlayerObject.IsPlayerObject(collider.gameObject) && ValidTarget(collider.transform))
            {
                parent.headsetColliding = true;
                parent.collidingWith = collider;
                parent.OnHeadsetCollisionDetect(SetHeadsetCollisionEvent(collider, transform));
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            EndCollision(collider);
        }

        protected virtual void Update()
        {
            if (parent.headsetColliding && (!parent.collidingWith || !parent.collidingWith.gameObject.activeInHierarchy))
            {
                EndCollision(parent.collidingWith);
            }
        }

        protected virtual HeadsetCollisionEventArgs SetHeadsetCollisionEvent(Collider collider, Transform currentTransform)
        {
            HeadsetCollisionEventArgs e;
            e.collider = collider;
            e.currentTransform = currentTransform;
            return e;
        }

        protected virtual bool ValidTarget(Transform target)
        {
            return (target && !(VRTK_PolicyList.Check(target.gameObject, targetListPolicy)));
        }
    }
}