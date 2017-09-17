// Interactive Haptics Input|Interactions|30103
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interactive Haptics script is attached on the same GameObject as an Interactable Object script and provides customizable haptic feedback curves for more detailed interactions.
    /// </summary>
    public class VRTK_InteractiveHapticsTransformInput : VRTK_InteractiveHapticsInput
    {
        public enum TransformType
        {
            Position,
            Rotation,
            Scale
        }

        public TransformType transformType;

        public enum Dimension
        {
            X,
            Y,
            Z
        }

        public Dimension dimension;

        public float lowerValue;
        public float upperValue;

        private Vector3 lastVector;
        
        protected virtual void FixedUpdate()
        {
            if (transform.hasChanged)
            {
                lastVector = GetTransformVector();
                
                OnInputProvided(NormalizedTargetDimension(lastVector));

                transform.hasChanged = false;
            }
        }        

        protected virtual float NormalizedTargetDimension(Vector3 vector)
        {
           // Debug.Log("(" + lowerValue + " - " + GetTargetDimensionValue(vector) + ") / (" + upperValue + " - " + lowerValue + ") = " + Mathf.Abs((lowerValue - GetTargetDimensionValue(vector)) / (upperValue - lowerValue)));
            return Mathf.Abs((lowerValue - GetTargetDimensionValue(vector)) / (upperValue - lowerValue));
        }

        private Vector3 GetTransformVector()
        {
            Vector3 vector;

            if (transformType == TransformType.Position)
            {
               vector = transform.localPosition;
            }
            else if (transformType == TransformType.Rotation)
            {
                vector = transform.localEulerAngles;
            }
            else
            {
                vector = transform.localScale;
            }

            return vector;
        }

        private float GetTargetDimensionValue(Vector3 vector)
        {
            float value;

            if(dimension == Dimension.X)
            {
                value = vector.x;
            }
            else if (dimension == Dimension.Y)
            {
                value = vector.y;
            }
            else
            {
                value = vector.z;
            }

            return value;
        }
    }
}