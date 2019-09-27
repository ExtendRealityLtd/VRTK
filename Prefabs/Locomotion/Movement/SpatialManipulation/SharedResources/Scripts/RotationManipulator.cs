namespace VRTK.Prefabs.Locomotion.Movement.SpatialManipulation
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;

    /// <summary>
    /// Manipulates the rotation of the given Target based on the forward/backward motion between the two sources.
    /// </summary>
    public class RotationManipulator : SpatialManipulator
    {
        #region Rotation Settings
        /// <summary>
        /// Whether to rotate around the offset point.
        /// </summary>
        [Serialized]
        [field: Header("Rotation Settings"), DocumentedByXml]
        public bool RotateAroundOffset { get; set; } = true;
        #endregion

        /// <summary>
        /// The previous frame rotation angle.
        /// </summary>
        protected Vector2 previousRotationAngle = Vector2.zero;
        /// <summary>
        /// The previous frame rotation data.
        /// </summary>
        protected Quaternion previousRotation;

        /// <summary>
        /// Processes the rotation manipulation.
        /// </summary>
        public override void Process()
        {
            if (ActivationAction == null || !ActivationAction.Value)
            {
                wasActivated = false;
                return;
            }

            bool primaryValid = IsObjectValid(PrimarySource);
            bool secondaryValid = IsObjectValid(SecondarySource);

            GameObject singleSource = primaryValid && !secondaryValid ? PrimarySource : !primaryValid && secondaryValid ? SecondarySource : null;

            if (!wasActivated)
            {
                wasActivated = true;
                previousRotationAngle = GetRotationDelta(PrimarySource, SecondarySource);
                previousRotation = singleSource != null ? singleSource.transform.localRotation : Quaternion.identity;
            }

            if (singleSource == null)
            {
                Vector2 currentRotationAngle = GetRotationDelta(PrimarySource, SecondarySource);
                float newAngle = Vector2.Angle(currentRotationAngle, previousRotationAngle) * Mathf.Sign(Vector3.Cross(currentRotationAngle, previousRotationAngle).z);
                RotateByAngle(newAngle);
                previousRotationAngle = currentRotationAngle;
            }
            else
            {
                Quaternion currentRotationAngle = singleSource.transform.localRotation;
                Quaternion rotationDelta = currentRotationAngle * Quaternion.Inverse(previousRotation);
                float theta = 2.0f * Mathf.Acos(Mathf.Clamp(rotationDelta.w, -1.0f, 1.0f));
                if (theta > Mathf.PI)
                {
                    theta -= 2.0f * Mathf.PI;
                }

                Vector3 angularVelocity = new Vector3(rotationDelta.x, rotationDelta.y, rotationDelta.z);
                if (angularVelocity.sqrMagnitude > 0.0f)
                {
                    angularVelocity = theta * (1.0f / Time.deltaTime) * angularVelocity.normalized;
                }

                RotateByAngle(angularVelocity.y);
                previousRotation = currentRotationAngle;
            }
        }

        /// <summary>
        /// Gets the delta of the rotation between the given sources from the previous frame to the current frame.
        /// </summary>
        /// <param name="primary">The primary source.</param>
        /// <param name="secondary">The secondary source.</param>
        /// <returns>The rotation delta.</returns>
        protected virtual Vector2 GetRotationDelta(GameObject primary, GameObject secondary)
        {
            return new Vector2((GetLocalPosition(primary) - GetLocalPosition(secondary)).x, (GetLocalPosition(primary) - GetLocalPosition(secondary)).z);
        }

        /// <summary>
        /// Rotates the <see cref="Target"/> by the given angle.
        /// </summary>
        /// <param name="angle">The angle to rotate by.</param>
        protected virtual void RotateByAngle(float angle)
        {
            ApplyRotation(Target, RotateAroundOffset ? Offset : null, angle * Multiplier);
        }

        /// <summary>
        /// Applies a rotation to the given target.
        /// </summary>
        /// <param name="rotationTarget">The target to rotate.</param>
        /// <param name="rotationOffset">The optional offset to rotate around.</param>
        /// <param name="rotationAngle">The angle to rotate by.</param>
        protected virtual void ApplyRotation(GameObject rotationTarget, GameObject rotationOffset, float rotationAngle)
        {
            if (Mathf.Abs(rotationAngle) >= ActivationThreshold)
            {
                if (IsObjectValid(rotationOffset))
                {
                    rotationTarget.transform.RotateAround(rotationOffset.transform.position, Vector3.up, rotationAngle);
                }
                else
                {
                    rotationTarget.transform.Rotate(Vector3.up * rotationAngle);
                }
            }
        }
    }
}