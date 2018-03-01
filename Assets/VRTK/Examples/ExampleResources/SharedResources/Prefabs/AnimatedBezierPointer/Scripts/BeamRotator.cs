namespace VRTK.Examples
{
    using UnityEngine;
    public class BeamRotator : MonoBehaviour
    {
        [Tooltip("Amount of degrees to rotate around the rotation axis per second.")]
        public float degreesPerSecond = 60.0f;

        [Tooltip("The rotation axis to rotate the object around.")]
        public Vector3 rotationAxis = Vector3.up;

        protected virtual void OnEnable()
        {
            rotationAxis.Normalize();
        }

        protected virtual void Update()
        {
            transform.Rotate(rotationAxis, degreesPerSecond * Time.deltaTime);
        }
    }
}