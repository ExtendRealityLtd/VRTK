namespace VRTK.Prefabs.Locomotion.Movement.SpatialManipulation
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Type;

    /// <summary>
    /// Manipulates the position of the given Target based on the forward/backward motion between the two sources.
    /// </summary>
    public class PositionManipulator : SpatialManipulator
    {
        #region Position Settings
        /// <summary>
        /// The axes that are considered valid for position manipulation.
        /// </summary>
        [Serialized]
        [field: Header("Position Settings"), DocumentedByXml]
        public Vector3State ValidMovementAxis { get; set; } = Vector3State.True;
        #endregion

        /// <summary>
        /// The previous position of the primary source.
        /// </summary>
        protected Vector3 previousPrimarySourcePosition;
        /// <summary>
        /// The previous position of the secondary source.
        /// </summary>
        protected Vector3 previousSecondarySourcePosition;

        /// <summary>
        /// Processes the position manipulation.
        /// </summary>
        public override void Process()
        {
            if (ActivationAction == null || !ActivationAction.Value)
            {
                wasActivated = false;
                return;
            }

            if (!wasActivated)
            {
                wasActivated = true;
                previousPrimarySourcePosition = GetLocalPosition(PrimarySource);
                previousSecondarySourcePosition = GetLocalPosition(SecondarySource);
            }

            Vector3 primaryMovementOffset = IsObjectValid(PrimarySource) ? GetLocalPosition(PrimarySource) - previousPrimarySourcePosition : Vector3.zero;
            Vector3 secondaryMovementOffset = IsObjectValid(SecondarySource) ? GetLocalPosition(SecondarySource) - previousSecondarySourcePosition : Vector3.zero;
            Vector3 combinedMovementOffset = primaryMovementOffset + secondaryMovementOffset;

            if (combinedMovementOffset.magnitude < ActivationThreshold)
            {
                return;
            }

            Vector3 targetMovementOffset = Target.transform.localRotation * combinedMovementOffset;

            Vector3 updatedPosition = Target.transform.localPosition - Vector3.Scale(targetMovementOffset * Multiplier, Target.transform.localScale);
            Target.transform.localPosition = new Vector3(ValidMovementAxis.xState ? updatedPosition.x : Target.transform.localPosition.x, ValidMovementAxis.yState ? updatedPosition.y : Target.transform.localPosition.y, ValidMovementAxis.zState ? updatedPosition.z : Target.transform.localPosition.z);

            previousPrimarySourcePosition = GetLocalPosition(PrimarySource);
            previousSecondarySourcePosition = GetLocalPosition(SecondarySource);
        }
    }
}