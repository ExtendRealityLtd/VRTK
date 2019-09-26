namespace VRTK.Prefabs.Locomotion.Movement.SpatialManipulation
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;

    /// <summary>
    /// Manipulates the scale of the given Target based on the distance between the two sources.
    /// </summary>
    public class ScaleManipulator : SpatialManipulator
    {
        #region Scale Settings
        /// <summary>
        /// The minimum the scaled value can reach.
        /// </summary>
        [Serialized]
        [field: Header("Scale Settings"), DocumentedByXml]
        public Vector3 MinimumScale { get; set; } = Vector3.one;
        /// <summary>
        /// The maximum the scaled value can reach.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Vector3 MaximumScale { get; set; } = Vector3.one * Mathf.Infinity;
        /// <summary>
        /// Whether to scale around the offset point.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool ScaleAroundOffset { get; set; } = true;
        #endregion

        /// <summary>
        /// The previous frame delta of the distance between the sources.
        /// </summary>
        protected float previousDistanceDelta;

        /// <summary>
        /// Processes the scale manipulation.
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
                previousDistanceDelta = GetDistanceDelta();
            }

            float currentDistanceDelata = GetDistanceDelta();
            float frameDistanceDelta = currentDistanceDelata - previousDistanceDelta;

            if (Mathf.Abs(frameDistanceDelta) >= ActivationThreshold)
            {
                bool validOffset = ScaleAroundOffset && IsObjectValid(Offset);

                float initialScale = Target.transform.localScale.x;
                Vector3 pivotDiff = validOffset ? Target.transform.position - Offset.transform.position : Vector3.zero;
                Vector3 newScale = Target.transform.localScale + (Vector3.one * Time.deltaTime * Mathf.Sign(frameDistanceDelta) * Multiplier);
                float relativeScale = newScale.x / Target.transform.localScale.x;

                Vector3 finalPosition = validOffset ? Offset.transform.position + pivotDiff * relativeScale : Target.transform.position;

                Target.transform.localScale = new Vector3(Mathf.Clamp(newScale.x, MinimumScale.x, MaximumScale.x), Mathf.Clamp(newScale.y, MinimumScale.y, MaximumScale.y), Mathf.Clamp(newScale.z, MinimumScale.z, MaximumScale.z));
                if (!Target.transform.localScale.x.ApproxEquals(initialScale))
                {
                    Target.transform.position = new Vector3(finalPosition.x, Target.transform.position.y, finalPosition.z);
                }
            }

            previousDistanceDelta = currentDistanceDelata;
        }

        /// <summary>
        /// Gets the delta of the distance between sources from the previous frame to the current frame.
        /// </summary>
        /// <returns></returns>
        protected virtual float GetDistanceDelta()
        {
            bool primaryValid = IsObjectValid(PrimarySource);
            bool secondaryValid = IsObjectValid(SecondarySource);

            if (primaryValid && secondaryValid)
            {
                return Vector3.Distance(GetLocalPosition(PrimarySource), GetLocalPosition(SecondarySource));
            }

            if (IsObjectValid(Offset) && (primaryValid || secondaryValid))
            {
                return Vector3.Distance(primaryValid ? GetLocalPosition(PrimarySource) : GetLocalPosition(SecondarySource), Offset.transform.position);
            }

            return 0f;
        }
    }
}