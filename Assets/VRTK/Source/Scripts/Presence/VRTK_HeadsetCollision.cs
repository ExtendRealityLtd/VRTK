﻿// Headset Collision|Presence|70010
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
    /// Denotes when the HMD is colliding with valid geometry.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_HeadsetCollision` script on any active scene GameObject.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/011_Camera_HeadSetCollisionFading` has collidable walls around the play area and if the user puts their head into any of the walls then the headset will fade to black.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_HeadsetCollision")]
    public class VRTK_HeadsetCollision : MonoBehaviour
    {
        [Tooltip("If this is checked then the headset collision will ignore colliders set to `Is Trigger = true`.")]
        public bool ignoreTriggerColliders = false;
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
        /// <returns>Returns `true` if the headset is currently colliding with a valid game object.</returns>
        public virtual bool IsColliding()
        {
            return headsetColliding;
        }

        /// <summary>
        /// The GetHeadsetColliderContainer method returns the auto generated GameObject that contains the headset collider.
        /// </summary>
        /// <returns>The auto generated headset collider GameObject.</returns>
        public virtual GameObject GetHeadsetColliderContainer()
        {
            return headsetColliderContainer;
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            headset = VRTK_DeviceFinder.HeadsetTransform();
            if (headset != null)
            {
                headsetColliding = false;
                SetupHeadset();
                VRTK_PlayerObject.SetPlayerObject(headsetColliderContainer.gameObject, VRTK_PlayerObject.ObjectTypes.Headset);
            }
        }

        protected virtual void OnDisable()
        {
            if (headset != null && headsetColliderScript != null)
            {
                headsetColliderScript.EndCollision(collidingWith);
                TearDownHeadset();
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            if (headsetColliderContainer != null && headsetColliderContainer.transform.parent != headset)
            {
                headsetColliderContainer.transform.SetParent(headset);
                headsetColliderContainer.transform.localPosition = Vector3.zero;
                headsetColliderContainer.transform.localRotation = headset.localRotation;
            }
        }

        protected virtual void CreateHeadsetColliderContainer()
        {
            if (headsetColliderContainer == null)
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
            Rigidbody headsetRigidbody = headset.GetComponentInChildren<Rigidbody>();
            if (headsetRigidbody == null)
            {
                CreateHeadsetColliderContainer();
                headsetRigidbody = headsetColliderContainer.AddComponent<Rigidbody>();
                headsetRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                generateRigidbody = true;
            }
            headsetRigidbody.isKinematic = true;
            headsetRigidbody.useGravity = false;

            Collider headsetCollider = headset.GetComponentInChildren<Collider>();
            if (headsetCollider == null)
            {
                CreateHeadsetColliderContainer();
                SphereCollider newCollider = headsetColliderContainer.gameObject.AddComponent<SphereCollider>();
                newCollider.radius = colliderRadius;
                headsetCollider = newCollider;
                generateCollider = true;
            }
            headsetCollider.isTrigger = true;

            if (headsetColliderScript == null)
            {
                GameObject attachTo = (headsetColliderContainer ? headsetColliderContainer : headset.gameObject);
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
            if (headsetColliderScript != null)
            {
                Destroy(headsetColliderScript);
            }
            if (headsetColliderContainer != null)
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
            if (collider == null || !VRTK_PlayerObject.IsPlayerObject(collider.gameObject))
            {
                parent.headsetColliding = false;
                parent.collidingWith = null;
                parent.OnHeadsetCollisionEnded(SetHeadsetCollisionEvent(collider, transform));
            }
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            if (parent.ignoreTriggerColliders && collider != null && collider.isTrigger)
            {
                return;
            }

            if (enabled && !VRTK_PlayerObject.IsPlayerObject(collider.gameObject) && ValidTarget(collider.transform))
            {
                parent.headsetColliding = true;
                parent.collidingWith = collider;
                parent.OnHeadsetCollisionDetect(SetHeadsetCollisionEvent(collider, transform));
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            if (parent.ignoreTriggerColliders && collider != null && collider.isTrigger)
            {
                return;
            }

            EndCollision(collider);
        }

        protected virtual void Update()
        {
            if (parent.headsetColliding && (parent.collidingWith == null || !parent.collidingWith.gameObject.activeInHierarchy))
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
            return (target != null && !(VRTK_PolicyList.Check(target.gameObject, targetListPolicy)));
        }
    }
}