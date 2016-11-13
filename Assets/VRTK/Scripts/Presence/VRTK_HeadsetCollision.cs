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
    /// The Headset Collision script is added to the `[CameraRig]` prefab. It will automatically create a script on the headset to deal with the collision events.
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

        private Transform headset;
        private VRTK_HeadsetCollider headsetColliderScript;
        private bool generateCollider = false;
        private bool generateRigidbody = false;

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
            headset = VRTK_DeviceFinder.HeadsetTransform();
            headsetColliding = false;
            Utilities.SetPlayerObject(headset.gameObject, VRTK_PlayerObject.ObjectTypes.Headset);
            SetupHeadset();
        }

        private void OnDisable()
        {
            headsetColliderScript.EndCollision(collidingWith);
            VRTK_ObjectCache.registeredHeadsetCollider = null;
            TearDownHeadset();
        }

        private void SetupHeadset()
        {
            var headsetCollider = headset.GetComponent<Collider>();
            if (!headsetCollider)
            {
                var newCollider = headset.gameObject.AddComponent<BoxCollider>();
                newCollider.size = new Vector3(0.1f, 0.1f, 0.1f);
                headsetCollider = newCollider;
                generateCollider = true;
            }
            headsetCollider.isTrigger = true;

            var headsetRigidbody = headset.GetComponent<Rigidbody>();
            if (!headsetRigidbody)
            {
                headsetRigidbody = headset.gameObject.AddComponent<Rigidbody>();
                generateRigidbody = true;
            }
            headsetRigidbody.isKinematic = true;
            headsetRigidbody.useGravity = false;

            if (!headsetColliderScript)
            {
                headsetColliderScript = headset.gameObject.AddComponent<VRTK_HeadsetCollider>();
                headsetColliderScript.SetParent(gameObject);
                headsetColliderScript.SetIgnoreTarget(ignoreTargetWithTagOrClass, targetTagOrScriptListPolicy);
            }
        }

        private void TearDownHeadset()
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
        }
    }

    public class VRTK_HeadsetCollider : MonoBehaviour
    {
        private VRTK_HeadsetCollision parent;
        private string ignoreTargetWithTagOrClass;
        private VRTK_TagOrScriptPolicyList targetTagOrScriptListPolicy;

        public void SetParent(GameObject setParent)
        {
            parent = setParent.GetComponent<VRTK_HeadsetCollision>();
        }

        public void SetIgnoreTarget(string ignore, VRTK_TagOrScriptPolicyList list = null)
        {
            ignoreTargetWithTagOrClass = ignore;
            targetTagOrScriptListPolicy = list;
        }

        public void EndCollision(Collider collider)
        {
            if (!collider || !collider.GetComponent<VRTK_PlayerObject>())
            {
                parent.headsetColliding = false;
                parent.collidingWith = null;
                parent.OnHeadsetCollisionEnded(SetHeadsetCollisionEvent(collider, transform));
            }
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
                parent.headsetColliding = true;
                parent.collidingWith = collider;
                parent.OnHeadsetCollisionDetect(SetHeadsetCollisionEvent(collider, transform));
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            EndCollision(collider);
        }

        private void Update()
        {
            if (parent.headsetColliding && (!parent.collidingWith || !parent.collidingWith.gameObject.activeInHierarchy))
            {
                EndCollision(parent.collidingWith);
            }
        }
    }
}