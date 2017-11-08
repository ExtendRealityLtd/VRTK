// Rigidbody Follow|Utilities|90120
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Changes one GameObject's rigidbody to follow another GameObject's rigidbody.
    /// </summary>
    [AddComponentMenu("VRTK/Scripts/Utilities/Object Follow/VRTK_RigidbodyFollow")]
    public class VRTK_RigidbodyFollow : VRTK_ObjectFollow
    {
        /// <summary>
        /// Specifies how to position and rotate the rigidbody.
        /// </summary>
        public enum MovementOption
        {
            /// <summary>
            /// Use Rigidbody.position and Rigidbody.rotation.
            /// </summary>
            Set,
            /// <summary>
            /// Use Rigidbody.MovePosition and Rigidbody.MoveRotation.
            /// </summary>
            Move,
            /// <summary>
            /// Use Rigidbody.AddForce(Vector3) and Rigidbody.AddTorque(Vector3).
            /// </summary>
            Add,
            /// <summary>
            /// Use velocity and angular velocity with MoveTowards.
            /// </summary>
            Track
        }

        [Header("Follow Settings")]

        [Tooltip("Specifies how to position and rotate the rigidbody.")]
        public MovementOption movementOption = MovementOption.Set;

        [Header("Track Movement Settings")]

        [Tooltip("The maximum distance the tracked `Game Object To Change` Rigidbody can be from the `Game Object To Follow` Rigidbody before the position is forcibly set to match the position.")]
        public float trackMaxDistance = 0.25f;

        protected Rigidbody rigidbodyToFollow;
        protected Rigidbody rigidbodyToChange;
        protected float maxDistanceDelta = 10f;

        /// <summary>
        /// Follow `gameObjectToFollow` using the current settings.
        /// </summary>
        public override void Follow()
        {
            CacheRigidbodies();
            base.Follow();
        }

        protected virtual void OnDisable()
        {
            rigidbodyToFollow = null;
            rigidbodyToChange = null;
        }

        protected virtual void FixedUpdate()
        {
            Follow();
        }

        protected virtual void CacheRigidbodies()
        {
            if (gameObjectToFollow == null || gameObjectToChange == null || (rigidbodyToFollow != null && rigidbodyToChange != null))
            {
                return;
            }

            rigidbodyToFollow = gameObjectToFollow.GetComponent<Rigidbody>();
            rigidbodyToChange = gameObjectToChange.GetComponent<Rigidbody>();
        }

        protected override Vector3 GetPositionToFollow()
        {
            return (rigidbodyToFollow != null ? rigidbodyToFollow.position : Vector3.zero);
        }

        protected override Quaternion GetRotationToFollow()
        {
            return (rigidbodyToFollow != null ? rigidbodyToFollow.rotation : Quaternion.identity);
        }

        protected override Vector3 GetScaleToFollow()
        {
            return (rigidbodyToFollow != null ? rigidbodyToFollow.transform.localScale : Vector3.zero);
        }

        protected override void SetPositionOnGameObject(Vector3 newPosition)
        {
            switch (movementOption)
            {
                case MovementOption.Set:
                    rigidbodyToChange.position = newPosition;
                    break;
                case MovementOption.Move:
                    rigidbodyToChange.MovePosition(newPosition);
                    break;
                case MovementOption.Add:
                    // TODO: Test if this is correct
                    rigidbodyToChange.AddForce(newPosition - rigidbodyToChange.position);
                    break;
                case MovementOption.Track:
                    TrackPosition(newPosition);
                    break;
            }
        }

        protected override void SetRotationOnGameObject(Quaternion newRotation)
        {
            switch (movementOption)
            {
                case MovementOption.Set:
                    rigidbodyToChange.rotation = newRotation;
                    break;
                case MovementOption.Move:
                    rigidbodyToChange.MoveRotation(newRotation);
                    break;
                case MovementOption.Add:
                    // TODO: Test if this is correct
                    rigidbodyToChange.AddTorque(newRotation * Quaternion.Inverse(rigidbodyToChange.rotation).eulerAngles);
                    break;
                case MovementOption.Track:
                    TrackRotation(newRotation);
                    break;
            }
        }

        protected virtual void TrackPosition(Vector3 newPosition)
        {
            if (rigidbodyToFollow == null)
            {
                return;
            }

            if (Vector3.Distance(rigidbodyToChange.position, rigidbodyToFollow.position) > trackMaxDistance)
            {
                rigidbodyToChange.position = rigidbodyToFollow.position;
                rigidbodyToChange.rotation = rigidbodyToFollow.rotation;
            }

            float trackVelocityLimit = float.PositiveInfinity;
            Vector3 positionDelta = newPosition - rigidbodyToChange.position;
            Vector3 velocityTarget = positionDelta / Time.fixedDeltaTime;
            Vector3 calculatedVelocity = Vector3.MoveTowards(rigidbodyToChange.velocity, velocityTarget, maxDistanceDelta);

            if (trackVelocityLimit == float.PositiveInfinity || calculatedVelocity.sqrMagnitude < trackVelocityLimit)
            {
                rigidbodyToChange.velocity = calculatedVelocity;
            }
        }

        protected virtual void TrackRotation(Quaternion newRotation)
        {
            if (rigidbodyToFollow == null)
            {
                return;
            }

            float trackAngularVelocityLimit = float.PositiveInfinity;
            Quaternion rotationDelta = newRotation * Quaternion.Inverse(rigidbodyToChange.rotation);

            float angle;
            Vector3 axis;
            rotationDelta.ToAngleAxis(out angle, out axis);

            angle = ((angle > 180) ? angle -= 360 : angle);

            if (angle != 0)
            {
                Vector3 angularTarget = angle * axis;
                Vector3 calculatedAngularVelocity = Vector3.MoveTowards(rigidbodyToChange.angularVelocity, angularTarget, maxDistanceDelta);
                if (trackAngularVelocityLimit == float.PositiveInfinity || calculatedAngularVelocity.sqrMagnitude < trackAngularVelocityLimit)
                {
                    rigidbodyToChange.angularVelocity = calculatedAngularVelocity;
                }
            }
        }
    }
}