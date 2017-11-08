﻿// Body Physics|Presence|70060
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="target">The target GameObject the event is dealing with.</param>
    /// <param name="collider">An optional collider that the body physics is colliding with.</param>
    public struct BodyPhysicsEventArgs
    {
        public GameObject target;
        public Collider collider;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="BodyPhysicsEventArgs"/></param>
    public delegate void BodyPhysicsEventHandler(object sender, BodyPhysicsEventArgs e);

    /// <summary>
    /// Allows the play area to be affected by physics and detect collisions with other valid geometry.
    /// </summary>
    /// <remarks>
    /// **Optional Components:**
    ///  * `VRTK_BasicTeleport` - A Teleporter script to use when snapping the play area to the nearest floor when releasing from grab.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_BodyPhysics` script on any active scene GameObject.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad but the user cannot pass through the objects as they are collidable and the rigidbody physics won't allow the intersection to occur.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_BodyPhysics")]
    public class VRTK_BodyPhysics : VRTK_DestinationMarker
    {
        /// <summary>
        /// Options for testing if a play space fall is valid
        /// </summary>
        public enum FallingRestrictors
        {
            /// <summary>
            /// Always drop to nearest floor when the headset is no longer over the current standing object.
            /// </summary>
            NoRestriction,
            /// <summary>
            /// Don't drop to nearest floor  if the Left Controller is still over the current standing object even if the headset isn't.
            /// </summary>
            LeftController,
            /// <summary>
            /// Don't drop to nearest floor  if the Right Controller is still over the current standing object even if the headset isn't.
            /// </summary>
            RightController,
            /// <summary>
            /// Don't drop to nearest floor  if Either Controller is still over the current standing object even if the headset isn't.
            /// </summary>
            EitherController,
            /// <summary>
            /// Don't drop to nearest floor only if Both Controllers are still over the current standing object even if the headset isn't.
            /// </summary>
            BothControllers,
            /// <summary>
            /// Never drop to nearest floor when the headset is no longer over the current standing object.
            /// </summary>
            AlwaysRestrict
        }

        [Header("Body Collision Settings")]

        [Tooltip("If checked then the body collider and rigidbody will be used to check for rigidbody collisions.")]
        public bool enableBodyCollisions = true;
        [Tooltip("If this is checked then any items that are grabbed with the controller will not collide with the body collider. This is very useful if the user is required to grab and wield objects because if the collider was active they would bounce off the collider.")]
        public bool ignoreGrabbedCollisions = true;
        [Tooltip("An array of GameObjects that will not collide with the body collider.")]
        public GameObject[] ignoreCollisionsWith;
        [Tooltip("The collider which is created for the user is set at a height from the user's headset position. If the collider is required to be lower to allow for room between the play area collider and the headset then this offset value will shorten the height of the generated collider.")]
        public float headsetYOffset = 0.2f;
        [Tooltip("The amount of movement of the headset between the headset's current position and the current standing position to determine if the user is walking in play space and to ignore the body physics collisions if the movement delta is above this threshold.")]
        public float movementThreshold = 0.0015f;
        [Tooltip("The amount of movement of the play area between the play area's current position and the previous position to determine if the user is moving play space.")]
        public float playAreaMovementThreshold = 0.00075f;
        [Tooltip("The maximum number of samples to collect of headset position before determining if the current standing position within the play space has changed.")]
        public int standingHistorySamples = 5;
        [Tooltip("The `y` distance between the headset and the object being leaned over, if object being leaned over is taller than this threshold then the current standing position won't be updated.")]
        public float leanYThreshold = 0.5f;

        [Header("Step Settings")]

        [Tooltip("The maximum height to consider when checking if an object can be stepped upon to.")]
        public float stepUpYOffset = 0.15f;
        [Tooltip("The width/depth of the foot collider in relation to the radius of the body collider.")]
        [Range(0.1f, 0.9f)]
        public float stepThicknessMultiplier = 0.5f;
        [Tooltip("The distance between the current play area Y position and the new stepped up Y position to consider a valid step up. A higher number can help with juddering on slopes or small increases in collider heights.")]
        public float stepDropThreshold = 0.08f;

        [Header("Snap To Floor Settings")]

        [Tooltip("A custom raycaster to use when raycasting to find floors.")]
        public VRTK_CustomRaycast customRaycast;
        [Tooltip("A check to see if the drop to nearest floor should take place. If the selected restrictor is still over the current floor then the drop to nearest floor will not occur. Works well for being able to lean over ledges and look down. Only works for falling down not teleporting up.")]
        public FallingRestrictors fallRestriction = FallingRestrictors.NoRestriction;
        [Tooltip("When the `y` distance between the floor and the headset exceeds this distance and `Enable Body Collisions` is true then the rigidbody gravity will be used instead of teleport to drop to nearest floor.")]
        public float gravityFallYThreshold = 1.0f;
        [Tooltip("The `y` distance between the floor and the headset that must change before a fade transition is initiated. If the new user location is at a higher distance than the threshold then the headset blink transition will activate on teleport. If the new user location is within the threshold then no blink transition will happen, which is useful for walking up slopes, meshes and terrains to prevent constant blinking.")]
        public float blinkYThreshold = 0.15f;
        [Tooltip("The amount the `y` position needs to change by between the current floor `y` position and the previous floor `y` position before a change in floor height is considered to have occurred. A higher value here will mean that a `Drop To Floor` will be less likely to happen if the `y` of the floor beneath the user hasn't changed as much as the given threshold.")]
        public float floorHeightTolerance = 0.001f;
        [Range(1, 10)]
        [Tooltip("The amount of rounding on the play area Y position to be applied when checking if falling is occuring.")]
        public int fallCheckPrecision = 5;

        [Header("Custom Settings")]

        [Tooltip("The VRTK Teleport script to use when snapping to floor. If this is left blank then a Teleport script will need to be applied to the same GameObject.")]
        public VRTK_BasicTeleport teleporter;
        [Tooltip("A custom Rigidbody to apply to the play area. If one is not provided, then if an existing rigidbody is found on the play area GameObject it will be used, otherwise a default one will be created.")]
        public Rigidbody customPlayAreaRigidbody = null;
        [Tooltip("A GameObject to represent a custom body collider container. It should contain a collider component that will be used for detecting body collisions. If one isn't provided then it will be auto generated.")]
        public GameObject customBodyColliderContainer = null;
        [Tooltip("A GameObject to represent a custom foot collider container. It should contain a collider component that will be used for detecting step collisions. If one isn't provided then it will be auto generated.")]
        public GameObject customFootColliderContainer = null;

        /// <summary>
        /// Emitted when a fall begins.
        /// </summary>
        public event BodyPhysicsEventHandler StartFalling;
        /// <summary>
        /// Emitted when a fall ends.
        /// </summary>
        public event BodyPhysicsEventHandler StopFalling;
        /// <summary>
        /// Emitted when movement in the play area begins.
        /// </summary>
        public event BodyPhysicsEventHandler StartMoving;
        /// <summary>
        /// Emitted when movement in the play area ends.
        /// </summary>
        public event BodyPhysicsEventHandler StopMoving;
        /// <summary>
        /// Emitted when the body collider starts colliding with another game object.
        /// </summary>
        public event BodyPhysicsEventHandler StartColliding;
        /// <summary>
        /// Emitted when the body collider stops colliding with another game object.
        /// </summary>
        public event BodyPhysicsEventHandler StopColliding;
        /// <summary>
        /// Emitted when the body collider starts leaning over another game object.
        /// </summary>
        public event BodyPhysicsEventHandler StartLeaning;
        /// <summary>
        /// Emitted when the body collider stops leaning over another game object.
        /// </summary>
        public event BodyPhysicsEventHandler StopLeaning;
        /// <summary>
        /// Emitted when the body collider starts touching the ground.
        /// </summary>
        public event BodyPhysicsEventHandler StartTouchingGround;
        /// <summary>
        /// Emitted when the body collider stops touching the ground.
        /// </summary>
        public event BodyPhysicsEventHandler StopTouchingGround;

        protected Transform playArea;
        protected Transform headset;
        protected Rigidbody bodyRigidbody;
        protected GameObject bodyColliderContainer;
        protected GameObject footColliderContainer;

        protected CapsuleCollider bodyCollider;
        protected CapsuleCollider footCollider;

        protected VRTK_CollisionTracker collisionTracker;
        protected bool currentBodyCollisionsSetting;
        protected GameObject currentCollidingObject = null;
        protected GameObject currentValidFloorObject = null;

        protected float lastFrameFloorY;
        protected float hitFloorYDelta = 0f;
        protected bool initialFloorDrop = false;
        protected bool resetPhysicsAfterTeleport = false;
        protected bool storedCurrentPhysics = false;
        protected bool retogglePhysicsOnCanFall = false;
        protected bool storedRetogglePhysics;
        protected Vector3 lastPlayAreaPosition = Vector3.zero;
        protected Vector2 currentStandingPosition;
        protected List<Vector2> standingPositionHistory = new List<Vector2>();
        protected float playAreaHeightAdjustment = 0.009f;
        protected float bodyMass = 100f;
        protected float bodyRadius = 0.15f;
        protected float leanForwardLengthAddition = 0.05f;
        protected float playAreaPositionThreshold = 0.002f;
        protected float gravityPush = -0.001f;
        protected int decimalPrecision = 3;

        protected bool isFalling = false;
        protected bool isMoving = false;
        protected bool isLeaning = false;
        protected bool onGround = true;
        protected bool preventSnapToFloor = false;
        protected bool generateRigidbody = false;
        protected Vector3 playAreaVelocity = Vector3.zero;
        protected string footColliderContainerNameCheck;
        protected const string BODY_COLLIDER_CONTAINER_NAME = "BodyColliderContainer";
        protected const string FOOT_COLLIDER_CONTAINER_NAME = "FootColliderContainer";
        protected bool enableBodyCollisionsStartingValue;
        protected float fallMinTime;
        protected List<GameObject> ignoreCollisionsOnGameObjects = new List<GameObject>();
        protected Transform cachedGrabbedObjectTransform = null;
        protected VRTK_InteractableObject cachedGrabbedObject;
        protected LayerMask defaultIgnoreLayer = Physics.IgnoreRaycastLayer;
        protected Coroutine restoreCollisionsRoutine;

        // Draws a sphere for current standing position and a sphere for current headset position.
        // Set to `true` to view the debug spheres.
        protected bool drawDebugGizmo = false;

        /// <summary>
        /// The ArePhysicsEnabled method determines whether the body physics are set to interact with other scene physics objects.
        /// </summary>
        /// <returns>Returns `true` if the body physics will interact with other scene physics objects and `false` if the body physics will ignore other scene physics objects.</returns>
        public virtual bool ArePhysicsEnabled()
        {
            return (bodyRigidbody != null ? !bodyRigidbody.isKinematic : false);
        }

        /// <summary>
        /// The ApplyBodyVelocity method applies a given velocity to the rigidbody attached to the body physics.
        /// </summary>
        /// <param name="velocity">The velocity to apply.</param>
        /// <param name="forcePhysicsOn">If `true` will toggle the body collision physics back on if enable body collisions is true.</param>
        /// <param name="applyMomentum">If `true` then the existing momentum of the play area will be applied as a force to the resulting velocity.</param>
        public virtual void ApplyBodyVelocity(Vector3 velocity, bool forcePhysicsOn = false, bool applyMomentum = false)
        {
            if (enableBodyCollisions && forcePhysicsOn)
            {
                TogglePhysics(true);
            }

            if (ArePhysicsEnabled())
            {
                Vector3 appliedGravity = new Vector3(0f, gravityPush, 0f);
                bodyRigidbody.velocity = velocity + appliedGravity;
                ApplyBodyMomentum(applyMomentum);
                StartFall(currentValidFloorObject);
            }
        }

        /// <summary>
        /// The ToggleOnGround method sets whether the body is considered on the ground or not.
        /// </summary>
        /// <param name="state">If `true` then body physics are set to being on the ground.</param>
        public virtual void ToggleOnGround(bool state)
        {
            onGround = state;
            if (onGround)
            {
                OnStartTouchingGround(SetBodyPhysicsEvent(currentValidFloorObject, null));
            }
            else
            {
                OnStopTouchingGround(SetBodyPhysicsEvent(null, null));
            }
        }

        /// <summary>
        /// The PreventSnapToFloor method sets whether the snap to floor mechanic should be used.
        /// </summary>
        /// <param name="state">If `true` the the snap to floor mechanic will not execute.</param>
        public virtual void TogglePreventSnapToFloor(bool state)
        {
            preventSnapToFloor = state;
        }

        /// <summary>
        /// The ForceSnapToFloor method disables the prevent snap to floor and forces the snap to nearest floor action.
        /// </summary>
        public virtual void ForceSnapToFloor()
        {
            TogglePreventSnapToFloor(false);
            SnapToNearestFloor();
        }

        /// <summary>
        /// The IsFalling method returns the falling state of the body.
        /// </summary>
        /// <returns>Returns `true` if the body is currently falling via gravity or via teleport.</returns>
        public virtual bool IsFalling()
        {
            return isFalling;
        }

        /// <summary>
        /// The IsMoving method returns the moving within play area state of the body.
        /// </summary>
        /// <returns>Returns true if the user is currently walking around their play area space.</returns>
        public virtual bool IsMoving()
        {
            return isMoving;
        }

        /// <summary>
        /// The IsLeaning method returns the leaning state of the user.
        /// </summary>
        /// <returns>Returns `true` if the user is considered to be leaning over an object.</returns>
        public virtual bool IsLeaning()
        {
            return isLeaning;
        }

        /// <summary>
        /// The OnGround method returns whether the user is currently standing on the ground or not.
        /// </summary>
        /// <returns>Returns `true` if the play area is on the ground and false if the play area is in the air.</returns>
        public virtual bool OnGround()
        {
            return onGround;
        }

        /// <summary>
        /// The GetVelocity method returns the velocity of the body physics rigidbody.
        /// </summary>
        /// <returns>The velocity of the body physics rigidbody.</returns>
        public virtual Vector3 GetVelocity()
        {
            return (bodyRigidbody != null ? bodyRigidbody.velocity : Vector3.zero);
        }

        /// <summary>
        /// The GetAngularVelocity method returns the angular velocity of the body physics rigidbody.
        /// </summary>
        /// <returns>The angular velocity of the body physics rigidbody.</returns>
        public virtual Vector3 GetAngularVelocity()
        {
            return (bodyRigidbody != null ? bodyRigidbody.angularVelocity : Vector3.zero);
        }

        /// <summary>
        /// The ResetVelocities method sets the rigidbody velocity and angular velocity to zero to stop the Play Area rigidbody from continuing to move if it has a velocity already.
        /// </summary>
        public virtual void ResetVelocities()
        {
            bodyRigidbody.velocity = Vector3.zero;
            bodyRigidbody.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// The ResetFalling method force stops any falling states and conditions that might be set on this object.
        /// </summary>
        public virtual void ResetFalling()
        {
            StopFall();
        }

        /// <summary>
        /// The GetBodyColliderContainer method returns the auto generated GameObject that contains the body colliders.
        /// </summary>
        /// <returns>The auto generated body collider GameObject.</returns>
        /// <returns></returns>
        public virtual GameObject GetBodyColliderContainer()
        {
            return bodyColliderContainer;
        }

        /// <summary>
        /// The GetFootColliderContainer method returns the auto generated GameObject that contains the foot colliders.
        /// </summary>
        /// <returns>The auto generated foot collider GameObject.</returns>
        /// <returns></returns>
        public virtual GameObject GetFootColliderContainer()
        {
            return footColliderContainer;
        }

        /// <summary>
        /// The GetCurrentCollidingObject method returns the object that the body physics colliders are currently colliding with.
        /// </summary>
        /// <returns>The GameObject that is colliding with the body physics colliders.</returns>
        public virtual GameObject GetCurrentCollidingObject()
        {
            return currentCollidingObject;
        }

        /// <summary>
        /// The ResetIgnoredCollisions method is used to clear any stored ignored colliders in case the `Ignore Collisions On` array parameter is changed at runtime. This needs to be called manually if changes are made at runtime.
        /// </summary>
        public virtual void ResetIgnoredCollisions()
        {
            //Go through all the existing set up ignored colliders and reset their collision state
            for (int i = 0; i < ignoreCollisionsOnGameObjects.Count; i++)
            {
                if (ignoreCollisionsOnGameObjects[i] != null)
                {
                    Collider[] objectColliders = ignoreCollisionsOnGameObjects[i].GetComponentsInChildren<Collider>();
                    for (int j = 0; j < objectColliders.Length; j++)
                    {
                        ManagePhysicsCollider(objectColliders[j], false);
                    }
                }
            }

            ignoreCollisionsOnGameObjects.Clear();
        }

        /// <summary>
        /// The SweepCollision method tests to see if a collision will occur with the body collider in a given direction and distance.
        /// </summary>
        /// <param name="direction">The direction to test for the potential collision.</param>
        /// <param name="maxDistance">The maximum distance to check for a potential collision.</param>
        /// <returns>Returns `true` if a collision will occur on the given direction over the given maxium distance. Returns `false` if there is no collision about to happen.</returns>
        public virtual bool SweepCollision(Vector3 direction, float maxDistance)
        {
            Vector3 point1 = bodyCollider.transform.parent.TransformPoint(bodyCollider.transform.localPosition + (bodyCollider.center)) + (Vector3.up * ((bodyCollider.height * 0.5f) - bodyCollider.radius));
            Vector3 point2 = bodyCollider.transform.parent.TransformPoint(bodyCollider.transform.localPosition + (bodyCollider.center)) - (Vector3.up * ((bodyCollider.height * 0.5f) - bodyCollider.radius));
            RaycastHit collisionHit;
            return VRTK_CustomRaycast.CapsuleCast(customRaycast, point1, point2, bodyCollider.radius, direction, maxDistance, out collisionHit, defaultIgnoreLayer, QueryTriggerInteraction.Ignore);
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetupPlayArea();
            SetupHeadset();
            footColliderContainerNameCheck = VRTK_SharedMethods.GenerateVRTKObjectName(true, FOOT_COLLIDER_CONTAINER_NAME);
            enableBodyCollisionsStartingValue = enableBodyCollisions;
            EnableDropToFloor();
            EnableBodyPhysics();
            SetupIgnoredCollisions();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DisableDropToFloor();
            DisableBodyPhysics();
            ManageCollisionListeners(false);
            ResetIgnoredCollisions();
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void FixedUpdate()
        {
            CheckBodyCollisionsSetting();
            ManageFalling();
            CalculateVelocity();
            UpdateCollider();

            lastPlayAreaPosition = (playArea != null ? playArea.position : Vector3.zero);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (CheckValidCollision(collision.gameObject))
            {
                CheckStepUpCollision(collision);
                currentCollidingObject = collision.gameObject;
                OnStartColliding(SetBodyPhysicsEvent(currentCollidingObject, collision.collider));
            }
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (CheckValidCollision(collider.gameObject))
            {
                currentCollidingObject = collider.gameObject;
                OnStartColliding(SetBodyPhysicsEvent(currentCollidingObject, collider));
            }

        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (CheckExistingCollision(collision.gameObject))
            {
                OnStopColliding(SetBodyPhysicsEvent(currentCollidingObject, collision.collider));
                currentCollidingObject = null;
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            if (CheckExistingCollision(collider.gameObject))
            {
                OnStopColliding(SetBodyPhysicsEvent(currentCollidingObject, collider));
                currentCollidingObject = null;
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (drawDebugGizmo && headset != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(new Vector3(headset.position.x, headset.position.y - 0.3f, headset.position.z), 0.075f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector3(currentStandingPosition.x, headset.position.y - 0.3f, currentStandingPosition.y), 0.05f);
            }
        }

        protected virtual bool CheckValidCollision(GameObject checkObject)
        {
            return (!VRTK_PlayerObject.IsPlayerObject(checkObject) && (!onGround || (currentValidFloorObject != null && currentValidFloorObject != checkObject)));
        }

        protected virtual bool CheckExistingCollision(GameObject checkObject)
        {
            return (currentCollidingObject != null && currentCollidingObject == checkObject);
        }

        protected virtual void SetupPlayArea()
        {
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            if (playArea != null)
            {
                lastPlayAreaPosition = playArea.position;
                collisionTracker = playArea.GetComponent<VRTK_CollisionTracker>();
                if (collisionTracker == null)
                {
                    collisionTracker = playArea.gameObject.AddComponent<VRTK_CollisionTracker>();
                }
                ManageCollisionListeners(true);
            }
        }

        protected virtual void SetupHeadset()
        {
            headset = VRTK_DeviceFinder.HeadsetTransform();
            if (headset != null)
            {
                currentStandingPosition = new Vector2(headset.position.x, headset.position.z);
            }
        }

        protected virtual void ManageCollisionListeners(bool state)
        {
            if (collisionTracker != null)
            {
                if (state)
                {
                    collisionTracker.CollisionEnter += CollisionTracker_CollisionEnter;
                    collisionTracker.CollisionExit += CollisionTracker_CollisionExit;
                    collisionTracker.TriggerEnter += CollisionTracker_TriggerEnter;
                    collisionTracker.TriggerExit += CollisionTracker_TriggerExit;
                }
                else
                {
                    collisionTracker.CollisionEnter -= CollisionTracker_CollisionEnter;
                    collisionTracker.CollisionExit -= CollisionTracker_CollisionExit;
                    collisionTracker.TriggerEnter -= CollisionTracker_TriggerEnter;
                    collisionTracker.TriggerExit -= CollisionTracker_TriggerExit;
                }
            }
        }

        protected virtual void CollisionTracker_TriggerExit(object sender, CollisionTrackerEventArgs e)
        {
            OnTriggerExit(e.collider);
        }

        protected virtual void CollisionTracker_TriggerEnter(object sender, CollisionTrackerEventArgs e)
        {
            OnTriggerEnter(e.collider);
        }

        protected virtual void CollisionTracker_CollisionExit(object sender, CollisionTrackerEventArgs e)
        {
            OnCollisionExit(e.collision);
        }

        protected virtual void CollisionTracker_CollisionEnter(object sender, CollisionTrackerEventArgs e)
        {
            OnCollisionEnter(e.collision);
        }

        protected virtual void OnStartFalling(BodyPhysicsEventArgs e)
        {
            if (StartFalling != null)
            {
                StartFalling(this, e);
            }
        }

        protected virtual void OnStopFalling(BodyPhysicsEventArgs e)
        {
            if (StopFalling != null)
            {
                StopFalling(this, e);
            }
        }

        protected virtual void OnStartMoving(BodyPhysicsEventArgs e)
        {
            if (StartMoving != null)
            {
                StartMoving(this, e);
            }
        }

        protected virtual void OnStopMoving(BodyPhysicsEventArgs e)
        {
            if (StopMoving != null)
            {
                StopMoving(this, e);
            }
        }

        protected virtual void OnStartColliding(BodyPhysicsEventArgs e)
        {
            if (StartColliding != null)
            {
                StartColliding(this, e);
            }
        }

        protected virtual void OnStopColliding(BodyPhysicsEventArgs e)
        {
            if (StopColliding != null)
            {
                StopColliding(this, e);
            }
        }

        protected virtual void OnStartLeaning(BodyPhysicsEventArgs e)
        {
            if (StartLeaning != null)
            {
                StartLeaning(this, e);
            }
        }

        protected virtual void OnStopLeaning(BodyPhysicsEventArgs e)
        {
            if (StopLeaning != null)
            {
                StopLeaning(this, e);
            }
        }

        protected virtual void OnStartTouchingGround(BodyPhysicsEventArgs e)
        {
            if (StartTouchingGround != null)
            {
                StartTouchingGround(this, e);
            }
        }

        protected virtual void OnStopTouchingGround(BodyPhysicsEventArgs e)
        {
            if (StopTouchingGround != null)
            {
                StopTouchingGround(this, e);
            }
        }

        protected virtual BodyPhysicsEventArgs SetBodyPhysicsEvent(GameObject target, Collider collider)
        {
            BodyPhysicsEventArgs e;
            e.target = target;
            e.collider = collider;
            return e;
        }

        protected virtual void CalculateVelocity()
        {
            playAreaVelocity = (playArea != null ? (playArea.position - lastPlayAreaPosition) / Time.fixedDeltaTime : Vector3.zero);
        }

        protected virtual void TogglePhysics(bool state)
        {
            if (bodyRigidbody != null)
            {
                bodyRigidbody.isKinematic = !state;
            }
            if (bodyCollider != null)
            {
                bodyCollider.isTrigger = !state;
            }
            if (footCollider != null)
            {
                footCollider.isTrigger = !state;
            }

            currentBodyCollisionsSetting = state;
        }

        protected virtual void ManageFalling()
        {
            if (!isFalling)
            {
                CheckHeadsetMovement();
                SnapToNearestFloor();
            }
            else
            {
                CheckFalling();
            }
        }

        protected virtual void CheckBodyCollisionsSetting()
        {
            if (enableBodyCollisions != currentBodyCollisionsSetting)
            {
                TogglePhysics(enableBodyCollisions);
            }
        }

        protected virtual void CheckFalling()
        {
            if (isFalling && fallMinTime < Time.time && VRTK_SharedMethods.RoundFloat(lastPlayAreaPosition.y, fallCheckPrecision) == VRTK_SharedMethods.RoundFloat(playArea.position.y, fallCheckPrecision))
            {
                StopFall();
            }
        }

        protected virtual void SetCurrentStandingPosition()
        {
            if (playArea != null && playArea.transform.position != lastPlayAreaPosition)
            {
                Vector3 playareaDifference = playArea.transform.position - lastPlayAreaPosition;
                currentStandingPosition = new Vector2(currentStandingPosition.x + playareaDifference.x, currentStandingPosition.y + playareaDifference.z);
            }
        }

        protected virtual void SetIsMoving(Vector2 currentHeadsetPosition)
        {
            float moveDistance = Vector2.Distance(currentHeadsetPosition, currentStandingPosition);
            float playareaDistance = (playArea != null ? Vector3.Distance(playArea.transform.position, lastPlayAreaPosition) : 0f);
            isMoving = (moveDistance > movementThreshold ? true : false);
            if (playArea != null && (playareaDistance > playAreaMovementThreshold || !onGround))
            {
                isMoving = false;
            }
        }

        protected virtual void CheckLean()
        {
            if (bodyCollider != null)
            {
                //Cast a ray down from the current standing position
                Vector3 standingDownRayStartPosition = (headset != null ? new Vector3(currentStandingPosition.x, headset.position.y, currentStandingPosition.y) : Vector3.zero);
                Vector3 rayDirection = (playArea != null ? -playArea.up : Vector3.zero);
                Ray standingDownRay = new Ray(standingDownRayStartPosition, rayDirection);
                RaycastHit standingDownRayCollision;

                //Determine the current valid floor that the user is standing over
                currentValidFloorObject = (VRTK_CustomRaycast.Raycast(customRaycast, standingDownRay, out standingDownRayCollision, defaultIgnoreLayer, Mathf.Infinity, QueryTriggerInteraction.Ignore) ? standingDownRayCollision.collider.gameObject : null);

                //Don't bother checking for lean if body collisions are disabled
                if (headset == null || playArea == null || !enableBodyCollisions)
                {
                    return;
                }

                //reset the headset x rotation so the forward ray is always horizontal regardless of the headset rotation
                Quaternion storedRotation = headset.rotation;
                headset.rotation = new Quaternion(0f, headset.rotation.y, headset.rotation.z, headset.rotation.w);

                Ray forwardRay = new Ray(headset.position, headset.forward);
                RaycastHit forwardRayCollision;
                //Determine the maximum forward distance to cast the forward ray
                float forwardLength = bodyCollider.radius + leanForwardLengthAddition;

                // Cast a ray forward just outside the body collider radius to see if anything is blocking your path
                // If nothing is blocking your path and you're currently standing over a valid floor
                if (!VRTK_CustomRaycast.Raycast(customRaycast, forwardRay, out forwardRayCollision, defaultIgnoreLayer, forwardLength, QueryTriggerInteraction.Ignore) && currentValidFloorObject != null)
                {
                    CalculateLean(standingDownRayStartPosition, forwardLength, standingDownRayCollision.distance);
                }

                //put the headset rotation back
                headset.rotation = storedRotation;
            }
        }

        protected virtual void CalculateLean(Vector3 startPosition, float forwardLength, float originalRayDistance)
        {
            //Cast the new down ray based on the position of the end of the forward ray but still at a flat plane of the headset forward (i.e. no headset rotation)
            Vector3 rayDownStartPosition = startPosition + (headset.forward * forwardLength);
            rayDownStartPosition = new Vector3(rayDownStartPosition.x, startPosition.y, rayDownStartPosition.z);

            Ray downRay = new Ray(rayDownStartPosition, -playArea.up);
            RaycastHit downRayCollision;

            //Cast a ray down from the end of the forward ray position
            if (VRTK_CustomRaycast.Raycast(customRaycast, downRay, out downRayCollision, defaultIgnoreLayer, Mathf.Infinity, QueryTriggerInteraction.Ignore))
            {
                //Determine the difference between the original down ray and the projected forward a bit downray
                float rayDownDelta = VRTK_SharedMethods.RoundFloat(originalRayDistance - downRayCollision.distance, decimalPrecision);
                //Determine the difference between the current play area position and the last play area position
                float playAreaPositionDelta = VRTK_SharedMethods.RoundFloat(Vector3.Distance(playArea.transform.position, lastPlayAreaPosition), decimalPrecision);
                //If the play area is not moving and the delta between the down rays is greater than 0 then you're probably walking forward over something you can stand on
                isMoving = (onGround && playAreaPositionDelta <= playAreaPositionThreshold && rayDownDelta > 0f ? true : isMoving);

                //If the item your standing over is too high to walk on top of then allow leaning over it.
                isLeaning = (onGround && rayDownDelta > leanYThreshold ? true : false);
                if (isLeaning)
                {
                    OnStartLeaning(SetBodyPhysicsEvent(downRayCollision.collider.gameObject, downRayCollision.collider));
                }
                else
                {
                    OnStopLeaning(SetBodyPhysicsEvent(null, null));
                }
            }
        }

        protected virtual void UpdateStandingPosition(Vector2 currentHeadsetPosition)
        {
            standingPositionHistory.Add(currentHeadsetPosition);
            if (standingPositionHistory.Count > standingHistorySamples)
            {
                if (!isLeaning && currentCollidingObject == null)
                {
                    bool resetStandingPosition = true;
                    for (int i = 0; i < standingHistorySamples; i++)
                    {
                        float currentDistance = Vector2.Distance(standingPositionHistory[i], standingPositionHistory[standingHistorySamples]);
                        resetStandingPosition = (currentDistance <= movementThreshold ? resetStandingPosition : false);
                    }

                    currentStandingPosition = (resetStandingPosition ? currentHeadsetPosition : currentStandingPosition);
                }
                standingPositionHistory.Clear();
            }
        }

        protected virtual void CheckHeadsetMovement()
        {
            bool currentIsMoving = isMoving;
            Vector2 currentHeadsetPosition = (headset != null ? new Vector2(headset.position.x, headset.position.z) : Vector2.zero);
            SetCurrentStandingPosition();
            SetIsMoving(currentHeadsetPosition);
            CheckLean();
            UpdateStandingPosition(currentHeadsetPosition);
            if (enableBodyCollisions)
            {
                TogglePhysics(!isMoving);
            }

            if (currentIsMoving != isMoving)
            {
                MovementChanged(isMoving);
            }
        }

        protected virtual void MovementChanged(bool movementState)
        {
            if (movementState)
            {
                OnStartMoving(SetBodyPhysicsEvent(null, null));
            }
            else
            {
                OnStopMoving(SetBodyPhysicsEvent(null, null));
            }
        }

        protected virtual void EnableDropToFloor()
        {
            initialFloorDrop = false;
            retogglePhysicsOnCanFall = false;
            teleporter = (teleporter != null ? teleporter : FindObjectOfType<VRTK_BasicTeleport>());
            if (teleporter != null)
            {
                teleporter.Teleported += Teleported;
            }
        }

        protected virtual void DisableDropToFloor()
        {
            if (teleporter != null)
            {
                teleporter.Teleported -= Teleported;
            }
        }

        protected virtual void Teleported(object sender, DestinationMarkerEventArgs e)
        {
            initialFloorDrop = false;
            StopFall();
            if (resetPhysicsAfterTeleport)
            {
                TogglePhysics(storedCurrentPhysics);
            }
        }

        protected virtual void EnableBodyPhysics()
        {
            currentBodyCollisionsSetting = enableBodyCollisions;

            CreateCollider();
            InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), true);
            InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), true);
        }

        protected virtual void DisableBodyPhysics()
        {
            DestroyCollider();
            InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), false);
            InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), false);
        }

        protected virtual void SetupIgnoredCollisions()
        {
            ResetIgnoredCollisions();
            if (ignoreCollisionsWith == null)
            {
                return;
            }

            for (int i = 0; i < ignoreCollisionsWith.Length; i++)
            {
                Collider[] objectColliders = ignoreCollisionsWith[i].GetComponentsInChildren<Collider>();
                for (int j = 0; j < objectColliders.Length; j++)
                {
                    ManagePhysicsCollider(objectColliders[j], true);
                }

                if (objectColliders.Length > 0)
                {
                    ignoreCollisionsOnGameObjects.Add(ignoreCollisionsWith[i]);
                }
            }
        }

        protected virtual void ManagePhysicsCollider(Collider collider, bool state)
        {
            Physics.IgnoreCollision(bodyCollider, collider, state);
            Physics.IgnoreCollision(footCollider, collider, state);
        }

        protected virtual void CheckStepUpCollision(Collision collision)
        {
            if (bodyCollider != null && footCollider != null && collision.contacts.Length > 0 && collision.contacts[0].thisCollider.transform.name == footColliderContainerNameCheck)
            {
                float stepYIncrement = 0.55f;
                float boxCastHeight = 0.01f;

                Vector3 colliderWorldCenter = playArea.TransformPoint(footCollider.center);
                Vector3 castStart = new Vector3(colliderWorldCenter.x, colliderWorldCenter.y + (CalculateStepUpYOffset() * stepYIncrement), colliderWorldCenter.z);
                Vector3 castExtents = new Vector3(bodyCollider.radius, boxCastHeight, bodyCollider.radius);
                RaycastHit floorCheckHit;
                float castDistance = castStart.y - playArea.position.y;
                if (Physics.BoxCast(castStart, castExtents, Vector3.down, out floorCheckHit, Quaternion.identity, castDistance) && (floorCheckHit.point.y - playArea.position.y) > stepDropThreshold)
                {
                    //If there is a teleporter attached then use that to move
                    if (teleporter != null && enableTeleport)
                    {
                        hitFloorYDelta = playArea.position.y - floorCheckHit.point.y;
                        TeleportFall(floorCheckHit.point.y, floorCheckHit);
                        lastFrameFloorY = floorCheckHit.point.y;
                    }
                    //If there isn't a teleporter then just force the position
                    else
                    {
                        playArea.position = new Vector3((floorCheckHit.point.x - (headset.position.x - playArea.position.x)), floorCheckHit.point.y, (floorCheckHit.point.z - (headset.position.z - playArea.position.z)));
                    }
                }
            }
        }

        protected virtual GameObject CreateColliderContainer(string name, Transform parent)
        {
            GameObject generatedContainer = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, name));
            generatedContainer.transform.SetParent(parent);
            generatedContainer.transform.localPosition = Vector3.zero;
            generatedContainer.transform.localRotation = Quaternion.identity;
            generatedContainer.transform.localScale = Vector3.one;

            generatedContainer.layer = LayerMask.NameToLayer("Ignore Raycast");
            VRTK_PlayerObject.SetPlayerObject(generatedContainer, VRTK_PlayerObject.ObjectTypes.Collider);

            return generatedContainer;
        }

        protected virtual GameObject InstantiateColliderContainer(GameObject objectToClone, string name, Transform parent)
        {
            GameObject generatedContainer = Instantiate(objectToClone, parent);
            generatedContainer.name = VRTK_SharedMethods.GenerateVRTKObjectName(true, name);
            VRTK_PlayerObject.SetPlayerObject(generatedContainer, VRTK_PlayerObject.ObjectTypes.Collider);

            return generatedContainer;
        }

        protected virtual void GenerateRigidbody()
        {
            if (customPlayAreaRigidbody != null)
            {
                HasExistingRigidbody();
                bodyRigidbody.mass = customPlayAreaRigidbody.mass;
                bodyRigidbody.drag = customPlayAreaRigidbody.drag;
                bodyRigidbody.angularDrag = customPlayAreaRigidbody.angularDrag;
                bodyRigidbody.useGravity = customPlayAreaRigidbody.useGravity;
                bodyRigidbody.isKinematic = customPlayAreaRigidbody.isKinematic;
                bodyRigidbody.interpolation = customPlayAreaRigidbody.interpolation;
                bodyRigidbody.collisionDetectionMode = customPlayAreaRigidbody.collisionDetectionMode;
                bodyRigidbody.constraints = customPlayAreaRigidbody.constraints;
            }
            else
            {
                if (!HasExistingRigidbody())
                {
                    bodyRigidbody.mass = bodyMass;
                    bodyRigidbody.freezeRotation = true;
                }
            }
        }

        protected virtual bool HasExistingRigidbody()
        {
            Rigidbody existingRigidbody = playArea.GetComponent<Rigidbody>();
            if (existingRigidbody != null)
            {
                generateRigidbody = false;
                bodyRigidbody = existingRigidbody;
                return true;
            }
            else
            {
                generateRigidbody = true;
                bodyRigidbody = playArea.gameObject.AddComponent<Rigidbody>();
                return false;
            }
        }

        protected virtual CapsuleCollider GenerateCapsuleCollider(GameObject parent, float setRadius)
        {
            CapsuleCollider foundCollider = parent.GetComponent<CapsuleCollider>();
            if (foundCollider == null)
            {
                foundCollider = parent.AddComponent<CapsuleCollider>();
                foundCollider.radius = setRadius;
            }

            return foundCollider;
        }

        protected virtual void GenerateBodyCollider()
        {
            if (bodyColliderContainer == null)
            {
                if (customBodyColliderContainer != null)
                {
                    bodyColliderContainer = InstantiateColliderContainer(customBodyColliderContainer, BODY_COLLIDER_CONTAINER_NAME, playArea);
                    bodyCollider = bodyColliderContainer.GetComponent<CapsuleCollider>();
                }
                else
                {
                    bodyColliderContainer = CreateColliderContainer(BODY_COLLIDER_CONTAINER_NAME, playArea);
                    bodyColliderContainer.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                }

                bodyCollider = GenerateCapsuleCollider(bodyColliderContainer, bodyRadius);

                GenerateFootCollider();
            }
        }

        protected virtual void GenerateFootCollider()
        {
            if (CalculateStepUpYOffset() > 0f)
            {
                if (customFootColliderContainer != null)
                {
                    footColliderContainer = InstantiateColliderContainer(customFootColliderContainer, FOOT_COLLIDER_CONTAINER_NAME, bodyColliderContainer.transform);
                }
                else
                {
                    footColliderContainer = CreateColliderContainer(FOOT_COLLIDER_CONTAINER_NAME, bodyColliderContainer.transform);
                    footColliderContainer.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                }

                footCollider = GenerateCapsuleCollider(footColliderContainer, 0f);
            }
        }

        protected virtual void CreateCollider()
        {
            generateRigidbody = false;

            if (playArea == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
                return;
            }

            VRTK_PlayerObject.SetPlayerObject(playArea.gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);

            GenerateRigidbody();
            GenerateBodyCollider();

            if (playArea.gameObject.layer == 0)
            {
                playArea.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
            TogglePhysics(enableBodyCollisions);
        }

        protected virtual void DestroyCollider()
        {
            if (generateRigidbody && bodyRigidbody != null)
            {
                Destroy(bodyRigidbody);
            }

            if (bodyColliderContainer != null)
            {
                Destroy(bodyColliderContainer);
            }
        }

        protected virtual void UpdateCollider()
        {
            if (bodyCollider != null && headset != null)
            {
                float newpresenceColliderYSize = (headset.position.y - playArea.position.y) - (headsetYOffset + CalculateStepUpYOffset());
                float newpresenceColliderYCenter = Mathf.Max((newpresenceColliderYSize * 0.5f) + CalculateStepUpYOffset() + playAreaHeightAdjustment, bodyCollider.radius + playAreaHeightAdjustment);

                bodyCollider.height = Mathf.Max(newpresenceColliderYSize, bodyCollider.radius);
                bodyCollider.center = new Vector3(headset.localPosition.x, newpresenceColliderYCenter, headset.localPosition.z);

                if (footCollider != null)
                {
                    float footThickness = bodyCollider.radius * stepThicknessMultiplier;
                    footCollider.radius = footThickness;
                    footCollider.height = CalculateStepUpYOffset();
                    footCollider.center = new Vector3(headset.localPosition.x, CalculateStepUpYOffset() * 0.5f, headset.localPosition.z);
                }
            }
        }

        protected virtual float CalculateStepUpYOffset()
        {
            return stepUpYOffset * 2f;
        }

        protected virtual void InitControllerListeners(GameObject mappedController, bool state)
        {
            if (mappedController != null)
            {
                IgnoreCollisions(mappedController.GetComponentsInChildren<Collider>(), true);

                VRTK_InteractGrab grabbingController = mappedController.GetComponentInChildren<VRTK_InteractGrab>();
                if (grabbingController != null && ignoreGrabbedCollisions)
                {
                    if (state)
                    {
                        grabbingController.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
                        grabbingController.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUngrabObject);
                    }
                    else
                    {
                        grabbingController.ControllerGrabInteractableObject -= new ObjectInteractEventHandler(OnGrabObject);
                        grabbingController.ControllerUngrabInteractableObject -= new ObjectInteractEventHandler(OnUngrabObject);
                    }
                }
            }
        }

        protected virtual IEnumerator RestoreCollisions(GameObject obj)
        {
            yield return new WaitForEndOfFrame();
            if (obj != null)
            {
                VRTK_InteractableObject objScript = obj.GetComponent<VRTK_InteractableObject>();
                if (objScript != null && !objScript.IsGrabbed())
                {
                    IgnoreCollisions(obj.GetComponentsInChildren<Collider>(), false);
                }
            }
        }

        protected virtual void IgnoreCollisions(Collider[] colliders, bool state)
        {
            if (bodyColliderContainer != null)
            {
                Collider[] playareaColliders = bodyColliderContainer.GetComponentsInChildren<Collider>();
                for (int i = 0; i < playareaColliders.Length; i++)
                {
                    Collider collider = playareaColliders[i];
                    if (collider.gameObject.activeInHierarchy)
                    {
                        for (int j = 0; j < colliders.Length; j++)
                        {
                            Collider controllerCollider = colliders[j];
                            if (controllerCollider.gameObject.activeInHierarchy)
                            {
                                Physics.IgnoreCollision(collider, controllerCollider, state);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.target != null)
            {
                if (restoreCollisionsRoutine != null)
                {
                    StopCoroutine(restoreCollisionsRoutine);
                }
                IgnoreCollisions(e.target.GetComponentsInChildren<Collider>(), true);
            }
        }

        protected virtual void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (gameObject.activeInHierarchy && playArea.gameObject.activeInHierarchy)
            {
                restoreCollisionsRoutine = StartCoroutine(RestoreCollisions(e.target));
            }
        }

        protected virtual bool FloorIsGrabbedObject(RaycastHit collidedObj)
        {
            if (cachedGrabbedObjectTransform != collidedObj.transform)
            {
                cachedGrabbedObjectTransform = collidedObj.transform;
                cachedGrabbedObject = collidedObj.transform.GetComponent<VRTK_InteractableObject>();
            }
            return (cachedGrabbedObject != null && cachedGrabbedObject.IsGrabbed());
        }

        protected virtual bool FloorHeightChanged(float currentY)
        {
            float yDelta = Mathf.Abs(currentY - lastFrameFloorY);
            return (yDelta > floorHeightTolerance);
        }

        protected virtual bool ValidDrop(bool rayHit, RaycastHit rayCollidedWith, float floorY)
        {
            return (rayHit && teleporter != null && teleporter.ValidLocation(rayCollidedWith.transform, rayCollidedWith.point) && !FloorIsGrabbedObject(rayCollidedWith) && FloorHeightChanged(floorY));
        }

        protected virtual float ControllerHeightCheck(GameObject controllerObj)
        {
            Ray ray = new Ray(controllerObj.transform.position, -playArea.up);
            RaycastHit rayCollidedWith;
            VRTK_CustomRaycast.Raycast(customRaycast, ray, out rayCollidedWith, defaultIgnoreLayer, Mathf.Infinity, QueryTriggerInteraction.Ignore);
            return controllerObj.transform.position.y - rayCollidedWith.distance;
        }

        protected virtual bool ControllersStillOverPreviousFloor()
        {
            if (fallRestriction == FallingRestrictors.NoRestriction)
            {
                return false;
            }
            if (fallRestriction == FallingRestrictors.AlwaysRestrict)
            {
                return true;
            }

            float controllerDropThreshold = 0.05f;
            GameObject rightController = VRTK_DeviceFinder.GetControllerRightHand();
            GameObject leftController = VRTK_DeviceFinder.GetControllerLeftHand();
            float previousY = playArea.position.y;
            bool rightCheck = (rightController.activeInHierarchy && Mathf.Abs(ControllerHeightCheck(rightController) - previousY) < controllerDropThreshold);
            bool leftCheck = (leftController.activeInHierarchy && Mathf.Abs(ControllerHeightCheck(leftController) - previousY) < controllerDropThreshold);

            if (fallRestriction == FallingRestrictors.LeftController)
            {
                rightCheck = false;
            }

            if (fallRestriction == FallingRestrictors.RightController)
            {
                leftCheck = false;
            }

            if (fallRestriction == FallingRestrictors.BothControllers)
            {
                return (rightCheck && leftCheck);
            }

            return (rightCheck || leftCheck);
        }

        protected virtual void SnapToNearestFloor()
        {
            if (!preventSnapToFloor && (enableBodyCollisions || enableTeleport) && headset != null && headset.transform.position.y > playArea.position.y)
            {
                Ray ray = new Ray(headset.transform.position, -playArea.up);
                RaycastHit rayCollidedWith;
                bool rayHit = VRTK_CustomRaycast.Raycast(customRaycast, ray, out rayCollidedWith, defaultIgnoreLayer, Mathf.Infinity, QueryTriggerInteraction.Ignore);
                hitFloorYDelta = playArea.position.y - rayCollidedWith.point.y;

                if (initialFloorDrop && (ValidDrop(rayHit, rayCollidedWith, rayCollidedWith.point.y) || retogglePhysicsOnCanFall))
                {
                    storedCurrentPhysics = ArePhysicsEnabled();
                    resetPhysicsAfterTeleport = false;
                    TogglePhysics(false);

                    HandleFall(rayCollidedWith.point.y, rayCollidedWith);
                }
                initialFloorDrop = true;
                lastFrameFloorY = rayCollidedWith.point.y;
            }
        }

        protected virtual bool PreventFall(float hitFloorY)
        {
            return (hitFloorY < playArea.position.y && ControllersStillOverPreviousFloor());
        }

        protected virtual void HandleFall(float hitFloorY, RaycastHit rayCollidedWith)
        {
            if (PreventFall(hitFloorY))
            {
                if (!retogglePhysicsOnCanFall)
                {
                    retogglePhysicsOnCanFall = true;
                    storedRetogglePhysics = storedCurrentPhysics;
                }
            }
            else
            {
                if (retogglePhysicsOnCanFall)
                {
                    storedCurrentPhysics = storedRetogglePhysics;
                    retogglePhysicsOnCanFall = false;
                }

                if (enableBodyCollisions && (teleporter == null || !enableTeleport || hitFloorYDelta > gravityFallYThreshold))
                {
                    GravityFall(rayCollidedWith);
                }
                else if (teleporter != null && enableTeleport)
                {
                    TeleportFall(hitFloorY, rayCollidedWith);
                }
            }
        }

        protected virtual void StartFall(GameObject targetFloor)
        {
            if (IsLeaning())
            {
                OnStopLeaning(SetBodyPhysicsEvent(null, null));
            }
            if (OnGround())
            {
                OnStopTouchingGround(SetBodyPhysicsEvent(null, null));
            }
            isFalling = true;
            isMoving = false;
            isLeaning = false;
            onGround = false;
            fallMinTime = Time.time + (Time.fixedDeltaTime * 3.0f); // Wait at least 3 fixed update frames before declaring falling finished 
            OnStartFalling(SetBodyPhysicsEvent(targetFloor, null));
        }

        protected virtual void StopFall()
        {
            bool wasFalling = isFalling;
            if (!OnGround())
            {
                OnStartTouchingGround(SetBodyPhysicsEvent(currentValidFloorObject, null));
            }
            isFalling = false;
            onGround = true;
            enableBodyCollisions = enableBodyCollisionsStartingValue;

            if (wasFalling)
            {
                OnStopFalling(SetBodyPhysicsEvent(null, null));
            }
        }

        protected virtual void GravityFall(RaycastHit rayCollidedWith)
        {
            StartFall(rayCollidedWith.collider.gameObject);
            TogglePhysics(true);
            ApplyBodyVelocity(Vector3.zero);
        }

        protected virtual void TeleportFall(float floorY, RaycastHit rayCollidedWith)
        {
            StartFall(rayCollidedWith.collider.gameObject);
            GameObject currentFloor = rayCollidedWith.transform.gameObject;
            Vector3 newPosition = new Vector3(playArea.position.x, floorY, playArea.position.z);
            float originalblinkTransitionSpeed = teleporter.blinkTransitionSpeed;
            teleporter.blinkTransitionSpeed = (Mathf.Abs(hitFloorYDelta) > blinkYThreshold ? originalblinkTransitionSpeed : 0f);
            OnDestinationMarkerSet(SetDestinationMarkerEvent(rayCollidedWith.distance, currentFloor.transform, rayCollidedWith, newPosition, null, true, null));
            teleporter.blinkTransitionSpeed = originalblinkTransitionSpeed;

            resetPhysicsAfterTeleport = true;
        }

        protected virtual void ApplyBodyMomentum(bool applyMomentum = false)
        {
            if (applyMomentum)
            {
                float rigidBodyMagnitude = bodyRigidbody.velocity.magnitude;
                Vector3 appliedMomentum = playAreaVelocity / (rigidBodyMagnitude < 1f ? 1f : rigidBodyMagnitude);
                bodyRigidbody.AddRelativeForce(appliedMomentum, ForceMode.VelocityChange);
            }
        }
    }
}
