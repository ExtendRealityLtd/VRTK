namespace VRTK.Examples.Prefabs.WarehouseDoor
{
    using UnityEngine;
    using Zinnia.Data.Type;
    using Zinnia.Extension;

    /// <summary>
    /// Controls the motion of the warehouse door.
    /// </summary>
    public class WarehouseDoor : MonoBehaviour
    {
        /// <summary>
        /// The door to control.
        /// </summary>
        [Tooltip("The door to control.")]
        public GameObject door;
        /// <summary>
        /// The minimum and maximum height of the door.
        /// </summary>
        [Tooltip("The minimum and maximum height of the door.")]
        public FloatRange heightLimits = FloatRange.Zero;

        /// <summary>
        /// The step to increase or decrease the height of the door by.
        /// </summary>
        public float HeightStep { get; set; }

        protected virtual void Update()
        {
            if (!HeightStep.ApproxEquals(0f))
            {
                float newHeight = Mathf.Clamp(door.transform.localScale.y + HeightStep, heightLimits.minimum, heightLimits.maximum);
                door.transform.localScale = new Vector3(1f, newHeight, 1f);
            }
        }
    }
}